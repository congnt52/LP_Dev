namespace _5GAutoTool
{
    public class BBU
    {
        XeonD xeonD;
        OAM oam;
        Server sever;
        string bbuVersion = "0";
        public Form5GAT mainForm;
        public string Name = "BBU";
        public BBU(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
            oam = new OAM(mainForm);
            xeonD = new XeonD(mainForm);
            sever = new Server(mainForm);
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "BBU");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        public void BBUVersion(string Version_BBU)
        {
            bbuVersion = Version_BBU;
        }

        public void Connection(string IP, string Username, string Password, out bool statusConnection)
        {
            statusConnection = false;
            if (bbuVersion == "XEON D")
            {
                xeonD.Connection(IP, Username, Password, out statusConnection);
                Name = bbuVersion;
            }
            else
            {
                Log("Kiem tra phien ban BBU");
            }

        }
        public void Disconnect()
        {
            if (bbuVersion == "XEON Gold")
            {
                xeonD.Disconnect();
            }
            else
            {

            }
        }
        public void Generate_NRTM(string NRTM)
        {
            //xeonD.KillHardcode();
            //xeonD.GenerateNRTM(NRTM);
            oam.NRTM(NRTM);
            System.Threading.Thread.Sleep(1000);
        }
        public void Generate_TAE()
        {
            xeonD.GenerateNRTM("TAE");
        }
        public void SW_Start()
        {
            if (bbuVersion == "XEON D")
            {
                //  xeonD.SwStart();
            }
        }

        public void LoadDriver()
        {
            if (bbuVersion == "XEON D")
            {
                //  xeonD.DriverLoad();
            }
        }
        public void KillHardcode()
        {
            if (bbuVersion == "XEON D")
            {
                xeonD.KillHardcode();
            }
        }
        public void SwitchPort(string port)
        {
            xeonD.SwitchPort(port);
        }

        #region OAM CLI
        public void CLICmd(string cmd)
        {
            oam.cliCommand(cmd);
        }
        public void loginOAM(string IP, int port, out bool flagConnected)
        {
            oam.connection(IP, port, out flagConnected);
        }
        public void CHG_Freq(string freq)
        {
            oam.CHGFREQ(freq);
        }

        public void NEW_RXSENS(out string TP)
        {
            oam.NEW_RXSENS(out TP);
        }

        public void RXSENS_V2(out string TP)
        {
            oam.RXSENS_V2(out TP);
        }
        public void RXSENS_V3(out string TP)
        {
            oam.RXSENS_V3(out TP);
        }
        public bool STR_RXSENS_V2(int AntPort)
        {
            return oam.STR_RXSENS_V2(AntPort);
        }
        public bool STR_RXSENS_V3(int AntPort)
        {
            return oam.STR_RXSENS_V3(AntPort);
        }

        public void NEW_RXSENS(string RbOffset, out string TP)
        {
            oam.NEW_RXSENS(RbOffset, out TP);
        }

        public void RXDYN(int port, out string TP)
        {
            oam.RXDYN(port, out TP);
        }

        public void NEW_RXDYN(out string TP)
        {
            oam.NEW_RXDYN(out TP);
        }

        public void NEW_RXDYN(string RbOffset, out string TP)
        {
            oam.NEW_RXDYN(RbOffset, out TP);
        }
        public void StopProcess()
        {
            oam.STPTRX();
        }
        public void Noise(out string[] noise)
        {
            oam.RXNOISE(out noise);
        }
        public void RRUVER(out string builDate)
        {
            oam.RRUVER(out builDate);
        }
        public void CHG_RRU_POWER(int power)
        {
            oam.CHGRRU_POWER(power);
        }
        public void PREPARE_CALIB_RRU(string port, string gainDefault)
        {
            oam.PREPARE_CALIB_RRU(port, gainDefault);
        }
        public void CALIB_TX(string expectedPower, string measuredPower, out string rruReadingPower, out string gain)
        {
            oam.CALIBRRU(expectedPower, measuredPower, out rruReadingPower, out gain);
        }

        public void writeSerial(string component, int pos, string newSerial)
        {
            oam.Write_SN(component, pos, newSerial);
        }

        public bool SyncCheck(bool Sync)
        {
            bool duSync = false, rruSync = false;
            duSync = oam.DUSynCheck(duSync);
            rruSync = oam.RRUSynCheck(rruSync);
            Sync = duSync && rruSync;
            return Sync;
        }
        public bool RruSyncCheck(bool rruSync)
        {
            rruSync = oam.RRUSynCheck(rruSync);
            return rruSync;
        }
        public void Switch(string i)
        {
            oam.SwitchChip(i);
        }
        public void Switch(int port)
        {
            switch (mainForm.RruType)
            {
                case "8T8R":
                    if (port < 4)
                    {
                        oam.SwitchChip("1");
                    }
                    else if ((port >= 4) && (port < 8))
                    {
                        oam.SwitchChip("2");
                    }
                    break;

                case "32T32R":
                    if (port < 8)
                    {
                        oam.SwitchChip("1");
                    }
                    else if ((port >= 8) && (port < 16))
                    {
                        oam.SwitchChip("3");
                    }
                    else if ((port >= 16) && (port < 24))
                    {
                        oam.SwitchChip("4");
                    }
                    else if ((port >= 24) && (port < 32))
                    {
                        oam.SwitchChip("2");
                    }
                    else
                    {
                        Log("WARN: RRU is not supported higher than 32 channel", LogLevel.WARN);
                    }
                    break;

                default:
                    break;
            }

        }
        public void SwitchGroup(string i)
        {
            oam.SwitchGroup(i);
        }
        public void AISGCheck(out bool flag)
        {
            oam.AISG(out flag);
        }
        public void VSWR(out string[] vswr)
        {
            oam.VSWR(out vswr);
        }
        public void TAEAVG(int port, out string taeAVG)
        {
            oam.TaeAvg(port, out taeAVG);
        }
        public void RSTSW()
        {
            oam.RSTSW();
        }
        public void GetSN(string sn_rru, out string[] sn)
        {
            sever.Connect(sn_rru, out sn);
        }
        public void ReadSN(string deive, out string sn1, out string sn2)
        {
            oam.Show_SN(deive, out sn1, out sn2);
        }
        public void StartRXCalib(out bool flag)
        {
            oam.START_RX_Calib(out flag);
        }
        public void SampleRXCalib(int port, out double rxPowerMean)
        {
            oam.Sample_RX_Calib(port, out rxPowerMean);
        }
        public void FinishRXCalib(out double rxMinPower)
        {
            oam.Finish_RX_Calib(out rxMinPower);
        }
        public void RFINFO(out string antPower)
        {
            oam.RF_INFO(out antPower);
        }
        public bool VALIDATEFILEOUT_RXSENS_V2()
        {
            return oam.VALIDATEFILEOUT_RXSENS_V2();
        }
        public bool VALIDATEFILEOUT_RXSENS_V3()
        {
            return oam.VALIDATEFILEOUT_RXSENS_V3();
        }

        public void STOP_L1TESTMAC()
        {
            oam.STOP_L1TESTMAC();
        }
        public void STOP_L1TESTMAC_V3()
        {
            oam.STOP_L1TESTMAC_V3();
        }

        public void CHGmac(int index, string newMac)
        {
            oam.CHG_Mac(index, newMac);
        }
        public void SHWmac(out string mac0, out string mac1)
        {
            oam.SHW_Mac(out mac0, out mac1);
        }
        public void Dispose()
        {
            oam.Dispose();
        }


        // Calib V2

        public void Calib_RRU(string mode)
        {
            oam.Calib_RRU(mode);
        }

        public string SHW_TX_GAIN(string port, out string gain)
        {
            oam.SHW_TX_GAIN(port, out gain);
            return gain;
        }

        public void CHG_TX_GAIN(string port, string gain)
        {
            oam.CHG_TX_GAIN(port, gain);
        }

        public void CHG_RX_GAIN(string port, string gain)
        {
            oam.CHG_RX_GAIN(port, gain);
        }

        public void CAL_RRU_PWR(string totalchn, string channarr, string mode, string vsaPower)
        {
            oam.CAL_RRU_PWR(totalchn, channarr, mode, vsaPower);
        }
        public void CLB_XADC(string totalchn, string channarr)
        {
            oam.CLB_XADC(totalchn, channarr);
        }

        public void CALIB_RRU(string totalchn, string channarr, string lowRange, string highRange)
        {
            oam.CALIB_RRU(totalchn, channarr, lowRange, highRange);
        }
        public void TDDMODE(string mode)
        {
            oam.TDD_MODE(mode);
        }
        public void FACTORY_ANT_CALIB(string mode)
        {
            oam.FACTORY_ANT_CALIB(mode);
        }
        public int check_CPRI_SYNC()
        {
            int i = oam.SYNC_CPRI();
            int j = oam.Init_DFE();
            int x = 0;
            if (i == 1 || j == 1)
            { x = 1; }
            return x;
        }
        #endregion
    }
}
