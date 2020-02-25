using RFID.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;


namespace MSRC
{
    public partial class MSRC : Form
    {
        private bool DEBUG_VIEW = true;
        private bool CONSOLE_DEBUG = false;

        public const int CEILING_WET = 200;
        public const int CEILING_DAMP = 300;

        private int TSSI_MAX = 24;
        private int TSSI_MIN = 1;

        enum TagModel
        {
            Magnus1 = 0x00,
            Magnus2 = 0x01,
            Magnus3 = 0x02
        }

        TagModel tagModel = TagModel.Magnus2;

        const double N = 15.0;
        const double k = 2.0 / (N + 1.0);

        const int TAGINFO_TSSI = 0;
        const int TAGINFO_SCDE = 1;
        const int TAGINFO_TEMP = 2;
        const int TAGINFO_RSSI = 3;
        const int TAGINFO_SCAV = 4;
        const int TAGINFO_RDCT = 5;
        const int TAGINFO_CUMM = 6;
        const int TAGINFO_WTDY = 7; // Wet/Dry
        const int TAGINFO_FRQ1 = 9; // TSSI pass
        const int TAGINFO_FRQ2 = 10; // SCDE pass

        const Byte TARGET_A = 0;
        const Byte TARGET_B = 1;

        byte QueryTarget_TSSI = TARGET_A;
        byte QueryTarget_SCDE = TARGET_B;

        const Byte S0 = 0;
        const Byte S1 = 1;
        const Byte S2 = 2;
        const Byte S3 = 3;

        Byte Session = S2;

        private int[] frequencyTable;
        private const int SENSOR_CODE_MIN = 5;
        private const int SENSOR_CODE_MAX = 28;

        RFIDEngineeringReader reader = null;
        HeartbeatListener heartbeatListener = null;
        Dictionary<string, string[]> currentReaders = null;
        Dictionary<string, string[]> companyInformation = null;
        //ConcurrentDictionary<string, double[]> allTagInformation = null;
        ConcurrentDictionary<string, TagObject> newAllTagInformation = null;
        //Dictionary<string, double[]> tagInformation = null;
        Dictionary<string, TagObject> newTagInformation = null;
        Object allTagInfoLock = new Object();
        List<ListViewItem> currentReaderList = null;
        List<ListViewItem> currentTagList = null;
        List<ListViewItem> detailedTagList = null;

        Thread findTagsThread = null;
        Thread displayDampTag = null;
        Thread displayDryTag = null;
        Thread updateMap = null;
        Thread detailedView = null;

        private System.Threading.Timer readTimeTimer;

        private ManualResetEvent mreFind = new ManualResetEvent(false);
        private ManualResetEvent mreDisplayWet = new ManualResetEvent(false);
        private ManualResetEvent mreDisplayDry = new ManualResetEvent(false);
        private ManualResetEvent mreDisplayDamp = new ManualResetEvent(false);
        private ManualResetEvent mreUpdateMap = new ManualResetEvent(false);
        private ManualResetEvent mredetailedView = new ManualResetEvent(false);

        private double[] powerArray = { 33.0, 31.5, 30, 28, 26, 24, 22, 20, 18, 16, 14, 12 };
        double currentPowerLevel = 0;

        int readerCount = 0;
        bool currentlyReading = false;
        bool goodTSSI = false;
        bool programEnding = false;
        bool readerConnected = false;
        string tagListFileName = "taglist.bin";
        string companyFileName = "companyList.bin";
        string connectedReaderMAC = Properties.Settings.Default["MACAddress"].ToString();
        string readerType = " ";

        private SoundPlayer tagBeep = new SoundPlayer("beep-08b.wav");

        string debugFileName = "debug.csv";
        StreamWriter debugDataStream = null;

        bool starting = true;

        public MSRC()
        {
            InitializeComponent();

            int numWorkerThreads;
            int numComplPortThreads;

            ThreadPool.GetMinThreads(out numWorkerThreads, out numComplPortThreads);
            Console.WriteLine("Minimum: # Worker Threads {0}", numWorkerThreads);

            ThreadPool.SetMinThreads(12, 8);

            heartbeatListener = new HeartbeatListener();
            currentReaders = new Dictionary<string, string[]>();
            companyInformation = new Dictionary<string, string[]>();
            currentReaderList = new List<ListViewItem>();
            currentTagList = new List<ListViewItem>();
            detailedTagList = new List<ListViewItem>();

            this.readTimeTimer = new System.Threading.Timer(ReadTimeTimer_Tick, null, System.Threading.Timeout.Infinite, 1000);

            //Dictionary arrays: (TSSI, SCDE, TEMP, RSSI, SCDE Average, Times Read, Total SCDE Values)
            // TODO: Add gps coordinates to the end of these dictionaries
            //allTagInformation = new ConcurrentDictionary<string, double[]>();
            newAllTagInformation = new ConcurrentDictionary<string, TagObject>();
            //tagInformation = new Dictionary<string, double[]>();
            newTagInformation = new Dictionary<string, TagObject>();

            this.frequencyTable = new int[50];

            for (int idx = 0; idx < this.frequencyTable.Length; idx++)
                this.frequencyTable[idx] = 902750 + 500 * idx;

            this.cbxMinPower.Items.Clear();
            this.cbxMaxPower.Items.Clear();

            for (int idx = 0; idx < powerArray.Length; idx++)
            {
                this.cbxMinPower.Items.Add(powerArray[idx]);
                this.cbxMaxPower.Items.Add(powerArray[idx]);
            }

            // Force the window handle to be created so that the heartbeat
            // service can get access to its resources. Without this, heartbeat
            // messages can come in before they are able to be displayed causing
            // them to be permanently absent from the reader list.

            if (!this.IsHandleCreated)
                this.CreateHandle();

            readerCount = 0;
            heartbeatListener.heartbeatEvent += heartbeatHandler;
            heartbeatListener.Start();
            DeserializeCompanyInformation();
            CreateDatabase();
            admin.BringToFront();

            //Create Threads on startup of program
            updateMap = new Thread(updateMapConsole);
            findTagsThread = new Thread(findTags);
            displayDryTag = new Thread(mapDisplayDry);
            displayDampTag = new Thread(mapDisplayDamp);
            //checkReaderConnection = new Thread(checkConnection);
            detailedView = new Thread(detailedListview);

            //Start needed Threads and block on entry
            findTagsThread.Start();
            displayDryTag.Start();
            displayDampTag.Start();
            updateMap.Start();
            detailedView.Start();

            checkBox1.Checked = Properties.Settings.Default.Antenna1;
            checkBox2.Checked = Properties.Settings.Default.Antenna2;
            checkBox3.Checked = Properties.Settings.Default.Antenna3;
            checkBox4.Checked = Properties.Settings.Default.Antenna4;
            cbTR.Checked = Properties.Settings.Default.TimesRead;
            cbTSSI.Checked = Properties.Settings.Default.TSSI;
            cbTSSIAvg.Checked = Properties.Settings.Default.TSSIAvg;
            cbSCDE.Checked = Properties.Settings.Default.SCDE;
            cbSCDEAvg.Checked = Properties.Settings.Default.SCDEAvg;
            cbSTdDev.Checked = Properties.Settings.Default.StdDev;
            cbTS.Checked = Properties.Settings.Default.TagState;
            cbTemp.Checked = Properties.Settings.Default.Temperature;


            this.starting = true;
            cbxMinPower.SelectedIndex = Properties.Settings.Default.PowerMinIndex;
            cbxMaxPower.SelectedIndex = Properties.Settings.Default.PowerMaxIndex;
            this.starting = false;

            // These should be done in this order so that error check in the
            // change event handler doesn't complain.
            //nudTSSIMax.Value = Properties.Settings.Default.TssiMaxIndex;
            //nudTSSIMin.Value = Properties.Settings.Default.TssiMinIndex;



            tagModel = (TagModel)Properties.Settings.Default.TagType;

            switch ((TagModel)Properties.Settings.Default.TagType)
            {
                case TagModel.Magnus2:
                    radMagnusS2.Checked = true;
                    break;

                case TagModel.Magnus3:
                    radMagnusS3.Checked = true;
                    break;
            }

            if (!DEBUG_VIEW)
            {
                detailedTagView.Columns.Remove(colTSSI);
                detailedTagView.Columns.Remove(colTSSIAvg);
                detailedTagView.Columns.Remove(colSCDE);
            }

            if (DEBUG_VIEW)
                try
                {
                    this.debugDataStream = new StreamWriter(this.debugFileName);
                }
                catch
                {

                }
        }

        //Buttons & Console Actions///////////////////////////////////////////////////////////////////////////////////////
        private void MapRoof_Click(object sender, EventArgs e)
        {
            String Start = DateTime.Now.ToString("yyyy-MM-dd hh:mm:s");
            String SQLCommand = "";
            Int64 LastCompany = 0;
            UInt32 antennaCount = GetSelectedAntennaCount();

            // TODO: put this back together for GP Console
            //if (companyInformation.Count == 0)
            //    MessageBox.Show("No Companies Entered Into The Database Please Fill Company Information Before Mapping Roof", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //else 
            if (antennaCount == 0)
                MessageBox.Show("Please Select At Least One Antenna For Operation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                SQLCommand = "SELECT * FROM CompanyTable ORDER BY ID DESC LIMIT 1";
                using (SQLiteConnection con = new SQLiteConnection("data source=tagDatabaseFile.db3"))
                {
                    using (SQLiteCommand com = new SQLiteCommand(con))
                    {
                        try
                        {
                            con.Open();                             // Open the connection to the database
                            com.CommandText = SQLCommand;
                            com.ExecuteNonQuery();
                            using (SQLiteDataReader tableReader = com.ExecuteReader())
                            {
                                while (tableReader.Read())
                                {
                                    LastCompany = (long)tableReader["Id"];
                                }
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            Console.WriteLine("Console failed to Read Last Company " + ex);
                        }

                        SQLCommand = @"INSERT INTO ScanTable (Start, Stop, Company)
                                    VALUES
                                        ('" + Start + "', '0', " + LastCompany + ")";
                        try
                        {
                            com.CommandText = SQLCommand;
                            com.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            Console.WriteLine("Console failed to write scantable entry " + ex);
                        }
                        con.Close();
                    }
                }
                MapRoof.BringToFront();
            }

        }

        private uint GetSelectedAntennaCount()
        {
            uint antennaCount = 0;

            if (checkBox1.Checked)
                antennaCount++;
            if (checkBox2.Checked)
                antennaCount++;
            if (checkBox3.Checked)
                antennaCount++;
            if (checkBox4.Checked)
                antennaCount++;

            return antennaCount;
        }

        private void btnChangeSettings_Click(object sender, EventArgs e)
        {
            //Int64 CurrentRead = 0;
            //String StopTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:s");
            //String SQLCommand = "SELECT * FROM ScanTable ORDER BY ID DESC LIMIT 1";
            //using (SQLiteConnection con = new SQLiteConnection("data source=tagDatabaseFile.db3"))
            //{
            //    using (SQLiteCommand com = new SQLiteCommand(con))
            //    {
            //        try
            //        {
            //            con.Open();                             // Open the connection to the database
            //            com.CommandText = SQLCommand;
            //            com.ExecuteNonQuery();
            //            using (SQLiteDataReader tableReader = com.ExecuteReader())
            //            {
            //                while (tableReader.Read())
            //                {
            //                    CurrentRead = (long)tableReader["Id"];
            //                }
            //            }
            //        }
            //        catch (SQLiteException ex)
            //        {
            //            Console.WriteLine("Console failed to Read Last Company " + ex);
            //        }
            //        SQLCommand = @"UPDATE ScanTable 
            //                  SET Stop = '" + StopTime + "' Where Id = " + CurrentRead;
            //        try
            //        { 
            //            com.CommandText = SQLCommand;
            //            com.ExecuteNonQuery();
            //        }
            //        catch (SQLiteException ex)
            //        {
            //            Console.WriteLine("Console failed to update endtime scantable entry " + ex);
            //        }
            //        con.Close();
            //    }

            //}

            main.BringToFront();
            //currentlyReading = false;
            //readingStatus.BackColor = Color.FromArgb(204, 51, 0);
            //btnPauseStartReading.Invoke(new MethodInvoker(delegate { btnPauseStartReading.Text = "Start Reading"; }));
            //searchTagReadStatus.BackColor = Color.Silver;
            //searchMoistureRead.BackColor = Color.Silver;
        }

        private void ClearReaderList_Click(object sender, EventArgs e)
        {
            currentReaders.Clear();
            Readers.Items.Clear();
            readerCount = 0;
        }

        private void btnPauseStartReading_Click(object sender, EventArgs e)
        {
            if (GetSelectedAntennaCount() == 0)
            {
                MessageBox.Show("Please go to Settings and select at least one antenna For operation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int requestedReadTime = 0;

            if ((this.txtRequestedReadTime.Text.Trim() != "") && (!int.TryParse(this.txtRequestedReadTime.Text, out requestedReadTime)))
            {
                MessageBox.Show("Please enter a valid time for the read operation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!currentlyReading)
            {
                this.lblReportedElapsedTime.Text = "0";
                this.readTimeTimer.Change(1000, 1000);

                currentlyReading = true;
                readingStatus.BackColor = Color.FromArgb(0, 192, 0);

                btnPauseStartReading.Invoke(new MethodInvoker(delegate { btnPauseStartReading.Text = "Stop Reading"; }));
                btnStartStop.Invoke(new MethodInvoker(delegate { btnStartStop.Text = "Stop Reading"; }));
                mreFind.Set();
                mreUpdateMap.Set();
                mredetailedView.Set();
                btnChangeSettings.Enabled = false;
                btnClearTagList.Enabled = false;
            }
            else
            {
                this.readTimeTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                currentlyReading = false;
                readingStatus.BackColor = Color.FromArgb(204, 51, 0);
                tagReadStatus.BackColor = Color.Silver;
                btnPauseStartReading.Invoke(new MethodInvoker(delegate { btnPauseStartReading.Text = "Start Reading"; }));
                btnStartStop.Invoke(new MethodInvoker(delegate { btnStartStop.Text = "Start Reading"; }));
                btnChangeSettings.Enabled = true;
                btnClearTagList.Enabled = true;
            }
        }

        private void btnClearTagList_Click(object sender, EventArgs e)
        {
            DialogResult results = MessageBox.Show("Are you sure you want to clear all tag data?", "Clear", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (results == DialogResult.OK)
            {
                SerializeTagList();
                //allTagInformation.Clear();
                newAllTagInformation.Clear();
                //tagInformation.Clear();
                newTagInformation.Clear();
                detailedTagList.Clear();

                if (DEBUG_VIEW)
                {
                    this.debugDataStream.Close();
                    this.debugDataStream = new StreamWriter(this.debugFileName);
                }

                detailedTagView.Invoke(new MethodInvoker(delegate { detailedTagView.Items.Clear(); }));
                lstTagView.Invoke(new MethodInvoker(delegate { lstTagView.Items[0].SubItems[1].Text = "0"; }));
                lstTagView.Invoke(new MethodInvoker(delegate { lstTagView.Items[1].SubItems[1].Text = "0"; }));
                lstTagView.Invoke(new MethodInvoker(delegate { lstTagView.Items[3].SubItems[1].Text = "0"; }));
                lstTagView.Invoke(new MethodInvoker(delegate { lstTagView.Items[2].SubItems[1].Text = "0"; }));
                lstTagView.Invoke(new MethodInvoker(delegate { lstTagView.Items[4].SubItems[1].Text = "0"; }));
            }
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            String SQLCommand = "";
            if (txtCompanyName.Text == "" || txtCity.Text == "" || txtState.Text == "")
            {
                if (txtCompanyName.Text == "")
                    lblCompanyName.ForeColor = Color.Red;
                else
                    lblCompanyName.ForeColor = Color.FromName("ControlText");

                if (txtCity.Text == "")
                    lblCity.ForeColor = Color.Red;
                else
                    lblCity.ForeColor = Color.FromName("ControlText");

                if (txtState.Text == "")
                    lblState.ForeColor = Color.Red;
                else
                    lblState.ForeColor = Color.FromName("ControlText");

                MessageBox.Show("Please Fill in Required Boxes to Save Company Information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string uniqueID = txtCompanyName.Text + txtBuildingNumb.Text;
                string[] compData = { txtCompanyName.Text, txtBuildingNumb.Text, txtAddress.Text, txtCity.Text, txtState.Text, txtZip.Text };

                if (!companyInformation.ContainsKey(uniqueID))
                {
                    companyInformation.Add(uniqueID, compData);
                    MessageBox.Show("Company Information Added", "Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblCompanyName.ForeColor = Color.FromName("ControlText");
                    lblCity.ForeColor = Color.FromName("ControlText");
                    lblState.ForeColor = Color.FromName("ControlText");
                    SQLCommand = @"INSERT INTO CompanyTable (CompanyName, BuildingNumber, Address, City, State, ZipCode)
                                 VALUES
                                    ('" + txtCompanyName.Text + "', '" + txtBuildingNumb.Text + "', '" + txtAddress.Text + "', '" + txtCity.Text + "', '" + txtState.Text + "', '" + txtZip.Text + "' )";

                    using (SQLiteConnection con = new SQLiteConnection("data source=tagDatabaseFile.db3"))
                    {
                        using (SQLiteCommand com = new SQLiteCommand(con))
                        {
                            try
                            {
                                con.Open();                             // Open the connection to the database


                                com.CommandText = SQLCommand;
                                com.ExecuteNonQuery();

                                con.Close();        // Close the connection to the database
                            }
                            catch (SQLiteException ex)
                            {
                                Console.WriteLine("Console failed to add new company information " + ex);
                            }

                        }
                    }
                    SerializeCompanyInformation();

                }
                else
                {
                    DialogResult result = MessageBox.Show("Company Information for " + txtCompanyName.Text + " is already in the database. Would you like to overwrite that entry?", "Added", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.Yes))
                    {
                        companyInformation[uniqueID] = compData;
                        MessageBox.Show("Company Information Added", "Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                companyName.Invoke(new MethodInvoker(delegate { companyName.Text = "Company:"; }));
                companyAddress.Invoke(new MethodInvoker(delegate { companyAddress.Text = "Address:"; }));
                buildingNumber.Invoke(new MethodInvoker(delegate { buildingNumber.Text = "Building:"; }));

                companyNameValue.Invoke(new MethodInvoker(delegate { companyNameValue.Text = txtCompanyName.Text; }));
                companyAddressValue.Invoke(new MethodInvoker(delegate { companyAddressValue.Text = txtAddress.Text; }));
                buildingNumberValue.Invoke(new MethodInvoker(delegate { buildingNumberValue.Text = txtBuildingNumb.Text; }));

                //searchCompanyName.Invoke(new MethodInvoker(delegate { searchCompanyName.Text = "Company:"; }));
                //searchCompanyAddress.Invoke(new MethodInvoker(delegate { searchCompanyAddress.Text = "Address:"; }));
                //searchBuildingNumber.Invoke(new MethodInvoker(delegate { searchBuildingNumber.Text = "Building:"; }));

                //searchCompanyNameValue.Invoke(new MethodInvoker(delegate { searchCompanyNameValue.Text = txtCompanyName.Text; }));
                //searchCompanyAddressValue.Invoke(new MethodInvoker(delegate { searchCompanyAddressValue.Text = txtAddress.Text; }));
                //searchBuildingNumberValue.Invoke(new MethodInvoker(delegate { searchBuildingNumberValue.Text = txtBuildingNumb.Text; }));
            }
        }

        private void Readers_SelectedIndexChanged(object sender, EventArgs e)
        {
            connectedReaderMAC = Readers.SelectedItems[0].SubItems[2].Text;
            readerType = Readers.SelectedItems[0].SubItems[6].Text;

            String readerName = currentReaders[connectedReaderMAC][0];
            reader = new RFIDEngineeringReader(currentReaders[connectedReaderMAC][1], 5000);
            this.Text = String.Format("Sensor Console: <{0}>", readerName);
            readerConnected = true;
            MapRoof.BringToFront();
        }

        private void btnMapShowWetTags_Click(object sender, EventArgs e)
        {
            generateWetTagList();
            wetTagList.Invoke(new MethodInvoker(delegate { wetTagList.BringToFront(); }));
        }

        private void btnReturnHome_Click(object sender, EventArgs e)
        {
            Int64 CurrentRead = 0;
            String StopTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:s");
            String SQLCommand = "SELECT * FROM ScanTable ORDER BY ID DESC LIMIT 1";
            using (SQLiteConnection con = new SQLiteConnection("data source=tagDatabaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        con.Open();                             // Open the connection to the database
                        com.CommandText = SQLCommand;
                        com.ExecuteNonQuery();
                        using (SQLiteDataReader tableReader = com.ExecuteReader())
                        {
                            while (tableReader.Read())
                            {
                                CurrentRead = (long)tableReader["Id"];
                            }
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Console failed to Read Last Company " + ex);
                    }
                    SQLCommand = @"UPDATE ScanTable 
                              SET Stop = '" + StopTime + "' Where Id = " + CurrentRead;
                    try
                    {
                        com.CommandText = SQLCommand;
                        com.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Console failed to update endtime scantable entry " + ex);
                    }
                    con.Close();
                }
            }

            readingStatus.BackColor = Color.FromArgb(204, 51, 0);
            //searchReadingStatus.BackColor = Color.FromArgb(204, 51, 0);
            tagReadStatus.BackColor = Color.Silver;
            moistureStatus.BackColor = Color.Silver;
            //searchTagReadStatus.BackColor = Color.Silver;
            //searchMoistureRead.BackColor = Color.Silver;
            MapRoof.Invoke(new MethodInvoker(delegate { MapRoof.BringToFront(); }));
        }

        private void btnCalInfoBack_Click(object sender, EventArgs e)
        {
            MapRoof.BringToFront();
        }

        //HeartBeat//////////////////////////////////////////////////////////////////////////////////////

        private void heartbeatHandler(object sender, RFID.Notifications.HeartbeatEventArgs e)
        {
            if (!programEnding)
            {
                try
                {
                    HeartbeatProperties hbp = e.HeartbeatProperties;
                    string readerName = hbp["ReaderName"];
                    if (!hbp.ContainsKey("IPAddress")) return; // remove any heartbeats that do not contain a IP address
                    string IPAddress = hbp["IPAddress"];
                    string MACAddress = hbp["MACAddress"];
                    string UpTime = hbp.ContainsKey("Uptime") ? hbp["Uptime"] : "0";
                    string LatLon = "";
                    string[] LatLonParts = new string[2];
                    string ReaderType = hbp["ReaderType"];
                    if (hbp.ContainsKey("LatLon"))
                    {
                        LatLon = hbp["LatLon"]; // allow handling of both SA and Extreme
                        LatLonParts = LatLon.Split(',');
                    }
                    else
                    {
                        LatLonParts[0] = "";
                        LatLonParts[1] = "";
                    }

                    string[] reader = { readerName, IPAddress, MACAddress, UpTime, LatLonParts[0], LatLonParts[1], ReaderType };

                    if (currentReaders.ContainsKey(MACAddress))
                    {
                        for (int i = 0; i < readerCount; i++)
                        {
                            var item = currentReaderList[i];

                            currentReaders.Remove(MACAddress);
                            currentReaders.Add(MACAddress, reader);

                            if (item.SubItems[2].Text == reader[2])
                            {
                                Readers.Invoke(new MethodInvoker(delegate
                                {
                                    item.SubItems[0].Text = reader[0];
                                    item.SubItems[1].Text = reader[1];
                                    item.SubItems[3].Text = reader[3];
                                    //item.SubItems[4].Text = reader[6];
                                }));
                            }
                        }
                    }
                    else
                    {
                        currentReaders.Add(MACAddress, reader);
                        currentReaderList.Add(new ListViewItem(currentReaders[reader[2]]));
                        Readers.Invoke(new MethodInvoker(delegate { Readers.Items.Add(currentReaderList[readerCount]); }));
                        readerCount = currentReaderList.Count;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Printing Heartbeat");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        //Console Functions//////////////////////////////////////////////////////////////////////////////

        private void findTags()
        {
            while (true) //Always sit in thread, but block until needed/called.
            {
                mreFind.WaitOne();
                mreFind.Reset();

                if (programEnding) break;

                int[] GUIAntSel;
                int antennaCount = 0;
                int sequence = 0;
                int powerIndex = 0;
                int frequencyIndex = 0;
                int minPowerIndex = 0;
                int maxPowerIndex = 0;

                setupReader(); // place the initial settings onto the connected reader

                antennaCount = GetSelectedAntennas(out GUIAntSel);

                int[] antennas = new int[antennaCount];
                for (int idx = 3; idx >= 0; idx--)
                {
                    if (GUIAntSel[idx] == 1)
                    {
                        antennas[sequence++] = idx;
                    }
                }

                sequence = 0;

                main.Invoke(new MethodInvoker(delegate
                {
                    minPowerIndex = cbxMinPower.SelectedIndex;
                    maxPowerIndex = cbxMaxPower.SelectedIndex;

                    //TSSI_MAX = (int)nudTSSIMax.Value;
                    //TSSI_MIN = (int)nudTSSIMin.Value;
                }));

                powerIndex = maxPowerIndex;
                frequencyIndex = 16;

                this.ChangeFrequency(frequencyIndex);
                this.ChangePower(powerIndex, antennas[sequence]);
 
               while (true)
                {
                    // Clear the tag information gathered during each cycle so that we
                    // are not mixing TSSI/SCDE reading between antennas and power levels
                    //tagInformation.Clear();
                    newTagInformation.Clear();

                    readTSSI();

                    if (goodTSSI)
                        readSensorCode();
                    else if (CONSOLE_DEBUG)
                        Console.WriteLine("!!!!!!!!!!!!!!!!!! Skipping Sensor Code Read: goodTSSI = false");

                    if (tagModel == TagModel.Magnus3)
                        readTempValue(); // Read the temperature value regardless of what we return for TSSI as it shouldn't matter

                    // Merge valid readings from this cycle into master dictionary.
                    this.MergeReadResults();
                    goodTSSI = false;

                    frequencyIndex++;
                    if (frequencyIndex >= this.frequencyTable.Length)
                        frequencyIndex = 0;

                    this.ChangeFrequency(frequencyIndex);

                    if (powerIndex > minPowerIndex)
                    {
                        powerIndex = maxPowerIndex;
                        sequence++;
                    }

                    if (sequence >= antennaCount)
                        sequence = 0;

                    this.ChangePower(powerIndex++, antennas[sequence]);

                    if (!currentlyReading)
                    {
                        MapRoof.Invoke(new MethodInvoker(delegate
                        {
                            tagReadStatus.BackColor = Color.Silver;
                            moistureStatus.BackColor = Color.Silver;
                        }));

                        break;
                    }
                }
            }
        }

        private int GetSelectedAntennas(out int[] GUIAntSel)
        {
            int antennaCount = 0;
            GUIAntSel = new int[4];

            if (readerType == "SensArray Pro, Model SP13350" ||
                readerType == "SensArray Enterprise, Model SE24370")
            {
                if (checkBox1.Checked)
                {
                    GUIAntSel[0] = 1;
                    antennaCount++;
                }
                if (checkBox2.Checked)
                {
                    GUIAntSel[3] = 1;
                    antennaCount++;
                }
                if (checkBox3.Checked)
                {
                    GUIAntSel[2] = 1;
                    antennaCount++;
                }
                if (checkBox4.Checked)
                {
                    GUIAntSel[1] = 1;
                    antennaCount++;
                }
            }
            else if (readerType == "SensX, Model SX11480")
            {
                if (checkBox1.Checked)
                {
                    GUIAntSel[0] = 1;
                    antennaCount++;
                }
                if (checkBox2.Checked)
                {
                    GUIAntSel[1] = 1;
                    antennaCount++;
                }
                if (checkBox3.Checked)
                {
                    GUIAntSel[2] = 1;
                    antennaCount++;
                }
                if (checkBox4.Checked)
                {
                    GUIAntSel[3] = 1;
                    antennaCount++;
                }
            }

            return antennaCount;
        }

        // This method needs to merge the TSSI/SCDE information into the master
        // dictionary (allTagInformation) in a concurrent, thread-safe manner.

        private void MergeReadResults()
        {
            lock (allTagInfoLock)
            {
                foreach (KeyValuePair<string, TagObject> entry in newTagInformation)
                {
                    TagObject allTagObject = new TagObject();

                    if (DEBUG_VIEW)
                    {
                        debugDataStream.Write("{0}, {1}, {2}, ", DateTime.Now.ToString(), entry.Key, this.currentPowerLevel);
                        debugDataStream.Write("{0}, ", entry.Value.TSSI);
                        debugDataStream.Write("{0}, ", entry.Value.SCDEraw);
                        debugDataStream.Write("{0}, ", entry.Value.SCDE);
                        debugDataStream.Write("{0}, ", entry.Value.TEMP);
                        debugDataStream.Write("{0}, ", entry.Value.RSSI);
                        debugDataStream.Write("{0}, ", entry.Value.SCDEAvg);
                        debugDataStream.Write("{0}, ", entry.Value.SCDEStdDev);
                        debugDataStream.Write("{0}, ", entry.Value.TimesRead);
                        debugDataStream.Write("{0}, ", entry.Value.TotalSCDEValue);
                        debugDataStream.Write("{0}, ", entry.Value.Lat);
                        debugDataStream.Write("{0}, ", entry.Value.Lon);
                        debugDataStream.Write("{0}, ", entry.Value.freq_TSSI);
                        debugDataStream.Write("{0}, ", entry.Value.freq_SCDE);
                    }

                    if (CONSOLE_DEBUG)
                    {
                        Console.Write("Read: ");
                        Console.Write("{0}, {1}, {2}, ", DateTime.Now.ToString(), entry.Key, this.currentPowerLevel);
                        Console.Write("{0}, ", entry.Value.TSSI);
                        Console.Write("{0}, ", entry.Value.SCDEraw);
                        Console.Write("{0}, ", entry.Value.SCDE);
                        Console.Write("{0}, ", entry.Value.TEMP);
                        Console.Write("{0}, ", entry.Value.RSSI);
                        Console.Write("{0}, ", entry.Value.SCDEAvg);
                        Console.Write("{0}, ", entry.Value.SCDEStdDev);
                        Console.Write("{0}, ", entry.Value.TimesRead);
                        Console.Write("{0}, ", entry.Value.TotalSCDEValue);
                        Console.Write("{0}, ", entry.Value.Lat);
                        Console.Write("{0}, ", entry.Value.Lon);
                        Console.Write("{0}, ", entry.Value.freq_TSSI);
                        Console.Write("{0}, ", entry.Value.freq_SCDE);
                        Console.WriteLine();
                    }

                    if (newAllTagInformation.TryGetValue(entry.Key, out allTagObject))
                    {

                        // Entry found. Update
                        if (entry.Value.TSSI > 0 && entry.Value.SCDE > 0)
                        {
                            allTagObject.TSSI = entry.Value.TSSI;
                            allTagObject.TSSIValues.AddLast(entry.Value.TSSI);
                            allTagObject.SCDE = entry.Value.SCDE;
                            allTagObject.SCDEValues.AddLast(entry.Value.SCDE);
                            allTagObject.SCDEStdDev = calculateLLStdDeviation(allTagObject.SCDEValues);
                            allTagObject.TEMP = entry.Value.TEMP;
                            allTagObject.RSSI = entry.Value.RSSI;

                            //if (allTagObject.TSSIValues.Count > 10)
                            //    allTagObject.TSSIValues.RemoveFirst();
                            //if (allTagObject.SCDEValues.Count > 10)
                            //    allTagObject.SCDEValues.RemoveFirst();

                            if (allTagObject.SCDEAvg == 0)
                                allTagObject.SCDEAvg = entry.Value.SCDE;
                            else
                            {
                                //if (allTagObject.SCDEValues.Count < 5)
                                //    allTagObject.SCDEAvg = allTagObject.SCDEAvg * (1.0 - k) + entry.Value.SCDE * k;
                                //else
                                    allTagObject.SCDEAvg = calculateLLAverage(allTagObject.SCDEValues);
                            }
                            if (allTagObject.TSSIAvg == 0)
                                allTagObject.TSSIAvg = entry.Value.TSSI;
                            else
                            {
                                //if (allTagObject.TSSIValues.Count < 5)
                                //    allTagObject.TSSIAvg = allTagObject.TSSIAvg * (1.0 - k) + entry.Value.TSSI * k;
                                //else
                                    allTagObject.TSSIAvg = calculateLLAverage(allTagObject.TSSIValues);
                            }
                                

                            allTagObject.TimesRead++;       
                            allTagObject.TotalSCDEValue = allTagObject.TotalSCDEValue + entry.Value.SCDE;
                        }
                        else
                            allTagObject.RSSI = entry.Value.RSSI;

                        if (entry.Value.TEMP != 0)
                            allTagObject.TEMP = entry.Value.TEMP;

                        if (DEBUG_VIEW)
                        {
                            debugDataStream.Write("{0}, ", allTagObject.TSSI);
                            debugDataStream.Write("{0}, ", allTagObject.SCDE);
                            debugDataStream.Write("{0}, ", allTagObject.TEMP);
                            debugDataStream.Write("{0}, ", allTagObject.RSSI);
                            debugDataStream.Write("{0}, ", allTagObject.SCDEAvg);
                            debugDataStream.Write("{0}, ", allTagObject.SCDEStdDev);
                            debugDataStream.Write("{0}, ", allTagObject.TimesRead);
                            debugDataStream.Write("{0}, ", allTagObject.TotalSCDEValue);
                            debugDataStream.Write("{0}, ", allTagObject.Lat);
                            debugDataStream.Write("{0}, ", allTagObject.Lon);
                            debugDataStream.Write("{0}, ", allTagObject.freq_TSSI);
                            debugDataStream.Write("{0}, ", allTagObject.freq_SCDE);

                            debugDataStream.WriteLine();
                            debugDataStream.Flush();
                        }

                        if (CONSOLE_DEBUG)
                        {
                            Console.Write("Update: ");
                            Console.Write("{0}, ", allTagObject.TSSI);
                            Console.Write("{0}, ", allTagObject.SCDE);
                            Console.Write("{0}, ", allTagObject.TEMP);
                            Console.Write("{0}, ", allTagObject.RSSI);
                            Console.Write("{0}, ", allTagObject.SCDEAvg);
                            Console.Write("{0}, ", allTagObject.SCDEStdDev);
                            Console.Write("{0}, ", allTagObject.TimesRead);
                            Console.Write("{0}, ", allTagObject.TotalSCDEValue);
                            Console.Write("{0}, ", allTagObject.Lat);
                            Console.Write("{0}, ", allTagObject.Lon);
                            Console.Write("{0}, ", allTagObject.freq_TSSI);
                            Console.Write("{0}, ", allTagObject.freq_SCDE);

                            Console.WriteLine();
                        }

                        //allTagInformation.TryUpdate(entry.Key, newValues, oldValues);
                    }
                    else
                    {
                        // Entry not found. Add.
                        if (entry.Value.TSSI > 0 && entry.Value.SCDE > 0)
                        {
                            entry.Value.SCDEAvg = entry.Value.SCDE;
                            entry.Value.TotalSCDEValue = entry.Value.SCDE;
                            entry.Value.TimesRead = 1;

                            this.tagBeep.Play();
                        }
                        else
                        {
                            entry.Value.TSSI = 0;
                            entry.Value.SCDE = 0;
                            entry.Value.SCDEAvg = 0;
                            entry.Value.SCDEStdDev = 0;
                        }

                        if (DEBUG_VIEW)
                        {
                            debugDataStream.Write("{0}, ", entry.Value.TSSI);
                            debugDataStream.Write("{0}, ", entry.Value.SCDE);
                            debugDataStream.Write("{0}, ", entry.Value.TEMP);
                            debugDataStream.Write("{0}, ", entry.Value.RSSI);
                            debugDataStream.Write("{0}, ", entry.Value.SCDEAvg);
                            debugDataStream.Write("{0}, ", entry.Value.TimesRead);
                            debugDataStream.Write("{0}, ", entry.Value.TotalSCDEValue);
                            debugDataStream.Write("{0}, ", entry.Value.Lat);
                            debugDataStream.Write("{0}, ", entry.Value.Lon);
                            debugDataStream.Write("{0}, ", entry.Value.freq_TSSI);
                            debugDataStream.Write("{0}, ", entry.Value.freq_SCDE);

                            debugDataStream.WriteLine();
                            debugDataStream.Flush();
                        }

                        if (CONSOLE_DEBUG)
                        {
                            Console.Write("add: ");
                            Console.Write("{0}, ", entry.Value.TSSI);
                            Console.Write("{0}, ", entry.Value.SCDE);
                            Console.Write("{0}, ", entry.Value.TEMP);
                            Console.Write("{0}, ", entry.Value.RSSI);
                            Console.Write("{0}, ", entry.Value.SCDEAvg);
                            Console.Write("{0}, ", entry.Value.TimesRead);
                            Console.Write("{0}, ", entry.Value.TotalSCDEValue);
                            Console.Write("{0}, ", entry.Value.Lat);
                            Console.Write("{0}, ", entry.Value.Lon);
                            Console.Write("{0}, ", entry.Value.freq_TSSI);
                            Console.Write("{0}, ", entry.Value.freq_SCDE);

                            Console.WriteLine();
                        }

                        newAllTagInformation.TryAdd(entry.Key, entry.Value);
                    }
                }
            }
        }

        private double calculateLLAverage(LinkedList<double> workingList)
        {
            LinkedList<double> tempList = new LinkedList<double>();

            foreach(double value in workingList)
                tempList.AddLast(value);

            double average = 0;

            if(tempList.Count > 5)
            {
                tempList.Remove(tempList.Max());
                tempList.Remove(tempList.Max());

                tempList.Remove(tempList.Min());
                tempList.Remove(tempList.Min());
            }
            

            foreach (double value in tempList)
            {
                average += value;
            }
            average /= tempList.Count;


            return average;
        }

        private double calculateLLStdDeviation(LinkedList<double> workingList)
        {
            double stdDeviation = 0;
            double average = 0;
            LinkedList<double> tempList = new LinkedList<double>();

            if (workingList.Count > 1)
            {
                foreach (double value in workingList)
                    tempList.AddLast(value);

                if(tempList.Count > 6)
                {
                    tempList.Remove(tempList.Max());
                    tempList.Remove(tempList.Max());

                    tempList.Remove(tempList.Min());
                    tempList.Remove(tempList.Min());
                }
            
                foreach (double value in tempList)
                    average += value;

                average /= tempList.Count;

                foreach (double value in tempList)
                    stdDeviation += Math.Pow((value - average), 2);

                stdDeviation = Math.Sqrt(stdDeviation / (double)(tempList.Count - 1));
            }

            return stdDeviation;
        }

        private double calculateFreqCorrection(TagObject workingObject)
        {
            double correctedValue = 0;

            //correctedValue = workingObject.SCDEraw + (workingObject.FreqCorNumerator / workingObject.FreqCorDenominator) * (workingObject.freq_TSSI - workingObject.MidFreq);
            correctedValue = workingObject.SCDEraw * (1.0 + (workingObject.FreqCorNumerator / workingObject.FreqCorDenominator / workingObject.PowerCorDenominator) * (workingObject.freq_TSSI - workingObject.MidFreq));

            return correctedValue;
        }

        private void updateMapConsole()
        {
            while (true)
            {
                mreUpdateMap.WaitOne();
                mreUpdateMap.Reset();
                int dryTagCount = 0;
                int wetTagCount = 0;
                int dampTagCount = 0;
                int badTagCount = 0;
                if (programEnding) break;

                while (currentlyReading)
                {
                    dryTagCount = 0;
                    wetTagCount = 0;
                    dampTagCount = 0;
                    badTagCount = 0;

                    tagReadStatus.BackColor = Color.Silver;
                    moistureStatus.BackColor = Color.Silver;
                    lstTagView.Invoke(new MethodInvoker(delegate { lstTagView.Items[0].SubItems[1].Text = newAllTagInformation.Count().ToString(); }));


                    for (int idx = 0; idx < newAllTagInformation.Count(); idx++)
                    {
                        if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg >= CEILING_DAMP)
                            dryTagCount++;
                        else if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg <= CEILING_WET && newAllTagInformation.ElementAt(idx).Value.SCDEAvg != 0)
                            wetTagCount++;
                        else if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg != 0)
                            dampTagCount++;
                        else
                            badTagCount++;
                    }

                    lstTagView.Invoke(new MethodInvoker(delegate
                    {
                        lstTagView.Items[1].SubItems[1].Text = dryTagCount.ToString();
                        lstTagView.Items[3].SubItems[1].Text = wetTagCount.ToString();
                        lstTagView.Items[2].SubItems[1].Text = dampTagCount.ToString();
                        lstTagView.Items[4].SubItems[1].Text = badTagCount.ToString();
                    }));

                    Thread.Sleep(200);
                }

                MapRoof.Invoke(new MethodInvoker(delegate { tagReadStatus.BackColor = Color.Silver; }));
                MapRoof.Invoke(new MethodInvoker(delegate { moistureStatus.BackColor = Color.Silver; }));
            }
        }

        private void mapDisplayDamp()
        {
            while (true)
            {
                mreDisplayDamp.WaitOne();
                mreDisplayDamp.Reset();

                if (programEnding)
                {
                    break;
                }

                dampStatus.BackColor = Color.FromArgb(255, 204, 0);
                Thread.Sleep(1000);
                dampStatus.BackColor = Color.White;
            }
        }

        private void mapDisplayDry()
        {
            while (true)
            {
                mreDisplayDry.WaitOne();
                mreDisplayDry.Reset();

                if (programEnding) break;

                dryStatus.BackColor = Color.FromArgb(0, 192, 0);
                Thread.Sleep(1000);
                dryStatus.BackColor = Color.White;
            }
        }

        private void generateWetTagList()
        {
            int Tags = 0;
            currentTagList.Clear();
            lstWetTags.Items.Clear();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Display Wet Tags
            for (int idx = 0; idx < newAllTagInformation.Count; idx++)
            {
                if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg< CEILING_WET & newAllTagInformation.ElementAt(idx).Value.SCDEAvg != 0)
                {
                    currentTagList.Add(new ListViewItem(newAllTagInformation.ElementAt(idx).Key));
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.TimesRead.ToString());
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.Lat.ToString());
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.Lon.ToString());
                    currentTagList[Tags].SubItems.Add("Wet (" + newAllTagInformation.ElementAt(idx).Value.SCDEAvg.ToString() + ")");
                    lstWetTags.Invoke(new MethodInvoker(delegate { lstWetTags.Items.Add(currentTagList[Tags]); }));
                    Tags++;
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Display Damp Tags
            /*
            for (int idx = 0; idx < newAllTagInformation.Count; idx++)
            {
                if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg > WET & newAllTagInformation.ElementAt(idx).Value.SCDEAvg < DRY & newAllTagInformation.ElementAt(idx).Value.SCDEAvg != 0) 
                {
                    currentTagList.Add(new ListViewItem(newAllTagInformation.ElementAt(idx).Key));
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.TimesRead.ToString());
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.Lat.ToString());
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.Lon.ToString());
                    currentTagList[Tags].SubItems.Add("Damp (" + newAllTagInformation.ElementAt(idx).Value.SCDEAvg.ToString() + ")");
                    lstWetTags.Invoke(new MethodInvoker(delegate { lstWetTags.Items.Add(currentTagList[Tags]); }));
                    Tags++;
                }
            }
            */
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Display Dry Tags // Will not be used in current version, leaving code here if Wet and Dry tags are 
            /*
            for (int idx = 0; idx < newAllTagInformation.Count; idx++)
            {
                if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg  > DRY || newAllTagInformation.ElementAt(idx).Value.SCDEAvg  == 0) //Display Dry Tags
                {
                    currentTagList.Add(new ListViewItem(newAllTagInformation.ElementAt(idx).Key));
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.TimesRead.ToString());
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.Lat.ToString());
                    currentTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.Lon.ToString());
                    currentTagList[Tags].SubItems.Add("Dry (" + newAllTagInformation.ElementAt(idx).Value.SCDEAvg.ToString() + ")");
                    lstWetTags.Invoke(new MethodInvoker(delegate { lstWetTags.Items.Add(currentTagList[Tags]); }));
                    Tags++;
                }
            }
            */

        }

        private void detailedListview()
        {
            while (true) //Continually sit in thread, but block until needed.
            {
                mredetailedView.WaitOne();
                mredetailedView.Reset();
                if (programEnding)
                    break;
                int Tags = 0;
                int[] values;

                if (DEBUG_VIEW)
                    values = new int[] { TAGINFO_RDCT, TAGINFO_TSSI, TAGINFO_SCDE, TAGINFO_WTDY, TAGINFO_SCAV, TAGINFO_TEMP };
                else
                    values = new int[] { TAGINFO_RDCT, TAGINFO_WTDY, TAGINFO_SCAV, TAGINFO_TEMP };

                detailedTagList.Clear();
                detailedTagView.Invoke(new MethodInvoker(delegate { detailedTagView.Items.Clear(); }));

                while (currentlyReading)
                {
                    lock (allTagInfoLock)
                    {
                        detailedTagView.Invoke(new MethodInvoker(delegate
                        {
                            Tags = 0;
                            detailedTagList.Clear();
                            detailedTagView.Items.Clear();

                            if (newAllTagInformation.Count != detailedTagView.Items.Count)
                            {
                                for (int idx = detailedTagView.Items.Count; idx < newAllTagInformation.Count; idx++)
                                {
                                    // Only display tags where TSSI/Sensor Code have been read
                                    if (newAllTagInformation.ElementAt(idx).Value.TimesRead == 0)
                                        continue;

                                    ListViewItem listViewItem = new ListViewItem(newAllTagInformation.ElementAt(idx).Key);
                                    listViewItem.Name = newAllTagInformation.ElementAt(idx).Key;

                                    detailedTagList.Add(listViewItem);
                                    if(cbTR.Checked)
                                    {
                                        if (newAllTagInformation.ElementAt(idx).Value.TimesRead == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else
                                            detailedTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.TimesRead.ToString());
                                    }

                                    if(cbTSSI.Checked && DEBUG_VIEW)
                                    {
                                        if (newAllTagInformation.ElementAt(idx).Value.TSSI == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else
                                            detailedTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.TSSI.ToString());
                                    }
                                        
                                    if(cbTSSIAvg.Checked && DEBUG_VIEW)
                                    {
                                        if (newAllTagInformation.ElementAt(idx).Value.TSSIAvg == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else
                                        {
                                            int tssiAvg = (int)newAllTagInformation.ElementAt(idx).Value.TSSIAvg;
                                            detailedTagList[Tags].SubItems.Add(tssiAvg.ToString());
                                        }
                                    }

                                    if (cbSCDE.Checked && DEBUG_VIEW)
                                    {
                                        if (newAllTagInformation.ElementAt(idx).Value.SCDE == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else
                                        {
                                            int SCDEval = (int)newAllTagInformation.ElementAt(idx).Value.SCDE;
                                            detailedTagList[Tags].SubItems.Add(SCDEval.ToString());
                                        }
                                    }

                                    if (cbSCDEAvg.Checked)
                                    {
                                        if (newAllTagInformation.ElementAt(idx).Value.SCDEAvg == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else
                                            detailedTagList[Tags].SubItems.Add((String.Format("{0:0.0}", newAllTagInformation.ElementAt(idx).Value.SCDEAvg).ToString()));
                                    }

                                    if (cbSTdDev.Checked && DEBUG_VIEW)
                                    {
                                        String stdDev = String.Format("{0:0.0}", newAllTagInformation.ElementAt(idx).Value.SCDEStdDev);
                                        detailedTagList[Tags].SubItems.Add(stdDev.ToString());
                                    }
                                        


                                    //int scdeAvg = (int)newAllTagInformation.ElementAt(idx).Value.SCDEAvg;
                                    //detailedTagList[Tags].SubItems.Add(scdeAvg.ToString());                                    //int scdeAvg = (int)newAllTagInformation.ElementAt(idx).Value.SCDEAvg;
                                    //detailedTagList[Tags].SubItems.Add(scdeAvg.ToString());


                                    // Wet/Dry indicator
                                    if (cbTS.Checked)
                                    {
                                        int scdeAvg = (int)newAllTagInformation.ElementAt(idx).Value.SCDEAvg;

                                        if (scdeAvg == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else if (scdeAvg <= CEILING_WET)
                                            detailedTagList[Tags].SubItems.Add("WET");
                                        else if (scdeAvg <= CEILING_DAMP)
                                            detailedTagList[Tags].SubItems.Add("DAMP");
                                        else
                                            detailedTagList[Tags].SubItems.Add("DRY");
                                    }


                                    //if (allTagInformation.ElementAt(idx).Value[7] == 0)
                                    //    detailedTagList[Tags].SubItems.Add("-");
                                    //else
                                    //    detailedTagList[Tags].SubItems.Add(allTagInformation.ElementAt(idx).Value[7].ToString());

                                    //if (allTagInformation.ElementAt(idx).Value[8] == 0)
                                    //    detailedTagList[Tags].SubItems.Add("-");
                                    //else
                                    //    detailedTagList[Tags].SubItems.Add(allTagInformation.ElementAt(idx).Value[8].ToString());

                                    

                                    if(cbTemp.Checked)
                                    {
                                        if (newAllTagInformation.ElementAt(idx).Value.TEMP == 0)
                                            detailedTagList[Tags].SubItems.Add("-");
                                        else
                                            detailedTagList[Tags].SubItems.Add(newAllTagInformation.ElementAt(idx).Value.TEMP.ToString());
                                    }


                                    detailedTagView.Items.Add(detailedTagList[Tags]);
                                    Tags++;
                                }
                            }

                            //for (int idx = 0; idx < detailedTagView.Items.Count; idx++)
                            //{
                            //    for (int idc = 1; idc < detailedTagList[idx].SubItems.Count; idc++)
                            //    {
                            //        if ((detailedTagList[idx].SubItems[idc].Text != allTagInformation.ElementAt(idx).Value[values[idc - 1]].ToString()) &&
                            //                (allTagInformation.ElementAt(idx).Value[values[idc - 1]] != 0))
                            //        {
                            //            detailedTagList[idx].SubItems[idc].Text = ((int)allTagInformation.ElementAt(idx).Value[values[idc - 1]]).ToString();
                            //        }

                            //        if (idc == 4)
                            //        {
                            //            int scdeAvg = (int)allTagInformation.ElementAt(idx).Value[TAGINFO_SCAV];

                            //            if (scdeAvg == 0)
                            //                detailedTagList[idx].SubItems[idc].Text = "-";
                            //            else if (scdeAvg <= CEILING_WET)
                            //                detailedTagList[idx].SubItems[idc].Text = "WET";
                            //            else if (scdeAvg <= CEILING_DAMP)
                            //                detailedTagList[idx].SubItems[idc].Text = "DAMP";
                            //            else
                            //                detailedTagList[idx].SubItems[idc].Text = "DRY";
                            //        }
                            //    }
                            //}
                        }));
                    }

                    Thread.Sleep(300);//small pause so we dont look glitchy
                }
            }
        }

        //Utilities and Setup///////////////////////////////////////////////////////////////////////////

        //private void connectedReaderChecker()
        //{
        //    bool found = false;
        //    for (int idx = 120; idx > 0; idx--)
        //    {
        //        if (currentReaders.ContainsKey(connectedReaderMAC))
        //        {
        //            reader = new RFIDEngineeringReader(currentReaders[connectedReaderMAC][1], 5000);
        //            found = true;
        //            readerConnected = true;
        //            main.Invoke(new MethodInvoker(delegate { main.BringToFront(); }));
        //            break;
        //        }
        //        else
        //        {
        //            if (!programEnding)
        //            {
        //                Thread.Sleep(1000);
        //                if (!programEnding)
        //                {
        //                    try
        //                    {
        //                        numberTimeRemaining.Invoke(new MethodInvoker(delegate { numberTimeRemaining.Text = idx.ToString(); }));
        //                    }
        //                    catch (Exception) { };
        //                }

        //            }
        //        }
        //    }
        //    if (!found && !programEnding)
        //        admin.Invoke(new MethodInvoker(delegate { admin.BringToFront(); }));

        //}

        private void checkConnection()
        {
            string readerName = "";
            while (!programEnding)
            {
                if (readerConnected && !currentlyReading)
                {
                    try
                    {
                        reader.GetReaderName(out readerName);
                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        DialogResult results = MessageBox.Show("Connection to reader has been lost", "Connection Lost", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (results.Equals(DialogResult.OK))
                        {
                            admin.Invoke(new MethodInvoker(delegate { admin.BringToFront(); }));
                        }
                        readerConnected = false;
                    }
                }
            }
        } //not using yet/Doesnt work yet

        private void readTSSI()
        {
            if (CONSOLE_DEBUG)
                Console.WriteLine("TSSI: ");

            byte[] data = new byte[8];

            byte[] select = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x30, 0x00, this.Session, QueryTarget_TSSI, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Copy(select, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set TSSI Query Target", select[5], data);

            Array.Clear(data, 0, 8);

            byte[] TSSI = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0xe2, 0x01, (Byte)this.tagModel, 0x00, 0x01, 0x00, 0x00, 0x03, 0x00, 0x88, 0xd4 };
            Array.Copy(TSSI, 6, data, 0, 8);
            GetAndDisplayAccessCommandResults(TSSI[5], data);
        }

        private void readSensorCode()
        {
            if (CONSOLE_DEBUG)
                Console.WriteLine("SCDE: ");

            byte[] data = new byte[8];

            byte[] select = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x30, 0x00, this.Session, QueryTarget_SCDE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Copy(select, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set SCDE Query Target", select[5], data);

            Array.Clear(data, 0, 8);

            byte[] sensorCode = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0xe2, 0x02, (Byte)this.tagModel, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x88, 0xd4 };
            Array.Copy(sensorCode, 6, data, 0, 8);
            GetAndDisplayAccessCommandResults(sensorCode[5], data);
        }

        private void readTempValue()
        {
            if (CONSOLE_DEBUG)
                Console.WriteLine("TEMP: ");

            byte[] data = new byte[8];
            byte[] sensorCode = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0xe2, 0x00, (Byte)this.tagModel, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x88, 0xd4 };
            Array.Copy(sensorCode, 6, data, 0, 8);
            GetAndDisplayAccessCommandResults(sensorCode[5], data);
        }

        private void ChangePower(int powerIndex, int antenna)
        {
            byte[] powerLevelSet = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x12, 0x00, 0x2C, 0x01, 0x00, 0x00, 0x00, 0x20, 0x03, 0x3d, 0x91 };
            byte[] data = new byte[8];
            int selectedPower = 0;

            selectedPower = (int)(powerArray[powerIndex] * 10);

            powerLevelSet[7] = (byte)(selectedPower & 0xFF);
            powerLevelSet[8] = (byte)((selectedPower >> 8) & 0xFF);

            powerLevelSet[13] = (byte)antenna;

            Array.Copy(powerLevelSet, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set Power level", powerLevelSet[5], data);
        }

        private void ChangeFrequency(int frequencyIndex)
        {
            byte[] setSingleFrequencyCommand = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x82, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3d, 0x91 };
            byte[] data = new byte[8];

            int frequency = this.frequencyTable[frequencyIndex];

            setSingleFrequencyCommand[7] = (Byte)(frequency & 0x00FF);
            setSingleFrequencyCommand[8] = (Byte)((frequency >> 8) & 0x00FF);
            setSingleFrequencyCommand[9] = (Byte)((frequency >> 16) & 0x00FF);
            setSingleFrequencyCommand[10] = (Byte)((frequency >> 24) & 0x00FF);

            Array.Copy(setSingleFrequencyCommand, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set active frequency", setSingleFrequencyCommand[5], data);
        }

        private void setupReader()
        {
            byte[] hopFrequency = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x82, 0x00, 0x32, 0xF7, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // Not currently used. Can be used to select a particular frequency
            byte[] Antenna1Active = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] queryGroup = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x30, 0x00, this.Session, this.QueryTarget_TSSI, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            //byte[] gen2Settings = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x34, 0x00, 0x03, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] gen2Settings = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x34, 0x00, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] shortTagData = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // Will not be used, but this will allow for changing to the shorter tag data
            byte[] extendedTagData = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x0E, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] endPacketDelay = { 0x43, 0x49, 0x54, 0x4d, 0xff, 0x06, 0x06, 0x02, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // 2 ms End-of-packet delay.
            byte[] data = new byte[8];


            Array.Copy(extendedTagData, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Activate extended tag info reporting", extendedTagData[5], data);
            Array.Clear(data, 0, 8);

            Array.Copy(Antenna1Active, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set active antenna", Antenna1Active[5], data);
            Array.Clear(data, 0, 8);

            Array.Copy(queryGroup, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set Query Target", queryGroup[5], data);
            Array.Clear(data, 0, 8);

            Array.Copy(gen2Settings, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set Gen2 settings", gen2Settings[5], data);
            Array.Clear(data, 0, 8);

            Array.Copy(endPacketDelay, 6, data, 0, 8);
            GetAndDisplayRegisterContents("Set End Packet delay", endPacketDelay[5], data);
            Array.Clear(data, 0, 8);
        }

        private void GetAndDisplayRegisterContents(String commandInfo, byte command, byte[] data)
        {
            byte[] registerContents;

            if (command == 0x12) // Set Power
                currentPowerLevel = ((double)((data[2] << 8) | data[1])) / 10.0;

            try
            {
                if (CONSOLE_DEBUG)
                    Console.WriteLine("{2}: {0:X} {1}", command, BitConverter.ToString(data), commandInfo);
                reader.GetRegisterContents(command, data, out registerContents);
                if (CONSOLE_DEBUG)
                    Console.WriteLine(BitConverter.ToString(registerContents));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting register content " + commandInfo + Environment.NewLine + ex);
            }
        }

        private void GetAndDisplayAccessCommandResults(byte command, byte[] data)
        {
            byte[] registerContents = new byte[1];
            Boolean isGPTag = true;
            Boolean exists = false;
            Boolean newSession = false;
            Int64 currentTagList = 0;
            String dataType;

            SoundPlayer tagBeep = new SoundPlayer("beep-08b.wav");
            string EPC = "";

            try
            {
                if (CONSOLE_DEBUG)
                    Console.WriteLine("{0:X} {1}", command, BitConverter.ToString(data));
                reader.GetRegisterContents(command, data, out registerContents);
                if (CONSOLE_DEBUG)
                    Console.WriteLine(BitConverter.ToString(registerContents));
            }
            catch (Exception ex)
            {
                Console.WriteLine("error getting register content " + ex);
            }

            switch (data[0])
            {
                case 0x00:
                    dataType = "TEMP";
                    break;
                case 0x01:
                    dataType = "TSSI";
                    break;
                case 0x02:
                    dataType = "SCDE";
                    break;
                default:
                    dataType = "OTHR";
                    break;
            }
            try
            {
                while (registerContents[0] != 'E')
                {
                    reader.GetAccessResponse(out registerContents);
                    if (CONSOLE_DEBUG)
                        Console.WriteLine(BitConverter.ToString(registerContents));

                    if (registerContents[6] != 0xF0)
                    {
                        int startIndex, endIndex;

                        switch ((char)registerContents[0])
                        {
                            case 'I':
                                if ((registerContents[7] & 0x08) != 0)
                                {
                                    startIndex = 36;
                                    endIndex = 48;
                                }
                                else
                                {
                                    startIndex = 28; endIndex = 40;
                                }

                                EPC = BitConverter.ToString(registerContents, startIndex, endIndex - startIndex);
                                // blank spaces for tagInformation dictionary (TSSI, SCDE, TEMP, RSSI, SCDEAvg, TimesRead, TotalSCDEValue, Lat, Lon, freq_TSSI, freq_SCDE)
                                //double[] tempValue = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                TagObject tempValObj = new TagObject();

                                if (isGPTag)
                                {

                                    tagReadStatus.BackColor = Color.FromArgb(0, 192, 0);
                                    if (!newTagInformation.ContainsKey(EPC)) // Add new tags to the dictionary
                                    {
                                        newTagInformation.Add(EPC, tempValObj);
                                        
                                    }
                                    if(!newTagInformation.ContainsKey(EPC))
                                    {
                                        TagObject tagObject = new TagObject();
                                        newTagInformation.Add(EPC, tagObject);
                                    }
                                        

                                    int rssiEncoded = registerContents[18];
                                    double mantissa = rssiEncoded & 0x07;
                                    double exponent = (rssiEncoded >> 3) & 0x1F;
                                    double nbRSSI = 20.0 * Math.Log10(Math.Pow(2.0, exponent) * (1.0 + mantissa / 8.0));
                                    //newTagInformation[EPC][TAGINFO_RSSI] = (int)nbRSSI; // Collecting the RSSI for potential future usage
                                    newTagInformation[EPC].RSSI = (int)nbRSSI;
                                    if (currentPowerLevel == 33.0 && currentReaders[connectedReaderMAC][4] != "")
                                    {
                                        //tagInformation[EPC][7] = int.Parse(currentReaders[connectedReaderMAC][4]);
                                        //tagInformation[EPC][8] = int.Parse(currentReaders[connectedReaderMAC][5]);
                                        newTagInformation[EPC].Lat = int.Parse(currentReaders[connectedReaderMAC][4]);
                                        newTagInformation[EPC].Lon = int.Parse(currentReaders[connectedReaderMAC][5]);
                                    }

                                    if (dataType == "TSSI")
                                    {
                                        //tagInformation[EPC][TAGINFO_FRQ1] = (double)(registerContents[33] << 24 | registerContents[32] << 16 |
                                        //                                             registerContents[31] << 8 | registerContents[30]);
                                        newTagInformation[EPC].freq_TSSI = (double)(registerContents[33] << 24 | registerContents[32] << 16 |
                                                                                     registerContents[31] << 8 | registerContents[30]);
                                    }
                                    else if (dataType == "SCDE")
                                    {
                                        //tagInformation[EPC][TAGINFO_FRQ2] = (double)(registerContents[33] << 24 | registerContents[32] << 16 |
                                        //                                             registerContents[31] << 8 | registerContents[30]);
                                        newTagInformation[EPC].freq_SCDE = (double)(registerContents[33] << 24 | registerContents[32] << 16 |
                                                                                     registerContents[31] << 8 | registerContents[30]);
                                    }
                                }

                                break;

                            case 'A':
                                int value = (registerContents[26] << 8) | registerContents[27];

                                if (value >= 512 && dataType != "TEMP")
                                {
                                    Console.WriteLine("===================================> Value ({0}) >= 512, Query type: {1}", value, dataType);
                                    value = 0;
                                }

                                if (isGPTag)
                                {
                                    try
                                    {
                                        if (dataType == "TEMP")
                                        {
                                            //tagInformation[EPC][TAGINFO_TEMP] = value / 100; //data not currently be collected, but this may be wanted in the future so I'm leaving room for it.
                                            newTagInformation[EPC].TEMP = value / 100;
                                        }
                                        else if (dataType == "SCDE")
                                        {
                                            if ((newTagInformation[EPC].TSSI >= TSSI_MIN) && (newTagInformation[EPC].TSSI <= TSSI_MAX))
                                            {
                                                if (tagModel == TagModel.Magnus2)
                                                    value = value << 4;

                                                //tagInformation[EPC][TAGINFO_SCDE] = value; //SCDE value
                                                //tagInformation[EPC][TAGINFO_CUMM] += value; //Total SCDE Value for Average

                                                newTagInformation[EPC].SCDEraw = value;


                                                if (newTagInformation[EPC].SCDEraw != 0)
                                                {
                                                    newTagInformation[EPC].SCDE = calculateFreqCorrection(newTagInformation[EPC]);
                                                    newTagInformation[EPC].TotalSCDEValue += value;
                                                    newTagInformation[EPC].TimesRead++;        //tagInformation[EPC][TAGINFO_SCAV] = (int)(tagInformation[EPC][TAGINFO_CUMM] / tagInformation[EPC][5]); //Average Value for SCDE

                                                    // Do the Sensor Code averaging using an exponential moving average.
                                                    // To get faster convergence, seed with the first value received rather
                                                    // try to build up from a base value of 0.

                                                    if (newTagInformation[EPC].SCDEAvg == 0)
                                                    {
                                                        //tagInformation[EPC][TAGINFO_SCAV] = value;
                                                        newTagInformation[EPC].SCDEAvg = value;
                                                    }
                                                    else
                                                    {
                                                        //tagInformation[EPC][TAGINFO_SCAV] = value * k + tagInformation[EPC][TAGINFO_SCAV] * (1.0 - k);
                                                        newTagInformation[EPC].SCDEAvg = value * k + newTagInformation[EPC].SCDEAvg * (1.0 - k);
                                                    }
                                                        

                                                    AddReadingToDatabase(ref exists, newSession, ref currentTagList, EPC);
                                                }

                                                moistureStatus.BackColor = Color.FromArgb(0, 192, 0);

                                                if (newTagInformation[EPC].SCDEAvg < CEILING_WET && newTagInformation[EPC].SCDEAvg != 0)
                                                    mreDisplayWet.Set();
                                                else if (newTagInformation[EPC].SCDEAvg > CEILING_DAMP)
                                                    mreDisplayDry.Set();
                                                else
                                                    mreDisplayDamp.Set();
                                            }
                                        }
                                        else
                                        {
                                            //tagInformation[EPC][TAGINFO_TSSI] = value;
                                            newTagInformation[EPC].TSSI = value;

                                            // Check if any of the tags have a TSSI that is in the correct range, 
                                            // if so raise a flag to read the Sensor code on the next pass
                                            if ((value >= TSSI_MIN) && (value <= TSSI_MAX))
                                                goodTSSI = true;
                                            else
                                                goodTSSI = false;
                                        }
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        Console.WriteLine("*********** Dictionary entry not found. Primary inventory record missing *************");
                                    }

                                    EPC = "";
                                }

                                break;

                            case 'B':
                                //Console.WriteLine("Begin packet rcvd");
                                break;

                            case 'E':
                                //Console.WriteLine("End packet rcvd");
                                break;

                            default:
                                //startIndex = 0; endIndex = registerContents.Length;
                                //for (int idx = startIndex; idx < endIndex; idx++)
                                //    Console.Write("0x{0:X2},", registerContents[idx]);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading tag: " + ex);
            }
        }

        private void AddReadingToDatabase(ref bool exists, bool newSession, ref long currentTagList, string EPC)
        {
            string SQLCommand;
            using (SQLiteConnection con = new SQLiteConnection("data source=tagDatabaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        con.Open(); // Open the connection to the database
                        SQLCommand = "SELECT * FROM ScanTable ORDER BY ID DESC LIMIT 1";
                        com.CommandText = SQLCommand;
                        com.ExecuteNonQuery();
                        using (SQLiteDataReader tableReader = com.ExecuteReader())
                        {
                            while (tableReader.Read())
                            {
                                currentTagList = (long)tableReader["Id"];
                            }
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Console failed to Read Current Tag List " + ex);
                    }

                    try
                    {
                        try
                        {
                            SQLCommand = "SELECT * FROM TagInfoTable WHERE ScanTableID = " + currentTagList +
                                                " AND EPC = '" + EPC + "'";
                            com.CommandText = SQLCommand;
                            com.ExecuteNonQuery();
                            using (SQLiteDataReader tableReader = com.ExecuteReader())
                            {
                                exists = tableReader.Read();
                                exists = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            Console.WriteLine("Console failed to write TagInfoTable entry " + ex);
                        }

                        if (!exists || newSession)
                        {
                            SQLCommand = @"INSERT INTO TagInfoTable (ScanTableID, EPC, TSSI, SCDE, Temperature, RSSI, SCDEAvg, TimesRead, TotalSCDEValue, Latitude, Longitude) 
                                                                    VALUES ( " + currentTagList + ", '" + EPC + "', " + newTagInformation[EPC].TSSI + ", " + newTagInformation[EPC].SCDE + ", " + newTagInformation[EPC].TEMP + ", " +
                            newTagInformation[EPC].RSSI + ", " + newTagInformation[EPC].SCDEAvg + ", " + newTagInformation[EPC].TimesRead + ", " + newTagInformation[EPC].TotalSCDEValue + ", " +
                            newTagInformation[EPC].Lat + ", " + newTagInformation[EPC].Lon + ")";

                            try
                            {
                                com.CommandText = SQLCommand;
                                com.ExecuteNonQuery();
                            }
                            catch (SQLiteException ex)
                            {
                                Console.WriteLine("Console failed to write TagInfoTable entry " + ex);
                            }
                        }
                        else
                        {
                            SQLCommand = @"UPDATE TagInfoTable 
                                                                SET TSSI = " + newTagInformation[EPC].TSSI + ", " + "SCDE = " +
                            newTagInformation[EPC].SCDE + ", Temperature = " + newTagInformation[EPC].TEMP +
                            ", RSSI = " + newTagInformation[EPC].RSSI + ", SCDEAvg = " + newTagInformation[EPC].SCDEAvg +
                            ", TimesRead = " + newTagInformation[EPC].TimesRead + ", TotalSCDEValue = " +
                            newTagInformation[EPC].TotalSCDEValue + ", Latitude = '" + newTagInformation[EPC].Lat + "', Longitude = '" +
                            newTagInformation[EPC].Lon + "'" +
                            "WHERE EPC = '" + EPC + "'" + "AND ScanTableID = " + currentTagList;
                            try
                            {
                                com.CommandText = SQLCommand;
                                com.ExecuteNonQuery();
                            }
                            catch (SQLiteException ex)
                            {
                                Console.WriteLine("Console failed to write TagInfoTable entry " + ex);
                            }
                        }

                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Console failed to write search TagInfoTable entry " + ex);
                    }


                    con.Close();
                }
            }
        }

        private void MSRC_FormClosing(object sender, FormClosingEventArgs e)
        {
            heartbeatListener.Stop();

            programEnding = true; //provide flag to threads that the program is terminating
            //allow all threads to resume and exit gracefully...
            mreFind.Set();
            mreDisplayWet.Set();
            mreDisplayDry.Set();
            mreDisplayDamp.Set();
            mreUpdateMap.Set();
            mredetailedView.Set();

            Properties.Settings.Default["MACAddress"] = connectedReaderMAC;
            Properties.Settings.Default.Save();
        }

        private void SerializeTagList()
        {
            Stream tagListStream = null;

            try
            {
                tagListStream = File.Create(tagListFileName);

                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(tagListStream, newAllTagInformation);
                tagListStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialized Tag List Error: " + ex.Message);
            }
            finally
            {
                if (tagListStream != null)
                    tagListStream.Close();
            }
        }

        private void DeserializeTagList()
        {
            Stream tagListStream = null;

            try
            {
                tagListStream = File.OpenRead(tagListFileName);

                BinaryFormatter deserializer = new BinaryFormatter();
                newTagInformation = (Dictionary<string, TagObject>)deserializer.Deserialize(tagListStream);
                tagListStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialize Tag List Error: " + ex.Message);
            }
            finally
            {
                if (tagListStream != null)
                    tagListStream.Close();
            }
        }

        private void SerializeCompanyInformation()
        {
            Stream companyListStream = null;

            try
            {
                companyListStream = File.Create(companyFileName);

                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(companyListStream, companyInformation);
                companyListStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialized Company Information Error: " + ex.Message);
            }
            finally
            {
                if (companyListStream != null)
                    companyListStream.Close();
            }
        }

        private void DeserializeCompanyInformation()
        {
            Stream companyListStream = null;

            try
            {
                companyListStream = File.OpenRead(companyFileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                companyInformation = (Dictionary<string, string[]>)deserializer.Deserialize(companyListStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization Company Information Error: " + ex.Message);
            }
            finally
            {
                if (companyListStream != null)
                    companyListStream.Close();
            }
        }

        private void CreateDatabase()
        {
            string createScanTable = @"CREATE TABLE IF NOT EXISTS ScanTable(
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
                                        , Start text NOT NULL
                                        , Stop text NOT NULL
                                        , Company bigint NOT NULL 
                                        , FOREIGN KEY(Company) REFERENCES CompanyTable(Id)  
                                        )";

            string createCompanyTable = @"CREATE TABLE IF NOT EXISTS CompanyTable (
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
                                        , CompanyName text NOT NULL
                                        , BuildingNumber text NOT NULL
                                        , Address text NOT NULL
                                        , City text NOT NULL
                                        , State text NOT NULL
                                        , ZipCode text NOT NULL
                                        )";

            string createTagInfoTable = @"CREATE TABLE IF NOT EXISTS TagInfoTable (
                                          Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
                                        , ScanTableID bigint NOT NULL
                                        , EPC text NOT NULL
                                        , TSSI bigint NOT NULL
                                        , SCDE bigint NOT NULL
                                        , Temperature bigint NOT NULL
                                        , RSSI bigint NOT NULL
                                        , SCDEAvg bigint NOT NULL
                                        , TimesRead bigint NOT NULL
                                        , TotalSCDEValue bigint NOT NULL
                                        , Latitude text NOT NULL
                                        , Longitude text NOT NULL
                                        , FOREIGN KEY(ScanTableID) REFERENCES ScanTable(Id)
                                        )";

            if (!File.Exists("tagDatabaseFile.db3"))
            {
                SQLiteConnection.CreateFile("tagDatabaseFile.db3");
            }

            using (SQLiteConnection con = new SQLiteConnection("data source=tagDatabaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        con.Open();                             // Open the connection to the database


                        com.CommandText = createCompanyTable;
                        com.ExecuteNonQuery();
                        com.CommandText = createScanTable;     // Set CommandText to our query that will create the table
                        com.ExecuteNonQuery();
                        com.CommandText = createTagInfoTable;
                        com.ExecuteNonQuery();

                        con.Close();        // Close the connection to the database
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Console failed to launch new database " + ex);
                    }

                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Antenna2 = checkBox2.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Antenna1 = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Antenna3 = checkBox3.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Antenna4 = checkBox4.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnDetailedView_Click(object sender, EventArgs e)
        {
            DetermineColumns();
            tagListDisplay.BringToFront();
        }

        private void DetermineColumns()
        {
            int idx = 1;
            if (!cbTR.Checked)
                detailedTagView.Columns.Remove(colTR_1);
            else if (detailedTagView.Columns.Contains(colTR_1))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colTR_1);

            if (!cbTSSI.Checked)
                detailedTagView.Columns.Remove(colTSSI);
            else if (detailedTagView.Columns.Contains(colTSSI))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colTSSI);

            if (!cbTSSIAvg.Checked)
                detailedTagView.Columns.Remove(colTSSIAvg);
            else if (detailedTagView.Columns.Contains(colTSSIAvg))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colTSSIAvg);

            if (!cbSCDE.Checked)
                detailedTagView.Columns.Remove(colSCDE);
            else if (detailedTagView.Columns.Contains(colSCDE))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colSCDE);

            if (!cbSCDEAvg.Checked)
                detailedTagView.Columns.Remove(colSCDEAvg);
            else if (detailedTagView.Columns.Contains(colSCDEAvg))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colSCDEAvg);
                

            if (!cbSTdDev.Checked)
                detailedTagView.Columns.Remove(colStdDev);
            else if (detailedTagView.Columns.Contains(colStdDev))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colStdDev);

            if (!cbTS.Checked)
                detailedTagView.Columns.Remove(colTS);
            else if (detailedTagView.Columns.Contains(colTS))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colTS);

            if (!cbTemp.Checked)
                detailedTagView.Columns.Remove(colTemp);
            else if (detailedTagView.Columns.Contains(colTemp))
                idx++;
            else
                detailedTagView.Columns.Insert(idx++, colTemp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MapRoof.BringToFront();
        }

        private void maxPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.starting)
                return;

            if (cbxMaxPower.SelectedIndex > cbxMinPower.SelectedIndex) // if the index is higher the number is lower so we flag this as an error to user
            {
                MessageBox.Show("Maximum cannot be less than the minimum power level", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxMinPower.SelectedIndex = cbxMaxPower.SelectedIndex;
            }

            Properties.Settings.Default.PowerMaxIndex = cbxMaxPower.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void minPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.starting)
                return;

            if (cbxMaxPower.SelectedIndex > cbxMinPower.SelectedIndex) // if the index is higher the number is lower so we flag this as an error to user
            {
                MessageBox.Show("Maximum cannot be less than the minimum power level", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxMinPower.SelectedIndex = cbxMaxPower.SelectedIndex;
            }

            Properties.Settings.Default.PowerMinIndex = cbxMinPower.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void btnCalInfo_Click(object sender, EventArgs e)
        {
            richTextBox1.LoadFile("test.rtf");
            panelCalInfo.BringToFront();
        }

        private void nudTSSIMax_ValueChanged(object sender, EventArgs e)
        {
            if (nudTSSIMax.Value < nudTSSIMin.Value) // if the index is higher the number is lower so we flag this as an error to user
            {
                MessageBox.Show("Maximum cannot be less than the minimum power level", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nudTSSIMin.Value = nudTSSIMax.Value;
            }

            Properties.Settings.Default.TssiMaxIndex = (int)nudTSSIMax.Value;
            Properties.Settings.Default.Save();
        }

        private void nudTSSIMin_ValueChanged(object sender, EventArgs e)
        {
            if (nudTSSIMax.Value < nudTSSIMin.Value) // if the index is higher the number is lower so we flag this as an error to user
            {
                MessageBox.Show("Maximum cannot be less than the minimum power level", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nudTSSIMin.Value = nudTSSIMax.Value;
            }

            Properties.Settings.Default.TssiMinIndex = (int)nudTSSIMin.Value;
            Properties.Settings.Default.Save();
        }

        private void radMagnusS2_CheckedChanged(object sender, EventArgs e)
        {
            if (radMagnusS2.Checked)
            {
                tagModel = TagModel.Magnus2;
                Properties.Settings.Default.TagType = (int)TagModel.Magnus2;
                Properties.Settings.Default.Save();
            }
        }

        private void radMagnusS3_CheckedChanged(object sender, EventArgs e)
        {
            if (radMagnusS3.Checked)
            {
                tagModel = TagModel.Magnus3;
                Properties.Settings.Default.TagType = (int)TagModel.Magnus3;
                Properties.Settings.Default.Save();
            }
        }

        private void ReadTimeTimer_Tick(Object obj)
        {
            this.Invoke(new MethodInvoker(delegate 
            {
                int elapsedTime = int.Parse(this.lblReportedElapsedTime.Text);
                int requestedTime = int.Parse(this.txtRequestedReadTime.Text);

                if (requestedTime > 0 && elapsedTime >= requestedTime)
                {
                    this.btnPauseStartReading_Click(null, null);
                }
                else
                    this.lblReportedElapsedTime.Text = (elapsedTime + 1).ToString();
            }));
        }

        private void cbTR_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TimesRead = cbTR.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbTSSI_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TSSI = cbTSSI.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbTSSIAvg_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TSSIAvg = cbTSSIAvg.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbSCDE_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SCDE = cbSCDE.Checked;
            Properties.Settings.Default.Save();
        }

        private void SCDEAvg_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SCDEAvg = cbSCDEAvg.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbSTdDev_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.StdDev = cbSTdDev.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbTS_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TagState = cbTS.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbTemp_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Temperature = cbTemp.Checked;
            Properties.Settings.Default.Save();
        }

        private void detailedTagView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        selectAllToolStripMenuItem_Click(null, null);
                        break;

                    case Keys.C:
                        copyCtrlCToolStripMenuItem_Click(null, null);
                        break;
                }
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.detailedTagView.Items)
            {
                item.Selected = true;
            }

            this.detailedTagView.Select();
        }

        private void copyCtrlCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder buffer = new StringBuilder();

            foreach (ColumnHeader ch in this.detailedTagView.Columns)
                buffer.Append(ch.Text + "\t");
            buffer.Remove(buffer.Length - 1, 1);

            foreach (ListViewItem item in this.detailedTagView.Items)
            {
                buffer.Append("\n");
                for (int idx = 0; idx < item.SubItems.Count - 1; idx++)
                {
                    buffer.Append(item.SubItems[idx].Text);
                    buffer.Append("\t");
                }

                buffer.Append(item.SubItems[item.SubItems.Count - 1].Text);
            }

            Clipboard.SetText(buffer.ToString());
        }
    }
}

public class TagObject
{
    public double TSSI, TSSIAvg, SCDE, SCDEraw, SCDEAvg, TEMP, RSSI, SCDEStdDev, TimesRead, TotalSCDEValue, Lat, Lon, freq_TSSI, freq_SCDE;
    public double FreqCorNumerator, FreqCorDenominator, PowerCorDenominator, MidFreq;
    public LinkedList<double> SCDEValues;
    public LinkedList<double> TSSIValues;


     

    public TagObject()
    {
        this.TSSI           = 0;
        this.TSSIAvg        = 0;
        this.SCDE           = 0;
        this.TEMP           = 0;
        this.RSSI           = 0;
        this.SCDEAvg        = 0;
        this.SCDEStdDev     = 0;
        this.TimesRead      = 0;
        this.TotalSCDEValue = 0;
        this.Lat            = 0;
        this.Lon            = 0;
        this.freq_TSSI      = 0;
        this.freq_SCDE      = 0;
        this.FreqCorNumerator = 45;
        this.FreqCorDenominator = 11500;
        this.PowerCorDenominator = 325;
        this.MidFreq = 913250;
        this.SCDEValues     = new LinkedList<double>();
        this.TSSIValues     = new LinkedList<double>();
    }
}
public class ProgressBarEx : ProgressBar
{
    private SolidBrush brush = null;

    public ProgressBarEx()
    {
        this.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (brush == null || brush.Color != this.ForeColor)
            brush = new SolidBrush(this.ForeColor);

        Rectangle rec = new Rectangle(0, 0, this.Width, this.Height);
        if (ProgressBarRenderer.IsSupported)
            ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);
        rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
        rec.Height = rec.Height - 4;
        e.Graphics.FillRectangle(brush, 2, 2, rec.Width, rec.Height);
    }
}

public class GridLayout
{

    public int numOfCells;
    public int cellSize;

    private void PictureBox1_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        numOfCells = 200;
        cellSize = 5;
        Pen p = new Pen(Color.Black);

        for (int y = 0; y < numOfCells; ++y)
        {
            g.DrawLine(p, 0, y * cellSize, numOfCells * cellSize, y * cellSize);
        }

        for (int x = 0; x < numOfCells; ++x)
        {
            g.DrawLine(p, x * cellSize, 0, x * cellSize, numOfCells * cellSize);
        }
    }
}
