namespace MSRC
{
    partial class MSRC
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label lblReading;
            System.Windows.Forms.Label lblTagRead;
            System.Windows.Forms.Label lblMoisture;
            System.Windows.Forms.Panel panelTagData;
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Unique Tags Read",
            "0"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Dry Tags Read",
            "0"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Damp Tags Read",
            "0"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Wet Tags Read",
            "0"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "No Moisture Read *",
            "0"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSRC));
            this.lstTagView = new System.Windows.Forms.ListView();
            this.reading = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Number = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblCompanyName = new System.Windows.Forms.Label();
            this.lblBuildingNumber = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lblCity = new System.Windows.Forms.Label();
            this.lblStreetAddress = new System.Windows.Forms.Label();
            this.lblZipCode = new System.Windows.Forms.Label();
            this.main = new System.Windows.Forms.Panel();
            this.gbCompanyInformation = new System.Windows.Forms.GroupBox();
            this.btnAddLocation = new System.Windows.Forms.Button();
            this.txtState = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtBuildingNumb = new System.Windows.Forms.TextBox();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.txtZip = new System.Windows.Forms.TextBox();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.gbSelCol = new System.Windows.Forms.GroupBox();
            this.cbTemp = new System.Windows.Forms.CheckBox();
            this.cbTS = new System.Windows.Forms.CheckBox();
            this.cbSTdDev = new System.Windows.Forms.CheckBox();
            this.cbSCDEAvg = new System.Windows.Forms.CheckBox();
            this.cbSCDE = new System.Windows.Forms.CheckBox();
            this.cbTSSIAvg = new System.Windows.Forms.CheckBox();
            this.cbTSSI = new System.Windows.Forms.CheckBox();
            this.cbTR = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radMagnusS3 = new System.Windows.Forms.RadioButton();
            this.radMagnusS2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudTSSIMin = new System.Windows.Forms.NumericUpDown();
            this.nudTSSIMax = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AdminMapRoof = new System.Windows.Forms.Panel();
            this.btnMapRoof = new System.Windows.Forms.Button();
            this.gbPowerRange = new System.Windows.Forms.GroupBox();
            this.lblMinPower = new System.Windows.Forms.Label();
            this.lblMaxPower = new System.Windows.Forms.Label();
            this.cbxMinPower = new System.Windows.Forms.ComboBox();
            this.cbxMaxPower = new System.Windows.Forms.ComboBox();
            this.gbActiveAntennas = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.MapRoof = new System.Windows.Forms.Panel();
            this.lblMoistureInfo = new System.Windows.Forms.Label();
            this.btnCalInfo = new System.Windows.Forms.Button();
            this.btnDetailedView = new System.Windows.Forms.Button();
            this.buildingNumberValue = new System.Windows.Forms.Label();
            this.companyAddressValue = new System.Windows.Forms.Label();
            this.companyNameValue = new System.Windows.Forms.Label();
            this.companyName = new System.Windows.Forms.Label();
            this.companyAddress = new System.Windows.Forms.Label();
            this.buildingNumber = new System.Windows.Forms.Label();
            this.btnPauseStartReading = new System.Windows.Forms.Button();
            this.btnClearTagList = new System.Windows.Forms.Button();
            this.lblWet = new System.Windows.Forms.Label();
            this.lblDamp = new System.Windows.Forms.Label();
            this.lblDry = new System.Windows.Forms.Label();
            this.wetStatus = new System.Windows.Forms.Button();
            this.dampStatus = new System.Windows.Forms.Button();
            this.dryStatus = new System.Windows.Forms.Button();
            this.moistureStatus = new System.Windows.Forms.Button();
            this.tagReadStatus = new System.Windows.Forms.Button();
            this.readingStatus = new System.Windows.Forms.Button();
            this.btnChangeSettings = new System.Windows.Forms.Button();
            this.admin = new System.Windows.Forms.Panel();
            this.lblUserPrompt = new System.Windows.Forms.Label();
            this.btnClearReaderList = new System.Windows.Forms.Button();
            this.Readers = new System.Windows.Forms.ListView();
            this.rdrName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rdrIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rdrMAC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rdrActive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.wetTagList = new System.Windows.Forms.Panel();
            this.btnReturnHome = new System.Windows.Forms.Button();
            this.lstWetTags = new System.Windows.Forms.ListView();
            this.colEPC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLAT = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLON = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tagListDisplay = new System.Windows.Forms.Panel();
            this.txtRequestedReadTime = new System.Windows.Forms.TextBox();
            this.lblReportedElapsedTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRequestedReadTime = new System.Windows.Forms.Label();
            this.btnClearTagListDetailPage = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.detailedTagView = new System.Windows.Forms.ListView();
            this.colEPC_1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTR_1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTSSI = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTSSIAvg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSCDE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSCDEAvg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStdDev = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTemp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuClipboardActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCtrlCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelCalInfo = new System.Windows.Forms.Panel();
            this.btnCalInfoBack = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.sqLiteCommand1 = new System.Data.SQLite.SQLiteCommand();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            lblReading = new System.Windows.Forms.Label();
            lblTagRead = new System.Windows.Forms.Label();
            lblMoisture = new System.Windows.Forms.Label();
            panelTagData = new System.Windows.Forms.Panel();
            panelTagData.SuspendLayout();
            this.main.SuspendLayout();
            this.gbCompanyInformation.SuspendLayout();
            this.gbSettings.SuspendLayout();
            this.gbSelCol.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTSSIMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTSSIMax)).BeginInit();
            this.AdminMapRoof.SuspendLayout();
            this.gbPowerRange.SuspendLayout();
            this.gbActiveAntennas.SuspendLayout();
            this.MapRoof.SuspendLayout();
            this.admin.SuspendLayout();
            this.wetTagList.SuspendLayout();
            this.tagListDisplay.SuspendLayout();
            this.mnuClipboardActions.SuspendLayout();
            this.panelCalInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblReading
            // 
            lblReading.Anchor = System.Windows.Forms.AnchorStyles.None;
            lblReading.AutoSize = true;
            lblReading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            lblReading.Location = new System.Drawing.Point(92, 138);
            lblReading.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblReading.Name = "lblReading";
            lblReading.Size = new System.Drawing.Size(164, 39);
            lblReading.TabIndex = 7;
            lblReading.Text = "Reading?";
            // 
            // lblTagRead
            // 
            lblTagRead.Anchor = System.Windows.Forms.AnchorStyles.None;
            lblTagRead.AutoSize = true;
            lblTagRead.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            lblTagRead.Location = new System.Drawing.Point(92, 277);
            lblTagRead.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblTagRead.Name = "lblTagRead";
            lblTagRead.Size = new System.Drawing.Size(186, 39);
            lblTagRead.TabIndex = 9;
            lblTagRead.Text = "Tag Read?";
            // 
            // lblMoisture
            // 
            lblMoisture.Anchor = System.Windows.Forms.AnchorStyles.None;
            lblMoisture.AutoSize = true;
            lblMoisture.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            lblMoisture.Location = new System.Drawing.Point(92, 415);
            lblMoisture.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            lblMoisture.Name = "lblMoisture";
            lblMoisture.Size = new System.Drawing.Size(261, 39);
            lblMoisture.TabIndex = 11;
            lblMoisture.Text = "Moisture Read?";
            // 
            // panelTagData
            // 
            panelTagData.Anchor = System.Windows.Forms.AnchorStyles.None;
            panelTagData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panelTagData.Controls.Add(this.lstTagView);
            panelTagData.Location = new System.Drawing.Point(880, 561);
            panelTagData.Margin = new System.Windows.Forms.Padding(6);
            panelTagData.Name = "panelTagData";
            panelTagData.Size = new System.Drawing.Size(763, 340);
            panelTagData.TabIndex = 21;
            // 
            // lstTagView
            // 
            this.lstTagView.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lstTagView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.reading,
            this.Number});
            this.lstTagView.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTagView.FullRowSelect = true;
            this.lstTagView.GridLines = true;
            this.lstTagView.HideSelection = false;
            this.lstTagView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.lstTagView.Location = new System.Drawing.Point(0, 0);
            this.lstTagView.Margin = new System.Windows.Forms.Padding(6);
            this.lstTagView.Name = "lstTagView";
            this.lstTagView.Size = new System.Drawing.Size(756, 333);
            this.lstTagView.TabIndex = 0;
            this.lstTagView.UseCompatibleStateImageBehavior = false;
            this.lstTagView.View = System.Windows.Forms.View.Details;
            // 
            // reading
            // 
            this.reading.Text = "";
            this.reading.Width = 242;
            // 
            // Number
            // 
            this.Number.Text = "Number";
            this.Number.Width = 167;
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblCompanyName.AutoSize = true;
            this.lblCompanyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblCompanyName.Location = new System.Drawing.Point(90, 74);
            this.lblCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(218, 32);
            this.lblCompanyName.TabIndex = 7;
            this.lblCompanyName.Text = "Company Name";
            // 
            // lblBuildingNumber
            // 
            this.lblBuildingNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblBuildingNumber.AutoSize = true;
            this.lblBuildingNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblBuildingNumber.Location = new System.Drawing.Point(90, 188);
            this.lblBuildingNumber.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBuildingNumber.Name = "lblBuildingNumber";
            this.lblBuildingNumber.Size = new System.Drawing.Size(322, 32);
            this.lblBuildingNumber.TabIndex = 8;
            this.lblBuildingNumber.Text = "Building Number (If Any)";
            // 
            // lblState
            // 
            this.lblState.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblState.AutoSize = true;
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblState.Location = new System.Drawing.Point(937, 295);
            this.lblState.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(82, 32);
            this.lblState.TabIndex = 9;
            this.lblState.Text = "State";
            // 
            // lblCity
            // 
            this.lblCity.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblCity.AutoSize = true;
            this.lblCity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblCity.Location = new System.Drawing.Point(930, 190);
            this.lblCity.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size(64, 32);
            this.lblCity.TabIndex = 10;
            this.lblCity.Text = "City";
            // 
            // lblStreetAddress
            // 
            this.lblStreetAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblStreetAddress.AutoSize = true;
            this.lblStreetAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblStreetAddress.Location = new System.Drawing.Point(937, 74);
            this.lblStreetAddress.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblStreetAddress.Name = "lblStreetAddress";
            this.lblStreetAddress.Size = new System.Drawing.Size(202, 32);
            this.lblStreetAddress.TabIndex = 11;
            this.lblStreetAddress.Text = "Street Address";
            // 
            // lblZipCode
            // 
            this.lblZipCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblZipCode.AutoSize = true;
            this.lblZipCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblZipCode.Location = new System.Drawing.Point(1095, 295);
            this.lblZipCode.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblZipCode.Name = "lblZipCode";
            this.lblZipCode.Size = new System.Drawing.Size(130, 32);
            this.lblZipCode.TabIndex = 12;
            this.lblZipCode.Text = "Zip Code";
            // 
            // main
            // 
            this.main.Controls.Add(this.gbSettings);
            this.main.Controls.Add(this.gbCompanyInformation);
            this.main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main.Location = new System.Drawing.Point(0, 0);
            this.main.Margin = new System.Windows.Forms.Padding(6);
            this.main.Name = "main";
            this.main.Size = new System.Drawing.Size(1769, 1036);
            this.main.TabIndex = 0;
            // 
            // gbCompanyInformation
            // 
            this.gbCompanyInformation.Controls.Add(this.btnAddLocation);
            this.gbCompanyInformation.Controls.Add(this.txtState);
            this.gbCompanyInformation.Controls.Add(this.txtCity);
            this.gbCompanyInformation.Controls.Add(this.txtAddress);
            this.gbCompanyInformation.Controls.Add(this.txtBuildingNumb);
            this.gbCompanyInformation.Controls.Add(this.txtCompanyName);
            this.gbCompanyInformation.Controls.Add(this.txtZip);
            this.gbCompanyInformation.Controls.Add(this.lblZipCode);
            this.gbCompanyInformation.Controls.Add(this.lblStreetAddress);
            this.gbCompanyInformation.Controls.Add(this.lblCity);
            this.gbCompanyInformation.Controls.Add(this.lblState);
            this.gbCompanyInformation.Controls.Add(this.lblBuildingNumber);
            this.gbCompanyInformation.Controls.Add(this.lblCompanyName);
            this.gbCompanyInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbCompanyInformation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbCompanyInformation.Location = new System.Drawing.Point(101, 98);
            this.gbCompanyInformation.Margin = new System.Windows.Forms.Padding(6);
            this.gbCompanyInformation.Name = "gbCompanyInformation";
            this.gbCompanyInformation.Padding = new System.Windows.Forms.Padding(6);
            this.gbCompanyInformation.Size = new System.Drawing.Size(1584, 439);
            this.gbCompanyInformation.TabIndex = 30;
            this.gbCompanyInformation.TabStop = false;
            this.gbCompanyInformation.Text = "Company Information";
            this.gbCompanyInformation.Visible = false;
            // 
            // btnAddLocation
            // 
            this.btnAddLocation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddLocation.Location = new System.Drawing.Point(97, 321);
            this.btnAddLocation.Margin = new System.Windows.Forms.Padding(6);
            this.btnAddLocation.Name = "btnAddLocation";
            this.btnAddLocation.Size = new System.Drawing.Size(323, 74);
            this.btnAddLocation.TabIndex = 26;
            this.btnAddLocation.Text = "Add Location";
            this.btnAddLocation.UseVisualStyleBackColor = true;
            this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
            // 
            // txtState
            // 
            this.txtState.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtState.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtState.Location = new System.Drawing.Point(946, 334);
            this.txtState.Margin = new System.Windows.Forms.Padding(6);
            this.txtState.MaxLength = 2;
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(76, 39);
            this.txtState.TabIndex = 17;
            // 
            // txtCity
            // 
            this.txtCity.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtCity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtCity.Location = new System.Drawing.Point(944, 231);
            this.txtCity.Margin = new System.Windows.Forms.Padding(6);
            this.txtCity.MaxLength = 50;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(461, 39);
            this.txtCity.TabIndex = 16;
            // 
            // txtAddress
            // 
            this.txtAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtAddress.Location = new System.Drawing.Point(944, 116);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(6);
            this.txtAddress.MaxLength = 50;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(602, 39);
            this.txtAddress.TabIndex = 15;
            // 
            // txtBuildingNumb
            // 
            this.txtBuildingNumb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBuildingNumb.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtBuildingNumb.Location = new System.Drawing.Point(97, 231);
            this.txtBuildingNumb.Margin = new System.Windows.Forms.Padding(6);
            this.txtBuildingNumb.MaxLength = 50;
            this.txtBuildingNumb.Name = "txtBuildingNumb";
            this.txtBuildingNumb.Size = new System.Drawing.Size(319, 39);
            this.txtBuildingNumb.TabIndex = 14;
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtCompanyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtCompanyName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtCompanyName.Location = new System.Drawing.Point(97, 116);
            this.txtCompanyName.Margin = new System.Windows.Forms.Padding(6);
            this.txtCompanyName.MaxLength = 50;
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(462, 39);
            this.txtCompanyName.TabIndex = 13;
            // 
            // txtZip
            // 
            this.txtZip.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtZip.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtZip.Location = new System.Drawing.Point(1102, 334);
            this.txtZip.Margin = new System.Windows.Forms.Padding(6);
            this.txtZip.MaxLength = 11;
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(145, 39);
            this.txtZip.TabIndex = 25;
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.gbPowerRange);
            this.gbSettings.Controls.Add(this.gbSelCol);
            this.gbSettings.Controls.Add(this.groupBox2);
            this.gbSettings.Controls.Add(this.groupBox1);
            this.gbSettings.Controls.Add(this.AdminMapRoof);
            this.gbSettings.Controls.Add(this.gbActiveAntennas);
            this.gbSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSettings.Location = new System.Drawing.Point(51, 44);
            this.gbSettings.Margin = new System.Windows.Forms.Padding(6);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Padding = new System.Windows.Forms.Padding(6);
            this.gbSettings.Size = new System.Drawing.Size(1657, 932);
            this.gbSettings.TabIndex = 31;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Settings";
            // 
            // gbSelCol
            // 
            this.gbSelCol.Controls.Add(this.cbTemp);
            this.gbSelCol.Controls.Add(this.cbTS);
            this.gbSelCol.Controls.Add(this.cbSTdDev);
            this.gbSelCol.Controls.Add(this.cbSCDEAvg);
            this.gbSelCol.Controls.Add(this.cbSCDE);
            this.gbSelCol.Controls.Add(this.cbTSSIAvg);
            this.gbSelCol.Controls.Add(this.cbTSSI);
            this.gbSelCol.Controls.Add(this.cbTR);
            this.gbSelCol.Location = new System.Drawing.Point(50, 631);
            this.gbSelCol.Margin = new System.Windows.Forms.Padding(6);
            this.gbSelCol.Name = "gbSelCol";
            this.gbSelCol.Padding = new System.Windows.Forms.Padding(6);
            this.gbSelCol.Size = new System.Drawing.Size(490, 260);
            this.gbSelCol.TabIndex = 32;
            this.gbSelCol.TabStop = false;
            this.gbSelCol.Text = "Detailed View";
            // 
            // cbTemp
            // 
            this.cbTemp.AutoSize = true;
            this.cbTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTemp.Location = new System.Drawing.Point(275, 201);
            this.cbTemp.Margin = new System.Windows.Forms.Padding(6);
            this.cbTemp.Name = "cbTemp";
            this.cbTemp.Size = new System.Drawing.Size(195, 35);
            this.cbTemp.TabIndex = 7;
            this.cbTemp.Text = "Temperature";
            this.cbTemp.UseVisualStyleBackColor = true;
            this.cbTemp.CheckedChanged += new System.EventHandler(this.cbTemp_CheckedChanged);
            // 
            // cbTS
            // 
            this.cbTS.AutoSize = true;
            this.cbTS.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTS.Location = new System.Drawing.Point(275, 150);
            this.cbTS.Margin = new System.Windows.Forms.Padding(6);
            this.cbTS.Name = "cbTS";
            this.cbTS.Size = new System.Drawing.Size(158, 35);
            this.cbTS.TabIndex = 6;
            this.cbTS.Text = "Tag State";
            this.cbTS.UseVisualStyleBackColor = true;
            this.cbTS.CheckedChanged += new System.EventHandler(this.cbTS_CheckedChanged);
            // 
            // cbSTdDev
            // 
            this.cbSTdDev.AutoSize = true;
            this.cbSTdDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSTdDev.Location = new System.Drawing.Point(275, 98);
            this.cbSTdDev.Margin = new System.Windows.Forms.Padding(6);
            this.cbSTdDev.Name = "cbSTdDev";
            this.cbSTdDev.Size = new System.Drawing.Size(137, 35);
            this.cbSTdDev.TabIndex = 5;
            this.cbSTdDev.Text = "Std Dev";
            this.cbSTdDev.UseVisualStyleBackColor = true;
            this.cbSTdDev.CheckedChanged += new System.EventHandler(this.cbSTdDev_CheckedChanged);
            // 
            // cbSCDEAvg
            // 
            this.cbSCDEAvg.AutoSize = true;
            this.cbSCDEAvg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSCDEAvg.Location = new System.Drawing.Point(275, 46);
            this.cbSCDEAvg.Margin = new System.Windows.Forms.Padding(6);
            this.cbSCDEAvg.Name = "cbSCDEAvg";
            this.cbSCDEAvg.Size = new System.Drawing.Size(170, 35);
            this.cbSCDEAvg.TabIndex = 4;
            this.cbSCDEAvg.Text = "SCDE Avg";
            this.cbSCDEAvg.UseVisualStyleBackColor = true;
            this.cbSCDEAvg.CheckedChanged += new System.EventHandler(this.SCDEAvg_CheckedChanged);
            // 
            // cbSCDE
            // 
            this.cbSCDE.AutoSize = true;
            this.cbSCDE.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSCDE.Location = new System.Drawing.Point(29, 201);
            this.cbSCDE.Margin = new System.Windows.Forms.Padding(6);
            this.cbSCDE.Name = "cbSCDE";
            this.cbSCDE.Size = new System.Drawing.Size(116, 35);
            this.cbSCDE.TabIndex = 3;
            this.cbSCDE.Text = "SCDE";
            this.cbSCDE.UseVisualStyleBackColor = true;
            this.cbSCDE.CheckedChanged += new System.EventHandler(this.cbSCDE_CheckedChanged);
            // 
            // cbTSSIAvg
            // 
            this.cbTSSIAvg.AutoSize = true;
            this.cbTSSIAvg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTSSIAvg.Location = new System.Drawing.Point(29, 150);
            this.cbTSSIAvg.Margin = new System.Windows.Forms.Padding(6);
            this.cbTSSIAvg.Name = "cbTSSIAvg";
            this.cbTSSIAvg.Size = new System.Drawing.Size(155, 35);
            this.cbTSSIAvg.TabIndex = 2;
            this.cbTSSIAvg.Text = "TSSI Avg";
            this.cbTSSIAvg.UseVisualStyleBackColor = true;
            this.cbTSSIAvg.CheckedChanged += new System.EventHandler(this.cbTSSIAvg_CheckedChanged);
            // 
            // cbTSSI
            // 
            this.cbTSSI.AutoSize = true;
            this.cbTSSI.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTSSI.Location = new System.Drawing.Point(29, 98);
            this.cbTSSI.Margin = new System.Windows.Forms.Padding(6);
            this.cbTSSI.Name = "cbTSSI";
            this.cbTSSI.Size = new System.Drawing.Size(101, 35);
            this.cbTSSI.TabIndex = 1;
            this.cbTSSI.Text = "TSSI";
            this.cbTSSI.UseVisualStyleBackColor = true;
            this.cbTSSI.CheckedChanged += new System.EventHandler(this.cbTSSI_CheckedChanged);
            // 
            // cbTR
            // 
            this.cbTR.AutoSize = true;
            this.cbTR.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbTR.Location = new System.Drawing.Point(29, 46);
            this.cbTR.Margin = new System.Windows.Forms.Padding(6);
            this.cbTR.Name = "cbTR";
            this.cbTR.Size = new System.Drawing.Size(186, 35);
            this.cbTR.TabIndex = 0;
            this.cbTR.Text = "Times Read";
            this.cbTR.UseVisualStyleBackColor = true;
            this.cbTR.CheckedChanged += new System.EventHandler(this.cbTR_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox2.Controls.Add(this.radMagnusS3);
            this.groupBox2.Controls.Add(this.radMagnusS2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(136, 142);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox2.Size = new System.Drawing.Size(422, 284);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tag Type";
            // 
            // radMagnusS3
            // 
            this.radMagnusS3.AutoSize = true;
            this.radMagnusS3.Location = new System.Drawing.Point(119, 150);
            this.radMagnusS3.Margin = new System.Windows.Forms.Padding(4);
            this.radMagnusS3.Name = "radMagnusS3";
            this.radMagnusS3.Size = new System.Drawing.Size(183, 36);
            this.radMagnusS3.TabIndex = 1;
            this.radMagnusS3.Text = "Magnus S3";
            this.radMagnusS3.UseVisualStyleBackColor = true;
            this.radMagnusS3.CheckedChanged += new System.EventHandler(this.radMagnusS3_CheckedChanged);
            // 
            // radMagnusS2
            // 
            this.radMagnusS2.AutoSize = true;
            this.radMagnusS2.Checked = true;
            this.radMagnusS2.Location = new System.Drawing.Point(119, 81);
            this.radMagnusS2.Margin = new System.Windows.Forms.Padding(4);
            this.radMagnusS2.Name = "radMagnusS2";
            this.radMagnusS2.Size = new System.Drawing.Size(183, 36);
            this.radMagnusS2.TabIndex = 0;
            this.radMagnusS2.TabStop = true;
            this.radMagnusS2.Text = "Magnus S2";
            this.radMagnusS2.UseVisualStyleBackColor = true;
            this.radMagnusS2.CheckedChanged += new System.EventHandler(this.radMagnusS2_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.Controls.Add(this.nudTSSIMin);
            this.groupBox1.Controls.Add(this.nudTSSIMax);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(1197, 607);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(422, 284);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tag Sensor Strength Range";
            this.groupBox1.Visible = false;
            // 
            // nudTSSIMin
            // 
            this.nudTSSIMin.Location = new System.Drawing.Point(253, 150);
            this.nudTSSIMin.Margin = new System.Windows.Forms.Padding(4);
            this.nudTSSIMin.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudTSSIMin.Name = "nudTSSIMin";
            this.nudTSSIMin.Size = new System.Drawing.Size(119, 39);
            this.nudTSSIMin.TabIndex = 5;
            this.nudTSSIMin.ValueChanged += new System.EventHandler(this.nudTSSIMin_ValueChanged);
            // 
            // nudTSSIMax
            // 
            this.nudTSSIMax.Location = new System.Drawing.Point(253, 79);
            this.nudTSSIMax.Margin = new System.Windows.Forms.Padding(4);
            this.nudTSSIMax.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudTSSIMax.Name = "nudTSSIMax";
            this.nudTSSIMax.Size = new System.Drawing.Size(119, 39);
            this.nudTSSIMax.TabIndex = 4;
            this.nudTSSIMax.ValueChanged += new System.EventHandler(this.nudTSSIMax_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F);
            this.label1.Location = new System.Drawing.Point(50, 151);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 36);
            this.label1.TabIndex = 3;
            this.label1.Text = "Minimum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F);
            this.label2.Location = new System.Drawing.Point(50, 81);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 36);
            this.label2.TabIndex = 2;
            this.label2.Text = "Maximum";
            // 
            // AdminMapRoof
            // 
            this.AdminMapRoof.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AdminMapRoof.BackColor = System.Drawing.Color.Transparent;
            this.AdminMapRoof.Controls.Add(this.btnMapRoof);
            this.AdminMapRoof.Location = new System.Drawing.Point(612, 663);
            this.AdminMapRoof.Margin = new System.Windows.Forms.Padding(6);
            this.AdminMapRoof.Name = "AdminMapRoof";
            this.AdminMapRoof.Size = new System.Drawing.Size(431, 164);
            this.AdminMapRoof.TabIndex = 18;
            // 
            // btnMapRoof
            // 
            this.btnMapRoof.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnMapRoof.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btnMapRoof.Location = new System.Drawing.Point(26, 28);
            this.btnMapRoof.Margin = new System.Windows.Forms.Padding(6);
            this.btnMapRoof.Name = "btnMapRoof";
            this.btnMapRoof.Size = new System.Drawing.Size(380, 118);
            this.btnMapRoof.TabIndex = 1;
            this.btnMapRoof.Text = "Measure Tags";
            this.btnMapRoof.UseVisualStyleBackColor = true;
            this.btnMapRoof.Click += new System.EventHandler(this.MapRoof_Click);
            // 
            // gbPowerRange
            // 
            this.gbPowerRange.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gbPowerRange.Controls.Add(this.lblMinPower);
            this.gbPowerRange.Controls.Add(this.lblMaxPower);
            this.gbPowerRange.Controls.Add(this.cbxMinPower);
            this.gbPowerRange.Controls.Add(this.cbxMaxPower);
            this.gbPowerRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPowerRange.Location = new System.Drawing.Point(618, 142);
            this.gbPowerRange.Margin = new System.Windows.Forms.Padding(6);
            this.gbPowerRange.Name = "gbPowerRange";
            this.gbPowerRange.Padding = new System.Windows.Forms.Padding(6);
            this.gbPowerRange.Size = new System.Drawing.Size(422, 284);
            this.gbPowerRange.TabIndex = 29;
            this.gbPowerRange.TabStop = false;
            this.gbPowerRange.Text = "Power Range";
            // 
            // lblMinPower
            // 
            this.lblMinPower.AutoSize = true;
            this.lblMinPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F);
            this.lblMinPower.Location = new System.Drawing.Point(79, 151);
            this.lblMinPower.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMinPower.Name = "lblMinPower";
            this.lblMinPower.Size = new System.Drawing.Size(134, 36);
            this.lblMinPower.TabIndex = 3;
            this.lblMinPower.Text = "Minimum";
            // 
            // lblMaxPower
            // 
            this.lblMaxPower.AutoSize = true;
            this.lblMaxPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F);
            this.lblMaxPower.Location = new System.Drawing.Point(79, 81);
            this.lblMaxPower.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMaxPower.Name = "lblMaxPower";
            this.lblMaxPower.Size = new System.Drawing.Size(141, 36);
            this.lblMaxPower.TabIndex = 2;
            this.lblMaxPower.Text = "Maximum";
            // 
            // cbxMinPower
            // 
            this.cbxMinPower.FormattingEnabled = true;
            this.cbxMinPower.Items.AddRange(new object[] {
            "33",
            "31.5",
            "30",
            "28",
            "26",
            "24",
            "22",
            "20",
            "18",
            "16",
            "14",
            "12",
            "10"});
            this.cbxMinPower.Location = new System.Drawing.Point(240, 150);
            this.cbxMinPower.Margin = new System.Windows.Forms.Padding(6);
            this.cbxMinPower.Name = "cbxMinPower";
            this.cbxMinPower.Size = new System.Drawing.Size(105, 40);
            this.cbxMinPower.TabIndex = 1;
            this.cbxMinPower.Text = "14";
            this.cbxMinPower.SelectedIndexChanged += new System.EventHandler(this.minPower_SelectedIndexChanged);
            // 
            // cbxMaxPower
            // 
            this.cbxMaxPower.FormattingEnabled = true;
            this.cbxMaxPower.Items.AddRange(new object[] {
            "33",
            "31.5",
            "30",
            "28",
            "26",
            "24",
            "22",
            "20",
            "18",
            "16",
            "14",
            "12",
            "10"});
            this.cbxMaxPower.Location = new System.Drawing.Point(240, 79);
            this.cbxMaxPower.Margin = new System.Windows.Forms.Padding(6);
            this.cbxMaxPower.Name = "cbxMaxPower";
            this.cbxMaxPower.Size = new System.Drawing.Size(105, 40);
            this.cbxMaxPower.TabIndex = 0;
            this.cbxMaxPower.Text = "31.5";
            this.cbxMaxPower.SelectedIndexChanged += new System.EventHandler(this.maxPower_SelectedIndexChanged);
            // 
            // gbActiveAntennas
            // 
            this.gbActiveAntennas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gbActiveAntennas.Controls.Add(this.checkBox1);
            this.gbActiveAntennas.Controls.Add(this.checkBox4);
            this.gbActiveAntennas.Controls.Add(this.checkBox2);
            this.gbActiveAntennas.Controls.Add(this.checkBox3);
            this.gbActiveAntennas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbActiveAntennas.Location = new System.Drawing.Point(1098, 142);
            this.gbActiveAntennas.Margin = new System.Windows.Forms.Padding(6);
            this.gbActiveAntennas.Name = "gbActiveAntennas";
            this.gbActiveAntennas.Padding = new System.Windows.Forms.Padding(6);
            this.gbActiveAntennas.Size = new System.Drawing.Size(422, 414);
            this.gbActiveAntennas.TabIndex = 28;
            this.gbActiveAntennas.TabStop = false;
            this.gbActiveAntennas.Text = "Active Antennas";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(105, 59);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(6);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(215, 46);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Antenna 1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox4.Location = new System.Drawing.Point(105, 325);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(6);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(215, 46);
            this.checkBox4.TabIndex = 9;
            this.checkBox4.Text = "Antenna 4";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.Location = new System.Drawing.Point(105, 148);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(6);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(215, 46);
            this.checkBox2.TabIndex = 7;
            this.checkBox2.Text = "Antenna 2";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.Location = new System.Drawing.Point(105, 236);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(6);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(215, 46);
            this.checkBox3.TabIndex = 8;
            this.checkBox3.Text = "Antenna 3";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // MapRoof
            // 
            this.MapRoof.Controls.Add(this.lblMoistureInfo);
            this.MapRoof.Controls.Add(this.btnCalInfo);
            this.MapRoof.Controls.Add(this.btnDetailedView);
            this.MapRoof.Controls.Add(this.buildingNumberValue);
            this.MapRoof.Controls.Add(this.companyAddressValue);
            this.MapRoof.Controls.Add(this.companyNameValue);
            this.MapRoof.Controls.Add(this.companyName);
            this.MapRoof.Controls.Add(this.companyAddress);
            this.MapRoof.Controls.Add(this.buildingNumber);
            this.MapRoof.Controls.Add(panelTagData);
            this.MapRoof.Controls.Add(this.btnPauseStartReading);
            this.MapRoof.Controls.Add(this.btnClearTagList);
            this.MapRoof.Controls.Add(this.lblWet);
            this.MapRoof.Controls.Add(this.lblDamp);
            this.MapRoof.Controls.Add(this.lblDry);
            this.MapRoof.Controls.Add(this.wetStatus);
            this.MapRoof.Controls.Add(this.dampStatus);
            this.MapRoof.Controls.Add(this.dryStatus);
            this.MapRoof.Controls.Add(this.moistureStatus);
            this.MapRoof.Controls.Add(lblMoisture);
            this.MapRoof.Controls.Add(this.tagReadStatus);
            this.MapRoof.Controls.Add(lblTagRead);
            this.MapRoof.Controls.Add(this.readingStatus);
            this.MapRoof.Controls.Add(lblReading);
            this.MapRoof.Controls.Add(this.btnChangeSettings);
            this.MapRoof.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapRoof.Location = new System.Drawing.Point(0, 0);
            this.MapRoof.Margin = new System.Windows.Forms.Padding(6);
            this.MapRoof.Name = "MapRoof";
            this.MapRoof.Size = new System.Drawing.Size(1769, 1036);
            this.MapRoof.TabIndex = 2;
            // 
            // lblMoistureInfo
            // 
            this.lblMoistureInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMoistureInfo.AutoSize = true;
            this.lblMoistureInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoistureInfo.Location = new System.Drawing.Point(911, 942);
            this.lblMoistureInfo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMoistureInfo.Name = "lblMoistureInfo";
            this.lblMoistureInfo.Size = new System.Drawing.Size(472, 29);
            this.lblMoistureInfo.TabIndex = 45;
            this.lblMoistureInfo.Text = "* No Moisture Read or a Non-Moisture Tag";
            // 
            // btnCalInfo
            // 
            this.btnCalInfo.Location = new System.Drawing.Point(130, 875);
            this.btnCalInfo.Margin = new System.Windows.Forms.Padding(6);
            this.btnCalInfo.Name = "btnCalInfo";
            this.btnCalInfo.Size = new System.Drawing.Size(220, 44);
            this.btnCalInfo.TabIndex = 44;
            this.btnCalInfo.Text = "Calibration Info";
            this.btnCalInfo.UseVisualStyleBackColor = true;
            this.btnCalInfo.Visible = false;
            this.btnCalInfo.Click += new System.EventHandler(this.btnCalInfo_Click);
            // 
            // btnDetailedView
            // 
            this.btnDetailedView.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDetailedView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnDetailedView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetailedView.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnDetailedView.ForeColor = System.Drawing.Color.White;
            this.btnDetailedView.Location = new System.Drawing.Point(73, 755);
            this.btnDetailedView.Margin = new System.Windows.Forms.Padding(6);
            this.btnDetailedView.Name = "btnDetailedView";
            this.btnDetailedView.Size = new System.Drawing.Size(323, 100);
            this.btnDetailedView.TabIndex = 43;
            this.btnDetailedView.Text = "Detailed View";
            this.btnDetailedView.UseVisualStyleBackColor = false;
            this.btnDetailedView.Click += new System.EventHandler(this.btnDetailedView_Click);
            // 
            // buildingNumberValue
            // 
            this.buildingNumberValue.AutoSize = true;
            this.buildingNumberValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildingNumberValue.Location = new System.Drawing.Point(1056, 133);
            this.buildingNumberValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.buildingNumberValue.Name = "buildingNumberValue";
            this.buildingNumberValue.Size = new System.Drawing.Size(0, 32);
            this.buildingNumberValue.TabIndex = 40;
            // 
            // companyAddressValue
            // 
            this.companyAddressValue.AutoSize = true;
            this.companyAddressValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.companyAddressValue.Location = new System.Drawing.Point(1056, 74);
            this.companyAddressValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.companyAddressValue.Name = "companyAddressValue";
            this.companyAddressValue.Size = new System.Drawing.Size(0, 32);
            this.companyAddressValue.TabIndex = 39;
            // 
            // companyNameValue
            // 
            this.companyNameValue.AutoSize = true;
            this.companyNameValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.companyNameValue.Location = new System.Drawing.Point(1056, 15);
            this.companyNameValue.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.companyNameValue.Name = "companyNameValue";
            this.companyNameValue.Size = new System.Drawing.Size(0, 32);
            this.companyNameValue.TabIndex = 38;
            // 
            // companyName
            // 
            this.companyName.AutoSize = true;
            this.companyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.companyName.Location = new System.Drawing.Point(909, 15);
            this.companyName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.companyName.Name = "companyName";
            this.companyName.Size = new System.Drawing.Size(0, 32);
            this.companyName.TabIndex = 35;
            // 
            // companyAddress
            // 
            this.companyAddress.AutoSize = true;
            this.companyAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.companyAddress.Location = new System.Drawing.Point(909, 74);
            this.companyAddress.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.companyAddress.Name = "companyAddress";
            this.companyAddress.Size = new System.Drawing.Size(0, 32);
            this.companyAddress.TabIndex = 37;
            // 
            // buildingNumber
            // 
            this.buildingNumber.AutoSize = true;
            this.buildingNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildingNumber.Location = new System.Drawing.Point(909, 133);
            this.buildingNumber.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.buildingNumber.Name = "buildingNumber";
            this.buildingNumber.Size = new System.Drawing.Size(0, 32);
            this.buildingNumber.TabIndex = 36;
            // 
            // btnPauseStartReading
            // 
            this.btnPauseStartReading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPauseStartReading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnPauseStartReading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPauseStartReading.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnPauseStartReading.ForeColor = System.Drawing.Color.White;
            this.btnPauseStartReading.Location = new System.Drawing.Point(73, 622);
            this.btnPauseStartReading.Margin = new System.Windows.Forms.Padding(6);
            this.btnPauseStartReading.Name = "btnPauseStartReading";
            this.btnPauseStartReading.Size = new System.Drawing.Size(323, 100);
            this.btnPauseStartReading.TabIndex = 20;
            this.btnPauseStartReading.Text = "Start Reading";
            this.btnPauseStartReading.UseVisualStyleBackColor = false;
            this.btnPauseStartReading.Click += new System.EventHandler(this.btnPauseStartReading_Click);
            // 
            // btnClearTagList
            // 
            this.btnClearTagList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClearTagList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnClearTagList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearTagList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClearTagList.ForeColor = System.Drawing.Color.White;
            this.btnClearTagList.Location = new System.Drawing.Point(425, 622);
            this.btnClearTagList.Margin = new System.Windows.Forms.Padding(6);
            this.btnClearTagList.Name = "btnClearTagList";
            this.btnClearTagList.Size = new System.Drawing.Size(323, 100);
            this.btnClearTagList.TabIndex = 19;
            this.btnClearTagList.Text = "Clear Tag List";
            this.btnClearTagList.UseVisualStyleBackColor = false;
            this.btnClearTagList.Click += new System.EventHandler(this.btnClearTagList_Click);
            // 
            // lblWet
            // 
            this.lblWet.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblWet.AutoSize = true;
            this.lblWet.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.lblWet.Location = new System.Drawing.Point(908, 402);
            this.lblWet.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblWet.Name = "lblWet";
            this.lblWet.Size = new System.Drawing.Size(79, 39);
            this.lblWet.TabIndex = 18;
            this.lblWet.Text = "Wet";
            // 
            // lblDamp
            // 
            this.lblDamp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDamp.AutoSize = true;
            this.lblDamp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.lblDamp.Location = new System.Drawing.Point(908, 295);
            this.lblDamp.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDamp.Name = "lblDamp";
            this.lblDamp.Size = new System.Drawing.Size(110, 39);
            this.lblDamp.TabIndex = 17;
            this.lblDamp.Text = "Damp";
            // 
            // lblDry
            // 
            this.lblDry.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDry.AutoSize = true;
            this.lblDry.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.lblDry.Location = new System.Drawing.Point(908, 185);
            this.lblDry.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDry.Name = "lblDry";
            this.lblDry.Size = new System.Drawing.Size(72, 39);
            this.lblDry.TabIndex = 16;
            this.lblDry.Text = "Dry";
            // 
            // wetStatus
            // 
            this.wetStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.wetStatus.BackColor = System.Drawing.Color.White;
            this.wetStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.wetStatus.Enabled = false;
            this.wetStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.wetStatus.Location = new System.Drawing.Point(1071, 380);
            this.wetStatus.Margin = new System.Windows.Forms.Padding(6);
            this.wetStatus.Name = "wetStatus";
            this.wetStatus.Size = new System.Drawing.Size(528, 114);
            this.wetStatus.TabIndex = 15;
            this.wetStatus.UseVisualStyleBackColor = false;
            // 
            // dampStatus
            // 
            this.dampStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dampStatus.BackColor = System.Drawing.Color.White;
            this.dampStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.dampStatus.Enabled = false;
            this.dampStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dampStatus.Location = new System.Drawing.Point(1071, 268);
            this.dampStatus.Margin = new System.Windows.Forms.Padding(6);
            this.dampStatus.Name = "dampStatus";
            this.dampStatus.Size = new System.Drawing.Size(528, 114);
            this.dampStatus.TabIndex = 14;
            this.dampStatus.UseVisualStyleBackColor = false;
            // 
            // dryStatus
            // 
            this.dryStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dryStatus.BackColor = System.Drawing.Color.White;
            this.dryStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.dryStatus.Enabled = false;
            this.dryStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dryStatus.Location = new System.Drawing.Point(1071, 155);
            this.dryStatus.Margin = new System.Windows.Forms.Padding(6);
            this.dryStatus.Name = "dryStatus";
            this.dryStatus.Size = new System.Drawing.Size(528, 114);
            this.dryStatus.TabIndex = 13;
            this.dryStatus.UseVisualStyleBackColor = false;
            // 
            // moistureStatus
            // 
            this.moistureStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.moistureStatus.BackColor = System.Drawing.Color.Silver;
            this.moistureStatus.Enabled = false;
            this.moistureStatus.Location = new System.Drawing.Point(418, 391);
            this.moistureStatus.Margin = new System.Windows.Forms.Padding(6);
            this.moistureStatus.Name = "moistureStatus";
            this.moistureStatus.Size = new System.Drawing.Size(196, 89);
            this.moistureStatus.TabIndex = 12;
            this.moistureStatus.UseVisualStyleBackColor = false;
            // 
            // tagReadStatus
            // 
            this.tagReadStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tagReadStatus.BackColor = System.Drawing.Color.Silver;
            this.tagReadStatus.Enabled = false;
            this.tagReadStatus.Location = new System.Drawing.Point(418, 253);
            this.tagReadStatus.Margin = new System.Windows.Forms.Padding(6);
            this.tagReadStatus.Name = "tagReadStatus";
            this.tagReadStatus.Size = new System.Drawing.Size(196, 89);
            this.tagReadStatus.TabIndex = 10;
            this.tagReadStatus.UseVisualStyleBackColor = false;
            // 
            // readingStatus
            // 
            this.readingStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.readingStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(51)))), ((int)(((byte)(0)))));
            this.readingStatus.Enabled = false;
            this.readingStatus.Location = new System.Drawing.Point(418, 114);
            this.readingStatus.Margin = new System.Windows.Forms.Padding(6);
            this.readingStatus.Name = "readingStatus";
            this.readingStatus.Size = new System.Drawing.Size(196, 89);
            this.readingStatus.TabIndex = 8;
            this.readingStatus.UseVisualStyleBackColor = false;
            // 
            // btnChangeSettings
            // 
            this.btnChangeSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnChangeSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnChangeSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnChangeSettings.ForeColor = System.Drawing.Color.White;
            this.btnChangeSettings.Location = new System.Drawing.Point(425, 755);
            this.btnChangeSettings.Margin = new System.Windows.Forms.Padding(6);
            this.btnChangeSettings.Name = "btnChangeSettings";
            this.btnChangeSettings.Size = new System.Drawing.Size(323, 100);
            this.btnChangeSettings.TabIndex = 1;
            this.btnChangeSettings.Text = "Settings";
            this.btnChangeSettings.UseVisualStyleBackColor = false;
            this.btnChangeSettings.Click += new System.EventHandler(this.btnChangeSettings_Click);
            // 
            // admin
            // 
            this.admin.Controls.Add(this.lblUserPrompt);
            this.admin.Controls.Add(this.btnClearReaderList);
            this.admin.Controls.Add(this.Readers);
            this.admin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.admin.Location = new System.Drawing.Point(0, 0);
            this.admin.Margin = new System.Windows.Forms.Padding(6);
            this.admin.Name = "admin";
            this.admin.Size = new System.Drawing.Size(1769, 1036);
            this.admin.TabIndex = 26;
            // 
            // lblUserPrompt
            // 
            this.lblUserPrompt.AutoSize = true;
            this.lblUserPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserPrompt.Location = new System.Drawing.Point(501, 57);
            this.lblUserPrompt.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblUserPrompt.Name = "lblUserPrompt";
            this.lblUserPrompt.Size = new System.Drawing.Size(643, 48);
            this.lblUserPrompt.TabIndex = 6;
            this.lblUserPrompt.Text = "Select Reader to Begin Operation";
            // 
            // btnClearReaderList
            // 
            this.btnClearReaderList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClearReaderList.Location = new System.Drawing.Point(763, 871);
            this.btnClearReaderList.Margin = new System.Windows.Forms.Padding(6);
            this.btnClearReaderList.Name = "btnClearReaderList";
            this.btnClearReaderList.Size = new System.Drawing.Size(209, 42);
            this.btnClearReaderList.TabIndex = 5;
            this.btnClearReaderList.Text = "Clear Reader List";
            this.btnClearReaderList.UseVisualStyleBackColor = true;
            this.btnClearReaderList.Click += new System.EventHandler(this.ClearReaderList_Click);
            // 
            // Readers
            // 
            this.Readers.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.rdrName,
            this.rdrIP,
            this.rdrMAC,
            this.rdrActive});
            this.Readers.FullRowSelect = true;
            this.Readers.HideSelection = false;
            this.Readers.Location = new System.Drawing.Point(425, 144);
            this.Readers.Margin = new System.Windows.Forms.Padding(6);
            this.Readers.MultiSelect = false;
            this.Readers.Name = "Readers";
            this.Readers.Size = new System.Drawing.Size(840, 713);
            this.Readers.TabIndex = 0;
            this.Readers.UseCompatibleStateImageBehavior = false;
            this.Readers.View = System.Windows.Forms.View.Details;
            this.Readers.SelectedIndexChanged += new System.EventHandler(this.Readers_SelectedIndexChanged);
            // 
            // rdrName
            // 
            this.rdrName.Text = "Reader Name";
            this.rdrName.Width = 118;
            // 
            // rdrIP
            // 
            this.rdrIP.Text = "IP Address";
            this.rdrIP.Width = 137;
            // 
            // rdrMAC
            // 
            this.rdrMAC.Text = "MAC Address";
            this.rdrMAC.Width = 116;
            // 
            // rdrActive
            // 
            this.rdrActive.Text = "Active Count";
            this.rdrActive.Width = 85;
            // 
            // wetTagList
            // 
            this.wetTagList.Controls.Add(this.btnReturnHome);
            this.wetTagList.Controls.Add(this.lstWetTags);
            this.wetTagList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wetTagList.Location = new System.Drawing.Point(0, 0);
            this.wetTagList.Margin = new System.Windows.Forms.Padding(6);
            this.wetTagList.Name = "wetTagList";
            this.wetTagList.Size = new System.Drawing.Size(1769, 1036);
            this.wetTagList.TabIndex = 4;
            // 
            // btnReturnHome
            // 
            this.btnReturnHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnReturnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReturnHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReturnHome.ForeColor = System.Drawing.Color.White;
            this.btnReturnHome.Location = new System.Drawing.Point(673, 842);
            this.btnReturnHome.Margin = new System.Windows.Forms.Padding(6);
            this.btnReturnHome.Name = "btnReturnHome";
            this.btnReturnHome.Size = new System.Drawing.Size(323, 109);
            this.btnReturnHome.TabIndex = 0;
            this.btnReturnHome.Text = "Back To Reading";
            this.btnReturnHome.UseVisualStyleBackColor = false;
            this.btnReturnHome.Click += new System.EventHandler(this.btnReturnHome_Click);
            // 
            // lstWetTags
            // 
            this.lstWetTags.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lstWetTags.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colEPC,
            this.colTR,
            this.colLAT,
            this.colLON,
            this.colTC});
            this.lstWetTags.FullRowSelect = true;
            this.lstWetTags.HideSelection = false;
            this.lstWetTags.Location = new System.Drawing.Point(279, 44);
            this.lstWetTags.Margin = new System.Windows.Forms.Padding(6);
            this.lstWetTags.MultiSelect = false;
            this.lstWetTags.Name = "lstWetTags";
            this.lstWetTags.Size = new System.Drawing.Size(1199, 783);
            this.lstWetTags.TabIndex = 1;
            this.lstWetTags.UseCompatibleStateImageBehavior = false;
            this.lstWetTags.View = System.Windows.Forms.View.Details;
            // 
            // colEPC
            // 
            this.colEPC.Text = "EPC";
            this.colEPC.Width = 214;
            // 
            // colTR
            // 
            this.colTR.Text = "Times Read";
            this.colTR.Width = 86;
            // 
            // colLAT
            // 
            this.colLAT.Text = "Latitude";
            this.colLAT.Width = 93;
            // 
            // colLON
            // 
            this.colLON.Text = "Longitude";
            this.colLON.Width = 95;
            // 
            // colTC
            // 
            this.colTC.Text = "Tile Condition";
            this.colTC.Width = 164;
            // 
            // tagListDisplay
            // 
            this.tagListDisplay.Controls.Add(this.txtRequestedReadTime);
            this.tagListDisplay.Controls.Add(this.lblReportedElapsedTime);
            this.tagListDisplay.Controls.Add(this.label3);
            this.tagListDisplay.Controls.Add(this.lblRequestedReadTime);
            this.tagListDisplay.Controls.Add(this.btnClearTagListDetailPage);
            this.tagListDisplay.Controls.Add(this.btnStartStop);
            this.tagListDisplay.Controls.Add(this.button1);
            this.tagListDisplay.Controls.Add(this.detailedTagView);
            this.tagListDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagListDisplay.Location = new System.Drawing.Point(0, 0);
            this.tagListDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.tagListDisplay.Name = "tagListDisplay";
            this.tagListDisplay.Size = new System.Drawing.Size(1769, 1036);
            this.tagListDisplay.TabIndex = 43;
            // 
            // txtRequestedReadTime
            // 
            this.txtRequestedReadTime.Location = new System.Drawing.Point(649, 881);
            this.txtRequestedReadTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtRequestedReadTime.Name = "txtRequestedReadTime";
            this.txtRequestedReadTime.Size = new System.Drawing.Size(98, 29);
            this.txtRequestedReadTime.TabIndex = 49;
            this.txtRequestedReadTime.Text = "0";
            // 
            // lblReportedElapsedTime
            // 
            this.lblReportedElapsedTime.Location = new System.Drawing.Point(649, 938);
            this.lblReportedElapsedTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReportedElapsedTime.Name = "lblReportedElapsedTime";
            this.lblReportedElapsedTime.Size = new System.Drawing.Size(97, 37);
            this.lblReportedElapsedTime.TabIndex = 48;
            this.lblReportedElapsedTime.Text = "0";
            this.lblReportedElapsedTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(490, 943);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 25);
            this.label3.TabIndex = 47;
            this.label3.Text = "Elapsed Time:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRequestedReadTime
            // 
            this.lblRequestedReadTime.AutoSize = true;
            this.lblRequestedReadTime.Location = new System.Drawing.Point(466, 881);
            this.lblRequestedReadTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRequestedReadTime.Name = "lblRequestedReadTime";
            this.lblRequestedReadTime.Size = new System.Drawing.Size(163, 25);
            this.lblRequestedReadTime.TabIndex = 46;
            this.lblRequestedReadTime.Text = "Read Time (sec):";
            // 
            // btnClearTagListDetailPage
            // 
            this.btnClearTagListDetailPage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClearTagListDetailPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnClearTagListDetailPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearTagListDetailPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClearTagListDetailPage.ForeColor = System.Drawing.Color.White;
            this.btnClearTagListDetailPage.Location = new System.Drawing.Point(880, 873);
            this.btnClearTagListDetailPage.Margin = new System.Windows.Forms.Padding(6);
            this.btnClearTagListDetailPage.Name = "btnClearTagListDetailPage";
            this.btnClearTagListDetailPage.Size = new System.Drawing.Size(323, 100);
            this.btnClearTagListDetailPage.TabIndex = 45;
            this.btnClearTagListDetailPage.Text = "Clear Tag List";
            this.btnClearTagListDetailPage.UseVisualStyleBackColor = false;
            this.btnClearTagListDetailPage.Click += new System.EventHandler(this.btnClearTagList_Click);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStartStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnStartStop.ForeColor = System.Drawing.Color.White;
            this.btnStartStop.Location = new System.Drawing.Point(73, 871);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(323, 100);
            this.btnStartStop.TabIndex = 44;
            this.btnStartStop.Text = "Start Reading";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.Click += new System.EventHandler(this.btnPauseStartReading_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(1362, 871);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(323, 100);
            this.button1.TabIndex = 43;
            this.button1.Text = "Back";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // detailedTagView
            // 
            this.detailedTagView.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.detailedTagView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colEPC_1,
            this.colTR_1,
            this.colTSSI,
            this.colTSSIAvg,
            this.colSCDE,
            this.colSCDEAvg,
            this.colStdDev,
            this.colTS,
            this.colTemp});
            this.detailedTagView.ContextMenuStrip = this.mnuClipboardActions;
            this.detailedTagView.GridLines = true;
            this.detailedTagView.HideSelection = false;
            this.detailedTagView.Location = new System.Drawing.Point(62, 57);
            this.detailedTagView.Margin = new System.Windows.Forms.Padding(6);
            this.detailedTagView.Name = "detailedTagView";
            this.detailedTagView.Size = new System.Drawing.Size(1643, 779);
            this.detailedTagView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.detailedTagView.TabIndex = 2;
            this.detailedTagView.UseCompatibleStateImageBehavior = false;
            this.detailedTagView.View = System.Windows.Forms.View.Details;
            this.detailedTagView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.detailedTagView_KeyDown);
            // 
            // colEPC_1
            // 
            this.colEPC_1.Text = "EPC";
            this.colEPC_1.Width = 216;
            // 
            // colTR_1
            // 
            this.colTR_1.Text = "Times Read";
            this.colTR_1.Width = 75;
            // 
            // colTSSI
            // 
            this.colTSSI.Text = "TSSI";
            this.colTSSI.Width = 57;
            // 
            // colTSSIAvg
            // 
            this.colTSSIAvg.Text = "TSSI Avg";
            // 
            // colSCDE
            // 
            this.colSCDE.Text = "SCDE (RT)";
            this.colSCDE.Width = 71;
            // 
            // colSCDEAvg
            // 
            this.colSCDEAvg.Text = "SCDE Avg";
            this.colSCDEAvg.Width = 109;
            // 
            // colStdDev
            // 
            this.colStdDev.Text = "SCDE Std Dev";
            this.colStdDev.Width = 81;
            // 
            // colTS
            // 
            this.colTS.Text = "Tag State";
            this.colTS.Width = 111;
            // 
            // colTemp
            // 
            this.colTemp.Text = "Temperature";
            this.colTemp.Width = 111;
            // 
            // mnuClipboardActions
            // 
            this.mnuClipboardActions.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.mnuClipboardActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.copyCtrlCToolStripMenuItem});
            this.mnuClipboardActions.Name = "mnuClipboardActions";
            this.mnuClipboardActions.Size = new System.Drawing.Size(262, 72);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(261, 34);
            this.selectAllToolStripMenuItem.Text = "Select All     Ctrl+A";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // copyCtrlCToolStripMenuItem
            // 
            this.copyCtrlCToolStripMenuItem.Name = "copyCtrlCToolStripMenuItem";
            this.copyCtrlCToolStripMenuItem.Size = new System.Drawing.Size(261, 34);
            this.copyCtrlCToolStripMenuItem.Text = "Copy           Ctrl+C";
            this.copyCtrlCToolStripMenuItem.Click += new System.EventHandler(this.copyCtrlCToolStripMenuItem_Click);
            // 
            // panelCalInfo
            // 
            this.panelCalInfo.Controls.Add(this.btnCalInfoBack);
            this.panelCalInfo.Controls.Add(this.richTextBox1);
            this.panelCalInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCalInfo.Location = new System.Drawing.Point(0, 0);
            this.panelCalInfo.Margin = new System.Windows.Forms.Padding(6);
            this.panelCalInfo.Name = "panelCalInfo";
            this.panelCalInfo.Size = new System.Drawing.Size(1769, 1036);
            this.panelCalInfo.TabIndex = 45;
            // 
            // btnCalInfoBack
            // 
            this.btnCalInfoBack.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCalInfoBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(202)))));
            this.btnCalInfoBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalInfoBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnCalInfoBack.ForeColor = System.Drawing.Color.White;
            this.btnCalInfoBack.Location = new System.Drawing.Point(700, 873);
            this.btnCalInfoBack.Margin = new System.Windows.Forms.Padding(6);
            this.btnCalInfoBack.Name = "btnCalInfoBack";
            this.btnCalInfoBack.Size = new System.Drawing.Size(323, 100);
            this.btnCalInfoBack.TabIndex = 2;
            this.btnCalInfoBack.Text = "Back";
            this.btnCalInfoBack.UseVisualStyleBackColor = false;
            this.btnCalInfoBack.Click += new System.EventHandler(this.btnCalInfoBack_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(101, 68);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1570, 783);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // sqLiteCommand1
            // 
            this.sqLiteCommand1.CommandText = null;
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // MSRC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1769, 1036);
            this.Controls.Add(this.main);
            this.Controls.Add(this.tagListDisplay);
            this.Controls.Add(this.admin);
            this.Controls.Add(this.panelCalInfo);
            this.Controls.Add(this.MapRoof);
            this.Controls.Add(this.wetTagList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "MSRC";
            this.Text = "Sensor Console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MSRC_FormClosing);
            panelTagData.ResumeLayout(false);
            this.main.ResumeLayout(false);
            this.gbCompanyInformation.ResumeLayout(false);
            this.gbCompanyInformation.PerformLayout();
            this.gbSettings.ResumeLayout(false);
            this.gbSelCol.ResumeLayout(false);
            this.gbSelCol.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTSSIMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTSSIMax)).EndInit();
            this.AdminMapRoof.ResumeLayout(false);
            this.gbPowerRange.ResumeLayout(false);
            this.gbPowerRange.PerformLayout();
            this.gbActiveAntennas.ResumeLayout(false);
            this.gbActiveAntennas.PerformLayout();
            this.MapRoof.ResumeLayout(false);
            this.MapRoof.PerformLayout();
            this.admin.ResumeLayout(false);
            this.admin.PerformLayout();
            this.wetTagList.ResumeLayout(false);
            this.tagListDisplay.ResumeLayout(false);
            this.tagListDisplay.PerformLayout();
            this.mnuClipboardActions.ResumeLayout(false);
            this.panelCalInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel main;
        public System.Windows.Forms.Button btnMapRoof;
        public System.Windows.Forms.Panel MapRoof;
        public System.Windows.Forms.Button btnChangeSettings;
        public System.Windows.Forms.TextBox txtState;
        public System.Windows.Forms.TextBox txtCity;
        public System.Windows.Forms.TextBox txtAddress;
        public System.Windows.Forms.TextBox txtBuildingNumb;
        public System.Windows.Forms.TextBox txtCompanyName;
        public System.Windows.Forms.Panel AdminMapRoof;
        public System.Windows.Forms.Button readingStatus;
        public System.Windows.Forms.Button moistureStatus;
        public System.Windows.Forms.Button tagReadStatus;
        public System.Windows.Forms.Button dampStatus;
        public System.Windows.Forms.Button dryStatus;
        public System.Windows.Forms.Button btnPauseStartReading;
        public System.Windows.Forms.Button btnClearTagList;
        public System.Windows.Forms.Label lblWet;
        public System.Windows.Forms.Label lblDamp;
        public System.Windows.Forms.Label lblDry;
        public System.Windows.Forms.Button wetStatus;
        public System.Windows.Forms.TextBox txtZip;
        public System.Windows.Forms.ListView lstTagView;
        public System.Windows.Forms.ColumnHeader reading;
        public System.Windows.Forms.ColumnHeader Number;
        public GridLayout tagGridLayout;
        public System.Windows.Forms.Panel admin;
        public System.Windows.Forms.ColumnHeader rdrName;
        public System.Windows.Forms.ColumnHeader rdrIP;
        public System.Windows.Forms.ColumnHeader rdrMAC;
        public System.Windows.Forms.ColumnHeader rdrActive;
        public System.Windows.Forms.ListView Readers;
        public System.Windows.Forms.Button btnClearReaderList;
        private System.Windows.Forms.Button btnAddLocation;
        public System.Windows.Forms.Label lblCompanyName;
        public System.Windows.Forms.Label lblBuildingNumber;
        private System.Windows.Forms.Label lblStreetAddress;
        private System.Windows.Forms.Label lblCity;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblZipCode;
        private System.Windows.Forms.Label companyAddress;
        private System.Windows.Forms.Label buildingNumber;
        private System.Windows.Forms.Label companyName;
        private System.Windows.Forms.Label buildingNumberValue;
        private System.Windows.Forms.Label companyAddressValue;
        private System.Windows.Forms.Label companyNameValue;
        private System.Windows.Forms.Panel wetTagList;
        public System.Windows.Forms.ListView lstWetTags;
        public System.Windows.Forms.ColumnHeader colEPC;
        public System.Windows.Forms.ColumnHeader colTR;
        public System.Windows.Forms.ColumnHeader colLAT;
        public System.Windows.Forms.ColumnHeader colLON;
        private System.Windows.Forms.ColumnHeader colTC;
        public System.Windows.Forms.Button btnReturnHome;
        private System.Windows.Forms.GroupBox gbActiveAntennas;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Panel tagListDisplay;
        public System.Windows.Forms.ListView detailedTagView;
        public System.Windows.Forms.ColumnHeader colEPC_1;
        public System.Windows.Forms.ColumnHeader colTR_1;
        public System.Windows.Forms.ColumnHeader colTSSI;
        public System.Windows.Forms.ColumnHeader colSCDE;
        private System.Windows.Forms.ColumnHeader colTS;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColumnHeader colSCDEAvg;
        private System.Windows.Forms.Button btnDetailedView;
        private System.Windows.Forms.ColumnHeader colTemp;
        private System.Windows.Forms.GroupBox gbCompanyInformation;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.GroupBox gbPowerRange;
        private System.Windows.Forms.Label lblMinPower;
        private System.Windows.Forms.Label lblMaxPower;
        private System.Windows.Forms.ComboBox cbxMinPower;
        private System.Windows.Forms.ComboBox cbxMaxPower;
        public System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lblUserPrompt;
        private System.Windows.Forms.Button btnCalInfo;
        private System.Windows.Forms.Panel panelCalInfo;
        public System.Windows.Forms.Button btnCalInfoBack;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label lblMoistureInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudTSSIMin;
        private System.Windows.Forms.NumericUpDown nudTSSIMax;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radMagnusS3;
        private System.Windows.Forms.RadioButton radMagnusS2;
        private System.Data.SQLite.SQLiteCommand sqLiteCommand1;
        private System.Windows.Forms.ColumnHeader colTSSIAvg;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private System.Windows.Forms.ColumnHeader colStdDev;
        public System.Windows.Forms.Button btnClearTagListDetailPage;
        private System.Windows.Forms.Label lblReportedElapsedTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblRequestedReadTime;
        private System.Windows.Forms.TextBox txtRequestedReadTime;
        private System.Windows.Forms.GroupBox gbSelCol;
        private System.Windows.Forms.CheckBox cbTemp;
        private System.Windows.Forms.CheckBox cbTS;
        private System.Windows.Forms.CheckBox cbSTdDev;
        private System.Windows.Forms.CheckBox cbSCDEAvg;
        private System.Windows.Forms.CheckBox cbSCDE;
        private System.Windows.Forms.CheckBox cbTSSIAvg;
        private System.Windows.Forms.CheckBox cbTSSI;
        private System.Windows.Forms.CheckBox cbTR;
        private System.Windows.Forms.ContextMenuStrip mnuClipboardActions;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyCtrlCToolStripMenuItem;
    }
}

