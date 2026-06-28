namespace _5GAutoTool
{
    partial class SetupBBU
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupBBU));
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
            this.picBBU = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtBBURespond = new System.Windows.Forms.TextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblResponse = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblConnPort = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblInterface = new System.Windows.Forms.Label();
            this.cboInterface = new System.Windows.Forms.ComboBox();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.cboBBUList = new System.Windows.Forms.ComboBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblBBUName = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.grpVSA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBBU)).BeginInit();
            this.SuspendLayout();
            // 
            // cboIP
            // 
            this.cboIP.FormattingEnabled = true;
            this.cboIP.Items.AddRange(new object[] {
            "192.168.120.186",
            "192.168.120.187",
            "192.168.120.3"});
            this.cboIP.Location = new System.Drawing.Point(212, 74);
            this.cboIP.Margin = new System.Windows.Forms.Padding(4);
            this.cboIP.Name = "cboIP";
            this.cboIP.Size = new System.Drawing.Size(385, 24);
            this.cboIP.TabIndex = 73;
            this.cboIP.Text = "192.168.120.195";
            this.cboIP.SelectedIndexChanged += new System.EventHandler(this.CboIP_SelectedIndexChanged);
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
            this.grpVSA.Controls.Add(this.picBBU);
            this.grpVSA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.grpVSA.Location = new System.Drawing.Point(645, 16);
            this.grpVSA.Margin = new System.Windows.Forms.Padding(4);
            this.grpVSA.Name = "grpVSA";
            this.grpVSA.Padding = new System.Windows.Forms.Padding(4);
            this.grpVSA.Size = new System.Drawing.Size(515, 327);
            this.grpVSA.TabIndex = 71;
            this.grpVSA.TabStop = false;
            this.grpVSA.Enter += new System.EventHandler(this.GrpVSA_Enter);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStatus.Location = new System.Drawing.Point(265, 224);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(121, 25);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Not Connect";
            // 
            // lblConnStatus
            // 
            this.lblConnStatus.AutoSize = true;
            this.lblConnStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblConnStatus.Location = new System.Drawing.Point(97, 224);
            this.lblConnStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnStatus.Name = "lblConnStatus";
            this.lblConnStatus.Size = new System.Drawing.Size(74, 25);
            this.lblConnStatus.TabIndex = 7;
            this.lblConnStatus.Text = "Status:";
            // 
            // lbSearialNumber
            // 
            this.lbSearialNumber.AutoSize = true;
            this.lbSearialNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbSearialNumber.Location = new System.Drawing.Point(265, 186);
            this.lbSearialNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSearialNumber.Name = "lbSearialNumber";
            this.lbSearialNumber.Size = new System.Drawing.Size(52, 25);
            this.lbSearialNumber.TabIndex = 7;
            this.lbSearialNumber.Text = "*****";
            // 
            // lblSerial
            // 
            this.lblSerial.AutoSize = true;
            this.lblSerial.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSerial.Location = new System.Drawing.Point(97, 186);
            this.lblSerial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(142, 25);
            this.lblSerial.TabIndex = 7;
            this.lblSerial.Text = "Serial Number:";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblModel.Location = new System.Drawing.Point(265, 148);
            this.lblModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(52, 25);
            this.lblModel.TabIndex = 7;
            this.lblModel.Text = "*****";
            // 
            // lblModelText
            // 
            this.lblModelText.AutoSize = true;
            this.lblModelText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblModelText.Location = new System.Drawing.Point(97, 148);
            this.lblModelText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblModelText.Name = "lblModelText";
            this.lblModelText.Size = new System.Drawing.Size(72, 25);
            this.lblModelText.TabIndex = 7;
            this.lblModelText.Text = "Model:";
            // 
            // lblMfrName
            // 
            this.lblMfrName.AutoSize = true;
            this.lblMfrName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMfrName.Location = new System.Drawing.Point(265, 111);
            this.lblMfrName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMfrName.Name = "lblMfrName";
            this.lblMfrName.Size = new System.Drawing.Size(52, 25);
            this.lblMfrName.TabIndex = 7;
            this.lblMfrName.Text = "*****";
            // 
            // lblManufacturer
            // 
            this.lblManufacturer.AutoSize = true;
            this.lblManufacturer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblManufacturer.Location = new System.Drawing.Point(97, 111);
            this.lblManufacturer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblManufacturer.Name = "lblManufacturer";
            this.lblManufacturer.Size = new System.Drawing.Size(133, 25);
            this.lblManufacturer.TabIndex = 7;
            this.lblManufacturer.Text = "Manufacturer:";
            // 
            // picBBU
            // 
            this.picBBU.Image = ((System.Drawing.Image)(resources.GetObject("picBBU.Image")));
            this.picBBU.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.picBBU.InitialImage = ((System.Drawing.Image)(resources.GetObject("picBBU.InitialImage")));
            this.picBBU.Location = new System.Drawing.Point(103, 31);
            this.picBBU.Margin = new System.Windows.Forms.Padding(4);
            this.picBBU.Name = "picBBU";
            this.picBBU.Size = new System.Drawing.Size(340, 59);
            this.picBBU.TabIndex = 6;
            this.picBBU.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOk.Location = new System.Drawing.Point(495, 297);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(104, 48);
            this.btnOk.TabIndex = 70;
            this.btnOk.Text = "CLOSE";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnConnect.Location = new System.Drawing.Point(212, 297);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(108, 47);
            this.btnConnect.TabIndex = 69;
            this.btnConnect.Text = "CONNECT";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtBBURespond
            // 
            this.txtBBURespond.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.txtBBURespond.Location = new System.Drawing.Point(212, 247);
            this.txtBBURespond.Margin = new System.Windows.Forms.Padding(5);
            this.txtBBURespond.Name = "txtBBURespond";
            this.txtBBURespond.Size = new System.Drawing.Size(385, 26);
            this.txtBBURespond.TabIndex = 66;
            this.txtBBURespond.TextChanged += new System.EventHandler(this.TxtBBURespond_TextChanged);
            // 
            // txtPass
            // 
            this.txtPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.txtPass.Location = new System.Drawing.Point(495, 191);
            this.txtPass.Margin = new System.Windows.Forms.Padding(5);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(103, 26);
            this.txtPass.TabIndex = 67;
            this.txtPass.Text = "root";
            this.txtPass.TextChanged += new System.EventHandler(this.TxtPass_TextChanged);
            // 
            // txtUser
            // 
            this.txtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.txtUser.Location = new System.Drawing.Point(212, 192);
            this.txtUser.Margin = new System.Windows.Forms.Padding(5);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(107, 26);
            this.txtUser.TabIndex = 68;
            this.txtUser.Text = "root";
            this.txtUser.TextChanged += new System.EventHandler(this.TxtUser_TextChanged);
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblResponse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblResponse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblResponse.Location = new System.Drawing.Point(31, 247);
            this.lblResponse.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(117, 29);
            this.lblResponse.TabIndex = 57;
            this.lblResponse.Text = "Respond:";
            this.lblResponse.Click += new System.EventHandler(this.LblResponse_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblPassword.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPassword.Location = new System.Drawing.Point(369, 192);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(72, 29);
            this.lblPassword.TabIndex = 60;
            this.lblPassword.Text = "Pass:";
            this.lblPassword.Click += new System.EventHandler(this.LblPassword_Click);
            // 
            // lblConnPort
            // 
            this.lblConnPort.AutoSize = true;
            this.lblConnPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblConnPort.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConnPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblConnPort.Location = new System.Drawing.Point(369, 129);
            this.lblConnPort.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblConnPort.Name = "lblConnPort";
            this.lblConnPort.Size = new System.Drawing.Size(57, 29);
            this.lblConnPort.TabIndex = 61;
            this.lblConnPort.Text = "Port";
            this.lblConnPort.Click += new System.EventHandler(this.LblConnPort_Click);
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblUser.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblUser.Location = new System.Drawing.Point(31, 188);
            this.lblUser.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(70, 29);
            this.lblUser.TabIndex = 59;
            this.lblUser.Text = "User:";
            this.lblUser.Click += new System.EventHandler(this.LblUser_Click);
            // 
            // lblInterface
            // 
            this.lblInterface.AutoSize = true;
            this.lblInterface.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblInterface.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInterface.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblInterface.Location = new System.Drawing.Point(31, 129);
            this.lblInterface.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblInterface.Name = "lblInterface";
            this.lblInterface.Size = new System.Drawing.Size(111, 29);
            this.lblInterface.TabIndex = 58;
            this.lblInterface.Text = "Interface:";
            this.lblInterface.Click += new System.EventHandler(this.LblInterface_Click);
            // 
            // cboInterface
            // 
            this.cboInterface.FormattingEnabled = true;
            this.cboInterface.Items.AddRange(new object[] {
            "OAM",
            "SSH",
            "Telnet"});
            this.cboInterface.Location = new System.Drawing.Point(212, 129);
            this.cboInterface.Margin = new System.Windows.Forms.Padding(4);
            this.cboInterface.Name = "cboInterface";
            this.cboInterface.Size = new System.Drawing.Size(107, 24);
            this.cboInterface.TabIndex = 72;
            this.cboInterface.Text = "SSH";
            this.cboInterface.SelectedIndexChanged += new System.EventHandler(this.CboInterface_SelectedIndexChanged);
            // 
            // cboPort
            // 
            this.cboPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Items.AddRange(new object[] {
            "23",
            "22"});
            this.cboPort.Location = new System.Drawing.Point(495, 129);
            this.cboPort.Margin = new System.Windows.Forms.Padding(5);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(103, 28);
            this.cboPort.TabIndex = 64;
            this.cboPort.Text = "1970";
            this.cboPort.SelectedIndexChanged += new System.EventHandler(this.CboPort_SelectedIndexChanged);
            // 
            // cboBBUList
            // 
            this.cboBBUList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cboBBUList.FormattingEnabled = true;
            this.cboBBUList.Items.AddRange(new object[] {
            "XEON Gold",
            "DL380",
            "XEON D"});
            this.cboBBUList.Location = new System.Drawing.Point(212, 16);
            this.cboBBUList.Margin = new System.Windows.Forms.Padding(5);
            this.cboBBUList.Name = "cboBBUList";
            this.cboBBUList.Size = new System.Drawing.Size(385, 28);
            this.cboBBUList.TabIndex = 65;
            this.cboBBUList.Text = "XEON D";
            this.cboBBUList.SelectedIndexChanged += new System.EventHandler(this.CboBBUList_SelectedIndexChanged);
            this.cboBBUList.SelectedValueChanged += new System.EventHandler(this.cboBBUList_SelectedValueChanged);
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblAddress.Location = new System.Drawing.Point(31, 74);
            this.lblAddress.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(134, 29);
            this.lblAddress.TabIndex = 62;
            this.lblAddress.Text = "IP address:";
            this.lblAddress.Click += new System.EventHandler(this.LblAddress_Click);
            // 
            // lblBBUName
            // 
            this.lblBBUName.AutoSize = true;
            this.lblBBUName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblBBUName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblBBUName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBBUName.Location = new System.Drawing.Point(31, 16);
            this.lblBBUName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblBBUName.Name = "lblBBUName";
            this.lblBBUName.Size = new System.Drawing.Size(142, 29);
            this.lblBBUName.TabIndex = 63;
            this.lblBBUName.Text = "BBU Select:";
            this.lblBBUName.Click += new System.EventHandler(this.LblBBUName_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDisconnect.Location = new System.Drawing.Point(345, 297);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(5);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(121, 47);
            this.btnDisconnect.TabIndex = 82;
            this.btnDisconnect.Text = "DISCONNECT";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // SetupBBU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1172, 358);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.cboIP);
            this.Controls.Add(this.grpVSA);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtBBURespond);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblConnPort);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblInterface);
            this.Controls.Add(this.cboInterface);
            this.Controls.Add(this.cboPort);
            this.Controls.Add(this.cboBBUList);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblBBUName);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SetupBBU";
            this.grpVSA.ResumeLayout(false);
            this.grpVSA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBBU)).EndInit();
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
        private System.Windows.Forms.PictureBox picBBU;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtBBURespond;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblConnPort;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblInterface;
        private System.Windows.Forms.ComboBox cboInterface;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.ComboBox cboBBUList;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblBBUName;
        private System.Windows.Forms.Button btnDisconnect;
    }
}