using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Driver;

namespace _5GAutoTool
{
    public class FSV3030
    {
        public Form5GAT mainForm;
        public string Name= "FSV3030";
        public FSV3030(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 fsv = new Ivi.Visa.Interop.FormattedIO488();
        //Form5GAT MainForm = new Form5GAT();

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "FSV3030");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        /*...........................................Connection...................................................................................*/
        public void takephoto()
        {
            string name;
            name = "_ACLR_P" + "_" + DateTime.Now.ToString("MMdd_hhmm").ToString();
            fsv.WriteString("MMEM:NAME " + "\"" + "C:/R_S/Instr/user/NR5G/images/" + name + ".png" + "\"");
        }
        public void Connection(string IP, string Cmd, string Instrument, out string manufacturer, out string model, out string serial)
        {
            manufacturer = "ERROR";
            model = "ERROR";
            serial = "ERROR";
            try
            {
                //Disconnect();
                fsv.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                fsv.IO.Timeout = 3000;
                fsv.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                manufacturer = tmp[0].ToString();
                model = tmp[1].ToString();
                serial = tmp[2].ToString();
                if (tmp[1].ToString() == Instrument)
                {
                    Log("FSV3030: connected to " + IP, LogLevel.SUCCESS);
                }
                else
                {
                    Log("ERROR: FSV3030 connection fail", LogLevel.ERROR);
                }
            }
            catch
            {
                Log("ERROR: FSV3030 connection fail", LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            if (fsv != null)
            {
                fsv.IO.Close();
            }
        }
        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                fsv.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch
            {
                Log("fsv3030 connect: fail", LogLevel.ERROR);
            }
        }
        /*....................................................Config FSV3030................................................................*/

        public void FreqCent(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString("FREQ:CENT " + freq + "Hz", true);
                Log("FSV3030: select center frequency: " + freq);

            }
            catch
            {
                Log("ERROR: FSV3030 select center frequency fail", LogLevel.ERROR);
            }
        }

        public void RLEV(string reflever_dBm)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString("DISP:TRAC:Y:RLEV " + reflever_dBm + " dBm", true);
                Log("FSV3030: ref lever: " + reflever_dBm);
            }
            catch
            {
                Log("ERROR: FSV3030 ref lever fail", LogLevel.ERROR);
            }
        }
        public void AutoScale()
        {
            try
            {
                fsv.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(10000);
            }
            catch
            {

            }
        }

        public void RLEVOFFS(string refLeverOffSetDB)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString("DISP:TRAC:Y:RLEV:OFFS " + refLeverOffSetDB + " dB", true);
                Log("FSV3030: reference lever Offset: " + refLeverOffSetDB);
            }
            catch
            {
                Log("ERROR: FSV3030 ref lever Offset fail", LogLevel.ERROR);
            }
        }
        public void PHASECOMP(string ONorOFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("CONF:UL:CC1:RFUC:STAT " + ONorOFF, true);
            }
            catch
            {
                Log("FSV3030 Phase Compensation: FAIL", LogLevel.ERROR);
            }
        }

        public void Swetime(string time)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("SENS:SWE:TIME " + time + "ms", true);
                Log("FSV3030: sweep time " + time + "ms");
            }
            catch
            {
                Log("ERROR: FSV3030 ref lever Offset fail", LogLevel.ERROR);
            }
        }
        /*....................................................Hàm thực hiện các bài đo................................................................*/

        public void LoadNRTMSetup(string NRTMxx)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("INST:DEL '5G NR'");
                fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/NRTM/7D1S2U/" + NRTMxx + "_7D1S2U'", true);
                Log("FSV3030 load NRTM measurement: " + NRTMxx);
            }
            catch
            {
                Log("FSV3030 Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void OFDMPower(out string power)
        {
            power = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(10000);
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:OSTP:AVER?", true);
                object[] fsv3030_Power;
                fsv3030_Power = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = fsv3030_Power[0].ToString();
                Log("OFDM Power: " + power);
            }
            catch
            {
                Log("FSV3030 Total power dynamic range Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void TXPower(int port, BBU bbu, out string power)
        {
            power = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(10000);
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:POW:AVER?", true);
                object[] fsv3030_Power;
                fsv3030_Power = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = fsv3030_Power[0].ToString();
                Log("Output Power " + power);
            }
            catch
            {
                Log("FSV3030 Total power dynamic range Measurement: FAIL", LogLevel.ERROR);
            }
        }
        string[] EVM = new string[13];
        public void EVMFreqErr(out string[] evm, out string freqErr)
        {
            evm = EVM;
            freqErr = "";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(8000);
                object[] Tmp;
                //Đọc giá trị EVM QPSK
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSQP:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[0] = Tmp[0].ToString();
                //Đọc giá trị EVM 16QAM
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSST:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[1] = Tmp[0].ToString();
                //Đọc giá trị EVM 64QAM
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSSF:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[2] = Tmp[0].ToString();
                //Đọc giá trị EVM 256QAM
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSTS:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[3] = Tmp[0].ToString();
                //Đọc giá trị EVM All
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:ALL:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[4] = Tmp[0].ToString();
                //Đọc giá trị EVM of all Physical Channel
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:PCH:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[5] = Tmp[0].ToString();
                //Đọc giá trị EVM of all Physical Signal
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:PSIG:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[6] = Tmp[0].ToString();
                //Đọc giá trị I/Q Offset
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:IQOF:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[7] = Tmp[0].ToString();
                //Đọc giá trị I/Q Gain Imbalance
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:GIMB:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[8] = Tmp[0].ToString();
                //Đọc giá trị I/Q Quadrature
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:QUAD:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[9] = Tmp[0].ToString();
                //Đọc giá trị OFDM Symbol Tx Power 
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:OSTP:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[10] = Tmp[0].ToString();
                //Đọc giá trị Tx Power 
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:POW:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[11] = Tmp[0].ToString(); ;
                //Đọc giá trị Crest Factor 
                fsv.WriteString(":FETC:CC1:ISRC:SUMM:CRES:AVER?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[12] = Tmp[0].ToString();
                fsv.WriteString("FETC:CC1:FRAM3:SUMM:FERR?", true);
                Tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                freqErr = Tmp[0].ToString();
                Log("FSV3030: modulation quality measurement" + "\n"
                    + "EVM QPSK: " + EVM[0] + "\n"
                    + "EVM 16QAM: " + EVM[1] + "\n"
                    + "EVM 64QAM: " + EVM[2] + "\n"
                    + "EVM 256QAM: " + EVM[3] + "\n"
                    + "EVM All: " + EVM[4] + "\n"
                    + "EVM of all Physical Channel: " + EVM[5] + "\n"
                    + "EVM of all Physical Signal: " + EVM[6] + "\n"
                    + "I/Q Offset: " + EVM[7] + "\n"
                    + "I/Q Gain Imbalance: " + EVM[8] + "\n"
                    + "I/Q Quadrature: " + EVM[9] + "\n"
                    + "OFDM Symbol Tx Power: " + EVM[10] + "\n"
                    + "Tx Power: " + EVM[11] + "\n"
                    + "Crest Factor: " + EVM[12] + "\n");
            }
            catch
            {
                Log("ERROR: FSV3030 EVM Measurement fail", LogLevel.ERROR);
            }
        }
        public void LoadACLRSetup(string BW, string TDDFDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsv.WriteString("INST:DEL '5G NR'");
                Log("LOG: VSA xoa cua so hien tai!");
                System.Threading.Thread.Sleep(2000);
                //fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/ACLR/ACLR_" + BW + "MHz_" + TDDFDD + "'", true);
                fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/ACLR/ACLR_100MHz_TDD'", true);
                System.Threading.Thread.Sleep(5000);
                //Log("LOG: Tai cai dat do ACLR tren VSA!");
                Log("FSV3030: Recall state: ACLR_" + BW + "_" + TDDFDD);
            }
            catch
            {
                Log("ERROR: FSV3030 recall state ACLR fail", LogLevel.ERROR);
            }
        }
        public void ACLRPower(int port, string TM, out string power, out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            string[] data = new string[] { "TXPOWER", "LOWER ABS ADJ", "UPPER ABS ADJ", "LOWER ADJ", "UPPER ADJ", "LOWER ABS ALT", "UPPER ABS ALT", "LOWER ALT", "UPPER ALT", "ACLR", "Worst Abs Power" };
            power = "ERROR";
            aclrAdjLower = "ERROR";
            aclrAdjUpper = "ERROR";
            aclrAlt1Lower = "ERROR";
            aclrAlt1Upper = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //    TakePhoto(mode); // luu anh

                //   System.Threading.Thread.Sleep(500);
                fsv.WriteString("CALC:MARK:FUNC:POW:RES:DET? ACP", true);
                object[] fsv3030_aclr;
                fsv3030_aclr = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");

                power = fsv3030_aclr[0].ToString();
                string aclrAdjabsLower = fsv3030_aclr[1].ToString();
                string aclrAdjabsUpper = fsv3030_aclr[2].ToString();
                string aclrAdtabsLower = fsv3030_aclr[5].ToString();
                string aclrAdtabsUpper = fsv3030_aclr[6].ToString();
                aclrAdjLower = fsv3030_aclr[3].ToString();
                aclrAdjUpper = fsv3030_aclr[4].ToString();
                aclrAlt1Lower = fsv3030_aclr[7].ToString();
                aclrAlt1Upper = fsv3030_aclr[8].ToString();
                Log("FSV3030: ACLR measurement" + "\n"
                    + "ADJ lower: " + aclrAdjLower + "\n"
                    + "ADJ upper: " + aclrAdjUpper + "\n"
                    + "ALT lower: " + aclrAlt1Lower + "\n"
                    + "ALT upper: " + aclrAlt1Upper + "\n");
                double tmp1 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp2 = Math.Max(double.Parse(aclrAdtabsLower), double.Parse(aclrAdtabsUpper));
                double tmp3 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp4 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                string[] result = new string[] { Math.Max(tmp1, tmp2).ToString(), Math.Max(tmp3, tmp4).ToString() };
                mainForm.DLCsvRecord(data, fsv3030_aclr, result, "ACLR", TM, fsv3030_aclr.Length, result.Length, port);
            }
            catch
            {
                Log("FSV3030 ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }
        /*........................................................OBW mesurement.....................................................................*/
        public void LoadOBWSetup(string BW, string TDD_or_FDD)
        {
            fsv.WriteString("INST:DEL '5G NR'");
            fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/OBW/OBW_" + BW + "MHz_" + TDD_or_FDD + "'", true);
            Log("FSV3030 load OBW measurement: OBW_" + BW + "_" + TDD_or_FDD);
        }
        public void OBWMeas(int port, out string OBWMeas)
        {
            OBWMeas = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(4000);
                fsv.WriteString("CALC:MARK:FUNC:POW:RES? OBW", true);
                object[] fsv3030_obw;
                fsv3030_obw = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                OBWMeas = fsv3030_obw[0].ToString();
                Log(OBWMeas);
            }
            catch
            {
                Log("FSV3030 load OBW measurement: FAIL", LogLevel.ERROR);
            }
        }
        /*..................................................................Transmitter ON/OFF Power.............................................................................*/
        public void LoadTransONOFFSetup(string BW, string SB)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsv.WriteString("INST:DEL '5G NR'");
                fsv.WriteString("MMEM:LOAD:STAT 1, 'C:/R_S/Instr/user/NR5G/ONOFF POWER/TransONOFF_" + BW + "MHz_" + SB + "'", true);
                Log("FSV3030 load Transmitter ON/OFF measurement: TransONOFF_" + BW + "_" + SB);
            }
            catch
            {
                Log("FSV3030 Load ON/OFF Power setup: FAIL", LogLevel.ERROR);
            }
        }

        public void TransONOFFMeas(string mode, string TM, out string offPower, out string transPeriod, int port)
        {
            offPower = "";
            transPeriod = "";
            string[] data = new string[] {"Start OFF", "Stop OFF", "Time at delta", "OFF Power Abs","OFF Power delta", "Falling Trans Period", "Rising Trans Period",
                                          "Start OFF", "Stop OFF", "Time at delta", "OFF Power Abs","OFF Power delta", "Falling Trans Period", "Rising Trans Period",
                                          "Worst Trans Period", "Worst OFF Power"};
            try
            {
                string OFF_Power1 = "";
                string OFF_Power2 = "";
                string Falling_Period1 = "";
                string Rising_Period1 = "";
                string Falling_Period2 = "";
                string Rising_Period2 = "";
                System.Threading.Thread.Sleep(10000);
                fsv.WriteString("TRAC6:DATA? LIST", true);
                object[] fsv3030_TransONOFF;
                fsv3030_TransONOFF = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                OFF_Power1 = fsv3030_TransONOFF[3].ToString();
                OFF_Power2 = fsv3030_TransONOFF[10].ToString();
                Falling_Period1 = fsv3030_TransONOFF[5].ToString();
                Rising_Period1 = fsv3030_TransONOFF[6].ToString();
                Falling_Period2 = fsv3030_TransONOFF[12].ToString();
                Rising_Period2 = fsv3030_TransONOFF[13].ToString();
                offPower = (Math.Max(double.Parse(OFF_Power1), double.Parse(OFF_Power2))).ToString();
                string tmp = (Math.Max(double.Parse(Falling_Period1), double.Parse(Rising_Period1))).ToString();
                string tmp1 = (Math.Max(double.Parse(Falling_Period2), double.Parse(Rising_Period2))).ToString();
                transPeriod = (Math.Max(double.Parse(tmp), double.Parse(tmp1)) * 1000000).ToString();
                string[] result = new string[] { transPeriod, (Math.Max(double.Parse(OFF_Power1), double.Parse(OFF_Power2))).ToString() };
                mainForm.DLCsvRecord(data, fsv3030_TransONOFF, result, mode, TM, fsv3030_TransONOFF.Length, result.Length, port);
            }
            catch
            {
                Log("FSV3030 load TransON/OFF measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void newTAEMeas(string mode, string testmodel, int port, ref string taeref, ref string[] tae)
        {
            // Array rfoffsettmp: mang ket qua tu VSA tra ve
            // Array tae: Do lech tae giua cac port
            object[] froffsettmp = new object[] { 0 };
            string taeport;
            try
            {
                string[] data = new string[] { "Frame start offset (ms)", "TAE (us)" };
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString(":FETC:CC1:ISRC:SUMM:TFR?", true);
                if (port == 1)
                {
                    froffsettmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any);
                    taeref = (froffsettmp[0]).ToString();
                    tae = new string[] { taeref };
                }
                else
                {
                    froffsettmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any);
                    taeport = froffsettmp[0].ToString();
                    //tae[port] = (double.Parse(taeport) - double.Parse(taeref)).ToString();
                    tae = new string[] { ((double.Parse(taeport) - double.Parse(taeref)) * 1000).ToString() };
                }
                string froffstart = (froffsettmp[0]).ToString();
                Log("Frame start offset: " + froffstart + "ms");
                mainForm.DLCsvRecord(data, froffsettmp, tae, mode, testmodel, data.Length - 1, 1, port);
            }
            catch
            {
                Log("FSV3030 TAE measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadTAESetup(string BW)
        {
            try
            {
                fsv.WriteString("INST:DEL '5G NR'");
                fsv.WriteString("MMEM:LOAD:STAT 1, 'C:/R_S/Instr/user/NR5G/TAE/tae_" + BW + "MHz_32t32r" + "'", true);
                Log("FSV3030 load TAE measurement: TAE_" + BW + "MHz_32t32r");
                bool Flag = true;
                System.Threading.Thread.Sleep(1);
                while (Flag)
                {
                    try
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        System.Threading.Thread.Sleep(1);
                        fsv.WriteString("FETC:TAER:CC:AP1001:MAX?", true);
                        object[] fsw26_TAE;
                        fsw26_TAE = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                        Flag = false;
                    }
                    catch
                    {
                        Flag = true;
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            catch
            {
                Log("FSV3030: Load TAE Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void TAEMeas(out string taeMeas)
        {
            taeMeas = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("FETC:TAER:CC:AP1001:MAX?", true);
                object[] tmp;
                tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                taeMeas = (double.Parse(tmp[0].ToString()) * 1000000000).ToString();
                Log("Time aligement error: " + taeMeas);
            }
            catch
            {
                Log("FSV3030 TAE measurement: FAIL", LogLevel.ERROR);
            }
        }

        /*.................................................................Operating band Unwanted emissions (SEM) ...........................................................................*/
        public void LoadSEMSetup(string BW, string TDDFDD)
        {
            fsv.WriteString("INST:DEL '5G NR'");
            fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/SEM/SEM_" + BW + "MHz_" + TDDFDD + "'", true);
            Log("FSV3030 load SEM measurement: SEM_" + BW + "_" + TDDFDD);
        }
        public void SEMMeas(int port, string TM, out double absPwr1, out double absPwr2, out double absPwr3, out double semLimit)
        {
            //semLimit = "ERROR";
            //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            //System.Threading.Thread.Sleep(1);
            //try
            //{
            //    string limit1;
            //    string limit2;
            //    string limit3;
            //    string limit4;
            //    string limit5;
            //    string limit6;
            //    System.Threading.Thread.Sleep(2000);
            //    fsv.WriteString("TRAC:DATA? LIST", true);
            //    object[] fsv3030_SEM;
            //    fsv3030_SEM = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
            //    limit1 = fsv3030_SEM[7].ToString();
            //    limit2 = fsv3030_SEM[18].ToString();
            //    limit3 = fsv3030_SEM[29].ToString();
            //    limit4 = fsv3030_SEM[40].ToString();
            //    limit5 = fsv3030_SEM[51].ToString();
            //    limit6 = fsv3030_SEM[62].ToString();
            //    string tmp1 = (Math.Max(double.Parse(limit1), double.Parse(limit2))).ToString();
            //    string tmp2 = (Math.Max(double.Parse(limit3), double.Parse(limit4))).ToString();
            //    string tmp3 = (Math.Max(double.Parse(limit5), double.Parse(limit6))).ToString();
            //    string tmp4 = (Math.Max(double.Parse(tmp1), double.Parse(tmp2))).ToString();
            //    semLimit = (Math.Max(double.Parse(tmp4), double.Parse(tmp3))).ToString();
            //    Log("FSV3030 measement: " + "\n"
            //        + "limit1: " + limit1 + "\n"
            //        + "limit2: " + limit2 + "\n"
            //        + "limit3: " + limit2 + "\n"
            //        + "limit4: " + limit2 + "\n"
            //        + "limit5: " + limit2 + "\n"
            //        + "limit6: " + limit2 + "\n");
            absPwr1 = 0;
            absPwr2 = 0;
            absPwr3 = 0;
            semLimit = 1;
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            try
            {
                System.Threading.Thread.Sleep(4000);
                fsv.WriteString("TRAC:DATA? LIST", true);
                object[] fsw26_SEM;
                fsw26_SEM = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                string limit1 = fsw26_SEM[7].ToString();
                string limit2 = fsw26_SEM[18].ToString();
                string limit3 = fsw26_SEM[29].ToString();
                string limit4 = fsw26_SEM[40].ToString();
                string limit5 = fsw26_SEM[51].ToString();
                string limit6 = fsw26_SEM[62].ToString();

                string absPower1 = fsw26_SEM[5].ToString();
                string absPower2 = fsw26_SEM[16].ToString();
                string absPower3 = fsw26_SEM[27].ToString();
                string absPower4 = fsw26_SEM[38].ToString();
                string absPower5 = fsw26_SEM[49].ToString();
                string absPower6 = fsw26_SEM[60].ToString();

                double tmp1 = Math.Max(double.Parse(limit1), double.Parse(limit2));
                double tmp2 = Math.Max(double.Parse(limit3), double.Parse(limit4));
                double tmp3 = Math.Max(double.Parse(limit5), double.Parse(limit6));
                double tmp4 = Math.Max(tmp1, tmp2);
                semLimit = Math.Max(tmp4, tmp3);
                Log("FSV3030 measement: " + "\n" + "absPower1: " + absPower1 + "\n" + "absPower2: " + absPower2 + "\n" + "absPower3: " + absPower3 + "\n" + "absPower4: " + absPower4 + "\n" + "absPower5: " + absPower5 + "\n" + "absPower6: " + absPower6);

                absPwr1 = Math.Round(Math.Max(double.Parse(absPower3), double.Parse(absPower4)), 1);
                absPwr2 = Math.Round(Math.Max(double.Parse(absPower2), double.Parse(absPower5)), 1);
                absPwr3 = Math.Round(Math.Max(double.Parse(absPower1), double.Parse(absPower6)), 1);           
            }
            catch
            {
                Log("FSV3030: SEM measurement: FAIL", LogLevel.ERROR);
            }
        }
        /*...............................................................................................................................................*/
        public void LoadTSESetup(string BW, string TDDFDD)
        {
            fsv.WriteString("INST:DEL '5G NR'");
            fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/SpuriousEmission/TSE_" + BW + "MHz_" + TDDFDD + "'", true);
            System.Threading.Thread.Sleep(2000);

        }
        string[] tmp = new string[5];
        public void TSEMeas(out string[] tse)
        {
            tse = tmp;
            //   string mode = "TSE";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(4000);
                fsv.WriteString("CALC:PSE:SUBR 1", true); //Sets 1 peaks per range to be stored in the list.
                System.Threading.Thread.Sleep(1000);
                fsv.WriteString("INIT:SPUR; *WAI", true); //Performs a spurious emission measurement and waits until the sweep has finished.
                System.Threading.Thread.Sleep(500);
                System.Threading.Thread.Sleep(500);
                fsv.WriteString("TRAC? SPUR", true);

                // fsv.WriteString("TRAC:DATA? SPUR", true);
                // doc mang du lieu 3x4 tra ve theo thu tu Freq , Power abs , Limit
                object[] transmitterSpuriousEmission;
                transmitterSpuriousEmission = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                tse[0] = transmitterSpuriousEmission[3].ToString();
                tse[1] = transmitterSpuriousEmission[6].ToString();
                tse[2] = transmitterSpuriousEmission[9].ToString();
                tse[3] = transmitterSpuriousEmission[12].ToString();
                tse[4] = transmitterSpuriousEmission[15].ToString();
                Log(tse[0] + "\n" + tse[1] + "\n" + tse[2] + "\n" + tse[3] + "\n" + tse[4]);
            }
            catch
            {
                Log("FSV3030 Spurious emission measurement: FAIL", LogLevel.ERROR);
            }
        }
    }
}

