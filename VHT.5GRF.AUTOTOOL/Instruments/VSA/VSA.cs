using System;
using _5GAutoTool.Instruments.VSA.VSA_Instruments;

namespace _5GAutoTool
{
    public class VSA
    {
        public string Name;
        public string Serial;
        public string IpAddress;
        public string Port;
        public string Manufacturer;
        public string Model;
        public string Status;
        public string CurrentRecallFile;

        public FSV3030 fsv3030;
        public N9020A n9020a;
        public FSW26 fsw26;
        public N9020B n9020b;
        public M9410A m9410a;
        public LitePoint iqfr1;
        string instrumentVSA = "";
        public Form5GAT mainForm;
        public VSA(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
            fsv3030 = new FSV3030(mainForm);
            n9020a = new N9020A(mainForm);
            fsw26 = new FSW26(mainForm);
            n9020b = new N9020B(mainForm);
            m9410a = new M9410A(mainForm);
            iqfr1  = new LitePoint(mainForm);
            
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "VSA");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        // Lua chon loai may VSA (Bat buoc)
        /*
            - fsv3030 : R&S FSV3030
            - n9030b: Keysight N9030B
            - n9020a: Keysight N9020A
            - mimo: Keysight PXI
        */
        /*..............................................Connection....................................................................*/
        public void Instrument(string VSA_Instrument)
        {
            instrumentVSA = VSA_Instrument;
        }
        public void RRUinfo(string rruinfo)
        {
            if (instrumentVSA == "FSV3030")
            {
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.RRUinfo(rruinfo);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.RRUinfo(rruinfo);
            }
        }

        public void Connection(string IP, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            switch (instrumentVSA)
            {
                case "FSV3030":
                    fsv3030.Connection(IP, Cmd, instrumentVSA, out Manufacturer, out Model, out Serial);
                    break;
                case "N9020A":
                    n9020a.Connection(IP, Cmd, instrumentVSA, out Manufacturer, out Model, out Serial);
                    break;
                case "N9020B":
                    n9020b.Connection(IP, Cmd, instrumentVSA, out Manufacturer, out Model, out Serial);
                    break;
                case "N9030B":
                    n9020b.Connection(IP, Cmd, instrumentVSA, out Manufacturer, out Model, out Serial);
                    break;
                case "FSW26":
                    fsw26.Connection(IP, Cmd, instrumentVSA, out Manufacturer, out Model, out Serial);
                    break;
                case "M9410A":
                    m9410a.Connect(IP, Cmd, out Manufacturer, out Model, out Serial);
                    break;
                case "IQFR1-RU":
                    int port = 24000;
                    iqfr1.Connect(IP, port, out Manufacturer, out Model, out Serial);
                    break;
                default:
                    Log("Kiem tra ma loai may VSA", LogLevel.WARN);
                    break;
            }
            Name = instrumentVSA + $"[S/N:{Serial}]";
        }

        public void Disconnect()
        {
            switch (instrumentVSA)
            {
                case "FSV3030":
                    fsv3030.Disconnect();
                    break;
                case "FSW26":
                    fsw26.Disconnect();
                    break;
                case "N9020A":
                    n9020a.Disconnect();
                    break;
                case "N9020B":
                    n9020b.Disconnect();
                    break;
                case "N9030B":
                    n9020b.Disconnect();
                    break;
                case "M9410A":
                    m9410a.Disconnect();
                    break;
                case "IQ-FR1":
                    iqfr1.Disconnect();
                    break;
            }
        }

        public void SendCmd(string Cmd, out string respond)
        {
            respond = "";
            switch (instrumentVSA)
            {

                case "FSV3030":
                    fsv3030.SendCmd(Cmd, out respond);
                    break;
                case "N9020A":
                    n9020a.SendCmd(Cmd, out respond);
                    break;
                case "FSW26":
                    fsw26.SendCmd(Cmd, out respond);
                    break;
                case "N9020B":
                    n9020b.SendCmd(Cmd, out respond);
                    break;
                case "N9030B":
                    n9020b.SendCmd(Cmd, out respond);
                    break;
                case "IQ-FR1":
                    iqfr1.SendCmd(Cmd);
                    respond = iqfr1.ReadResponse(1000);
                    break;
            }
        }
        /*..............................................Setup tham số................................................................*/
        public void SetParameter(string freq, string att)
        {
            switch (instrumentVSA)
            {
                case "FSV3030":
                    fsv3030.FreqCent(freq);
                    fsv3030.RLEVOFFS(att);
                    break;
                case "N9020A":
                    n9020a.SetFreqCent(freq);
                    n9020a.SetAtt(att);
                    break;
                case "FSW26":
                    fsw26.FreqCent(freq);
                    fsw26.RLEVOFFS(att);
                    break;
                case "N9020B":
                    n9020b.SetFreqCent(freq);
                    n9020b.SetAtt(att);
                    break;
                case "N9030B":
                    n9020b.SetFreqCent(freq);
                    n9020b.SetAtt(att);
                    break;
                case "M9410A":
                    m9410a.SetFreqCent(freq);
                    System.Threading.Thread.Sleep(2000);
                    m9410a.SetAtt(att);
                    System.Threading.Thread.Sleep(2000);
                    break;
                default:
                    Log("VSA Set Frequency: FAIL! ", LogLevel.ERROR);
                    break;
            }
        }
        public void AdjustTiming()
        {
            if (instrumentVSA == "FSW26")
            {
                fsw26.AdjustTiming();
            }
        }
        public void AutoScale()
        {
            switch (instrumentVSA)
            {
                case "FSV3030":
                    break;
                case "N9020A":
                    n9020a.AutoScale();
                    break;
                case "N9020B":
                    n9020b.AutoScale();
                    break;
                case "N9030B":
                    n9020b.AutoScale();
                    break;
                case "FSW26":
                    fsw26.AutoScale();
                    break;
                case "M9410A":
                    m9410a.AutoScale();
                    break;
            }
        }
        /*.............................................Load Setup tham số các bai do...............................................*/
        public void LoadMeasurementSetup(string selectmeasurement, string Freq)
        {
            if (instrumentVSA == "FSV3030")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    fsv3030.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    fsv3030.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    fsv3030.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    fsv3030.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    fsv3030.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    fsv3030.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    fsv3030.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100Mhz_TDD")
                {
                    fsv3030.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    fsv3030.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    fsv3030.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "TSE_100MHz_TDD")
                {
                    fsv3030.LoadTSESetup("100", "TDD");
                }
                else if (selectmeasurement == "TAE_100MHz_TDD")
                {
                    fsv3030.LoadTAESetup("100");
                }
                else if (selectmeasurement == "OutputPower_100MHz")
                {
                    fsv3030.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    fsv3030.LoadTransONOFFSetup("100", "3D");
                }
            }
            else if (instrumentVSA == "FSW26")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    fsw26.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    fsw26.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    fsw26.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    fsw26.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    fsw26.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    fsw26.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    fsw26.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100Mhz_TDD")
                {
                    fsw26.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    fsw26.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    fsw26.LoadSEMSetup("100", "TDD", Freq);
                }
                else if (selectmeasurement == "TAE_100MHz_TDD")
                {
                    fsw26.LoadTAESetup("100");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    fsw26.LoadTransONOFFSetup("100", "3D");
                }
            }
            else if (instrumentVSA == "N9020A")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    n9020a.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    n9020a.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    n9020a.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    n9020a.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    n9020a.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    n9020a.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    //n9020a.LoadTXPower();
                    n9020a.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100Mhz_TDD")
                {
                    n9020a.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    n9020a.LoadOBWSetup("100", "TDD");
                    n9020a.SetFreqCent(Freq);
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    n9020a.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    n9020a.LoadTransONOFFSetup("100", "3D");
                }
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    n9020b.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    n9020b.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    n9020b.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    n9020b.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    n9020b.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    n9020b.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    //n9020a.LoadTXPower();
                    n9020b.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100Mhz_TDD")
                {
                    n9020b.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    n9020b.LoadOBWSetup("100", "TDD");
                    n9020b.SetFreqCent(Freq);
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    n9020b.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    n9020b.LoadTransONOFFSetup("100", "3D");
                }
                else if (instrumentVSA == "M9410A")
                {
                    if (selectmeasurement == "NRTM_33")
                    {
                        m9410a.LoadNRTMSetup("NRTM_33");
                    }
                    else if (selectmeasurement == "NRTM_32")
                    {
                        m9410a.LoadNRTMSetup("NRTM_32");
                    }
                    else if (selectmeasurement == "NRTM_20")
                    {
                        m9410a.LoadNRTMSetup("NRTM_20");
                    }
                    else if (selectmeasurement == "NRTM_20a")
                    {
                        m9410a.LoadNRTMSetup("NRTM_20a");
                    }
                    else if (selectmeasurement == "NRTM_31")
                    {
                        m9410a.LoadNRTMSetup("NRTM_31");
                    }
                    else if (selectmeasurement == "NRTM_31a")
                    {
                        m9410a.LoadNRTMSetup("NRTM_31a");
                    }
                    else if (selectmeasurement == "NRTM_11")
                    {
                        m9410a.LoadNRTMSetup("NRTM_11");
                    }
                    else if (selectmeasurement == "ACLR")
                    {
                        m9410a.LoadACLRSetup("100", "TDD");
                    }
                    else if (selectmeasurement == "OBW_100MHz_TDD")
                    {
                        m9410a.LoadOBWSetup("100", "TDD");
                    }
                    else if (selectmeasurement == "SEM_100MHz_TDD")
                    {
                        m9410a.LoadSEMSetup("100", "TDD");
                    }
                    else if (selectmeasurement == "TAE_100MHz_TDD")
                    {
                    }
                    else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                    {
                    }
                }
            }
            else if (instrumentVSA == "M9410A")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    m9410a.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    m9410a.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    m9410a.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    m9410a.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    m9410a.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    m9410a.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    //n9020a.LoadTXPower();
                    m9410a.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100Mhz_TDD")
                {
                    m9410a.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    m9410a.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    m9410a.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                }
            }
            else if (instrumentVSA == "IQFR1-RU")
            {
                iqfr1.LoadMeasurementSetup(selectmeasurement,Freq);
            }
            else
            {
                Log("VSA select measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadMeasurementSetup(string selectmeasurement, string Freq, string mode, string RBW, string coupling)
        {
            if (instrumentVSA == "FSV3030")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    fsv3030.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    fsv3030.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    fsv3030.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    fsv3030.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    fsv3030.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    fsv3030.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    fsv3030.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100MHz_TDD")
                {
                    fsv3030.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    fsv3030.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    fsv3030.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "TSE_100MHz_TDD")
                {
                    fsv3030.LoadTSESetup("100", "TDD");
                }
                else if (selectmeasurement == "TAE_100MHz_TDD")
                {
                    fsv3030.LoadTAESetup("100");
                }
                else if (selectmeasurement == "OutputPower_100MHz")
                {
                    fsv3030.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    fsv3030.LoadTransONOFFSetup("100", "3D");
                }
            }
            else if (instrumentVSA == "FSW26")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    fsw26.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    fsw26.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    fsw26.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    fsw26.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    fsw26.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    fsw26.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    fsw26.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR")
                {
                    fsw26.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    fsw26.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    fsw26.LoadSEMSetup("100", "TDD", Freq);
                }
                else if (selectmeasurement == "Spur_100MHz_TDD")
                {
                    fsw26.LoadSpurSetup(mode, Freq, RBW);
                }
                else if (selectmeasurement == "TAE_100MHz_TDD")
                {
                    fsw26.LoadTAESetup("100");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    fsw26.LoadTransONOFFSetup("100", "3D");
                }
            }
            else if (instrumentVSA == "N9020A")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    n9020a.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    n9020a.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    n9020a.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    n9020a.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    n9020a.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    n9020a.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    //n9020a.LoadTXPower();
                    n9020a.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100MHz_TDD")
                {
                    n9020a.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    n9020a.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    n9020a.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    n9020a.LoadTransONOFFSetup("100", "3D");
                }
                else if (selectmeasurement == "Spur_100MHz_TDD")
                {
                    n9020a.LoadSpurSetup(coupling, mode, Freq, RBW);
                }
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    n9020b.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    n9020b.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    n9020b.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    n9020b.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    n9020b.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    n9020b.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    //n9020a.LoadTXPower();
                    n9020b.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100MHz_TDD")
                {
                    n9020b.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    n9020b.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    n9020a.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                    n9020b.LoadTransONOFFSetup("100", "3D");
                }
                else if (selectmeasurement == "Spur_100MHz_TDD")
                {
                    n9020b.LoadSpurSetup(coupling, mode, Freq, RBW);
                }
            }
            else if (instrumentVSA == "M9410A")
            {
                if (selectmeasurement == "NRTM_33")
                {
                    m9410a.LoadNRTMSetup("NRTM_33");
                }
                else if (selectmeasurement == "NRTM_32")
                {
                    m9410a.LoadNRTMSetup("NRTM_32");
                }
                else if (selectmeasurement == "NRTM_20")
                {
                    m9410a.LoadNRTMSetup("NRTM_20");
                }
                else if (selectmeasurement == "NRTM_20a")
                {
                    m9410a.LoadNRTMSetup("NRTM_20a");
                }
                else if (selectmeasurement == "NRTM_31")
                {
                    m9410a.LoadNRTMSetup("NRTM_31");
                }
                else if (selectmeasurement == "NRTM_31a")
                {
                    m9410a.LoadNRTMSetup("NRTM_31a");
                }
                else if (selectmeasurement == "NRTM_11")
                {
                    //n9020a.LoadTXPower();
                    m9410a.LoadNRTMSetup("NRTM_11");
                }
                else if (selectmeasurement == "ACLR_100MHz_TDD")
                {
                    m9410a.LoadACLRSetup("100", "TDD");
                }
                else if (selectmeasurement == "OBW_100MHz_TDD")
                {
                    m9410a.LoadOBWSetup("100", "TDD");
                }
                else if (selectmeasurement == "SEM_100MHz_TDD")
                {
                    m9410a.LoadSEMSetup("100", "TDD");
                }
                else if (selectmeasurement == "ONOFFPower_100Mhz_3D")
                {
                }
            }
            else if (instrumentVSA == "IQ-FR1")
            {
                iqfr1.LoadMeasurementSetup(selectmeasurement, Freq);
            }
            else
            {
                Log("VSA select measurement: FAIL! ", LogLevel.ERROR);
            }
        }
        public void LoadIMDSetup(string offSet, string Mode, string freq)
        {
            if (instrumentVSA == "N9020A")
            {
                if (Mode == "ACLR")
                {
                    n9020a.LoadIMDACLR(offSet);
                }
                else if (Mode == "SEM")
                {

                    n9020a.LoadIMDSEM(offSet);
                }
                else if (Mode == "TSE")
                {
                    //   n9020a.LoadIMDTSE(offSet);
                }
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                if (Mode == "ACLR")
                {
                    n9020b.LoadIMDACLR(offSet);
                }
                else if (Mode == "SEM")
                {

                    n9020b.LoadIMDSEM(offSet);
                }
                else if (Mode == "TSE")
                {
                    //   n9020a.LoadIMDTSE(offSet);
                }
            }
            else if (instrumentVSA == "FSW26")
            {
                if (Mode == "ACLR")
                {
                    fsw26.LoadIMDACLRSetup(offSet, freq);
                }
                else if (Mode == "SEM")
                {
                    fsw26.LoadIMDSEMSetup(offSet, freq);
                }
                else if (Mode == "TSE")
                {
                    fsw26.LoadIMDTSESetup(freq);
                }
            }
            else if (instrumentVSA == "M9410A")
            {
                if (Mode == "ACLR")
                {
                    m9410a.LoadIMDACLR(offSet);
                }
                else if (Mode == "SEM")
                {
                    m9410a.LoadIMDSEM(offSet);
                }
                else if (Mode == "TSE")
                {
                    m9410a.LoadIMDTSE(Mode, freq);
                }
            }
        }
        public void setPort(string Port)
        {
            if (instrumentVSA == "N9020A")
            {
                n9020a.Port(Port);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.Port(Port);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.Port(Port);
            }
            else if (instrumentVSA == "M9410A")
            {
            }
        }
        public void TXPowerMeasurement(string NRTM, string setAtt, int port, BBU bbu, string setFreq, out string power)
        {
            power = "ERROR";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.OFDMPower(out power);

            }
            if (instrumentVSA == "FSW26")
            {
                fsw26.OFDMPower(out power);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.TXPower(port, bbu, out power);
            }
        }

        string[] EVM = new string[13];
        public void EVMFreqErrMeasurement(int[] port, string NRTM, out string[] evm, out string[] freqErr)
        {
            evm = null; freqErr = null;
            if (instrumentVSA == "IQFR1-RU")
            {
                iqfr1.EVMFreqErr(port, out evm, out freqErr);
            }
        }
        public void EVMFreqErrMeasurement(int port, string NRTM, out string[] evm, out string freqErr)
        {
            evm = EVM;
            freqErr = "";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.EVMFreqErr(out evm, out freqErr);
            }
            if (instrumentVSA == "FSW26")
            {
                fsw26.EVMFreqErr(out evm, out freqErr);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.EVMFreqErr(port, NRTM, out evm, out freqErr);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.EVMFreqErr(port, NRTM, out evm, out freqErr);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.EVMFreqErr(port, NRTM, out evm, out freqErr);
            }
            else if (instrumentVSA == "IQ-FR1")
            {
                //iqfr1.EVMFreqErr(port, out freqErr);
            }
            else
            {
                Log("VSA select measurement EVM/FrequencyErr/power: FAIL! ", LogLevel.ERROR);
            }
        }
        public void OutputPowerMeasurement(int port, BBU bbu, out string power)
        {
            power = "ERROR";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.TXPower(port, bbu, out power);
            }
            else if (instrumentVSA == "IQFR1-RU")
            {
                //iqfr1.TXPower(port, out power);
            }
            else
            {
                Log("VSA OutputPowerMeasurement: FAIL", LogLevel.ERROR);
            }
        }
        public void OutputPowerMeasurement(int[] port, BBU bbu, out string[] power)
        {
            power = null;

           if (instrumentVSA == "IQFR1-RU")
            {
                iqfr1.TXPower(port, out power);
            }
            else
            {
                Log("VSA OutputPowerMeasurement: FAIL", LogLevel.ERROR);
            }
        }
        public void TotalPowerDR(string setAtt, string setFreq, out string power)
        {
            power = "ERROR";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.OFDMPower(out power);
            }
            if (instrumentVSA == "FSW26")
            {
                fsw26.OFDMPower(out power);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.OFDMPower(out power);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B" )
            {
                n9020b.OFDMPower(out power);
            }
            else
            {
                Log("VSA select measurement Total Power Dynamic Range: FAIL! ", LogLevel.ERROR);
            }
        }
        public void ACLRPowerMeasurement(int port, string TM, out string powerMeas, out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            powerMeas = "ERROR";
            aclrAdjLower = "ERROR";
            aclrAdjUpper = "ERROR";
            aclrAlt1Lower = "ERROR";
            aclrAlt1Upper = "ERROR";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.RLEV("20");
                fsv3030.ACLRPower(port, TM, out powerMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.RLEV("20");
                fsw26.ACLRPower(port, TM, out powerMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.ACLRPower(port, TM, out powerMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.ACLRPower(port, TM, out powerMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.ACLRPower(port, TM, out powerMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            else if (instrumentVSA == "IQ-FR1")
            {
                //iqfr1.ACLRPower(port, TM, out powerMeas);
            }
            else
            {
                Log("VSA select measurement for ACRL: FAIL!", LogLevel.ERROR);
            }
        }
        public void ACLRPowerMeasurement(int []port, string TM, out string[] ACLRMeas, out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            ACLRMeas = null;
            aclrAdjLower = "ERROR";
            aclrAdjUpper = "ERROR";
            aclrAlt1Lower = "ERROR";
            aclrAlt1Upper = "ERROR";
            if (instrumentVSA == "IQFR1-RU")
            {
                iqfr1.ACLRPower(port, TM ,out ACLRMeas);
            }
        }
        public void OBWMeasurement(int[] port, string TM, out string[] obwMeas)
        {
            obwMeas = null;
            if(instrumentVSA =="IQFR1-RU")
            {
                iqfr1.OBWMeas(port, TM, out obwMeas);
            }
        }
        public void OBWMeasurement(int port, out string obwMeas)
        {

            obwMeas = "ERROR";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.RLEV("35");
                fsv3030.OBWMeas(port, out obwMeas);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.RLEV("35");
                fsw26.OBWMeas(port, out obwMeas);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.OBWMeas(port, out obwMeas);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.OBWMeas(port, out obwMeas);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.OBWMeas(port, out obwMeas);
            }
            else
            {
                Log("VSA select measurement for OBW: FAIL! ", LogLevel.ERROR);
            }
        }
        public void SEMMeasurement(int []port, string TM, out double absP1, out double absP2, out double absP3, out string[] semLimit)
        {
            semLimit = null;
            absP1 = 0;
            absP2 = 0;
            absP3 = 0;
            iqfr1.SEMMeas(port, TM,out semLimit);
        }
        public void SEMMeasurement(int port, string TM, out double absP1, out double absP2, out double absP3, out double semLimit)
        {
            semLimit = 1;
            absP1 = 0;
            absP2 = 0;
            absP3 = 0;
            try
            {
                if (instrumentVSA == "FSV3030")
                {
                    fsv3030.SEMMeas(port, TM, out absP1, out absP2, out absP3, out semLimit);
                }
                if (instrumentVSA == "FSW26")
                {
                    fsw26.SEMMeas(port, TM, out absP1, out absP2, out absP3, out semLimit);
                }
                else if (instrumentVSA == "N9020A")
                {
                    n9020a.SEMMeas(port, TM, out absP1, out absP2, out absP3, out semLimit);
                }
                else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
                {
                    n9020b.SEMMeas(port, TM, out absP1, out absP2, out absP3, out semLimit);
                }
                else if (instrumentVSA == "M9410A")
                {
                    m9410a.SEMMeas(port, TM, out absP1, out absP2, out absP3, out semLimit);
                }
                else if (instrumentVSA == "IQ-FR1")
                {
                    //iqfr1.SEMMeas(port, TM, out semLimit);
                }
                else
                {
                    Log("VSA select measurement for SEM: FAIL! ", LogLevel.ERROR);
                }
            }
            catch
            {
                Log("VSA select measurement for SEM: FAIL!", LogLevel.ERROR);
            }
        }
        public void SEMMeasurement(int[] port, string TM, out string[] semLimit)
        {
            semLimit = null;
            if(instrumentVSA == "IQFR1-RU")
            {
                iqfr1.SEMMeas(port, TM, out semLimit);
            }
        }
        double?[] tmp = new double?[20];
        public void SpurMeasurement(int port, string setAtt, string setFreq, out double?[] spur)
        {

            spur = tmp;
            double tmp1 = 0;
            double tmp2 = 0;
            double tmp3 = 0;
            double tmp4 = 0;
            try
            {
                if (instrumentVSA == "FSW26")
                {
                    //fsw26.RLEVOFFS("0");
                    //AutoScale();
                    fsw26.SpurMeas(port, out spur);

                }
                else if (instrumentVSA == "N9020A")
                {
                    n9020a.LoadIMDTSE("DC", setFreq);
                    n9020a.SetAtt(setAtt);
                    AutoScale();
                    n9020a.IMDTSEMMeas("DC", out tmp1, out tmp2, out tmp3);
                    spur[0] = tmp1;
                    tmp[1] = tmp2;
                    n9020a.LoadIMDTSE("AC", setFreq);
                    n9020a.SetAtt(setAtt);
                    AutoScale();
                    n9020a.IMDTSEMMeas("AC", out tmp1, out tmp2, out tmp3);
                    tmp[2] = tmp1;
                    spur[1] = Math.Max((sbyte)tmp[1], (sbyte)tmp[2]);
                    spur[2] = tmp2;
                    spur[3] = tmp3;
                    //spur[4] = tmp4;
                }
                else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
                {
                    n9020b.LoadIMDTSE("DC", setFreq);
                    n9020b.SetAtt(setAtt);
                    AutoScale();
                    n9020b.IMDTSEMMeas("DC", out tmp1, out tmp2, out tmp3);
                    spur[0] = tmp1;
                    tmp[1] = tmp2;
                    n9020b.LoadIMDTSE("AC", setFreq);
                    n9020b.SetAtt(setAtt);
                    AutoScale();
                    n9020b.IMDTSEMMeas("AC", out tmp1, out tmp2, out tmp3);
                    tmp[2] = tmp1;
                    spur[1] = Math.Max((sbyte)tmp[1], (sbyte)tmp[2]);
                    spur[2] = tmp2;
                    spur[3] = tmp3;
                    //spur[4] = tmp4;
                }
            }
            catch
            {
                Log("VSA select measurement for SEM: FAIL! ", LogLevel.ERROR);
            }
        }
        string[] tae = new string[8];
        public void TAEMeasurement(string setFreq, out string taeMeas)
        {
            taeMeas = "ERROR";
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.TAEMeas(out taeMeas);
            }
            if (instrumentVSA == "FSW26")
            {
                fsw26.TAEMeas(out taeMeas);
            }
            if (instrumentVSA == "N9020A")
            {
            }
            else
            {
                Log("VSA select measurement Time Alignment Error: FAIL! ", LogLevel.ERROR);
            }
        }
        public void NewTAEMeasurement(string mode, string TM, int port, ref string taeref, ref string[] tae)
        {
            //taeref = "ERROR";
            tae = new string[] { };
            if (instrumentVSA == "FSV3030")
            {
                fsv3030.newTAEMeas(mode, TM, port, ref taeref, ref tae);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.newTAEMeas(mode, TM, port, ref taeref, ref tae);
            }
            else if (instrumentVSA == "N9020A")
            {
            }
            else
            {
                Log("VSA select measurement Time Alignment Error: FAIL! ", LogLevel.ERROR);
            }
        }
        public void OnOffMeasurement(string mode, string TM, out string OffPower, out string TransPeriod, int port)
        {
            OffPower = "";
            TransPeriod = "";
            if (instrumentVSA == "FSW26")
            {
                fsw26.AutoScale();
                fsw26.TransONOFFMeas(mode, TM, out OffPower, out TransPeriod, port);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.TransONOFFMeas(mode, TM, out OffPower, out TransPeriod, port);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.TransONOFFMeas(mode, TM, out OffPower, out TransPeriod, port);
            }
            else if (instrumentVSA == "FSV3030")
            {
                fsv3030.AutoScale();
                fsv3030.TransONOFFMeas(mode, TM, out OffPower, out TransPeriod, port);
            }
        }
        public void IMDACLRMeasurement(string offset, out double imdACLR, string TM, int port)
        {
            imdACLR = 0;
            if (instrumentVSA == "FSW26")
            {
                fsw26.RLEV("20");
                fsw26.Swetime("500");
                fsw26.IMDACLR(offset, out imdACLR, TM, port);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.IMDACLR(offset, out imdACLR);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.IMDACLR(offset, out imdACLR);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.IMDACLR(offset, out imdACLR);
            }
            else
            {
                Log("VSA select measurement for ACRL: FAIL! ", LogLevel.ERROR);
            }
        }
        public void IMDSEMMeasurement(string setAtt, string setFreq, string offSet, out string semLimit, string TM, int port)
        {
            semLimit = "ERROR";
            if (instrumentVSA == "FSW26")
            {
                //fsw26.RLEV("20");
                fsw26.Swetime("500");
                fsw26.IMDSEMMeas(offSet, out semLimit, TM, port);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.IMDSEM(offSet, out semLimit);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.IMDSEM(offSet, out semLimit);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.IMDSEM(offSet, out semLimit);
            }
            else
            {
                Log("VSA select measurement for SEM: FAIL! ", LogLevel.ERROR);
            }
        }
        public void IMDTSEMeasurement(int port, string setFreq, string setAtt, out double?[] imdTSE)
        {
            imdTSE = tmp;
            double tmp1 = 0;
            double tmp2 = 0;
            double tmp3 = 0;
            //double tmp4 = 0;
            try
            {
                if (instrumentVSA == "FSW26")
                {
                    //fsw26.RLEVOFFS(setAtt);
                    AutoScale();
                    fsw26.IMDTSEMeas(out imdTSE, port);

                }
                else if (instrumentVSA == "N9020A")
                {
                    n9020a.LoadIMDTSE("DC", setFreq);
                    n9020a.SetAtt(setAtt);
                    AutoScale();
                    n9020a.IMDTSEMMeas("DC", out tmp1, out tmp2, out tmp3);
                    imdTSE[0] = tmp1;
                    tmp[1] = tmp2;
                    n9020a.LoadIMDTSE("AC", setFreq);
                    n9020a.SetAtt(setAtt);
                    AutoScale();
                    n9020a.IMDTSEMMeas("AC", out tmp1, out tmp2, out tmp3);
                    tmp[2] = tmp1;
                    imdTSE[1] = Math.Max((sbyte)tmp[1], (sbyte)tmp[2]);
                    imdTSE[2] = tmp2;
                    imdTSE[3] = tmp3;
                    //imdTSE[4] = tmp4;
                }
            }
            catch
            {
                Log("VSA select measurement for SEM: FAIL! ", LogLevel.ERROR);
            }
        }

        public void ShowInfoCommand()
        {
            Log("VSA information:");
            Log($"Model:{Model}\tS/N:{Serial}\tManufacturer:{Manufacturer}");
            Log($"IP:{IpAddress}\tStatus:{Status}");
            Log($"Current Setting File: {CurrentRecallFile}");
        }

        //19.2.2024
        public void TakeScreenshot(string filePathVSA)
        {
            if (instrumentVSA == "FSW26")
            {
                fsw26.TakeScreenshot(filePathVSA);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.TakeScreenshot(filePathVSA);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.TakeScreenshot(filePathVSA);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.TakeScreenshot(filePathVSA);
            }
        }
        public void SingleSweepOnOff(int onOff)
        {
            if (instrumentVSA == "FSW26")
            {
                fsw26.SingleSweepOnOff(onOff);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.SingleSweepOnOff(onOff);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.SingleSweepOnOff(onOff);
            }
        }

        public void ACLRPowerMeasurement(int port, string TM, ref double intraACLR, string mode)
        {

            if (instrumentVSA == "FSV3030")
            {
                fsv3030.RLEV("20");
                //fsv3030.ACLRPower(port, TM, out intraACLR, mode);
            }
            else if (instrumentVSA == "FSW26")
            {
                fsw26.RLEV("20");
                fsw26.ACLRPower(port, TM, out intraACLR, mode);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.ACLRPower(port, TM, out intraACLR, mode);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.ACLRPower(port, TM, out intraACLR, mode);
            }
            else if (instrumentVSA == "M9410A")
            {
                //m9410a.ACLRPower(port, TM, out intraACLR, mode);
            }
            else
            {
                Log("VSA select measurement for ACRL: FAIL! ", LogLevel.ERROR);
            }
        }

        internal void SaveCSVFile(string filePath)
        {
            if (instrumentVSA == "FSW26")
            {
                fsw26.SaveCSVFile(filePath);
            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.SaveCSVFile(filePath);
            }
            else if (instrumentVSA == "N9020B" || instrumentVSA == "N9030B")
            {
                n9020b.SaveCSVFile(filePath);
            }
            else if (instrumentVSA == "M9410A")
            {
                m9410a.SaveCSVFile(filePath);
            }
        }
        public void MakeDir(string dir)
        {
            if (instrumentVSA == "FSW26")
            {
                fsw26.MakeDir(dir);
            }
        }

        //public void Trace(string mode)
        //{
        //    if (instrumentVSA == "FSW26")
        //    {
        //        fsw26.Trace(mode);
        //    }
        //}

        //14.6.2024 - setup bai do spurious theo mode Swept SA may do
        public void SetupIMDTSE(SpuriousRange range)
        {

            if (instrumentVSA == "FSV3030")
            {

            }
            else if (instrumentVSA == "FSW26")
            {

            }
            else if (instrumentVSA == "N9020A")
            {
                n9020a.SetupIMDTSE(range);
            }
            else
            {
                Log("VSA select measurement for ACRL: FAIL! ", LogLevel.ERROR);
            }
        }

        internal void Restart()
        {
            switch (instrumentVSA)
            {
                case "N9020A":
                    n9020a.Restart();
                    break;
                case "N9020B":
                    n9020b.Restart();
                    break;
                case "N9030B":
                    n9020b.Restart();
                    break;
            }
        }
    }
}
