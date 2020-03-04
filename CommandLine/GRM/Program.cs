using RFID.Notifications;
using RFID.RFIDEngineeringReader;
using AsyncRFIDReader;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace GRM
{
    class GRM
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

        Thread findTagsThread = null;

        private System.Threading.Timer readTimeTimer;

        private ManualResetEvent mreFind = new ManualResetEvent(false);

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


        string debugFileName = "debug.csv";
        StreamWriter debugDataStream = null;

        bool starting = true;

        public GRM()
        {


            int numWorkerThreads;
            int numComplPortThreads;

            ThreadPool.GetMinThreads(out numWorkerThreads, out numComplPortThreads);
            Console.WriteLine("Minimum: # Worker Threads {0}", numWorkerThreads);

            ThreadPool.SetMinThreads(12, 8);

            heartbeatListener = new HeartbeatListener();
            currentReaders = new Dictionary<string, string[]>();

            this.readTimeTimer = new System.Threading.Timer(ReadTimeTimer_Tick, null, System.Threading.Timeout.Infinite, 1000);

            //Dictionary arrays: (TSSI, SCDE, TEMP, RSSI, SCDE Average, Times Read, Total SCDE Values)
            newAllTagInformation = new ConcurrentDictionary<string, TagObject>();
            //tagInformation = new Dictionary<string, double[]>();
            newTagInformation = new Dictionary<string, TagObject>();

            this.frequencyTable = new int[50];

            for (int idx = 0; idx < this.frequencyTable.Length; idx++)
                this.frequencyTable[idx] = 902750 + 500 * idx;

            readerCount = 0;
            heartbeatListener.heartbeatEvent += heartbeatHandler;
            heartbeatListener.Start();
            DeserializeCompanyInformation();
            CreateDatabase();

            //Create Threads on startup of program
            findTagsThread = new Thread(findTags);

            //Start needed Threads and block on entry
            findTagsThread.Start();

            tagModel = (TagModel)Properties.Settings.Default.TagType;



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

         void Main(string[] args)
        {
            connectedReaderMAC = args[1];
            readerType = currentReaders[connectedReaderMAC][6];

            String readerName = currentReaders[connectedReaderMAC][0];
            reader = new RFIDEngineeringReader(currentReaders[connectedReaderMAC][1], 5000);
            readerConnected = true;
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
                        currentReaders.Remove(MACAddress);
                        currentReaders.Add(MACAddress, reader);
                    }
                    else
                    {
                        currentReaders.Add(MACAddress, reader);
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

                int antennaCount = 0;
                int sequence = 0;
                int powerIndex = 0;
                int frequencyIndex = 0;
                int minPowerIndex = 11;
                int maxPowerIndex = 0;

                setupReader(); // place the initial settings onto the connected reader

                antennaCount = 4;

                int[] antennas = new int[antennaCount];
                for (int idx = 3; idx >= 0; idx--) //this is mostly redudent without a GUI this should be cleaned up later.
                {
                    antennas[sequence++] = idx;
                }

                sequence = 0;

                powerIndex = maxPowerIndex;
                frequencyIndex = 16;

                this.ChangeFrequency(frequencyIndex);
                this.ChangePower(powerIndex, antennas[sequence]);

                while (true)
                {
                    // Clear the tag information gathered during each cycle so that we
                    // are not mixing TSSI/SCDE reading between antennas and power levels
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
                }
            }
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

            foreach (double value in workingList)
                tempList.AddLast(value);

            double average = 0;

            if (tempList.Count > 5)
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

                if (tempList.Count > 6)
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
                reader.GetRegisterContents(command, data, out registerContents); //need to get the newer version of RFID...
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

                                    if (!newTagInformation.ContainsKey(EPC)) // Add new tags to the dictionary
                                    {
                                        newTagInformation.Add(EPC, tempValObj);

                                    }
                                    if (!newTagInformation.ContainsKey(EPC))
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

        //private void MSRC_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    heartbeatListener.Stop();

        //    programEnding = true; //provide flag to threads that the program is terminating
        //                            //allow all threads to resume and exit gracefully...
        //    mreFind.Set();
        //    mreDisplayWet.Set();
        //    mreDisplayDry.Set();
        //    mreDisplayDamp.Set();
        //    mreUpdateMap.Set();
        //    mredetailedView.Set();

        //    Properties.Settings.Default["MACAddress"] = connectedReaderMAC;
        //    Properties.Settings.Default.Save();
        //}

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

    }

    public class TagObject
    {
        public double TSSI, TSSIAvg, SCDE, SCDEraw, SCDEAvg, TEMP, RSSI, SCDEStdDev, TimesRead, TotalSCDEValue, Lat, Lon, freq_TSSI, freq_SCDE;
        public double FreqCorNumerator, FreqCorDenominator, PowerCorDenominator, MidFreq;
        public LinkedList<double> SCDEValues;
        public LinkedList<double> TSSIValues;

        public TagObject()
        {
            this.TSSI = 0;
            this.TSSIAvg = 0;
            this.SCDE = 0;
            this.TEMP = 0;
            this.RSSI = 0;
            this.SCDEAvg = 0;
            this.SCDEStdDev = 0;
            this.TimesRead = 0;
            this.TotalSCDEValue = 0;
            this.Lat = 0;
            this.Lon = 0;
            this.freq_TSSI = 0;
            this.freq_SCDE = 0;
            this.FreqCorNumerator = 45;
            this.FreqCorDenominator = 11500;
            this.PowerCorDenominator = 325;
            this.MidFreq = 913250;
            this.SCDEValues = new LinkedList<double>();
            this.TSSIValues = new LinkedList<double>();
        }
    }
}
