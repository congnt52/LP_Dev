namespace _5GAutoTool
{
    partial class SetupVSG
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupVSG));
            this.cboIP1 = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.grpVSG = new System.Windows.Forms.GroupBox();
            this.lblStatus3 = new System.Windows.Forms.Label();
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.lblStatus1 = new System.Windows.Forms.Label();
            this.lblConnStatus3 = new System.Windows.Forms.Label();
            this.lblConnStatus2 = new System.Windows.Forms.Label();
            this.lblConnStatus1 = new System.Windows.Forms.Label();
            this.lbSearialNumber3 = new System.Windows.Forms.Label();
            this.lbSearialNumber2 = new System.Windows.Forms.Label();
            this.lbSearialNumber1 = new System.Windows.Forms.Label();
            this.lblSerial3 = new System.Windows.Forms.Label();
            this.lblSerial2 = new System.Windows.Forms.Label();
            this.lblSerial1 = new System.Windows.Forms.Label();
            this.lblModel3 = new System.Windows.Forms.Label();
            this.lblModel2 = new System.Windows.Forms.Label();
            this.lblModel1 = new System.Windows.Forms.Label();
            this.lblModelText3 = new System.Windows.Forms.Label();
            this.lblModelText2 = new System.Windows.Forms.Label();
            this.lblModelText1 = new System.Windows.Forms.Label();
            this.lblMfrName3 = new System.Windows.Forms.Label();
            this.lblMfrName2 = new System.Windows.Forms.Label();
            this.lblMfrName1 = new System.Windows.Forms.Label();
            this.lblManufacturer3 = new System.Windows.Forms.Label();
            this.lblManufacturer2 = new System.Windows.Forms.Label();
            this.lblManufacturer1 = new System.Windows.Forms.Label();
            this.picVSG3 = new System.Windows.Forms.PictureBox();
            this.picVSG2 = new System.Windows.Forms.PictureBox();
            this.picVSG = new System.Windows.Forms.PictureBox();
            this.btnCheckInf = new System.Windows.Forms.Button();
            this.txtRespond = new System.Windows.Forms.TextBox();
            this.lblResponse = new System.Windows.Forms.Label();
            this.lblConnPort = new System.Windows.Forms.Label();
            this.lblCmdText = new System.Windows.Forms.Label();
            this.lblInterface = new System.Windows.Forms.Label();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.cboInterface = new System.Windows.Forms.ComboBox();
            this.cboVSGList1 = new System.Windows.Forms.ComboBox();
            this.lblAddress1 = new System.Windows.Forms.Label();
            this.lblVSGName1 = new System.Windows.Forms.Label();
            this.cboCmd = new System.Windows.Forms.ComboBox();
            this.lblVSGName2 = new System.Windows.Forms.Label();
            this.cboVSGList2 = new System.Windows.Forms.ComboBox();
            this.lblAddress2 = new System.Windows.Forms.Label();
            this.cboIP2 = new System.Windows.Forms.ComboBox();
            this.lblVSGName3 = new System.Windows.Forms.Label();
            this.cboVSGList3 = new System.Windows.Forms.ComboBox();
            this.lblAddress3 = new System.Windows.Forms.Label();
            this.cboIP3 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SL_1VSG = new System.Windows.Forms.RadioButton();
            this.SL_2VSG = new System.Windows.Forms.RadioButton();
            this.SL_3VSG = new System.Windows.Forms.RadioButton();
            this.cboVSG = new System.Windows.Forms.ComboBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.grpVSG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVSG3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVSG2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVSG)).BeginInit();
            this.SuspendLayout();
            // 
            // cboIP1
            // 
            this.cboIP1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboIP1.FormattingEnabled = true;
            this.cboIP1.Items.AddRange(new object[] {
            "10.0.0.222"});
            this.cboIP1.Location = new System.Drawing.Point(372, 62);
            this.cboIP1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboIP1.Name = "cboIP1";
            this.cboIP1.Size = new System.Drawing.Size(180, 24);
            this.cboIP1.TabIndex = 41;
            this.cboIP1.Text = "3";
            this.cboIP1.SelectedIndexChanged += new System.EventHandler(this.cboIP1_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Location = new System.Drawing.Point(461, 296);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(93, 39);
            this.btnOk.TabIndex = 40;
            this.btnOk.Text = "CLOSE";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.btnSend.Location = new System.Drawing.Point(461, 226);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(93, 22);
            this.btnSend.TabIndex = 38;
            this.btnSend.Text = "SEND";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // grpVSG
            // 
            this.grpVSG.Controls.Add(this.lblStatus3);
            this.grpVSG.Controls.Add(this.lblStatus2);
            this.grpVSG.Controls.Add(this.lblStatus1);
            this.grpVSG.Controls.Add(this.lblConnStatus3);
            this.grpVSG.Controls.Add(this.lblConnStatus2);
            this.grpVSG.Controls.Add(this.lblConnStatus1);
            this.grpVSG.Controls.Add(this.lbSearialNumber3);
            this.grpVSG.Controls.Add(this.lbSearialNumber2);
            this.grpVSG.Controls.Add(this.lbSearialNumber1);
            this.grpVSG.Controls.Add(this.lblSerial3);
            this.grpVSG.Controls.Add(this.lblSerial2);
            this.grpVSG.Controls.Add(this.lblSerial1);
            this.grpVSG.Controls.Add(this.lblModel3);
            this.grpVSG.Controls.Add(this.lblModel2);
            this.grpVSG.Controls.Add(this.lblModel1);
            this.grpVSG.Controls.Add(this.lblModelText3);
            this.grpVSG.Controls.Add(this.lblModelText2);
            this.grpVSG.Controls.Add(this.lblModelText1);
            this.grpVSG.Controls.Add(this.lblMfrName3);
            this.grpVSG.Controls.Add(this.lblMfrName2);
            this.grpVSG.Controls.Add(this.lblMfrName1);
            this.grpVSG.Controls.Add(this.lblManufacturer3);
            this.grpVSG.Controls.Add(this.lblManufacturer2);
            this.grpVSG.Controls.Add(this.lblManufacturer1);
            this.grpVSG.Controls.Add(this.picVSG3);
            this.grpVSG.Controls.Add(this.picVSG2);
            this.grpVSG.Controls.Add(this.picVSG);
            this.grpVSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpVSG.Location = new System.Drawing.Point(588, 31);
            this.grpVSG.Name = "grpVSG";
            this.grpVSG.Size = new System.Drawing.Size(472, 304);
            this.grpVSG.TabIndex = 37;
            this.grpVSG.TabStop = false;
            this.grpVSG.Text = "Instruments";
            // 
            // lblStatus3
            // 
            this.lblStatus3.AutoSize = true;
            this.lblStatus3.ForeColor = System.Drawing.Color.Red;
            this.lblStatus3.Location = new System.Drawing.Point(351, 266);
            this.lblStatus3.Name = "lblStatus3";
            this.lblStatus3.Size = new System.Drawing.Size(98, 20);
            this.lblStatus3.TabIndex = 7;
            this.lblStatus3.Text = "Not Connect";
            // 
            // lblStatus2
            // 
            this.lblStatus2.AutoSize = true;
            this.lblStatus2.ForeColor = System.Drawing.Color.Red;
            this.lblStatus2.Location = new System.Drawing.Point(351, 174);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(98, 20);
            this.lblStatus2.TabIndex = 7;
            this.lblStatus2.Text = "Not Connect";
            // 
            // lblStatus1
            // 
            this.lblStatus1.AutoSize = true;
            this.lblStatus1.ForeColor = System.Drawing.Color.Red;
            this.lblStatus1.Location = new System.Drawing.Point(351, 84);
            this.lblStatus1.Name = "lblStatus1";
            this.lblStatus1.Size = new System.Drawing.Size(98, 20);
            this.lblStatus1.TabIndex = 7;
            this.lblStatus1.Text = "Not Connect";
            // 
            // lblConnStatus3
            // 
            this.lblConnStatus3.AutoSize = true;
            this.lblConnStatus3.Location = new System.Drawing.Point(225, 266);
            this.lblConnStatus3.Name = "lblConnStatus3";
            this.lblConnStatus3.Size = new System.Drawing.Size(60, 20);
            this.lblConnStatus3.TabIndex = 7;
            this.lblConnStatus3.Text = "Status:";
            // 
            // lblConnStatus2
            // 
            this.lblConnStatus2.AutoSize = true;
            this.lblConnStatus2.Location = new System.Drawing.Point(225, 174);
            this.lblConnStatus2.Name = "lblConnStatus2";
            this.lblConnStatus2.Size = new System.Drawing.Size(60, 20);
            this.lblConnStatus2.TabIndex = 7;
            this.lblConnStatus2.Text = "Status:";
            // 
            // lblConnStatus1
            // 
            this.lblConnStatus1.AutoSize = true;
            this.lblConnStatus1.Location = new System.Drawing.Point(225, 84);
            this.lblConnStatus1.Name = "lblConnStatus1";
            this.lblConnStatus1.Size = new System.Drawing.Size(60, 20);
            this.lblConnStatus1.TabIndex = 7;
            this.lblConnStatus1.Text = "Status:";
            // 
            // lbSearialNumber3
            // 
            this.lbSearialNumber3.AutoSize = true;
            this.lbSearialNumber3.Location = new System.Drawing.Point(351, 246);
            this.lbSearialNumber3.Name = "lbSearialNumber3";
            this.lbSearialNumber3.Size = new System.Drawing.Size(39, 20);
            this.lbSearialNumber3.TabIndex = 7;
            this.lbSearialNumber3.Text = "*****";
            // 
            // lbSearialNumber2
            // 
            this.lbSearialNumber2.AutoSize = true;
            this.lbSearialNumber2.Location = new System.Drawing.Point(351, 154);
            this.lbSearialNumber2.Name = "lbSearialNumber2";
            this.lbSearialNumber2.Size = new System.Drawing.Size(39, 20);
            this.lbSearialNumber2.TabIndex = 7;
            this.lbSearialNumber2.Text = "*****";
            // 
            // lbSearialNumber1
            // 
            this.lbSearialNumber1.AutoSize = true;
            this.lbSearialNumber1.Location = new System.Drawing.Point(351, 64);
            this.lbSearialNumber1.Name = "lbSearialNumber1";
            this.lbSearialNumber1.Size = new System.Drawing.Size(39, 20);
            this.lbSearialNumber1.TabIndex = 7;
            this.lbSearialNumber1.Text = "*****";
            // 
            // lblSerial3
            // 
            this.lblSerial3.AutoSize = true;
            this.lblSerial3.Location = new System.Drawing.Point(225, 246);
            this.lblSerial3.Name = "lblSerial3";
            this.lblSerial3.Size = new System.Drawing.Size(113, 20);
            this.lblSerial3.TabIndex = 7;
            this.lblSerial3.Text = "Serial Number:";
            // 
            // lblSerial2
            // 
            this.lblSerial2.AutoSize = true;
            this.lblSerial2.Location = new System.Drawing.Point(225, 154);
            this.lblSerial2.Name = "lblSerial2";
            this.lblSerial2.Size = new System.Drawing.Size(113, 20);
            this.lblSerial2.TabIndex = 7;
            this.lblSerial2.Text = "Serial Number:";
            // 
            // lblSerial1
            // 
            this.lblSerial1.AutoSize = true;
            this.lblSerial1.Location = new System.Drawing.Point(225, 64);
            this.lblSerial1.Name = "lblSerial1";
            this.lblSerial1.Size = new System.Drawing.Size(113, 20);
            this.lblSerial1.TabIndex = 7;
            this.lblSerial1.Text = "Serial Number:";
            // 
            // lblModel3
            // 
            this.lblModel3.AutoSize = true;
            this.lblModel3.Location = new System.Drawing.Point(351, 227);
            this.lblModel3.Name = "lblModel3";
            this.lblModel3.Size = new System.Drawing.Size(39, 20);
            this.lblModel3.TabIndex = 7;
            this.lblModel3.Text = "*****";
            // 
            // lblModel2
            // 
            this.lblModel2.AutoSize = true;
            this.lblModel2.Location = new System.Drawing.Point(351, 135);
            this.lblModel2.Name = "lblModel2";
            this.lblModel2.Size = new System.Drawing.Size(39, 20);
            this.lblModel2.TabIndex = 7;
            this.lblModel2.Text = "*****";
            // 
            // lblModel1
            // 
            this.lblModel1.AutoSize = true;
            this.lblModel1.Location = new System.Drawing.Point(351, 45);
            this.lblModel1.Name = "lblModel1";
            this.lblModel1.Size = new System.Drawing.Size(39, 20);
            this.lblModel1.TabIndex = 7;
            this.lblModel1.Text = "*****";
            // 
            // lblModelText3
            // 
            this.lblModelText3.AutoSize = true;
            this.lblModelText3.Location = new System.Drawing.Point(225, 227);
            this.lblModelText3.Name = "lblModelText3";
            this.lblModelText3.Size = new System.Drawing.Size(56, 20);
            this.lblModelText3.TabIndex = 7;
            this.lblModelText3.Text = "Model:";
            // 
            // lblModelText2
            // 
            this.lblModelText2.AutoSize = true;
            this.lblModelText2.Location = new System.Drawing.Point(225, 135);
            this.lblModelText2.Name = "lblModelText2";
            this.lblModelText2.Size = new System.Drawing.Size(56, 20);
            this.lblModelText2.TabIndex = 7;
            this.lblModelText2.Text = "Model:";
            // 
            // lblModelText1
            // 
            this.lblModelText1.AutoSize = true;
            this.lblModelText1.Location = new System.Drawing.Point(225, 45);
            this.lblModelText1.Name = "lblModelText1";
            this.lblModelText1.Size = new System.Drawing.Size(56, 20);
            this.lblModelText1.TabIndex = 7;
            this.lblModelText1.Text = "Model:";
            // 
            // lblMfrName3
            // 
            this.lblMfrName3.AutoSize = true;
            this.lblMfrName3.Location = new System.Drawing.Point(351, 207);
            this.lblMfrName3.Name = "lblMfrName3";
            this.lblMfrName3.Size = new System.Drawing.Size(39, 20);
            this.lblMfrName3.TabIndex = 7;
            this.lblMfrName3.Text = "*****";
            // 
            // lblMfrName2
            // 
            this.lblMfrName2.AutoSize = true;
            this.lblMfrName2.Location = new System.Drawing.Point(351, 115);
            this.lblMfrName2.Name = "lblMfrName2";
            this.lblMfrName2.Size = new System.Drawing.Size(39, 20);
            this.lblMfrName2.TabIndex = 7;
            this.lblMfrName2.Text = "*****";
            // 
            // lblMfrName1
            // 
            this.lblMfrName1.AutoSize = true;
            this.lblMfrName1.Location = new System.Drawing.Point(351, 25);
            this.lblMfrName1.Name = "lblMfrName1";
            this.lblMfrName1.Size = new System.Drawing.Size(39, 20);
            this.lblMfrName1.TabIndex = 7;
            this.lblMfrName1.Text = "*****";
            // 
            // lblManufacturer3
            // 
            this.lblManufacturer3.AutoSize = true;
            this.lblManufacturer3.Location = new System.Drawing.Point(225, 207);
            this.lblManufacturer3.Name = "lblManufacturer3";
            this.lblManufacturer3.Size = new System.Drawing.Size(108, 20);
            this.lblManufacturer3.TabIndex = 7;
            this.lblManufacturer3.Text = "Manufacturer:";
            // 
            // lblManufacturer2
            // 
            this.lblManufacturer2.AutoSize = true;
            this.lblManufacturer2.Location = new System.Drawing.Point(225, 115);
            this.lblManufacturer2.Name = "lblManufacturer2";
            this.lblManufacturer2.Size = new System.Drawing.Size(108, 20);
            this.lblManufacturer2.TabIndex = 7;
            this.lblManufacturer2.Text = "Manufacturer:";
            // 
            // lblManufacturer1
            // 
            this.lblManufacturer1.AutoSize = true;
            this.lblManufacturer1.Location = new System.Drawing.Point(225, 25);
            this.lblManufacturer1.Name = "lblManufacturer1";
            this.lblManufacturer1.Size = new System.Drawing.Size(108, 20);
            this.lblManufacturer1.TabIndex = 7;
            this.lblManufacturer1.Text = "Manufacturer:";
            // 
            // picVSG3
            // 
            this.picVSG3.Image = ((System.Drawing.Image)(resources.GetObject("picVSG3.Image")));
            this.picVSG3.InitialImage = ((System.Drawing.Image)(resources.GetObject("picVSG3.InitialImage")));
            this.picVSG3.Location = new System.Drawing.Point(17, 213);
            this.picVSG3.Name = "picVSG3";
            this.picVSG3.Size = new System.Drawing.Size(198, 73);
            this.picVSG3.TabIndex = 6;
            this.picVSG3.TabStop = false;
            // 
            // picVSG2
            // 
            this.picVSG2.Image = ((System.Drawing.Image)(resources.GetObject("picVSG2.Image")));
            this.picVSG2.InitialImage = ((System.Drawing.Image)(resources.GetObject("picVSG2.InitialImage")));
            this.picVSG2.Location = new System.Drawing.Point(17, 121);
            this.picVSG2.Name = "picVSG2";
            this.picVSG2.Size = new System.Drawing.Size(198, 73);
            this.picVSG2.TabIndex = 6;
            this.picVSG2.TabStop = false;
            // 
            // picVSG
            // 
            this.picVSG.Image = ((System.Drawing.Image)(resources.GetObject("picVSG.Image")));
            this.picVSG.InitialImage = ((System.Drawing.Image)(resources.GetObject("picVSG.InitialImage")));
            this.picVSG.Location = new System.Drawing.Point(17, 31);
            this.picVSG.Name = "picVSG";
            this.picVSG.Size = new System.Drawing.Size(198, 73);
            this.picVSG.TabIndex = 6;
            this.picVSG.TabStop = false;
            // 
            // btnCheckInf
            // 
            this.btnCheckInf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckInf.Location = new System.Drawing.Point(198, 297);
            this.btnCheckInf.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCheckInf.Name = "btnCheckInf";
            this.btnCheckInf.Size = new System.Drawing.Size(95, 38);
            this.btnCheckInf.TabIndex = 36;
            this.btnCheckInf.Text = "CONNECT";
            this.btnCheckInf.UseVisualStyleBackColor = true;
            this.btnCheckInf.Click += new System.EventHandler(this.btnCheckInf_Click);
            // 
            // txtRespond
            // 
            this.txtRespond.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRespond.Location = new System.Drawing.Point(198, 264);
            this.txtRespond.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRespond.Name = "txtRespond";
            this.txtRespond.Size = new System.Drawing.Size(356, 22);
            this.txtRespond.TabIndex = 35;
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResponse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblResponse.Location = new System.Drawing.Point(29, 263);
            this.lblResponse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(92, 24);
            this.lblResponse.TabIndex = 25;
            this.lblResponse.Text = "Respond:";
            // 
            // lblConnPort
            // 
            this.lblConnPort.AutoSize = true;
            this.lblConnPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnPort.Location = new System.Drawing.Point(305, 186);
            this.lblConnPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnPort.Name = "lblConnPort";
            this.lblConnPort.Size = new System.Drawing.Size(48, 24);
            this.lblConnPort.TabIndex = 28;
            this.lblConnPort.Text = "Port:";
            // 
            // lblCmdText
            // 
            this.lblCmdText.AutoSize = true;
            this.lblCmdText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCmdText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCmdText.Location = new System.Drawing.Point(29, 225);
            this.lblCmdText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCmdText.Name = "lblCmdText";
            this.lblCmdText.Size = new System.Drawing.Size(164, 24);
            this.lblCmdText.TabIndex = 27;
            this.lblCmdText.Text = "Verify Connection:";
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterface.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInterface.Location = new System.Drawing.Point(30, 186);
            this.lblInterface.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(86, 24);
            this.lblInterface.TabIndex = 26;
            this.lblInterface.Text = "Interface:";
            // 
            // cboPort
            // 
            this.cboPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Items.AddRange(new object[] {
            "23",
            "22"});
            this.cboPort.Location = new System.Drawing.Point(372, 186);
            this.cboPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(180, 24);
            this.cboPort.TabIndex = 31;
            // 
            // cboInterface
            // 
            this.cboInterface.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboInterface.FormattingEnabled = true;
            this.cboInterface.Items.AddRange(new object[] {
            "TCPIB",
            "GPIB"});
            this.cboInterface.Location = new System.Drawing.Point(199, 186);
            this.cboInterface.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboInterface.Name = "cboInterface";
            this.cboInterface.Size = new System.Drawing.Size(94, 24);
            this.cboInterface.TabIndex = 32;
            // 
            // cboVSGList1
            // 
            this.cboVSGList1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboVSGList1.FormattingEnabled = true;
            this.cboVSGList1.Items.AddRange(new object[] {
            "M9410A",
            "N5182B",
            "SMW200A",
            "CMW100"});
            this.cboVSGList1.Location = new System.Drawing.Point(198, 62);
            this.cboVSGList1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboVSGList1.Name = "cboVSGList1";
            this.cboVSGList1.Size = new System.Drawing.Size(93, 24);
            this.cboVSGList1.TabIndex = 33;
            this.cboVSGList1.Text = "M9410A";
            this.cboVSGList1.TextChanged += new System.EventHandler(this.cboInstrument_TextChanged);
            // 
            // lblAddress1
            // 
            this.lblAddress1.AutoSize = true;
            this.lblAddress1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddress1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAddress1.Location = new System.Drawing.Point(303, 62);
            this.lblAddress1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddress1.Name = "lblAddress1";
            this.lblAddress1.Size = new System.Drawing.Size(46, 24);
            this.lblAddress1.TabIndex = 29;
            this.lblAddress1.Text = "IP 1:";
            // 
            // lblVSGName1
            // 
            this.lblVSGName1.AutoSize = true;
            this.lblVSGName1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVSGName1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVSGName1.Location = new System.Drawing.Point(29, 62);
            this.lblVSGName1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVSGName1.Name = "lblVSGName1";
            this.lblVSGName1.Size = new System.Drawing.Size(69, 24);
            this.lblVSGName1.TabIndex = 30;
            this.lblVSGName1.Text = "VSG 1:";
            // 
            // cboCmd
            // 
            this.cboCmd.FormattingEnabled = true;
            this.cboCmd.Items.AddRange(new object[] {
            "*IDN?",
            "*OPC?",
            "FREQ 3700000000 Hz",
            ":POW -10DBM"});
            this.cboCmd.Location = new System.Drawing.Point(307, 225);
            this.cboCmd.Name = "cboCmd";
            this.cboCmd.Size = new System.Drawing.Size(143, 21);
            this.cboCmd.TabIndex = 44;
            this.cboCmd.Text = "*IDN?";
            // 
            // lblVSGName2
            // 
            this.lblVSGName2.AutoSize = true;
            this.lblVSGName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVSGName2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVSGName2.Location = new System.Drawing.Point(29, 102);
            this.lblVSGName2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVSGName2.Name = "lblVSGName2";
            this.lblVSGName2.Size = new System.Drawing.Size(69, 24);
            this.lblVSGName2.TabIndex = 30;
            this.lblVSGName2.Text = "VSG 2:";
            // 
            // cboVSGList2
            // 
            this.cboVSGList2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboVSGList2.FormattingEnabled = true;
            this.cboVSGList2.Items.AddRange(new object[] {
            "N5182B",
            "SMW200A",
            "SGS100A"});
            this.cboVSGList2.Location = new System.Drawing.Point(198, 102);
            this.cboVSGList2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboVSGList2.Name = "cboVSGList2";
            this.cboVSGList2.Size = new System.Drawing.Size(93, 24);
            this.cboVSGList2.TabIndex = 33;
            this.cboVSGList2.Text = "N5182B";
            this.cboVSGList2.TextChanged += new System.EventHandler(this.cboInstrument_TextChanged);
            // 
            // lblAddress2
            // 
            this.lblAddress2.AutoSize = true;
            this.lblAddress2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddress2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAddress2.Location = new System.Drawing.Point(303, 102);
            this.lblAddress2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddress2.Name = "lblAddress2";
            this.lblAddress2.Size = new System.Drawing.Size(46, 24);
            this.lblAddress2.TabIndex = 29;
            this.lblAddress2.Text = "IP 2:";
            // 
            // cboIP2
            // 
            this.cboIP2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboIP2.FormattingEnabled = true;
            this.cboIP2.Items.AddRange(new object[] {
            "10.61.61.107",
            "10.61.61.108",
            "10.61.61.109"});
            this.cboIP2.Location = new System.Drawing.Point(372, 102);
            this.cboIP2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboIP2.Name = "cboIP2";
            this.cboIP2.Size = new System.Drawing.Size(178, 24);
            this.cboIP2.TabIndex = 41;
            this.cboIP2.Text = "10.0.0.244";
            this.cboIP2.SelectedIndexChanged += new System.EventHandler(this.cboIP2_SelectedIndexChanged);
            // 
            // lblVSGName3
            // 
            this.lblVSGName3.AutoSize = true;
            this.lblVSGName3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVSGName3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVSGName3.Location = new System.Drawing.Point(31, 144);
            this.lblVSGName3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVSGName3.Name = "lblVSGName3";
            this.lblVSGName3.Size = new System.Drawing.Size(69, 24);
            this.lblVSGName3.TabIndex = 30;
            this.lblVSGName3.Text = "VSG 3:";
            // 
            // cboVSGList3
            // 
            this.cboVSGList3.Enabled = false;
            this.cboVSGList3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboVSGList3.FormattingEnabled = true;
            this.cboVSGList3.Items.AddRange(new object[] {
            "N5182B",
            "SMW200A",
            "SGS100A"});
            this.cboVSGList3.Location = new System.Drawing.Point(200, 144);
            this.cboVSGList3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboVSGList3.Name = "cboVSGList3";
            this.cboVSGList3.Size = new System.Drawing.Size(93, 24);
            this.cboVSGList3.TabIndex = 33;
            this.cboVSGList3.Text = "N5182B";
            this.cboVSGList3.TextChanged += new System.EventHandler(this.cboInstrument_TextChanged);
            // 
            // lblAddress3
            // 
            this.lblAddress3.AutoSize = true;
            this.lblAddress3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddress3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAddress3.Location = new System.Drawing.Point(305, 144);
            this.lblAddress3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddress3.Name = "lblAddress3";
            this.lblAddress3.Size = new System.Drawing.Size(46, 24);
            this.lblAddress3.TabIndex = 29;
            this.lblAddress3.Text = "IP 3:";
            // 
            // cboIP3
            // 
            this.cboIP3.Enabled = false;
            this.cboIP3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboIP3.FormattingEnabled = true;
            this.cboIP3.Items.AddRange(new object[] {
            "10.61.61.107",
            "10.61.61.108",
            "10.61.61.109"});
            this.cboIP3.Location = new System.Drawing.Point(372, 144);
            this.cboIP3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboIP3.Name = "cboIP3";
            this.cboIP3.Size = new System.Drawing.Size(180, 24);
            this.cboIP3.TabIndex = 41;
            this.cboIP3.SelectedIndexChanged += new System.EventHandler(this.cboIP3_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(29, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 24);
            this.label1.TabIndex = 30;
            this.label1.Text = "Số lượng:";
            // 
            // SL_1VSG
            // 
            this.SL_1VSG.AutoSize = true;
            this.SL_1VSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SL_1VSG.Location = new System.Drawing.Point(198, 21);
            this.SL_1VSG.Name = "SL_1VSG";
            this.SL_1VSG.Size = new System.Drawing.Size(80, 24);
            this.SL_1VSG.TabIndex = 45;
            this.SL_1VSG.Text = "1 VSG";
            this.SL_1VSG.UseVisualStyleBackColor = true;
            this.SL_1VSG.CheckedChanged += new System.EventHandler(this.SL_1VSG_CheckedChanged);
            // 
            // SL_2VSG
            // 
            this.SL_2VSG.AutoSize = true;
            this.SL_2VSG.Checked = true;
            this.SL_2VSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SL_2VSG.Location = new System.Drawing.Point(307, 21);
            this.SL_2VSG.Name = "SL_2VSG";
            this.SL_2VSG.Size = new System.Drawing.Size(80, 24);
            this.SL_2VSG.TabIndex = 45;
            this.SL_2VSG.TabStop = true;
            this.SL_2VSG.Text = "2 VSG";
            this.SL_2VSG.UseVisualStyleBackColor = true;
            this.SL_2VSG.CheckedChanged += new System.EventHandler(this.SL_2VSG_CheckedChanged);
            // 
            // SL_3VSG
            // 
            this.SL_3VSG.AutoSize = true;
            this.SL_3VSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SL_3VSG.Location = new System.Drawing.Point(416, 21);
            this.SL_3VSG.Name = "SL_3VSG";
            this.SL_3VSG.Size = new System.Drawing.Size(80, 24);
            this.SL_3VSG.TabIndex = 45;
            this.SL_3VSG.Text = "3 VSG";
            this.SL_3VSG.UseVisualStyleBackColor = true;
            this.SL_3VSG.CheckedChanged += new System.EventHandler(this.SL_3VSG_CheckedChanged);
            // 
            // cboVSG
            // 
            this.cboVSG.AutoCompleteCustomSource.AddRange(new string[] {
            "VGS1",
            "VSG2",
            "VSG3"});
            this.cboVSG.FormattingEnabled = true;
            this.cboVSG.Items.AddRange(new object[] {
            "VSG1",
            "VSG2",
            "VSG3"});
            this.cboVSG.Location = new System.Drawing.Point(198, 226);
            this.cboVSG.Name = "cboVSG";
            this.cboVSG.Size = new System.Drawing.Size(95, 21);
            this.cboVSG.TabIndex = 47;
            this.cboVSG.Text = "VSG1";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnect.Location = new System.Drawing.Point(331, 297);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(98, 38);
            this.btnDisconnect.TabIndex = 48;
            this.btnDisconnect.Text = "DISCONNECT";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // SetupVSG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 369);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.cboVSG);
            this.Controls.Add(this.SL_3VSG);
            this.Controls.Add(this.SL_2VSG);
            this.Controls.Add(this.SL_1VSG);
            this.Controls.Add(this.cboCmd);
            this.Controls.Add(this.cboIP3);
            this.Controls.Add(this.cboIP2);
            this.Controls.Add(this.cboIP1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.grpVSG);
            this.Controls.Add(this.btnCheckInf);
            this.Controls.Add(this.txtRespond);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.lblAddress3);
            this.Controls.Add(this.lblConnPort);
            this.Controls.Add(this.lblAddress2);
            this.Controls.Add(this.lblCmdText);
            this.Controls.Add(this.lblAddress1);
            this.Controls.Add(this.lblInterface);
            this.Controls.Add(this.cboVSGList3);
            this.Controls.Add(this.cboPort);
            this.Controls.Add(this.cboVSGList2);
            this.Controls.Add(this.lblVSGName3);
            this.Controls.Add(this.cboInterface);
            this.Controls.Add(this.lblVSGName2);
            this.Controls.Add(this.cboVSGList1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVSGName1);
            this.Name = "SetupVSG";
            this.Text = "VSG SET UP";
            this.grpVSG.ResumeLayout(false);
            this.grpVSG.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVSG3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVSG2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVSG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboIP1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox grpVSG;
        private System.Windows.Forms.Label lblStatus1;
        private System.Windows.Forms.Label lblConnStatus1;
        private System.Windows.Forms.Label lbSearialNumber1;
        private System.Windows.Forms.Label lblSerial1;
        private System.Windows.Forms.Label lblModel1;
        private System.Windows.Forms.Label lblModelText1;
        private System.Windows.Forms.Label lblMfrName1;
        private System.Windows.Forms.Label lblManufacturer1;
        private System.Windows.Forms.PictureBox picVSG;
        private System.Windows.Forms.Button btnCheckInf;
        private System.Windows.Forms.TextBox txtRespond;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.Label lblConnPort;
        private System.Windows.Forms.Label lblCmdText;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.ComboBox cboInterface;
        private System.Windows.Forms.ComboBox cboVSGList1;
        private System.Windows.Forms.Label lblAddress1;
        private System.Windows.Forms.Label lblVSGName1;
        private System.Windows.Forms.ComboBox cboCmd;
        private System.Windows.Forms.Label lblVSGName2;
        private System.Windows.Forms.ComboBox cboVSGList2;
        private System.Windows.Forms.Label lblAddress2;
        private System.Windows.Forms.ComboBox cboIP2;
        private System.Windows.Forms.Label lblVSGName3;
        private System.Windows.Forms.ComboBox cboVSGList3;
        private System.Windows.Forms.Label lblAddress3;
        private System.Windows.Forms.ComboBox cboIP3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton SL_1VSG;
        private System.Windows.Forms.RadioButton SL_2VSG;
        private System.Windows.Forms.RadioButton SL_3VSG;
        private System.Windows.Forms.Label lblStatus3;
        private System.Windows.Forms.Label lblStatus2;
        private System.Windows.Forms.Label lblConnStatus3;
        private System.Windows.Forms.Label lblConnStatus2;
        private System.Windows.Forms.Label lbSearialNumber3;
        private System.Windows.Forms.Label lbSearialNumber2;
        private System.Windows.Forms.Label lblSerial3;
        private System.Windows.Forms.Label lblSerial2;
        private System.Windows.Forms.Label lblModel3;
        private System.Windows.Forms.Label lblModel2;
        private System.Windows.Forms.Label lblModelText3;
        private System.Windows.Forms.Label lblModelText2;
        private System.Windows.Forms.Label lblMfrName3;
        private System.Windows.Forms.Label lblMfrName2;
        private System.Windows.Forms.Label lblManufacturer3;
        private System.Windows.Forms.Label lblManufacturer2;
        private System.Windows.Forms.PictureBox picVSG3;
        private System.Windows.Forms.PictureBox picVSG2;
        private System.Windows.Forms.ComboBox cboVSG;
        private System.Windows.Forms.Button btnDisconnect;
    }
}