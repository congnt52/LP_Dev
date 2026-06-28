
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace _5GAutoTool
{
    class OAM
    {
        public Form5GAT mainForm;
        public string Name="OAM";
        public OAM(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        OamCommunicator oam;
        public class Data
        {
            public int status { get; set; }
            public string errString { get; set; }
            public List<Root> data { get; set; }
        }

        public class Root
        {
            // 
            public string status { get; set; }

            public string state { get; set; }

            public string error_message { get; set; }
            public string RRU_8T8R_ID_LMK05028 { get; set; }
            public string RRU_8T8R_ID_HMC7044 { get; set; }
            public string RRU_8T8R_ID_SFP_1_VENDOR { get; set; }
            public string RRU_8T8R_ID_SFP_2_VENDOR { get; set; }
            public string RRU_8T8R_ID_INA_12V { get; set; }
            public string RRU_8T8R_ID_INA_0V85_VCCINT { get; set; }
            public string RRU_8T8R_ID_LM73_28V { get; set; }
            public string RRU_8T8R_ID_LM73_OCXO { get; set; }
            public string RRU_8T8R_ID_CPLD { get; set; }
            public string RRU_8T8R_ID_EEPROM { get; set; }
            public string RRU_8T8R_ID_PA { get; set; }
            public string RRU_8T8R_ID_ADRV9025_TRX1_4 { get; set; }
            public string RRU_8T8R_ID_ADRV9025_TRX5_8 { get; set; }

            // RRU VERSION
            [JsonProperty("binary-name")]
            public string BinaryName { get; set; }
            [JsonProperty("build-date")]
            public string BuildDate { get; set; }
            public string branch { get; set; }
            public string version { get; set; }
            public string information { get; set; }
            public string md5sum { get; set; }

            // SHW RRU FREQ
            public string name { get; set; }
            [JsonProperty("absolute-frequency-center")]
            public string AbsoluteFrequencyCenter { get; set; }

            // Sample app
            public string Sample0 { get; set; }
            public string Sample1 { get; set; }
            public string Sample2 { get; set; }

            // RX SENS
            [JsonProperty("TP ")]
            public string TP { get; set; }

            // Noise
            public string gain { get; set; }

            public string RX0 { get; set; }
            public string RX1 { get; set; }
            public string RX2 { get; set; }
            public string RX3 { get; set; }

            // Sync stage
            [JsonProperty("sync-state")]
            public string RRSyncState { get; set; }

            public string SyncState { get; set; }

            // Serial
            [JsonProperty("serial-num[0]")]
            public string SerialNum0 { get; set; }

            [JsonProperty("serial-num[1]")]
            public string SerialNum1 { get; set; }

            // AISG
            [JsonProperty("Aisg State")]
            public string AISG { get; set; }
            // VSWR
            [JsonProperty("vswr[0]")]
            public string Vswr0 { get; set; }

            [JsonProperty("vswr[1]")]
            public string Vswr1 { get; set; }

            [JsonProperty("vswr[2]")]
            public string Vswr2 { get; set; }

            [JsonProperty("vswr[3]")]
            public string Vswr3 { get; set; }

            [JsonProperty("vswr[4]")]
            public string Vswr4 { get; set; }

            [JsonProperty("vswr[5]")]
            public string Vswr5 { get; set; }

            [JsonProperty("vswr[6]")]
            public string Vswr6 { get; set; }

            [JsonProperty("vswr[7]")]
            public string Vswr7 { get; set; }

            // RRU infor
            [JsonProperty("TSSI(dBFS)")]
            public string TSSIDBFS { get; set; }

            [JsonProperty("RSSI(dBFS)")]
            public string RSSIDBFS { get; set; }

            [JsonProperty("FWD(dBFS)")]
            public string FWDDBFS { get; set; }

            [JsonProperty("GAIN(dB)")]
            public string GAINDB { get; set; }

            [JsonProperty("ANT(dB)")]
            public string ANTDB { get; set; }

            [JsonProperty("rx-power-min")]
            public string RxPowerMin { get; set; }

            [JsonProperty("error-message")]
            public string ErrorMessage { get; set; }

            [JsonProperty("rx-power-mean")]
            public string RxPowerMean { get; set; }

            [JsonProperty("rx-power-peak")]
            public string RxPowerPeak { get; set; }

            [JsonProperty("mac[0]")]
            public string Mac0 { get; set; }

            [JsonProperty("mac[1]")]
            public string Mac1 { get; set; }
            [JsonProperty("TAE AVG")]
            public string TAEAVG { get; set; }

            [JsonProperty("TAE ")]
            public string TAE { get; set; }

            public string type { get; set; }
        }
        private string IP = "192.168.120.187";
        private int Port = 1969;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "OAM");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void connection(string host, int port, out bool flagConnected)
        {
            flagConnected = false;
            try
            {
                oam = OamCommunicator.GetInstance(host, port);
                oam.Connect();
                if (oam.IsConnected) flagConnected = true;
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void cliCommand(string cli)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send(cli);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    dynamic js = JsonConvert.DeserializeObject(received);
                    Log(js);
                    if (json.status == 0)
                    {
                        Log(json.status.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public bool RRUSynCheck(bool rruSync)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW RUN_SYNC_STATE: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            Log("RRU sync stage :" + item.RRSyncState);
                            if (item.RRSyncState == "LOCKED")
                            {
                                rruSync = true;
                            }
                            else
                            {
                                rruSync = false;
                            }
                        }
                    }
                    else
                    {
                        Log("cli RRU sync stage status: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
            return rruSync;
        }
        public bool DUSynCheck(bool duSync)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("GET PHC2SYS: DUId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            Log("DU sync stage :" + item.SyncState);
                            if (item.SyncState == "LOCKED")
                            {
                                duSync = true;
                            }
                            else
                            {
                                duSync = false;
                            }
                        }
                    }
                    else
                    {
                        Log("cli DUSynCheck status: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM DUSynCheck Exception: " + e.Message);
            }
            return duSync;
        }

        public void SHWDIAGRRU()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW DIAG_RRU_CHIP: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("Status: " + json.status);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void RRUVER(out string buidDate)
        {
            buidDate = "";
            string[] arr = new string[10];
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW RRU_VERSION: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);

                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            buidDate += item.BuildDate + " ";
                        }
                    }
                    arr = buidDate.Split(' ');
                    buidDate = arr[0];
                    Log("Firmware release date :" + buidDate);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }

        }
        public void SHWFREQ()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW FREQ: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("[RECV]: " + json.status);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }

        }

        public void CHGFREQ(string Freq)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHG FREQ: CellId=1, absolute_frequency_center=" + Freq + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("[RECV]: " + json.status);
                    if (json.status == 0)
                    {
                        Log("CLI CHG Freq status: " + Freq + " " + json.status);
                    }
                    else
                    {
                        Log("CLI CHG Freq status: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void Switch_Mode_CALIB(string mode)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SWITCH TRX_CALIB: CellId=1, mode=" + mode + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("[RECV]: " + json.status);
                    if (json.status == 0)
                    {
                        Log("Switch Mode CALIB: " + mode + ": " + json.status);
                    }
                    else
                    {
                        Log("Switch Mode CALIB status: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CHGRRU_POWER(int power)
        {
            if (oam.IsConnected)
            {
                oam.Send("CHG RRU_PWR: CellId=1, rru_pwr=" + power + ";");
                var header = new OamHeader();
                var received = oam.Receive(header);
                Data json = JsonConvert.DeserializeObject<Data>(received);
                if (json.status == 0)
                {
                    Log("CHG RRU_PWR : " + power + "mW");
                }
                else
                {
                    Log("CLI CHG RRU_PWR status : " + json.status);
                }
            }
        }

        public void RBTRRU()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("RBT RRU: CellId = 1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void NRTM(string NRTM)
        {
            string TM = "TM1.1";
            switch (NRTM)
            {
                case "NRTM_11":
                    TM = "TM1.1";
                    break;
                case "NRTM_12":
                    TM = "TM1.2";
                    break;
                case "NRTM_20":
                    TM = "TM2.0";
                    break;
                case "NRTM_20a":
                    TM = "TM2.0a";
                    break;
                case "NRTM_31":
                    TM = "TM3.1";
                    break;
                case "NRTM_31a":
                    TM = "TM3.1a";
                    break;
                case "NRTM_32":
                    TM = "TM3.2";
                    break;
                case "NRTM_33":
                    TM = "TM3.3";
                    break;
            }
            try
            {
                if (oam.IsConnected)
                {
                    string cmd = $"TST NRTM: DUId=1, TM={TM};";
                    oam.Send(cmd);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            Log(item.Sample0 + "\n" + item.Sample1 + "\n" + item.Sample2);
                        }
                    }
                    else
                    {
                        Log("CLI NRTM command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }

        }

        //add bandwidth
        public void NRTM(string NRTM, string bandwidth)
        {
            string TM = "TM1.1";
            switch (NRTM)
            {
                case "NRTM_11":
                    TM = "TM1.1";
                    break;
                case "NRTM_12":
                    TM = "TM1.2";
                    break;
                case "NRTM_20":
                    TM = "TM2.0";
                    break;
                case "NRTM_20a":
                    TM = "TM2.0a";
                    break;
                case "NRTM_31":
                    TM = "TM3.1";
                    break;
                case "NRTM_31a":
                    TM = "TM3.1a";
                    break;
                case "NRTM_32":
                    TM = "TM3.2";
                    break;
                case "NRTM_33":
                    TM = "TM3.3";
                    break;
            }
            try
            {
                if (oam.IsConnected)
                {
                    string cmd = $"TST NRTM: DUId=1, TM={TM}, Bandwidth={bandwidth};";
                    oam.Send(cmd);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            Log(item.Sample0 + "\n" + item.Sample1 + "\n" + item.Sample2);
                        }
                    }
                    else
                    {
                        Log("CLI NRTM command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }

        }
        // TODO: RXSENS function for measuring SEN item
        public void RXSENS(int port, out double TP)
        {
            TP = 0;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("TST RXSENS: DUId=1, AntennaPort=" + port + ", NumberSlot=2000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            TP = Math.Round(double.Parse((item.TP).Substring(1, 5)), 1);
                            //TP = (item.TP).Substring(1, 5);//lay theo dung gia tri doc ve

                        }
                    }
                    else
                    {
                        Log("command RXSENS: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        // TODO: RXDYN function for measuring DYN item
        public void RXDYN(int port, out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    //TST RXDYN: DUId=1, AntennaPort=1, NumberSlot=8000;
                    oam.Send("TST RXDYN: DUId=1, AntennaPort=" + port + ", NumberSlot=2000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP = Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString();
                            TP = (item.TP).Substring(1, 5);//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXDYN: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void NEW_RXSENS(out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("TST RXSENS: DUId=1, NumberSlot=8000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP += Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString() + " ";
                            TP += (item.TP).Substring(1, 5) + " ";//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXSENS: FAIL");
                    }
                    Log(TP);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void NEW_RXSENS(string RBOffset, out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send($"TST RXSENS: DUId=1, NumberSlot=8000, RBOffset={RBOffset};");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP += Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString() + " ";
                            TP += (item.TP).Substring(1, 5) + " ";//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXSENS: FAIL");
                    }
                    Log(TP);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void NEW_RXDYN(out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("TST RXDYN: DUId=1, NumberSlot=8000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP += Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString() + " ";
                            TP += (item.TP).Substring(1, 5) + " ";//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXSENS: FAIL");
                    }
                    Log(TP);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void NEW_RXDYN(string RbOffset, out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send($"TST RXDYN: DUId=1, NumberSlot=8000; RBOffset={RbOffset}");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP += Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString() + " ";
                            TP += (item.TP).Substring(1, 5) + " ";//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXSENS: FAIL");
                    }
                    Log(TP);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void STPTRX()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("STP TRXTEST: DUId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("Kill TRX process");
                    }
                    else
                    {
                        Log("command kill process: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void PREPARE_CALIB_RRU(string port, string gainDefault)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("PREPARE CALIB_RRU: CellId=1, channel_index=" + port + ", gain=" + gainDefault + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    dynamic js = JsonConvert.DeserializeObject(received);
                    Log(js);
                    if (json.status == 0)
                    {
                        Log("Set gain TX" + port + " " + gainDefault);
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CALIBRRU(string expectedPower, string measuredPower, out string rruReadingPower, out string gain)
        {
            rruReadingPower = null;
            gain = null;
            try
            {
                string tmp = (double.Parse(expectedPower) * 1000).ToString();
                string tmp1 = (double.Parse(measuredPower) * 1000).ToString();
                tmp1 = tmp1.Substring(0, 5);
                if (oam.IsConnected)
                {
                    oam.Send("CALIB RRU: CellId=1, expected_power=" + tmp + ", measured_power=" + tmp1 + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    dynamic js = JsonConvert.DeserializeObject(received);
                    Log(js);
                    if (json.status == 0)
                    {
                        Log("CALIB RRU: CellId=1, expected_power=" + tmp + ", measured_power=" + tmp1 + ";");
                    }
                    else
                    {
                        Log("CLI Send command CALIB RRU: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        string[] tmp = new string[8];
        public void RXNOISE(out string[] noise)
        {
            noise = tmp;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("TST NOISE: DUId=1, NumberSlot=2000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    dynamic js = JsonConvert.DeserializeObject(received);
                    Log(js);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            int start = item.RX0.IndexOf("Noise :");
                            string tmp = item.RX0.Substring(start + 8, 6);
                            string tmp1 = item.RX1.Substring(start + 8, 6);
                            string tmp2 = item.RX2.Substring(start + 8, 6);
                            string tmp3 = item.RX3.Substring(start + 8, 6);
                            noise[0] = tmp;
                            noise[1] = tmp1;
                            noise[2] = tmp2;
                            noise[3] = tmp3;
                        }
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void Write_SN(string Component, int pos, string serial)
        {
            try
            {
                if (oam.IsConnected)
                {
                    Log("CHG RRU_SN: CellId=1, device_name=" + Component + ", position=" + pos + ", new_serial=" + serial + ";");
                    oam.Send("CHG RRU_SN: CellId=1, device_name=" + Component + ", position=" + pos + ", new_serial=" + serial + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("write " + Component + " Serial: " + serial);
                    }
                    else
                    {
                        Log("CLI command wite " + Component + ": FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CHG_Mac(int index, string newMac)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHG RRU_MAC: CellId=1, mac_index=" + index + ", new_mac=" + newMac + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("write Mac: " + index + " " + newMac + " " + json.status);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void SHW_Mac(out string mac0, out string mac1)
        {
            mac0 = "";
            mac1 = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW RRU_MAC: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            mac0 = item.Mac0;
                            mac1 = item.Mac1;
                            Log("mac0: " + mac0);
                            Log("mac1: " + mac1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        string[] tmp1 = new string[6];
        public void Show_SN(string device, out string serial1, out string serial2)
        {
            serial1 = "";
            serial2 = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW RRU_SN: CellId=1, device_name=" + device + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            if (device == "PA_BOARD")
                            {
                                serial1 = item.SerialNum0;
                                serial2 = item.SerialNum1;
                                Log("Serial PA1 " + serial1);
                                Log("Serial PA2 " + serial2);
                            }
                            else
                            {
                                serial1 = item.SerialNum0;
                                Log("Serial " + device + " :" + serial1);
                            }
                        }
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void SwitchChip(string i)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SWITCH RRU_RX_CHIP: CellId=1, chip_index=" + i + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("Switch chip" + i);
                    }
                    else
                    {
                        Log("CLI Switch chip: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        //8.7.2024 - Swwitch group Redhat
        public void SwitchGroup(string GroupId)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send($"SWT RXGRP: DUId=1, GroupId={GroupId};");
                    Log($"SWT RXGRP: DUId=1, GroupId={GroupId};");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("Switch to RX_group " + GroupId);
                    }
                    else
                    {
                        Log("CLI Switch group: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        //8.7.2024 - Swwitch group Redhat
        //public void SwitchGroup(string GroupId)
        //{
        //    try
        //    {
        //        if (oam.IsConnected)
        //        {                    
        //            oam.Send($"SWT RXGRP: DUId=1, GroupId={GroupId};");
        //            Log($"SWT RXGRP: DUId=1, GroupId={GroupId};");
        //            var header = new OamHeader();
        //            var received = oam.Receive(header);
        //            Data json = JsonConvert.DeserializeObject<Data>(received);
        //            if (json.status == 0)
        //            {
        //                Log("Switch to RX_group " + GroupId);
        //            }
        //            else
        //            {
        //                Log("CLI Switch group: FAIL");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log("OAM Exception: " + e.Message);
        //    }
        //}

        public void AISG(out bool flag)
        {
            flag = false;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHECK AISG: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    dynamic js = JsonConvert.DeserializeObject(received);
                    //Log(js);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            if (item.AISG == "succcess")
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                            Log("Aisg State: " + item.AISG);
                        }
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }


        public void TaeAvg(int port, out string taeAVG)
        {
            taeAVG = null;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("TST TAERX: DUId=1, AntennaPort=" + port + ", NumberSlot=2000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            taeAVG = item.TAEAVG.Trim();
                        }
                    }
                    else
                    {
                        Log("CLI TaeAVG command: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void NEW_TAE(out string TAE)
        {
            TAE = null;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("TST TAERX: DUId=1, NumberSlot=2000;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);

                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            TAE += Math.Round(double.Parse(item.TAE)).ToString() + " ";
                        }
                        Log(TAE);
                    }
                    else
                    {
                        Log("CLI TaeAVG command: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        string[] tmp3 = new string[8];
        public void VSWR(out string[] vswr)
        {
            vswr = tmp3;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW VSWR: CellId=1, MODE=ALL;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            vswr[0] = item.Vswr0.ToString();
                            vswr[1] = item.Vswr1.ToString();
                            vswr[2] = item.Vswr2.ToString();
                            vswr[3] = item.Vswr3.ToString();
                            vswr[4] = item.Vswr4.ToString();
                            vswr[5] = item.Vswr5.ToString();
                            vswr[6] = item.Vswr6.ToString();
                            vswr[7] = item.Vswr7.ToString();
                        }
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void START_RX_Calib(out bool flag)
        {
            flag = false;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("START RRU_RX: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        flag = true;
                        Log("Starting RRU RX calibration");
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void Sample_RX_Calib(int port, out double RxPowerMean)
        {
            RxPowerMean = 0;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SAMPLE RRU_RX: CellId=1, channel_index=" + port + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            double tmp;
                            double tmp1;
                            double.TryParse(item.RxPowerMean, out tmp);
                            double.TryParse(item.RxPowerPeak, out tmp1);
                            RxPowerMean = (tmp / 1000);
                            RxPowerMean = Math.Round(RxPowerMean, 2);
                        }
                        Log("RX MeanPower " + port.ToString() + ":" + RxPowerMean.ToString());
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void Finish_RX_Calib(out double rxMinPower)
        {
            rxMinPower = 0;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("FINISH RRU_RX: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            double tmp;
                            bool flag = double.TryParse(item.RxPowerMin, out tmp);
                            if (flag)
                            {
                                rxMinPower = (tmp / 1000);
                                Log("RxMinPower: " + rxMinPower + "dbfs");
                            }
                        }
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void RF_INFO(out string RSSI)
        {
            RSSI = null;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW RF_INFO: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            RSSI += item.ANTDB + " ";
                        }

                    }
                    else
                    {
                        Log("CLI command: Status " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void RSTSW()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("RST SW: ;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("Reset software");
                    }
                    else
                    {
                        Log("CLI command: FAIL");
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void Dispose()
        {
            bool flag;
            try
            {
                oam.Dispose();
                connection(IP, Port, out flag);
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        // Calib TX Ver2
        public void Calib_RRU(string mode) // mode STR, STP
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send(mode + " CALIB_RRU: CellId=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log(mode + " CalibTx Status :" + json.status);
                }
            }
            catch (Exception e)
            {
                Log(mode + " CalibTx Status: " + e.Message);
            }
        }

        public void SHW_TX_GAIN(string port, out string gainTX)
        {
            gainTX = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW TRX_GAIN: CellId=1, rru_type=TX, channel_index=" + port + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            gainTX = item.gain;
                            Log("Gain Tx" + port + " = " + gainTX);
                        }
                    }
                    else
                    {
                        Log("CLI SHW_TX_GAIN Status :" + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CHG_TX_GAIN(string port, string gain)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHG TRX_GAIN: CellId=1, rru_type=TX, channel_index=" + port + ", gain=" + gain + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("CLI CHG_TX_GAIN Status :" + json.status);
                    }
                    else
                    {
                        Log("CLI CHG_TX_GAIN Status :" + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void CHG_RX_GAIN(string port, string gain)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHG TRX_GAIN: CellId=1, rru_type=RX, channel_index=" + port + ", gain=" + gain + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("CLI CHG_RX_GAIN Status :" + json.status);
                    }
                    else
                    {
                        Log("CLI CHG_RX_GAIN Status :" + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CAL_RRU_PWR(string totalchn, string channarr, string mode, string vsaPower)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CAL RRU_PWR: CellId=1" + ", mode=" + mode + ", total_channel=" + totalchn + ", channel_arr=" + channarr + ", vsa_power=" + vsaPower + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("CLI CAL RRU_PWR Status :" + json.status);
                    }
                    else
                    {
                        Log("CLI CAL RRU_PWR Status :" + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CLB_XADC(string totalchn, string channarr)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CLB XADC: CellId=1" + ", total_channel=" + totalchn + ", channel_arr=" + channarr + "; ");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("CLI CLB_XADC Status :" + json.status);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void CALIB_RRU(string totalchn, string channarr, string lowRange, string highRange)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CALIB RRU: CellId=1" + ", total_channel=" + totalchn + ", channel_arr=" + channarr + ", gain_low_range=" + lowRange + ", gain_high_range=" + highRange + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("CLI CALIB_RRU Status :" + json.status);
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void TDD_MODE(string mode)
        {
            //mode = "CALIB";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHG TDD_MODE: CellId=1, tdd_mode=" + mode + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("TDD_MODE :" + mode + "  " + json.status);
                    }
                    else
                    {
                        Log("Switch TDD_MODE :" + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public void TEMP_CONTROL(string mode)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("CHG TEMP_CONTROL: CellId=1, status=" + mode + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    Log("TEMP_CONTROL :" + mode + "  " + json.status);
                }
            }
            catch (Exception e)
            {
                Log("CHG TEMP_CONTROL: " + e.Message);
            }
        }

        public void PA_VERSION(int index, out string type)
        {
            type = "";
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW PA_OPERATING: CellId=1, pa_index=" + index + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            type = item.type;
                            Log("PA type: " + type);
                        }
                    }
                    else
                    {
                        Log("SHOW PA_VERSION: " + json.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("SHOW PA_VERSION: " + e.Message);
            }
        }

        public void UpdateFw()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("ACT FW: UnitType=RRU, UnitIndex=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Log("ACT FW: UnitType=RRU, UnitIndex=1;");
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public int Init_DFE()
        {
            int i = 0;
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("SHW INIT_DFE_STATUS: CellId=1, dfe=1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    foreach (Root item in json.data)
                    {
                        if (item.status != "SUCCESS")
                        {
                            i = 1;
                        }
                        Log("status dfe 1: " + item.status);
                    }
                    oam.Send("SHW INIT_DFE_STATUS: CellId=1, dfe=2;");
                    json = JsonConvert.DeserializeObject<Data>(received);
                    foreach (Root item in json.data)
                    {
                        if (item.status != "SUCCESS")
                        {
                            i = 1;
                        }
                        Log("status dfe 2: " + item.status);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
            return i;
        }
        public int SYNC_CPRI()
        {
            int i = 0;
            try
            {
                for (int x = 1; x <= 6; x++)
                {
                    oam.Send("SHW SYNC_CPRI_STATUS: CellId=1, core=" + x + ";");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            if (item.state == "LOSS SYNC CPRI ")
                            {
                                i = 1;
                            }
                            Log("state core" + x + " :" + item.state);
                        }
                    }
                    else
                    {
                        i = 1;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
            return i;
        }

        public void RXSENS_V2(out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    string cli = "SHW RXSENS_V2: DUId=1;";
                    oam.Send(cli);
                    Log("Send command: " + cli);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    string json1 = JsonConvert.SerializeObject(json);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP += Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString() + " ";
                            TP = (item.TP).Substring(1, 5);//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXSENS_V2: FAIL", LogLevel.ERROR);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }
        public void RXSENS_V3(out string TP)
        {
            TP = "";
            try
            {
                if (oam.IsConnected)
                {
                    string cli = "SHW RXSENS_V3: DUId=1;";
                    oam.Send(cli);
                    Log("Send command: " + cli);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    string json1 = JsonConvert.SerializeObject(json);
                    if (json.status == 0)
                    {
                        foreach (Root item in json.data)
                        {
                            //TP += Math.Round(double.Parse((item.TP).Substring(1, 5)), 1).ToString() + " ";
                            TP = (item.TP).Substring(1, 5);//lay theo dung gia tri doc ve
                        }
                    }
                    else
                    {
                        Log("command RXSENS_V3: FAIL", LogLevel.ERROR);
                    }
                }
            }
            catch (Exception e)
            {
                Log("OAM Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        public bool STR_RXSENS_V2(int AntPort)
        {
            bool isSuccess = false;

            try
            {
                int ConvertPort = 0;
                string rruType = mainForm.RruType;
                Log($"PORT={AntPort}, RRU TYPE={rruType}");
                if (oam.IsConnected)
                {
                    switch (rruType)
                    {
                        case "8T8R":
                            ConvertPort = (AntPort % 4 == 0) ? 4 : (AntPort % 4);
                            break;
                        case "32T32R":
                            ConvertPort = (AntPort % 8 == 0) ? 8 : (AntPort % 8);
                            break;
                        default:
                            break;
                    }

                    //if (AntPort < 8)
                    //{
                    //    ConvertPort = AntPort;
                    //}
                    //else
                    //{
                    //    ConvertPort = AntPort - 8;
                    //}

                    string cli = "STR RXSENS_V2: DUId=1, AntennaPort=" + ConvertPort.ToString() + ";";
                    oam.Send(cli);
                    Log($"Send command: STR RXSENS_V2: DUId=1, AntennaPort={ConvertPort};");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    string json1 = JsonConvert.SerializeObject(json);
                    if (json.status == 0)
                    {
                        Log("command STR_RXSENS_V2: PASS", LogLevel.SUCCESS);
                        isSuccess = true;
                    }
                    else
                    {
                        Log("command STR_RXSENS_V2: FAIL", LogLevel.ERROR);
                    }
                }
            }
            catch (Exception e)
            {
                Log("<STR_RXSENS_V2> OAM Exception: " + e.Message, LogLevel.ERROR);
            }

            return isSuccess;
        }
        public bool STR_RXSENS_V3(int AntPort)
        {
            bool isSuccess = false;

            try
            {
                int ConvertPort = 0;
                string rruType = mainForm.RruType;
                Log($"PORT={AntPort}, RRU TYPE={rruType}");
                if (oam.IsConnected)
                {
                    switch (rruType)
                    {
                        case "8T8R":
                            ConvertPort = (AntPort % 4 == 0) ? 4 : (AntPort % 4);
                            break;
                        case "32T32R":
                            ConvertPort = (AntPort % 8 == 0) ? 8 : (AntPort % 8);
                            break;
                        default:
                            break;
                    }

                    string cli = "STR RXSENS_V3: DUId=1, AntennaPort=" + ConvertPort.ToString() + ";";
                    oam.Send(cli);
                    Log($"Send command: STR RXSENS_V3: DUId=1, AntennaPort={ConvertPort};");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    string json1 = JsonConvert.SerializeObject(json);
                    if (json.status == 0)
                    {
                        Log("command STR_RXSENS_V3: PASS", LogLevel.SUCCESS);
                        isSuccess = true;
                    }
                    else
                    {
                        Log("command STR_RXSENS_V3: FAIL", LogLevel.ERROR);
                    }
                }
            }
            catch (Exception e)
            {
                Log("<STR_RXSENS_V3> OAM Exception: " + e.Message, LogLevel.ERROR);
            }

            return isSuccess;
        }

        //Kiem tra xem RxSens_V2 da co File out ra chua?
        public bool VALIDATEFILEOUT_RXSENS_V2()
        {
            bool isExist = false;
            try
            {
                if (oam.IsConnected)
                {
                    string cli = "SHW RXSENS_V2: DUId=1;";
                    oam.Send(cli, false);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    string json1 = JsonConvert.SerializeObject(json);
                    if (json.status == 0)
                    {
                        isExist = true;
                    }
                    else
                    {
                        isExist = false;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return isExist;
        }
        public bool VALIDATEFILEOUT_RXSENS_V3()
        {
            bool isExist = false;
            try
            {
                if (oam.IsConnected)
                {
                    string cli = "SHW RXSENS_V3: DUId=1;";
                    oam.Send(cli, false);
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    string json1 = JsonConvert.SerializeObject(json);
                    if (json.status == 0)
                    {
                        isExist = true;
                    }
                    else
                    {
                        isExist = false;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return isExist;
        }

        public void STOP_L1TESTMAC()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("STP RXSENS_V2: DUId = 1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("STOP_L1_TESTMAC: Done!");
                    }
                    else
                    {
                        Log("STOP_L1_TESTMAC: Fail!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("STOP_L1_TESTMAC: Fail! " + ex.Message);
            }
        }

        public void STOP_L1TESTMAC_V3()
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("STP RXSENS_V3: DUId = 1;");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("STOP_L1_TESTMAC_V3: Done!");
                    }
                    else
                    {
                        Log("STOP_L1_TESTMAC_V3: Fail!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("STOP_L1_TESTMAC_V3: Fail! " + ex.Message);
            }
        }

        public void FACTORY_ANT_CALIB(string mode)
        {
            try
            {
                if (oam.IsConnected)
                {
                    oam.Send("STP RXSENS_V3: DUId = 1;");
                    oam.Send($"CLB FACTORY_CONTROL: CellId=1, mode={mode};");
                    var header = new OamHeader();
                    var received = oam.Receive(header);
                    Data json = JsonConvert.DeserializeObject<Data>(received);
                    if (json.status == 0)
                    {
                        Log("FACTORY_ANT_CALIB: Done!");
                    }
                    else
                    {
                        Log("FACTORY_ANT_CALIB: Fail!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("FACTORY_ANT_CALIB: Fail! " + ex.Message);
            }
        }
    }
}
