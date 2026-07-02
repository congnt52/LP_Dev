using _5GAutoTool.Measurements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Image = System.Drawing.Image;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using MES_DLL;
using SSO_DLL;

namespace _5GAutoTool
{
    /// <summary>
    /// Giao diện chính Tool đo kiểm tự động
    /// </summary>
    [Guid("5BB4EA84-AD3E-4597-8A47-4D996AC0D3F2")]
    public partial class Form5GAT : Form
    {
        public LogAdapter log;
        public Form5GAT()
        {
            InitializeComponent();
            //create log first
            log = new LogAdapter(lvwLog, this);
            log.setInstance(log);

            BBU = new BBU(this);
            RRU = new RRU(this);
            VSA = new VSA(this);
            VSG1 = new VSG(this);
            VSG2 = new VSG(this);
            VSG3 = new VSG(this);
            PS = new PowerSensor(this);
            rfSwitch1 = new RFSwitch(this);
            rfSwitch2 = new RFSwitch(this);
            rfSwitch3 = new RFSwitch(this);
            Measurement = new Measurements1(this);

            frmVSA = new SetupVSA(this);
            frmVSG = new SetupVSG(this);
            frmRFSwitch = new SetupRFSwitch(this);
            frmAttenuator = new SetupAttenuator();
            frmBBU = new SetupBBU(this);
            frmRRU = new SetupRRU(this);
            frmISFreqRange = new ISFreqRange(this);
            frmDuplicateFile = new DuplicateFile(this);
            frmCreateATTFile = new CreateATTFiles(this);
            // Initialize the Timer to check Device Connection
            connectionCheckTimer = new Timer();
            connectionCheckTimer.Interval = 60000; // 1 minute interval
            connectionCheckTimer.Tick += ConnectionCheckTimer_Tick;
            instruments = new List<IInstrument>();

            //define treeview icon
            imageList.Images.Clear();
            imageList.Images.Add("TestPlan", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "TestPlan.png")));
            imageList.Images.Add("Na", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Na.png")));
            imageList.Images.Add("Running", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Running.png")));
            imageList.Images.Add("Pending", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Pending.png")));
            imageList.Images.Add("Passed", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Passed.png")));
            imageList.Images.Add("Failed", Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "Images", "Failed.png")));
            trvTestCases.ImageList = imageList;
            TestcasePending = new List<TestcaseNode>();
            testingPortList = new List<int>();
            //lvwLog.Items.Clear();
            // Khai báo backgroundWorker
            measurementBackgroundWorker = new BackgroundWorker();
            measurementBackgroundWorker.WorkerSupportsCancellation = true; // cho phep dung tien trinh
            measurementBackgroundWorker.WorkerReportsProgress = true; // cho phep bao cao tien trinh
            measurementBackgroundWorker.DoWork += measurementBackgroundWorker_DoWork;
            measurementBackgroundWorker.RunWorkerCompleted += measurementBackgroundWorker_Completed;
            measurementBackgroundWorker.ProgressChanged += measurementBackgroundWorker_ProgressChanged;
            //api = new APIToolTest();

        }

        #region Declare variable
        //Khai báo biến\
        public BBU BBU;
        public RRU RRU;
        public VSA VSA;
        public VSG VSG1, VSG2, VSG3;
        public PowerSensor PS;
        Measurements1 Measurement;
        public RFSwitch rfSwitch1, rfSwitch2, rfSwitch3;
        public string ProcessInfo;

        //Frame
        SetupVSA frmVSA;
        SetupVSG frmVSG;
        SetupAttenuator frmAttenuator;
        SetupRFSwitch frmRFSwitch;
        SetupBBU frmBBU;
        SetupRRU frmRRU;
        ISFreqRange frmISFreqRange;
        DuplicateFile frmDuplicateFile;
        CreateATTFiles frmCreateATTFile;
        private Timer connectionCheckTimer;
        List<IInstrument> instruments;
        public BackgroundWorker measurementBackgroundWorker;
        UploadFIle uploadtoServer = new UploadFIle();

        //info thong tin cac bai do
        public DateTime now = DateTime.Now;
        public string testCase = null; //testCase name: "Downlink"; "Uplink"
        public string specName = null; //Specification  Abbreviation. Ex: Prat, ACLR, SEM....
        public double channelBWLower; // Channel bandwidth Lower Edge
        public double channelBWUpper; // Channel bandwidth Upper Edge
        public double deltaFoobValue = 60;
        public string RruType;

        //CONFIGURATION
        string setFrequency, setPower, setBandwidth;
        string instrBBU, ipBBU, userBBU, passBBU;
        bool[] setPort = new bool[32];   //kiểm tra lại khai báo mảng này
        List<int> testingPortList;
        int [][] Multiport;
        int numbArr;
        string fileReport = "";
        public string currNRTM = "";

        string[] getSN = new string[6];
        string[] readSN = new string[6];
        Result getresult;
        string pass = "PASS";
        string fail = "FAIL";
        string error = "ERROR";
        string taetmp = "ERROR";
        string productValue = "FG.GNODEB.32T32R.FPGA.V1";

        //INSTRUMENT
        string statusVSA = "";
        string statusVSG1 = "";
        string statusVSG2 = "";
        string statusVSG3 = "";
        string statusRFS = "";
        string statusRRU = "";
        string statusBBU = "";
        string ipVSA, ipRFS1, ipRFS2, ipRFS3, ipRRU, ipVSG1, ipVSG2, ipVSG3;
        string[] arrAttDL = new string[32];
        string[] arrAttUL = new string[32];
        string attIMD = "", attIMD1 = "";
        string attOnOff = "";
        string testingMode = "TX";


        string rfs1Ax, rfs2Ax, rfs3Ax;

        //TreeView_Cac bai do
        string[,] loadExcelClause = new string[50, 25];
        string[,] measClause = new string[50, 25];
        string[,] submeasClause = new string[50, 30];
        int numbersubmeasClause = 0;
        int numberMeasClause = 0; // số bài đo đã chọn
        int numberPortChecked = 0; // số Port đã chọn
        int count = 0; // số mục đã đo;
        int time = 0;
        DateTime timeStep;
        TimeSpan tactTime;
        //string beginningTime = "";
        string endingTime = "";
        //Data_Cac bai do 
        // data downlink
        string[,] dataCalibTx = new string[32, 5];
        string[,] dataPout = new string[32, 3];  // 8 port || 0 Result, 1 Testing Time, 2 Read Value.
        string[,] dataOBW = new string[32, 3];
        string[,,] dataACLR = new string[32, 2, 3];
        string[,,] dataSEM = new string[32, 2, 5];
        string[,,] dataEVM = new string[32, 6, 3];
        string[,,] dataFreqErr = new string[32, 6, 3];
        string[,] dataTotalDR = new string[32, 3];
        string[,] dataTXSUPR = new string[32, 7]; // 8 port || 0 Result, 1 testing time, 2 read value 1, 3 read value 2, 4 read value 3, 5 read value 4, 6 read value 5
        string[,] dataOnOffPower = new string[32, 3];
        string[,] dataTransient = new string[32, 3];
        string[,] dataTAE = new string[32, 3];
        List<SpuriousRange> spurRanges;
        //TestCaseTreeView testCaseTreeView;

        // data uplink
        string[,] dataCalibRx = new string[32, 5];
        string[,] dataPsen = new string[32, 4];     // 8 port || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,] dataDR = new string[32, 4];       // 8 port || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,] dataICS = new string[32, 4];  // 8 port  || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,,] dataACS = new string[32, 2, 4];  // 8 port  || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,,] dataINB = new string[32, 2, 4];  // 8 port  || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,,] dataNBB = new string[32, 2, 4];  // 8 port  || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,,] dataOBB = new string[32, 2, 4];  // 8 port  || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
        string[,] dataRXSUPR = new string[32, 5];
        string[,,,] dataRXIMD = new string[2, 32, 2, 4];
        public List<double[]> ListISFreqRange = new List<double[]>(); //25.3.2024 - List luu lai cac dai nhieu cho bai out of band Co-location

        // flag event
        bool clearFlag = false;
        public bool OOBexcludeColocationRange; //27.3.2024 - option for OOB measurement
        #endregion
        public string directionPathTemplate = @"C:\test3gpp\OQC\32T32R10W\templates\";
        public string directionPathLogs = @"C:\test3gpp\OQC\32T32R10W\logs\";
        public string directionPathResults = @"C:\test3gpp\OQC\32T32R10W\results\";
        public string portMappingFile = @"C:\test3gpp\OQC\32T32R10W\templates\AntennaPort_Mapping.xlsx"; //Port Mapping - Redhat
        bool enRecordLog = false;


        //khai bao thu muc / duong dan chua cac file CSV suy hao
        public string rruCSVFilePathTX = @"C:\test3gpp\Configurations\LossConfig\Transmitters\Txn";
        public string rruCSVFilePathIMDTX = @"C:\test3gpp\Configurations\LossConfig\Transmitters\TxIntermodulations";
        public string rruCSVFilePathOnOff = @"C:\test3gpp\Configurations\LossConfig\Transmitters\OnOffpowers";
        public string wsCSVFilePathRX = @"C:\test3gpp\Configurations\LossConfig\Receivers\Wantedsignal";
        public string isCSVFilePathRX = @"C:\test3gpp\Configurations\LossConfig\Receivers\Interfersignal";
        public string cwCSVFilePathRX = @"C:\test3gpp\Configurations\LossConfig\Receivers\Cwsignal";
        public string RXspurFilePath = @"C:\test3gpp\Configurations\LossConfig\Receivers\RXSpurious";
        public PortMapping currPort = new PortMapping();
        public List<PortMapping> PortMappings = new List<PortMapping>();
        public List<TestcaseNode> TestcaseNodes = new List<TestcaseNode>();
        string treeViewName = "root";
        ImageList imageList = new ImageList();
        List<TestcaseNode> TestcasePending;


        #region Factory declared
        //APIToolTest api;
        public string RruSerial = ""; // thông tin RRU
        public string processValue = ""; // Thông tin mã công đoạn
        //public string productValue = ""; // Thông tin mã sản phẩm 
        public string userID = ""; // Thông tin tên tester
        private void btnStart_Click(object sender, EventArgs e)
        {
            //Reading device information
            
            ProgressLabel("Reading Measurement Parameter...");
            RruSerial = txtRRUSerial.Text;

            //frmBBU.infor(out instrBBU, out ipBBU, out port, out userBBU, out passBBU);

            //Step 1 Login BBU
            ProgressLabel($"Logging in to OAM: {ipBBU}");
            BBU.loginOAM(ipBBU, port, out flagConnectedtoOAM);
            if (flagConnectedtoOAM)
            {
                Log($@"Socket connected to {ipBBU}", LogLevel.SUCCESS);
            }

            //Checking input validation
            ProgressLabel($"Validating RRU Serial in combobox...");
            if (RruSerial.Contains("RU") || RruSerial.Contains("AIO")) //&& rruSerial.Length == 10
            {
                Config();
                //clear TestcaseNodes status but still keep Parameters
                ResetTestcaseNodesStatus();
                UpdateToTreeView();

                TestcasePending = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Pending);
                if (TestcasePending.Count > 0 && testingPortList.Count >= 0)
                {
                    if (clearFlag)
                    {
                        DialogResult deleteResult = MessageBox.Show("Xóa dữ liệu hiển thị trên màn hình ?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (deleteResult == DialogResult.Yes)
                        {
                            ClearResult();
                        }
                        clearFlag = false;
                    }
                    RruSerial = txtRRUSerial.Text;

                    if (measurementBackgroundWorker.IsBusy)
                    {
                        measurementBackgroundWorker.CancelAsync();
                        measurementBackgroundWorker.RunWorkerAsync();
                    }
                    else
                    {
                        measurementBackgroundWorker.RunWorkerAsync();
                    }
                    timerProcess.Start();
                    //connectionCheckTimer.Start(); //tam thoi khong thuc hien kiem tra
                    btnStop.Enabled = true;
                    //calculate Progressbar
                    UpdateProgressBar(0);
                    //add instrument list
                    AddInstrumenttoList();
                }
            }
            else
            {
                DialogResult deleteResult = MessageBox.Show("Mã Scan sai định dạng hoặc đang bỏ trống");
                txtRRUSerial.Text = "";
            }
        }

        private void ChangeTestcaseNodeStatus(string tcName, TestcaseStatus status)
        {
            TestcaseNode nodeInList = TestcaseNodes.FirstOrDefault(n => n.Text == tcName);
            if (nodeInList != null)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => nodeInList.Status = status));
                }
                else
                {
                    nodeInList.Status = status;
                }
                //nodeInList.SetNodeProperties();
                UpdateToTreeView();
            }
        }
        private void ChangeTestcaseNodeStatus(TestcaseNode tc, TestcaseStatus status)
        {
            if (TestcaseNodes.Contains(tc))
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => tc.Status = status));
                }
                else
                {
                    tc.Status = status;
                }
                //nodeInList.SetNodeProperties();
                UpdateToTreeView();
            }
        }
        public void btnStop_Click(object sender, EventArgs e)
        {
            BBU.StopProcess();
            clearFlag = true;
            if (measurementBackgroundWorker.IsBusy == true)
            {
                measurementBackgroundWorker.CancelAsync();
                lblStatusProgress_TextChanged("Đang dừng");
            }
            timerProcess.Stop();
            btnConnect_Enable();
            instruments.Clear();
        }
        bool rfsFlagconnect = false, rruSync = false, duSync = false, sync = false, statusConnection = false, flagConnectedtoOAM = false;
        int port = 1970, portRFS = 0;
        string manufacturerVSA = "", instrVSA = "", modelVSA = "", serialVSA = "";
        private void btnConnect_Click(object sender, EventArgs e)
        {
            frmBBU.infor(out instrBBU, out ipBBU, out port, out userBBU, out passBBU);
            BBU.loginOAM(ipBBU, port, out flagConnectedtoOAM);
            while (RruType == "8T8R")
            {
                BBU.TDDMODE("CALIB");
                break;
            }
            BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
            if (flagConnectedtoOAM)
            {
                Log($@"Socket connected to {ipBBU}", LogLevel.SUCCESS);
                BBU.StopProcess();
                //lblStatusProgress.Text = "Đã đồng bộ";
                //btnConnect.Enabled = false;
                int sync_cpri = BBU.check_CPRI_SYNC();
                if (sync_cpri == 1)
                {
                    lblStatusProgress.Text = "Chưa đồng bộ";
                }
                else
                {

                    lblStatusProgress.Text = "Đã đồng bộ";
                    btnConnect.Enabled = false;
                }
            }
            else
            {
                lblStatusProgress.Text = "Chưa đồng bộ";
                Log("Connection refuse: Please run !!" + "\n" + "/nr_sw/running/scripts/sw_start.sh)", LogLevel.WARN);
            }
            if (testingMode == "TX")
            {
                frmVSA.info(out instrVSA, out ipVSA);
                if (manufacturerVSA == "")
                {
                    VSAConnection(ipVSA, "*IDN?", instrVSA, out manufacturerVSA, out modelVSA, out serialVSA);
                    if (manufacturerVSA == "Rohde&Schwarz") { frmVSA.statusConnection(manufacturerVSA, modelVSA, serialVSA); }
                    else if (manufacturerVSA == "Agilent Technologies") { frmVSA.statusConnection(manufacturerVSA, modelVSA, serialVSA); }
                    else if (manufacturerVSA == "Keysight Technologies") { frmVSA.statusConnection(manufacturerVSA, modelVSA, serialVSA); }
                }
            }
            else if (testingMode == "RX")
            {

            }
        }

        private void btnAttenuator_Click(object sender, EventArgs e)
        {
            frmAttenuator.ShowDialog();
        }
        #endregion
        public void ClearResult()
        {
            lvwResult.Items.Clear();
            prgLoad_ValueChange(1000, 0);
            timerProcess.Stop();
            time = 0;
            TimeSpan timeCount = TimeSpan.FromSeconds(time);
            lblRuntime.Text = timeCount.ToString();
            //TODO: xoa chart 21.2.2024
            chart1.Series.Clear();

            // xoa gia tri o mang cu

            dataCalibTx = new string[32, 5];
            dataPout = new string[32, 3];  // 8 port || 0 Result, 1 Testing Time, 2 Read Value.
            dataOBW = new string[32, 3];
            dataACLR = new string[32, 2, 3];
            dataSEM = new string[32, 2, 5];
            dataEVM = new string[32, 6, 3];
            dataFreqErr = new string[32, 6, 3];
            dataTotalDR = new string[32, 3];
            dataTXSUPR = new string[32, 7];

            dataPsen = new string[32, 4];     // 8 port || 0 PASS/FAIL, 1 testing time, 2 throughPut, 3 Testing Power
            dataDR = new string[32, 4];       // 8 port || 0 power, 1 PASS/FAIL, 2 tp
            dataICS = new string[32, 4];  // 8 port  || 0 lower , 1 upper || 0 power, 1 PASS/FAIL, 2 tp
            dataACS = new string[32, 2, 4];  // 8 port  || 0 lower , 1 upper || 0 power, 1 PASS/FAIL, 2 tp
            dataINB = new string[32, 2, 4];  // 8 port  || 0 lower , 1 upper || 0 power, 1 PASS/FAIL, 2 tp
            dataNBB = new string[32, 2, 4];  // 8 port  || 0 lower , 1 upper || 0 power, 1 PASS/FAIL, 2 tp
            dataOBB = new string[32, 2, 4];  // 8 port  || 0 lower , 1 upper || 0 power, 1 PASS/FAIL, 2 tp
            dataRXSUPR = new string[32, 5];
            dataRXIMD = new string[2, 32, 2, 4];

            //clear TestcaseNodes status but still keep Parameters
            //ResetTestcaseNodesStatus();
            //UpdateToTreeView();
        }
        public void Config()
        {
            ProgressLabel($"Configuring measurement parameters...");
            btnStart.Enabled = true;
            timerProcess.Stop();
            time = 0;
            numberMeasClause = 0;
            numberPortChecked = 0;
            prgLoad_ValueChange(1000, 0);
            VSA.RRUinfo(RruSerial);

            //Reseting measurement process parameters
            ProgressLabel($"Reseting process parameters...");
            Measurement.Reset();
            TimeSpan timeCount = TimeSpan.FromSeconds(time);
            lblRuntime.Text = timeCount.ToString();
            setFrequency = Convert.ToString((double.Parse(txtFrequency.Text) * 1e6));    // chuyen doi frequency tu MHz sang Hz
            setPower = txtPWR.Text;

            setBandwidth = textBandwidth.Text;
            //send value to RRU Properties
            double.TryParse(setFrequency, out RRU.SetFrequencyValue);
            double.TryParse(txtPower.Text, out RRU.SetPowerLevel);
            double.TryParse(setBandwidth, out RRU.SetBandWidth);

            //20.2.2024 - bandwidth edges - depend on setFrequency => n41 or n78 (tam thoi, do chua co cai dat tu giao dien)
            if (RRU.SetFrequencyValue >= 2496e6 & RRU.SetFrequencyValue <= 2690e6)
            {
                RRU.ChannelBWBottom = 2500e6; // kenh n41;
                RRU.ChannelBWTop = 2690e6; // kenh n41; tam thoi
            }
            else if (RRU.SetFrequencyValue >= 3600e6 & RRU.SetFrequencyValue <= 3800e6)
            {
                RRU.ChannelBWBottom = 3600e6; // kenh n78;
                RRU.ChannelBWTop = 3800e6; // kenh n78; tam thoi
            }

            channelBWLower = RRU.ChannelBWBottom;
            channelBWUpper = RRU.ChannelBWTop;
            RRU.Serial = RruSerial;
            //Collecting List of Testing Port
            //ProgressLabel($"Collecting List of Testing Port...");
            //PopulateTestingPortList();
            //====code cu =======
            //testingPortList.Clear();
            //for (int i = 0; i < setPort.Length; i++)
            //{
            //    string chkPortIndex = "chkPort" + (i + 1);
            //    CheckBox checkbox = (CheckBox)this.Controls.Find(chkPortIndex, true)[0];
            //    setPort[i] = checkbox.Checked;
            //    if (checkbox.Checked)
            //    {
            //        numberPortChecked++;
            //        //26.5.2024 - update cho phan redhat
            //        testingPortList.Add(i + 1); //add port  (i + 1) to list of testing port
            //    }
            //}
            //Log($"Testing Ports List: {string.Join(", ", testingPortList)}");
            //Setup RF switches system
            ProgressLabel($"Setting RF switches system...");
            //frmRFSwitch.RFSwitchSetup(out rfs1Ax, out rfs2Ax, out rfs3Ax);
        }

        private void txtSerial_TextChanged(string text)
        {
            Label status = lblStatusProgress;
            status.Invoke(new MethodInvoker(delegate ()
            {
                txtRRUSerial.Text = text;
            }));
        }
        private void btnConnect_Enable()
        {
            Button status = btnConnect;
            status.Invoke(new MethodInvoker(delegate ()
            {
                btnConnect.Enabled = true;
            }));
        }
        private void chkAllPort_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 32; i++)  // Loop from chkPort1->8
            {
                string chkboxIndex = "chkPort" + (i + 1);
                CheckBox checkbox = (CheckBox)this.Controls.Find(chkboxIndex, true)[0];
                checkbox.Checked = chkALL.Checked;
            }
        }


        //==========================================================================

        //++++++++++++++++++++ TOOLSTRIP MENU ITEMS ++++++++++++++++++++++++++++++++

        // File -> Load
        //
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = directionPathTemplate + "OQC_DL_SETUP_3GPP.xlsx";
            clTmp.Text = "NRTM";
            testingMode = "TX";
            OpenFileData(filename);
        }
        private void tsmUL_Click(object sender, EventArgs e)
        {
            string filename = directionPathTemplate + "OQC_UL_SETUP_3GPP.xlsx";
            clTmp.Text = "ThroughPut";
            testingMode = "RX";
            portMappingFromExcel(portMappingFile);
            OpenFileData(filename);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //
        // INSTRUMENT -> VSA

        private void VSAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVSA.flagConn(statusVSA);
            frmVSA.ShowDialog();
            frmVSA.StatusConnection(out ipVSA, out statusVSA);
        }
        //
        // INSTRUMENT -> VSG
        //
        private void VSGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVSG.flagConn1(statusVSG1);
            frmVSG.flagConn2(statusVSG2);
            frmVSG.flagConn3(statusVSG3);
            frmVSG.ShowDialog();
            frmVSG.StatusConnection1(out ipVSG1, out statusVSG1);
            frmVSG.StatusConnection2(out ipVSG2, out statusVSG2);
            frmVSG.StatusConnection3(out ipVSG3, out statusVSG3);
        }
        //
        // INSTRUMENT -> RFSWITCH
        //
        private void RFSwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRFSwitch.ShowDialog();
            //  frmRFSwitch.StatusConnection(out ipRFS, out statusRFS);
        }
        //
        // INSTRUMENT -> ATTENUATOR
        //
        private void AttenuatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAttenuator.ShowDialog();
        }
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // mở file theo đường dẫn
            string filename = directionPathTemplate + "Autotool_manual.doc";
            // Check if file exists
            if (!File.Exists(filename))
            {
                MessageBox.Show("File does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Process.Start(filename);
        }

        //++++++++++++++++++++ TREVIEW MENU ITEMS ++++++++++++++++++++++++++++++++



        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportListView();
        }


        public void Log(string text, LogLevel logLevel = LogLevel.INFO, string device = "")
        {
            log.Log(text, logLevel, device, "Form5GAT");
        }
        public void LogDashedLine(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }



        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmRRU.ShowDialog();
        }
        //26.2.2024 - viet lai ham UL CSVLog



        public void VSG1Connection(string IP1, string Cmd1, string instrument1, out string manufacturer1, out string model1, out string serial1)
        {
            VSG1.Instrument(instrument1);
            VSG1.Connection(IP1, Cmd1, out manufacturer1, out model1, out serial1);
        }


        //VSG2Connection
        //
        public void VSG2Connection(string IP2, string Cmd2, string instrument2, out string manufacturer2, out string model2, out string serial2)
        {
            VSG2.Instrument(instrument2);
            VSG2.Connection(IP2, Cmd2, out manufacturer2, out model2, out serial2);
        }

        private void DUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBBU.flagConn(statusBBU);
            frmBBU.ShowDialog();
            frmBBU.statusConnection(out ipBBU, out statusBBU);
        }

        private void Form5GAT_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(this, "Thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                if (measurementBackgroundWorker.IsBusy)
                {
                    measurementBackgroundWorker.CancelAsync();
                }
                e.Cancel = true;
            }
        }

        private void tsmiExport_Click(object sender, EventArgs e)
        {
            ExportReport();
        }

        private void chkOOB3_CheckedChanged(object sender, EventArgs e)
        {
            //CheckBox chkOOB = sender as CheckBox;
            //deltaFoob.Enabled = chkOOB.Checked;
            //txtOOB1.Enabled = chkOOB.Checked;
            //txtOOB2.Enabled = chkOOB.Checked;
        }
        public void insert(string userID)
        {
            txbUser.Text = userID;
        }

        private void txtPWR_TextChanged(object sender, EventArgs e)
        {
            if (Int32.Parse(txtPWR.Text) < 1000 || Int32.Parse(txtPWR.Text) > 40000)
            {
                MessageBox.Show("Giá trị công suất nằm ngoài dải cho phép", "Thông báo");
                txtPWR.Text = "40000";
            }
        }

        private void lvwLog_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lvwLog.FocusedItem != null && lvwLog.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenuStrip m = new ContextMenuStrip();

                    // Create context menu items
                    AddContextMenuItem(m, "ERROR", createTabError_Click);
                    AddContextMenuItem(m, "SUCCESS", createTabSuccess_Click);
                    AddContextMenuItem(m, "FAILED", createTabFailed_Click);
                    AddContextMenuItem(m, "WARN", createTabWarn_Click);
                    AddContextMenuItem(m, "INFO", createTabInfo_Click);

                    //position
                    m.Show(lvwLog, new Point(e.X, e.Y));
                }
            }
        }
        private void AddContextMenuItem(ContextMenuStrip menu, string text, EventHandler clickHandler)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(text)
            {
                Name = text,
                Size = new Size(180, 22)
            };
            menuItem.Click += clickHandler;
            menu.Items.Add(menuItem);
        }

        private void createTabError_Click(object sender, EventArgs e)
        {
            CreateFilteredLog(LogLevel.ERROR);
        }
        private void createTabSuccess_Click(object sender, EventArgs e)
        {
            CreateFilteredLog(LogLevel.SUCCESS);
        }
        private void createTabFailed_Click(object sender, EventArgs e)
        {
            CreateFilteredLog(LogLevel.FAILED);
        }
        private void createTabInfo_Click(object sender, EventArgs e)
        {
            CreateFilteredLog(LogLevel.INFO);
        }
        private void createTabWarn_Click(object sender, EventArgs e)
        {
            CreateFilteredLog(LogLevel.WARN);
        }

        private void CreateFilteredLog(LogLevel level)
        {
            //check if the tab already exists
            foreach (TabPage tab in tabLogs.TabPages)
            {
                if (tab.Text == level.ToString())
                {
                    tabLogs.SelectedTab = tab;
                    return;
                }
            }

            //else, create a new tab page
            TabPage newTab = new TabPage(level.ToString());            
            //create a label to close tab
            //Label closeButton = new Label
            //{
            //    Text = "x",
            //    ForeColor = Color.Red,
            //    AutoSize = true,
            //    Cursor = Cursors.Hand,
            //    Location = new Point(newTab.Width - 20, 5)
            //};
            //closeButton.Click += (s, e) => CloseTab(newTab);

            ListView logView = log.ViewLogs.FirstOrDefault(n => n.Name == level.ToString());
            //logView.FullRowSelect = true;
            //logView.Dock = DockStyle.Fill;
            //logView.View = lvwLog.View;
            //logView.FullRowSelect = lvwLog.FullRowSelect;
            //logView.GridLines = lvwLog.GridLines;
            //logView.MultiSelect = lvwLog.MultiSelect;
            //logView.HideSelection = lvwLog.HideSelection;
            //logView.Width = lvwLog.Width;
            //logView.Height = lvwLog.Height;
            //logView.Anchor = lvwLog.Anchor;
            //logView.Location = lvwLog.Location;
            logView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            if (logView.Items.Count >= 1)
            {
                logView.EnsureVisible(logView.Items.Count - 1);
            }

            newTab.Controls.Add(logView);

            tabLogs.TabPages.Add(newTab);
            tabLogs.SelectedTab = newTab;
            // Ensure the close button stays in the top right corner
            //newTab.Controls.Add(closeButton);
            //closeButton.BringToFront();
        }

        private void CloseTab(TabPage tab)
        {
            if (tab != null) tabLogs.TabPages.Remove(tab);
        }


        private void lblStatusProgress_TextChanged(string text)
        {
            Label status = lblStatusProgress;
            status.Invoke(new MethodInvoker(delegate ()
            {
                lblStatusProgress.Text = text;
            }));
        }

        private void cbbRruType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmbType = (ComboBox)sender;
            RruType = cmbType.SelectedItem.ToString();
            Console.WriteLine(RruType);
            switch (RruType)
            {
                case "8T8R":
                    //preset for 8T8R
                    break;
                case "32T32R":
                    //preset for 32T32R
                    break;
                default:
                    break;
            }

        }


        //VSG3Connection
        //
        public void VSG3Connection(string IP3, string Cmd3, string instrument3, out string manufacturer3, out string model3, out string serial3)
        {
            VSG3.Instrument(instrument3);
            VSG3.Connection(IP3, Cmd3, out manufacturer3, out model3, out serial3);
        }

        private void duplicateFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDuplicateFile.ShowDialog();
        }

        private void createATTFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCreateATTFile.ShowDialog();
        }
        private List<CheckBox> portCheckboxes;
        private void Form5GAT_Load(object sender, EventArgs e)
        {

            this.FormBorderStyle = FormBorderStyle.Sizable;
            portCheckboxes = new List<CheckBox>()
            {
                chkPort1, chkPort2, chkPort3, chkPort4,
                chkPort5, chkPort6, chkPort7, chkPort8,
                chkPort9, chkPort10, chkPort11, chkPort12,
                chkPort13, chkPort14, chkPort15, chkPort16,
                chkPort17, chkPort18, chkPort19, chkPort20,
                chkPort21, chkPort22, chkPort23, chkPort24,
                chkPort25, chkPort26, chkPort27, chkPort28,
                chkPort29, chkPort30, chkPort31, chkPort32,

            };
            //foreach (var cb in portCheckboxes)
            //{
            //    cb.Enabled = false; // Disable all port checkboxes initially
            //}
            for (int i = 0; i < 32; i++)
            {
                portCheckboxes[i].Enabled = false;
            }
            //log view
            lvwLog.Clear();
            lvwLog.View = System.Windows.Forms.View.Details;
            lvwLog.HeaderStyle = ColumnHeaderStyle.None;
            lvwLog.Columns.Add("Timestamp");
            lvwLog.Columns.Add("Level");
            lvwLog.Columns.Add("Device");
            lvwLog.Columns.Add("Message");
            lvwLog.Columns.Add("Source");
            lvwLog.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            log.setInstance(log);
            log.CreateLogViews(lvwLog);

            //Edit Log Tab
            tabLogs.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabLogs.DrawItem += tabLogs_DrawItem;
            tabLogs.MouseClick += tabLogs_MouseClick;
            tabLogs.ItemSize = new Size(50, 25);

            string filename = directionPathTemplate + "OQC_DL_SETUP_3GPP.xlsx";
            string textFile = directionPathTemplate + "config.txt";
            testingMode = "TX";
            cbbRruType.SelectedItem = "32T32R";
            try
            {
                string[] text = new string[File.ReadAllLines(textFile).Length];
                text = File.ReadAllLines(textFile);
                ipBBU = text[0].Substring(text[0].IndexOf(":") + 1).Trim();
                frmBBU.updateIP(ipBBU);
                ipRFS1 = text[1].Substring(text[1].IndexOf(":") + 1).Trim();
                ipRFS2 = text[2].Substring(text[2].IndexOf(":") + 1).Trim();
                ipRFS3 = text[3].Substring(text[3].IndexOf(":") + 1).Trim();
                frmRFSwitch.updateIP(ipRFS1, ipRFS2, ipRFS3);
                ipVSG1 = text[4].Substring(text[4].IndexOf(":") + 1).Trim();
                ipVSG2 = text[5].Substring(text[5].IndexOf(":") + 1).Trim();
                ipVSG3 = text[6].Substring(text[6].IndexOf(":") + 1).Trim();
                frmVSG.updateIP(ipVSG1, ipVSG2, ipVSG3);
                string VSA_name = text[7].Substring(text[7].IndexOf(":") + 1).Trim();
                ipVSA = text[8].Substring(text[8].IndexOf(":") + 1).Trim();
                frmVSA.updateIP(VSA_name, ipVSA);
                //openfile excel
                OpenFileData(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


           
        }

        private void UpdatePortSelection()
        {
            // Bỏ chọn tất cả trước
            foreach (var cb in portCheckboxes)
                cb.Checked = false;

            // Path1 -> P1-P8
            if (chkPath1.Checked)
                for (int i = 0; i < 8; i++)
                    portCheckboxes[i].Checked = true;

            // Path2 -> P9-P16
            if (chkPath2.Checked)
                for (int i = 8; i < 16; i++)
                    portCheckboxes[i].Checked = true;

            // Path3 -> P17-P24
            if (chkPath3.Checked)
                for (int i = 16; i < 24; i++)
                    portCheckboxes[i].Checked = true;

            // Path4 -> P25-P32
            if (chkPath4.Checked)
                for (int i = 24; i < 32; i++)
                    portCheckboxes[i].Checked = true;
        }

        private void chkPath_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePortSelection();
        }
        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            bool value = chkALL.Checked;

            chkPath1.Checked = value;
            chkPath2.Checked = value;
            chkPath3.Checked = value;
            chkPath4.Checked = value;

            UpdatePortSelection();
        }

        private void lvwLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        private void tabLogs_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage tabPage = tabLogs.TabPages[e.Index];
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            bool isSelected = e.Index == tabLogs.SelectedIndex;
            e.DrawBackground();

            // Draw the background color based on selection
            using (Brush backgroundBrush = new SolidBrush(isSelected ? Color.White : SystemColors.Control))
            {
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            }

            //e.Graphics.FillRectangle(isSelected ? Brushes.White : SystemColors.Control, e.Bounds);

            // Draw the tab header
            TextRenderer.DrawText(e.Graphics, "  " + tabPage.Text + "    ", e.Font, e.Bounds, tabPage.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);


            // Draw the close button for tabs except the first one
            if (isSelected && e.Index != 0) // Skip drawing the close button for the first tab
            {
                int closeButtonX = e.Bounds.Right - 15;
                Rectangle closeButtonRect = new Rectangle(closeButtonX, e.Bounds.Top + 2, 15, 15);

                e.Graphics.FillRectangle(Brushes.Transparent, closeButtonRect);
                e.Graphics.DrawString("x", e.Font, Brushes.Red, closeButtonRect); // Draw the "x"
            }
        }


        private void tabLogs_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if the user clicked on the close button
            for (int i = 1; i < tabLogs.TabCount; i++) // Start from 1 to skip the first tab
            {
                Rectangle closeButtonRect = new Rectangle(tabLogs.GetTabRect(i).Right - 20, tabLogs.GetTabRect(i).Top + 2, 16, 16);
                if (closeButtonRect.Contains(e.Location))
                {
                    tabLogs.TabPages.RemoveAt(i);
                    break; // Exit the loop after removing the tab
                }
            }
        }


        private void diagramsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chapter6_1 fm = new Chapter6_1();
            fm.ShowDialog();
        }

        public void VSGSendCmd(string vsg, string Cmd, out string respond)
        {
            if (vsg == "VSG1")
            {
                VSG1.SendCmd(Cmd, out respond);
            }
            else if (vsg == "VSG2")
            {
                VSG2.SendCmd(Cmd, out respond);
            }
            else if (vsg == "VSG3")
            {
                VSG3.SendCmd(Cmd, out respond);
            }
            else
            {
                respond = error;
            }
        }
        public void VSGD1isconnect()
        {
            VSG1.Disconnect();
        }
        public void VSGD2isconnect()
        {
            VSG1.Disconnect();
            VSG2.Disconnect();
        }
        public void VSGD3isconnect()
        {
            VSG1.Disconnect();
            VSG2.Disconnect();
            VSG3.Disconnect();
        }

        private void trvTestCases_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is TestcaseNode testcaseNode)
            {
                ShowParameters(grdParams, testcaseNode);
                PrintDataTable(testcaseNode.Parameters);
            }

        }

        //VSAConnection
        //
        public void VSAConnection(string IP, string Cmd, string instrument, out string manufacturer, out string model, out string serial)
        {
            VSA.Instrument(instrument);
            VSA.Connection(IP, Cmd, out manufacturer, out model, out serial);
        }
        public void VSADisconnect()
        {
            VSA.Disconnect();
        }
        //
        //RFSwitchConnCheck
        //

        public void RFSwitchConnection1(string IP, int port, out bool rfsFlagconnect)
        {
            rfSwitch1.Connection(IP, port, out rfsFlagconnect);
        }
        public void RFSwitchConnection2(string IP, int port, out bool rfsFlagconnect)
        {
            rfSwitch2.Connection(IP, port, out rfsFlagconnect);
        }
        public void RFSwitchConnection3(string IP, int port, out bool rfsFlagconnect)
        {
            rfSwitch3.Connection(IP, port, out rfsFlagconnect);
        }

        public void SetSwitchRFS1(string Aport, string Nport, out string Respon)
        {
            rfSwitch1.SetSwitch(Aport, Nport, out Respon);
        }
        public void SetSwitchRFS2(string Aport, string Nport, out string Respon)
        {
            rfSwitch2.SetSwitch(Aport, Nport, out Respon);
        }
        public void SetSwitchRFS3(string Aport, string Nport, out string Respon)
        {
            rfSwitch3.SetSwitch(Aport, Nport, out Respon);
        }
        //TimerMeasurementProgress
        //
        private void timerProcess_Tick(object sender, EventArgs e)
        {
            time++;
            TimeSpan timeCount = TimeSpan.FromSeconds(time);
            lblRuntime.Text = timeCount.ToString();
        }
        //Update progressbar

        //
        //Swap function
        //
        public void BBUConnection(string instrument, string IP, string User, string Pass, out bool connStatus)
        {
            BBU.BBUVersion(instrument);
            BBU.Connection(IP, User, Pass, out connStatus);
        }
        public void RRUConnection(string IP, string Username, string Password, out bool flagCon)
        {
            RRU.Connection(IP, Username, Password, out flagCon);
        }
        private void RFSwitchRRUPort(string testcase, int port)
        {
            string response;
            string nodeA = "";
            //Quy uoc port bat dau = 0
            if (testcase == "TX") nodeA = "A1";
            if (testcase == "RX") nodeA = "A2";
            //he 3 switch
            if (rfSwitch2.Status == "Connected" & rfSwitch3.Status == "Connected")  //05.3.2024 - tam thoi ngat bo kiem tra switch1
            {
                int station = port / 16; //station = 0,1
                int node = (port % 16) + 1; //node = 1~16             
                switch (station)
                {
                    case 0:
                        rfSwitch1.SetSwitch(nodeA, "N1", out response);
                        rfSwitch2.SetSwitch("A1", "N" + node, out response);
                        break;
                    case 1:
                        rfSwitch1.SetSwitch(nodeA, "N2", out response);
                        rfSwitch3.SetSwitch("A1", "N" + node, out response);
                        break;
                    default:
                        Log("ERROR: (RFSwitches) Port Number is out of range!", LogLevel.ERROR);
                        break;
                }
            }
            //he 1 switch
            else if (rfSwitch1.Status == "Connected")
            {
                rfSwitch1.SetSwitch(nodeA, "N" + (port + 1), out response);
            }
        }

        private void RFSwitchRRUPath(string testcase, int[] ports)
        {
            if (ports == null || ports.Length == 0) return;

            string response;
            string nodeA = "";
            if (testcase == "TX") nodeA = "A1";
            if (testcase == "RX") nodeA = "A2";
            if (string.IsNullOrEmpty(nodeA)) return;

            int pathIndex = (ports[0] - 1) / 8; // Path1..Path4 => 0..3
            int station = pathIndex / 2;        // Path1-2 use station 0, Path3-4 use station 1
            int nodeOffset = (pathIndex % 2) * 8;

            if (rfSwitch2.Status == "Connected" & rfSwitch3.Status == "Connected")
            {
                RFSwitch slaveSwitch = station == 0 ? rfSwitch2 : rfSwitch3;
                rfSwitch1.SetSwitch(nodeA, "N" + (station + 1), out response);

                for (int i = 0; i < ports.Length; i++)
                {
                    int litePointPort = i + 1;
                    int node = nodeOffset + litePointPort;
                    slaveSwitch.SetSwitch("A" + litePointPort, "N" + node, out response);
                }
            }
            else if (rfSwitch1.Status == "Connected")
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    int litePointPort = i + 1;
                    int node = pathIndex * 8 + litePointPort;
                    rfSwitch1.SetSwitch("A" + litePointPort, "N" + node, out response);
                }
            }
        }



        //19.02.2024 - Chart
        long chartCount = 0;
        public void PopulateMeasurementChart(string seriesName, string portName, string strValue, string strLimitL, string strLimitH)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PopulateMeasurementChart(seriesName, portName, strValue, strLimitL, strLimitH)));
                return;
            }

            double value, limitL, limitH;

            if (!double.TryParse(strLimitL, out limitL))
            {
                limitL = 0;
            }
            if (!double.TryParse(strLimitH, out limitH))
            {
                limitH = 0;
            }
            if (!double.TryParse(strValue, out value))
            {
                //Random rand = new Random();
                //value = rand.NextDouble()*(limitH-limitL +10) +limitH -5; //test-xoa bo
                //value = Math.Round(value, 3);
                value = 0;
            }

            ChartArea chartArea = chart1.ChartAreas[0];
            chartArea.CursorX.IsUserSelectionEnabled = true;

            // Check if the series with the given name already exists in the chart
            Series existingSeries = chart1.Series.FindByName(seriesName);
            Series limitLSeries = new Series($"Limit_L");
            limitLSeries.ChartType = SeriesChartType.Line;
            limitLSeries.Color = Color.Magenta;

            Series limitHSeries = new Series($"Limit_H");
            limitHSeries.ChartType = SeriesChartType.Line;
            limitHSeries.Color = Color.Magenta;

            if (existingSeries == null)
            {
                //clear chart's series
                chart1.Series.Clear();
                chartCount = 0;

                // Create a new series "seriesName"
                Series newSeries = new Series(seriesName);
                //Type
                if (seriesName.Contains("Blocking"))
                {
                    newSeries.ChartType = SeriesChartType.Line;
                }
                else
                {
                    newSeries.ChartType = SeriesChartType.Column;
                }

                newSeries.Color = Color.Blue;


                chart1.Series.Add(newSeries);
                existingSeries = newSeries;

                // clear limit line
                chartArea.AxisY.StripLines.Clear();
                limitLSeries.Points.Clear();
                chart1.Series.Add(limitLSeries);
                limitHSeries.Points.Clear();
                chart1.Series.Add(limitHSeries);
            }



            // Add data point to the series
            existingSeries.Points.AddXY(portName, value);
            chartCount++;

            //add limit lines
            limitLSeries.Points.AddXY(portName, limitL);
            StripLine stripline1 = new StripLine();
            stripline1.Interval = 0;
            stripline1.IntervalOffset = limitL;
            stripline1.StripWidth = 0.1;
            stripline1.BackColor = Color.Magenta;
            chartArea.AxisY.StripLines.Add(stripline1);

            limitHSeries.Points.AddXY(portName, limitH);
            StripLine stripline2 = new StripLine();
            stripline2.Interval = 0;
            stripline2.IntervalOffset = limitH;
            stripline2.StripWidth = 0.1;
            stripline2.BackColor = Color.Magenta;
            chartArea.AxisY.StripLines.Add(stripline2);

            chartArea.AxisX.Title = seriesName;
            chartArea.AxisY.Title = "Measurement Value";
            chartArea.AxisY.LabelStyle.Format = "0.00";

            // Set axis scrollbar properties
            chartArea.AxisX.ScrollBar.Enabled = true;
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;

            // Set the minimum and maximum values for the Y-axis
            double minValue = Math.Min(existingSeries.Points.FindMinByValue().YValues[0], limitL);
            double maxValue = Math.Max(existingSeries.Points.FindMaxByValue().YValues[0], limitH);
            double range = maxValue - minValue;
            double offset = range * 0.1; // 20% offset
            chartArea.AxisY.Minimum = minValue - offset;
            chartArea.AxisY.Maximum = maxValue + offset;

            // Adjust visible range X-axis
            int startIndex = Math.Max(0, existingSeries.Points.Count - 100);
            int endIndex = existingSeries.Points.Count - 1;
            chartArea.AxisX.ScaleView.Size = 50;
            chartArea.AxisX.ScaleView.Position = startIndex;

            foreach (DataPoint point in existingSeries.Points)
            {
                if (point.YValues[0] < limitL || point.YValues[0] > limitH)
                {
                    Invoke(new Action(() => point.Color = Color.Red));
                    point.IsValueShownAsLabel = true;
                }
                else
                {
                    Invoke(new Action(() => point.Color = existingSeries.Color));
                    point.IsValueShownAsLabel = false;
                }
            }
            Invoke(new Action(() => chart1.Invalidate()));
        }

        //10.4.2024
        private async void ConnectionCheckTimer_Tick(object sender, EventArgs e)
        {
            // Start all connection checks concurrently
            Task<bool>[] connectionTasks = instruments.Select(instrument => instrument.IsConnectedAsync()).ToArray();

            // Await all connection checks to complete
            bool[] isConnected = await Task.WhenAll(connectionTasks);

            // Process the results for each instrument
            for (int i = 0; i < instruments.Count; i++)
            {
                if (!isConnected[i])
                {
                    Console.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss:fff")}] {instruments[i].GetType().Name}  is not connected!");
                }
            }
        }
        private void PopulateTestingPortList()
        {
            testingPortList.Clear();
            for (int i = 0; i < setPort.Length; i++)
            {
                string chkPortIndex = "chkPort" + (i + 1);
                CheckBox checkbox = (CheckBox)this.Controls.Find(chkPortIndex, true)[0];
                setPort[i] = checkbox.Checked;
                if (checkbox.Checked)
                {
                    testingPortList.Add(i + 1);
                }
            }
        }
        private void ReArrangeTestingPortList(List<PortMapping> portMappings)
        {
            List<int> tmp = new List<int>();
            int groupNum = GetPortMappingGroupNumber(portMappings);
            for (int i = 1; i <= groupNum; i++)
            {
                foreach (PortMapping port in portMappings)
                {
                    if(port.XRanIndex == i && testingPortList.Contains(port.Port))
                    {
                        tmp.Add(port.Port);
                    }
                }
            }
            testingPortList.Clear();
            testingPortList = tmp;

        }
        private int GetPortMappingGroupNumber(List<PortMapping> portList)
        {
            int number = 0;
            foreach(PortMapping port in portList)
            {
                number = (port.XRanIndex > number)? port.XRanIndex : number;
            }
            return number;
        }
        private void AddInstrumenttoList()
        {
            //test
            if (VSG1.IsConnected)
            {
                instruments.Add(VSG1);
            }
            if (VSG2.IsConnected)
            {
                instruments.Add(VSG2);
            }
            if (VSG3.IsConnected)
            {
                instruments.Add(VSG3);
            }
        }

    }
}
