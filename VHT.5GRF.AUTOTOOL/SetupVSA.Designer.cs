namespace _5GAutoTool
{
    partial class SetupVSA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupVSA));
            this.cboIP = new System.Windows.Forms.ComboBox();
            this.grpVSA = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblConnStatus = new System.Windows.Forms.Label();
            this.lbSearialNumber = new System.Windows.Forms.Label();
            this.lblSerial = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblModelText = new System.Windows.Forms.Label();
            this.lblMfrName = new System.Windows.Forms.Label();
            this.lblManufacturer = new System.Windows.Forms.Label();
            this.picVSA = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCheckInfo = new System.Windows.Forms.Button();
            this.txtRespond = new System.Windows.Forms.TextBox();
            this.lblResponse = new System.Windows.Forms.Label();
            this.lblConnPort = new System.Windows.Forms.Label();
            this.lblCmdText = new System.Windows.Forms.Label();
            this.lblInterface = new System.Windows.Forms.Label();
            this.cboInterface = new System.Windows.Forms.ComboBox();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.cboVSAList = new System.Windows.Forms.ComboBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblVSAName = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.cboCmd = new System.Windows.Forms.ComboBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.grpVSA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVSA)).BeginInit();
            this.SuspendLayout();
            // 
            // cboIP
            // 
            this.cboIP.FormattingEnabled = true;
            this.cboIP.Items.AddRange(new object[] {
            "10.61.61.201",
            "192.168.185.212",
            "192.168.185.212",
            "10.0.0.110"});
            this.cboIP.Location = new System.Drawing.Point(247, 86);
            this.cboIP.Name = "cboIP";
            this.cboIP.Size = new System.Drawing.Size(307, 24);
            this.cboIP.TabIndex = 40;
            this.cboIP.Text = "10.0.0.112";
            // 
            // grpVSA
            // 
            this.grpVSA.Controls.Add(this.lblStatus);
            this.grpVSA.Controls.Add(this.lblConnStatus);
            this.grpVSA.Controls.Add(this.lbSearialNumber);
            this.grpVSA.Controls.Add(this.lblSerial);
            this.grpVSA.Controls.Add(this.lblModel);
            this.grpVSA.Controls.Add(this.lblModelText);
            this.grpVSA.Controls.Add(this.lblMfrName);
            this.grpVSA.Controls.Add(this.lblManufacturer);
            this.grpVSA.Controls.Add(this.picVSA);
            this.grpVSA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.grpVSA.Location = new System.Drawing.Point(592, 31);
            this.grpVSA.Name = "grpVSA";
            this.grpVSA.Size = new System.Drawing.Size(386, 304);
            this.grpVSA.TabIndex = 38;
            this.grpVSA.TabStop = false;
            this.grpVSA.Text = "Instruments";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStatus.Location = new System.Drawing.Point(192, 266);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(98, 20);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Not Connect";
            // 
            // lblConnStatus
            // 
            this.lblConnStatus.AutoSize = true;
            this.lblConnStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblConnStatus.Location = new System.Drawing.Point(66, 266);
            this.lblConnStatus.Name = "lblConnStatus";
            this.lblConnStatus.Size = new System.Drawing.Size(60, 20);
            this.lblConnStatus.TabIndex = 7;
            this.lblConnStatus.Text = "Status:";
            // 
            // lbSearialNumber
            // 
            this.lbSearialNumber.AutoSize = true;
            this.lbSearialNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbSearialNumber.Location = new System.Drawing.Point(192, 235);
            this.lbSearialNumber.Name = "lbSearialNumber";
            this.lbSearialNumber.Size = new System.Drawing.Size(39, 20);
            this.lbSearialNumber.TabIndex = 7;
            this.lbSearialNumber.Text = "*****";
            // 
            // lblSerial
            // 
            this.lblSerial.AutoSize = true;
            this.lblSerial.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSerial.Location = new System.Drawing.Point(66, 235);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(113, 20);
            this.lblSerial.TabIndex = 7;
            this.lblSerial.Text = "Serial Number:";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblModel.Location = new System.Drawing.Point(192, 204);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(39, 20);
            this.lblModel.TabIndex = 7;
            this.lblModel.Text = "*****";
            // 
            // lblModelText
            // 
            this.lblModelText.AutoSize = true;
            this.lblModelText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblModelText.Location = new System.Drawing.Point(66, 204);
            this.lblModelText.Name = "lblModelText";
            this.lblModelText.Size = new System.Drawing.Size(56, 20);
            this.lblModelText.TabIndex = 7;
            this.lblModelText.Text = "Model:";
            // 
            // lblMfrName
            // 
            this.lblMfrName.AutoSize = true;
            this.lblMfrName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMfrName.Location = new System.Drawing.Point(192, 174);
            this.lblMfrName.Name = "lblMfrName";
            this.lblMfrName.Size = new System.Drawing.Size(39, 20);
            this.lblMfrName.TabIndex = 7;
            this.lblMfrName.Text = "*****";
            // 
            // lblManufacturer
            // 
            this.lblManufacturer.AutoSize = true;
            this.lblManufacturer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblManufacturer.Location = new System.Drawing.Point(66, 174);
            this.lblManufacturer.Name = "lblManufacturer";
            this.lblManufacturer.Size = new System.Drawing.Size(108, 20);
            this.lblManufacturer.TabIndex = 7;
            this.lblManufacturer.Text = "Manufacturer:";
            // 
            // picVSA
            // 
            this.picVSA.Image = ((System.Drawing.Image)(resources.GetObject("picVSA.Image")));
            this.picVSA.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.picVSA.InitialImage = ((System.Drawing.Image)(resources.GetObject("picVSA.InitialImage")));
            this.picVSA.Location = new System.Drawing.Point(70, 56);
            this.picVSA.Name = "picVSA";
            this.picVSA.Size = new System.Drawing.Size(220, 105);
            this.picVSA.TabIndex = 6;
            this.picVSA.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOk.Location = new System.Drawing.Point(474, 297);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 38);
            this.btnOk.TabIndex = 37;
            this.btnOk.Text = "CLOSE";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCheckInfo
            // 
            this.btnCheckInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCheckInfo.Location = new System.Drawing.Point(247, 297);
            this.btnCheckInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btnCheckInfo.Name = "btnCheckInfo";
            this.btnCheckInfo.Size = new System.Drawing.Size(81, 38);
            this.btnCheckInfo.TabIndex = 34;
            this.btnCheckInfo.Text = "CONNECT";
            this.btnCheckInfo.UseVisualStyleBackColor = true;
            this.btnCheckInfo.Click += new System.EventHandler(this.btnCheckInfo_Click);
            // 
            // txtRespond
            // 
            this.txtRespond.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.txtRespond.Location = new System.Drawing.Point(247, 241);
            this.txtRespond.Margin = new System.Windows.Forms.Padding(4);
            this.txtRespond.Name = "txtRespond";
            this.txtRespond.Size = new System.Drawing.Size(307, 22);
            this.txtRespond.TabIndex = 32;
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblResponse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblResponse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblResponse.Location = new System.Drawing.Point(32, 241);
            this.lblResponse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(92, 24);
            this.lblResponse.TabIndex = 24;
            this.lblResponse.Text = "Respond:";
            // 
            // lblConnPort
            // 
            this.lblConnPort.AutoSize = true;
            this.lblConnPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblConnPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblConnPort.Location = new System.Drawing.Point(377, 136);
            this.lblConnPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnPort.Name = "lblConnPort";
            this.lblConnPort.Size = new System.Drawing.Size(43, 24);
            this.lblConnPort.TabIndex = 27;
            this.lblConnPort.Text = "Port";
            // 
            // lblCmdText
            // 
            this.lblCmdText.AutoSize = true;
            this.lblCmdText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblCmdText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCmdText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCmdText.Location = new System.Drawing.Point(32, 196);
            this.lblCmdText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCmdText.Name = "lblCmdText";
            this.lblCmdText.Size = new System.Drawing.Size(164, 24);
            this.lblCmdText.TabIndex = 26;
            this.lblCmdText.Text = "Verify Connection:";
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblInterface.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInterface.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblInterface.Location = new System.Drawing.Point(32, 136);
            this.lblInterface.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(86, 24);
            this.lblInterface.TabIndex = 25;
            this.lblInterface.Text = "Interface:";
            // 
            // cboInterface
            // 
            this.cboInterface.FormattingEnabled = true;
            this.cboInterface.Items.AddRange(new object[] {
            "TCPIB",
            "GPIB"});
            this.cboInterface.Location = new System.Drawing.Point(247, 136);
            this.cboInterface.Name = "cboInterface";
            this.cboInterface.Size = new System.Drawing.Size(81, 24);
            this.cboInterface.TabIndex = 39;
            // 
            // cboPort
            // 
            this.cboPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Items.AddRange(new object[] {
            "24000"});
            this.cboPort.Location = new System.Drawing.Point(474, 136);
            this.cboPort.Margin = new System.Windows.Forms.Padding(4);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(80, 24);
            this.cboPort.TabIndex = 30;
            // 
            // cboVSAList
            // 
            this.cboVSAList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cboVSAList.FormattingEnabled = true;
            this.cboVSAList.Items.AddRange(new object[] {
            "N9020A",
            "N9020B",
            "FSW26",
            "FSV3030",
            "M9410A",
            "IQFR1-RU"});
            this.cboVSAList.Location = new System.Drawing.Point(247, 31);
            this.cboVSAList.Margin = new System.Windows.Forms.Padding(4);
            this.cboVSAList.Name = "cboVSAList";
            this.cboVSAList.Size = new System.Drawing.Size(307, 24);
            this.cboVSAList.TabIndex = 31;
            this.cboVSAList.Text = "FSW26";
            this.cboVSAList.SelectedIndexChanged += new System.EventHandler(this.cboInstrument_TextChanged);
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblAddress.Location = new System.Drawing.Point(32, 86);
            this.lblAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(103, 24);
            this.lblAddress.TabIndex = 28;
            this.lblAddress.Text = "IP address:";
            // 
            // lblVSAName
            // 
            this.lblVSAName.AutoSize = true;
            this.lblVSAName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblVSAName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVSAName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblVSAName.Location = new System.Drawing.Point(32, 31);
            this.lblVSAName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVSAName.Name = "lblVSAName";
            this.lblVSAName.Size = new System.Drawing.Size(110, 24);
            this.lblVSAName.TabIndex = 29;
            this.lblVSAName.Text = "VSA Select:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(474, 196);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(80, 24);
            this.btnSend.TabIndex = 41;
            this.btnSend.Text = "SEND";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cboCmd
            // 
            this.cboCmd.FormattingEnabled = true;
            this.cboCmd.Items.AddRange(new object[] {
            "*IDN?",
            "*OPC?"});
            this.cboCmd.Location = new System.Drawing.Point(247, 196);
            this.cboCmd.Name = "cboCmd";
            this.cboCmd.Size = new System.Drawing.Size(173, 24);
            this.cboCmd.TabIndex = 44;
            this.cboCmd.Text = "*IDN?";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDisconnect.Location = new System.Drawing.Point(350, 297);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(97, 38);
            this.btnDisconnect.TabIndex = 45;
            this.btnDisconnect.Text = "DISCONNECT";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // SetupVSA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 403);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.cboCmd);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.cboIP);
            this.Controls.Add(this.grpVSA);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCheckInfo);
            this.Controls.Add(this.txtRespond);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.lblConnPort);
            this.Controls.Add(this.lblCmdText);
            this.Controls.Add(this.lblInterface);
            this.Controls.Add(this.cboInterface);
            this.Controls.Add(this.cboPort);
            this.Controls.Add(this.cboVSAList);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblVSAName);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SetupVSA";
            this.Text = "VSA SETUP";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SetupVSA_FormClosed);
            this.Load += new System.EventHandler(this.SetupVSA_Load);
            this.grpVSA.ResumeLayout(false);
            this.grpVSA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picVSA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboIP;
        private System.Windows.Forms.GroupBox grpVSA;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblConnStatus;
        private System.Windows.Forms.Label lbSearialNumber;
        private System.Windows.Forms.Label lblSerial;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblModelText;
        private System.Windows.Forms.Label lblMfrName;
        private System.Windows.Forms.Label lblManufacturer;
        private System.Windows.Forms.PictureBox picVSA;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCheckInfo;
        private System.Windows.Forms.TextBox txtRespond;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.Label lblConnPort;
        private System.Windows.Forms.Label lblCmdText;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.ComboBox cboInterface;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.ComboBox cboVSAList;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblVSAName;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ComboBox cboCmd;
        private System.Windows.Forms.Button btnDisconnect;
    }
}