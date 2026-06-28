using Agilent.SA.Vsa;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    public class FSW26
    {

        public Form5GAT mainForm;
        public string Name = "FSW26";
        public FSW26(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 fsw = new Ivi.Visa.Interop.FormattedIO488();
        //Form5GAT MainForm = new Form5GAT();
        /*...........................................Connection...................................................................................*/
        string rruinfo = "";
        public void RRUinfo(string rruSerial)
        {
            rruinfo = rruSerial;
        }

        string port = "";
        public void Port(string Port)
        {
            port = Port;
        }
        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "FSW26");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        public void Connection(string IP, string Cmd, string Instrument, out string manufacturer, out string model, out string serial)
        {
            manufacturer = "ERROR";
            model = "ERROR";
            serial = "ERROR";
            try
            {
                fsw.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                fsw.IO.Timeout = 2000;
                fsw.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                manufacturer = tmp[0].ToString();
                //model = tmp[1].ToString();  //tam thoi ngat bo
                model = "FSW"; //tam thoi su dung
                serial = tmp[2].ToString();
                if (tmp[1].ToString() == Instrument)
                {
                    Log("FSW26: connected to " + IP, LogLevel.SUCCESS);
                    //Preset configuration
                    fsw.WriteString("HCOP:DEV:COL ON"); //turn color printing on_use to save screenshot
                    fsw.WriteString("HCOP:DEV:LANG PNG");//file format for  a print job_use to save screenshot
                }

            }
            catch
            {
                Log("ERROR: FSW26 connection fail", LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            fsw.IO.Close();
        }
        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                fsw.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch
            {

            }
        }
        /*....................................................Config fsw26................................................................*/

        public void FreqCent(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsw.WriteString("FREQ:CENT " + freq + "Hz", true);
                Log("FSW26: select center frequency: " + freq);

            }
            catch
            {
                Log("ERROR: FSW26 select center frequency fail", LogLevel.ERROR);
            }
        }

        public void RLEV(string reflever_dBm)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsw.WriteString("DISP:TRAC:Y:RLEV " + reflever_dBm + " dBm", true);
                Log("FSW26: ref lever: " + reflever_dBm);
            }
            catch
            {
                Log("ERROR: FSW26 ref lever fail", LogLevel.ERROR);
            }
        }

        public void RLEVOFFS(string refLeverOffSetDB)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("DISP:WIND:SUBW:TRAC:Y:SCAL:RLEV:OFFS " + refLeverOffSetDB + " dB", true);
                Log("FSW26: reference lever Offset: " + refLeverOffSetDB);
            }
            catch
            {
                Log("ERROR: FSW26 ref lever Offset fail", LogLevel.ERROR);
            }
        }
        public void Swetime(string time)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("SENS:SWE:TIME 500ms", true);
                Log("FSW26: sweep time " + time + "ms");
            }
            catch
            {
                Log("ERROR: FSW26 ref lever Offset fail", LogLevel.ERROR);
            }
        }
        public void PHASECOMP(string ONorOFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("CONF:UL:CC1:RFUC:STAT " + ONorOFF, true);
                Log("FSW26 Phase Compensation: " + ONorOFF);
            }
            catch
            {
                Log("FSW26 Phase Compensation: FAIL", LogLevel.ERROR);
            }
        }
        public void AutoScale()
        {
            try
            {
                fsw.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(10000);
            }
            catch
            {

            }
        }

        /*....................................................Hàm thực hiện các bài đo................................................................*/

        public void LoadNRTMSetup(string NRTMxx)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("INST:DEL '5G NR'");
                // Support Slot S
                //fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/NRTM/7D1S2U/" + NRTMxx + "_7D1S2U" + "'", true);
                // Unsupport slot S
                fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/NRTM/7D0S2U/" + NRTMxx + "_7D1S2U" + "'", true);
                Log("FSW26 load NRTM measurement: " + NRTMxx);
            }
            catch
            {
                Log("FSW26 Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void OFDMPower(out string power)
        {
            power = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:OSTP:AVER?", true);
                object[] fsw26_Power;
                fsw26_Power = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = fsw26_Power[0].ToString();
                Log("OFDM Power: " + power);
            }
            catch
            {
                Log("FSW26 OFDM Power Measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void TXPower(int port, BBU bbu, out string power)
        {
            string[] data = new string[] { "OUTPUT POWER", "GAIN" };
            string gain = "";
            power = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsw.WriteString(":FETC:CC1:FRAM:SUMM:POW:MAX?", true); //01.04.2024 Sua lai thanh cong suat Power MAX
                object[] fsw26_Power;
                fsw26_Power = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = fsw26_Power[0].ToString();
                //bbu.SHW_TX_GAIN(port.ToString(), out gain);
                string[] powerresult = new string[] { power, bbu.SHW_TX_GAIN(port.ToString(), out gain) };
                mainForm.DLCsvRecord(data, powerresult, null, "POWER", "NRTM1.1", powerresult.Length, 0, port);
                Log("Output Power " + power);
            }
            catch
            {
                Log("FSW26 TX Power Measurement: FAIL", LogLevel.ERROR);
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
                //   TakePhoto(mode);
                //   System.Threading.Thread.Sleep(1000);
                object[] Tmp;
                //Đọc giá trị EVM QPSK
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSQP:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[0] = Tmp[0].ToString();
                //Đọc giá trị EVM 16QAM
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSST:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[1] = Tmp[0].ToString();
                //Đọc giá trị EVM 64QAM
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSSF:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[2] = Tmp[0].ToString();
                //Đọc giá trị EVM 256QAM
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:DSTS:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[3] = Tmp[0].ToString();
                //Đọc giá trị EVM All
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:ALL:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[4] = Tmp[0].ToString();
                //Đọc giá trị EVM of all Physical Channel
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:PCH:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[5] = Tmp[0].ToString();
                //Đọc giá trị EVM of all Physical Signal
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:EVM:PSIG:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[6] = Tmp[0].ToString();
                //Đọc giá trị I/Q Offset
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:IQOF:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[7] = Tmp[0].ToString();
                //Đọc giá trị I/Q Gain Imbalance
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:GIMB:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[8] = Tmp[0].ToString();
                //Đọc giá trị I/Q Quadrature
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:QUAD:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[9] = Tmp[0].ToString();
                //Đọc giá trị OFDM Symbol Tx Power 
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:OSTP:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[10] = Tmp[0].ToString();
                //Đọc giá trị Tx Power 
                fsw.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:POW:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[11] = Tmp[0].ToString();
                //Đọc giá trị Crest Factor 
                fsw.WriteString(":FETC:CC1:ISRC:SUMM:CRES:AVER?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                EVM[12] = Tmp[0].ToString();
                fsw.WriteString("FETC:CC1:FRAM3:SUMM:FERR?", true);
                Tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                freqErr = Tmp[0].ToString();
                Log("FSW: modulation quality measurement" + "\n"
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
                    + "Crest Factor: " + EVM[12] + "\n"
                    + "Frequency Error: " + freqErr);
            }
            catch
            {
                Log("ERROR: FSW26 EVM Measurement fail", LogLevel.ERROR);
            }
        }
        public void LoadACLRSetup(string BW, string TDDFDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsw.WriteString("INST:DEL '5G NR'");
                fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/ACLR/ACLR_" + BW + "Mhz_" + TDDFDD + "'", true);
                Log("FSW26: Recall state: ACLR_" + BW + "_" + TDDFDD);
            }
            catch
            {
                Log("ERROR: FSW26 recall state ACLR fail", LogLevel.ERROR);
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
                fsw.WriteString("CALC:MARK:FUNC:POW:RES:DET? ACP", true);
                object[] fsw26_aclr;
                fsw26_aclr = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");

                power = fsw26_aclr[0].ToString();
                string aclrAdjabsLower = fsw26_aclr[1].ToString();
                string aclrAdjabsUpper = fsw26_aclr[2].ToString();
                string aclrAdtabsLower = fsw26_aclr[5].ToString();
                string aclrAdtabsUpper = fsw26_aclr[6].ToString();
                aclrAdjLower = fsw26_aclr[3].ToString();
                aclrAdjUpper = fsw26_aclr[4].ToString();
                aclrAlt1Lower = fsw26_aclr[7].ToString();
                aclrAlt1Upper = fsw26_aclr[8].ToString();
                Log("FSW26: ACLR measurement" + "\n"
                    + "ADJ lower: " + aclrAdjLower + "\n"
                    + "ADJ upper: " + aclrAdjUpper + "\n"
                    + "ALT lower: " + aclrAlt1Lower + "\n"
                    + "ALT upper: " + aclrAlt1Upper + "\n");
                double tmp1 = Math.Max(double.Parse(aclrAdjLower), double.Parse(aclrAdjUpper));
                double tmp2 = Math.Max(double.Parse(aclrAlt1Lower), double.Parse(aclrAlt1Upper));
                double tmp3 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp4 = Math.Max(double.Parse(aclrAdtabsLower), double.Parse(aclrAdtabsUpper));
                string[] result = new string[] { Math.Max(tmp1, tmp2).ToString(), Math.Max(tmp3, tmp4).ToString() };
                mainForm.DLCsvRecord(data, fsw26_aclr, result, "ACLR", TM, fsw26_aclr.Length, result.Length, port);
            }
            catch
            {
                Log("FSW26 ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }
        /*........................................................OBW mesurement.....................................................................*/
        public void LoadOBWSetup(string BW, string TDD_or_FDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsw.WriteString("INST:DEL '5G NR'");
                System.Threading.Thread.Sleep(100);
                fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/OBW/OBW_" + BW + "MHz_" + TDD_or_FDD + "'", true);
                Log("FSW26 load OBW measurement: OBW_" + BW + "_" + TDD_or_FDD);
            }
            catch
            {
                Log("FSW26 Load OBW Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void OBWMeas(int port, out string OBWMeas)
        {
            OBWMeas = "ERROR";
            string[] data = new string[] { "Occupied Bandwidth" };
            //    string mode = "OBW";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsw.WriteString("CALC:MARK:FUNC:POW:RES? OBW", true);
                object[] fsw26_obw;
                fsw26_obw = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                OBWMeas = fsw26_obw[0].ToString();
                Log(OBWMeas);
                mainForm.DLCsvRecord(data, fsw26_obw, null, "OBW", "TM1.1", fsw26_obw.Length, 0, port);
            }
            catch
            {
                Log("FSW26 load OBW measurement: FAIL", LogLevel.ERROR);
            }
        }
        /*..................................................................Transmitter ON/OFF Power.............................................................................*/
        public void LoadTransONOFFSetup(string BW, string TDD_or_FDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsw.WriteString("INST:DEL '5G NR'");
                fsw.WriteString("MMEM:LOAD:STAT 1, 'C:/R_S/Instr/user/NR5G/TransONOFF/TransONOFF_" + BW + "MHz_" + TDD_or_FDD + "'", true);


                Log("FSW26 load TransmitterON/OFF measurement: TransONOFF_" + BW + "_" + TDD_or_FDD);
            }
            catch
            {
                Log("FSW26 Load ON/OFF Setup: FAIL", LogLevel.ERROR);
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
                fsw.WriteString("TRAC6:DATA? LIST", true);
                object[] fsw26_TransONOFF;
                fsw26_TransONOFF = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                OFF_Power1 = fsw26_TransONOFF[3].ToString();
                OFF_Power2 = fsw26_TransONOFF[10].ToString();
                Falling_Period1 = fsw26_TransONOFF[5].ToString();
                Rising_Period1 = fsw26_TransONOFF[6].ToString();
                Falling_Period2 = fsw26_TransONOFF[12].ToString();
                Rising_Period2 = fsw26_TransONOFF[13].ToString();
                offPower = (Math.Max(double.Parse(OFF_Power1), double.Parse(OFF_Power2))).ToString();
                string tmp = (Math.Max(double.Parse(Falling_Period1), double.Parse(Rising_Period1))).ToString();
                string tmp1 = (Math.Max(double.Parse(Falling_Period2), double.Parse(Rising_Period2))).ToString();
                transPeriod = (Math.Max(double.Parse(tmp), double.Parse(tmp1)) * 1000000).ToString();
                string[] result = new string[] { transPeriod, (Math.Max(double.Parse(OFF_Power1), double.Parse(OFF_Power2))).ToString() };

                mainForm.DLCsvRecord(data, fsw26_TransONOFF, result, mode, TM, fsw26_TransONOFF.Length, result.Length, port);
            }
            catch
            {
                Log("FSW26 TransON/OFF measurement: FAIL", LogLevel.ERROR);
            }
        }

        #region TAE
        public void LoadTAESetup(string BW)
        {
            try
            {
                fsw.WriteString("INST:DEL '5G NR'");
                fsw.WriteString("MMEM:LOAD:STAT 1, 'C:/R_S/Instr/user/NR5G/TAE/tae_" + BW + "MHz_32t32r" + "'", true);
                Log("FSW26 load TAE measurement: TAE_" + BW + "MHz_32t32r");
                bool Flag = true;
                System.Threading.Thread.Sleep(1);
                while (Flag)
                {
                    try
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        System.Threading.Thread.Sleep(1);
                        fsw.WriteString("FETC:TAER:CC:AP1001:MAX?", true);
                        object[] fsw26_TAE;
                        fsw26_TAE = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
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
                Log("FSW26: Load TAE Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void TAEMeas(out string taeMeas)
        {
            taeMeas = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("FETC:TAER:CC:AP1001:MAX?", true);
                object[] tmp;
                //object[] tmp2;
                tmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                taeMeas = (double.Parse(tmp[0].ToString()) * 1000000000).ToString();
                Log("Time aligement error: " + taeMeas);
            }
            catch
            {
                Log("FSW26 TAE measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void newTAEMeas(string mode, string testmodel, int port, ref string taeref, ref string[] tae)
        {
            object[] froffsettmp = new object[] { 0 };
            //taeref = "0";
            string taeport;
            try
            {
                string[] data = new string[] { "Frame start offset (s)", "TAE (ns)" };
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString(":FETC:CC1:ISRC:SUMM:TFR?", true);
                if (port == 1)
                {
                    froffsettmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any);
                    taeref = (froffsettmp[0]).ToString();
                    //taeref = "0"; // Gia tri frameoffset cua tung port
                    tae = new string[] { taeref };
                }
                else
                {
                    froffsettmp = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any);
                    taeport = froffsettmp[0].ToString();
                    //tae[port] = (double.Parse(taeport) - double.Parse(taeref)).ToString();
                    tae = new string[] { ((double.Parse(taeport) - double.Parse(taeref)) * 1000000000).ToString() };
                }
                string froffstart = (froffsettmp[0]).ToString();
                Log("Frame start offset: " + froffstart + " s");
                mainForm.DLCsvRecord(data, froffsettmp, tae, mode, testmodel, data.Length - 1, 1, port);
            }
            catch
            {
                Log("FSW26 TAE measurement: FAIL", LogLevel.ERROR);
            }
        }
        #endregion
        /*.................................................................Operating band Unwanted emissions (SEM) ...........................................................................*/
        public void LoadSEMSetup(string BW, string TDDFDD, string Freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsw.WriteString("INST:DEL '5G NR'");
                fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/SEM/SEM_" + BW + "MHz_" + TDDFDD + "_" + Freq.Substring(0, 4) + "MHz'", true); //SEM_100MHz_TDD_2550MHz.dfl
                Log("fsw26 load SEM measurement: SEM_" + BW + "MHz_" + TDDFDD + "_" + Freq.Substring(0, 4) + "MHz'");
            }
            catch (Exception)
            {
                Log("FSW26: Load SEM setup: fail", LogLevel.ERROR);
            }
        }
        public void SEMMeas(int port, string TM, out double absPwr1, out double absPwr2, out double absPwr3, out double semLimit)
        {
            absPwr1 = 0;
            absPwr2 = 0;
            absPwr3 = 0;
            semLimit = 1;
            string[] data = new string[] {
            "RANGE", "START FREQ 1", "STOP FREQ 1", "RBW 1", "FREQ 1", "ABS POWER 1", "RELATIVE POWER 1", "DELTA LIMIT 1", "WORST POWER 1","WORST LIMIT 1","FREQUENCY 1",
            "RANGE", "START FREQ 2", "STOP FREQ 2", "RBW 2", "FREQ 2", "ABS POWER 2", "RELATIVE POWER 2", "DELTA LIMIT 2", "WORST POWER 2","WORST LIMIT 2","FREQUENCY 2",
            "RANGE", "START FREQ 3", "STOP FREQ 3", "RBW 3", "FREQ 3", "ABS POWER 3", "RELATIVE POWER 3", "DELTA LIMIT 3", "WORST POWER 3","WORST LIMIT 3","FREQUENCY 3",
            "RANGE", "START FREQ 4", "STOP FREQ 4", "RBW 4", "FREQ 4", "ABS POWER 4", "RELATIVE POWER 4", "DELTA LIMIT 4", "WORST POWER 4","WORST LIMIT 4","FREQUENCY 4",
            "RANGE", "START FREQ 5", "STOP FREQ 5", "RBW 5", "FREQ 5", "ABS POWER 5", "RELATIVE POWER 5", "DELTA LIMIT 5", "WORST POWER 5","WORST LIMIT 5","FREQUENCY 5",
            "RANGE", "START FREQ 6", "STOP FREQ 6", "RBW 6", "FREQ 6", "ABS POWER 6", "RELATIVE POWER 6", "DELTA LIMIT 6", "WORST POWER 6","WORST LIMIT 6","FREQUENCY 6"};
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            try
            {
                System.Threading.Thread.Sleep(4000);
                fsw.WriteString("TRAC:DATA? LIST", true);
                object[] fsw26_SEM;
                fsw26_SEM = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                // relative power
                string limit1 = fsw26_SEM[7].ToString();
                string limit2 = fsw26_SEM[18].ToString();
                string limit3 = fsw26_SEM[29].ToString();
                string limit4 = fsw26_SEM[40].ToString();
                string limit5 = fsw26_SEM[51].ToString();
                string limit6 = fsw26_SEM[62].ToString();

                //abs power
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
                Log("FSW26 measement: " + "\n" + "absPower1: " + absPower1 + "\n" + "absPower2: " + absPower2 + "\n" + "absPower3: " + absPower3 + "\n" + "absPower4: " + absPower4 + "\n" + "absPower5: " + absPower5 + "\n" + "absPower6: " + absPower6);

                absPwr1 = Math.Round(Math.Max(double.Parse(absPower3), double.Parse(absPower4)), 1);
                absPwr2 = Math.Round(Math.Max(double.Parse(absPower2), double.Parse(absPower5)), 1);
                absPwr3 = Math.Round(Math.Max(double.Parse(absPower1), double.Parse(absPower6)), 1);

                string[] result = new string[] { };
                mainForm.DLCsvRecord(data, fsw26_SEM, result, "SEM", TM, fsw26_SEM.Length, result.Length, port);
            }
            catch
            {
                Log("FSW26: SEM measurement: FAIL", LogLevel.ERROR);
            }
        }
        /*...............................................................................................................................................*/
        public void LoadSpurSetup(string mode, string Freq, string RBW)
        {
            try
            {
                string filename = "";
                switch (mode)
                {
                    case "General":
                        if (double.Parse(Freq) < 2600000000)
                        {
                            filename = "C:/R_S/Instr/user/NR5G/TSE/SpurGeneral_" + "Bot";
                        }
                        else
                        {
                            filename = "C:/R_S/Instr/user/NR5G/TSE/SpurGeneral_" + "Top";
                        }
                        break;
                    case "Protection":
                        filename = "C:/R_S/Instr/user/NR5G/TSE/SpurProtection";
                        break;
                    case "Co-existance":
                        filename = "C:/R_S/Instr/user/NR5G/TSE/SpurCoex_RBW" + RBW;
                        break;
                    case "Co-location":
                        if (double.Parse(Freq) < 2600000000)
                        {
                            filename = "C:/R_S/Instr/user/NR5G/TSE/SpurColo_" + "B";
                        }
                        else
                        {
                            filename = "C:/R_S/Instr/user/NR5G/TSE/SpurColo_" + "T";
                        }
                        break;
                    case "Rx-Spurious":
                        filename = "C:/R_S/Instr/user/NR5G/RSE/Rx-Spurious";
                        break;
                }
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsw.WriteString($"MMEM:LOAD:STAT 1,'{filename}'", true);
                Log($"FSW26 load Spurious measurement: {filename}");
            }
            catch
            {
                Log("FSW26 Load Transmitter Spurious Emission Setup: FAIL", LogLevel.ERROR);
            }
        }
        double?[] tmp = new double?[20];
        public void SpurMeas(int port, out double?[] spur)
        {
            string[] data = new string[] { "RANGE", "RANGE LOW", "RANGE HIGH", "RBW", "FREQ", "ABS POWER", "delta Limit" };

            spur = tmp;
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("CALC:PSE:SUBR 1", true); //Sets 1 peaks per range to be stored in the list.
                System.Threading.Thread.Sleep(2000);
                fsw.WriteString("INIT:SPUR; *WAI", true); //Performs a spurious emission measurement and waits until the sweep has finished.
                System.Threading.Thread.Sleep(2000);
                fsw.WriteString("TRAC:DATA? LIST", true);// Queries the peak list of the spurious emission measurement 
                System.Threading.Thread.Sleep(5000);
                //convert data
                object[] Spur;
                Spur = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                while (Spur.Length % 11 == 0)
                {
                    int n = Spur.Length / 11;
                    string[] range = new string[n];
                    string[] rangelow = new string[n];
                    string[] rangehigh = new string[n];
                    string[] rbw = new string[n];
                    string[] freq = new string[n];
                    double[] abs = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        range[i] = Spur[i * 11].ToString();
                        rangelow[i] = Spur[i * 11 + 1].ToString();
                        rangehigh[i] = Spur[i * 11 + 2].ToString();
                        rbw[i] = Spur[i * 11 + 3].ToString();
                        freq[i] = Spur[i * 11 + 4].ToString();
                        abs[i] = double.Parse(Spur[i * 11 + 5].ToString());
                        spur[i] = double.Parse(Spur[i * 11 + 7].ToString());
                        Log($"Delta limit range {i + 1} : {spur[i]}");
                        object[] value = new object[] { range[i], rangelow[i], rangehigh[i], rbw[i], freq[i], abs[i], spur[i] };
                        mainForm.DLCsvRecord(data, value, null, "Spurious", "TM1.1", value.Length, 0, port);
                    }
                    break;
                }
            }
            catch
            {
                Log("FSW26 Spurious emission measurement: FAIL", LogLevel.ERROR);
            }
        }
        //........................................ TX IMD...................................................
        public void LoadIMDACLRSetup(string offSet, string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                fsw.WriteString("INST:DEL '5G NR'");
                fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/IMD/IMD_ACLR/ACLR_2600_IMD_OFF" + offSet + "MHz" + "'", true);
                System.Threading.Thread.Sleep(3000);
                Log("FSW26 load IMD ACLR measurement: Offset " + offSet + "Mhz");
                fsw.WriteString(":SENS:POW:ACH:SBL1:FREQ:CENT " + freq);
                System.Threading.Thread.Sleep(1000);
                fsw.WriteString(":SENS:POW:ACH:SBL1:CENT:CHAN1 " + freq);
                System.Threading.Thread.Sleep(1000);
            }
            catch
            {
                Log("FSW26 Load ACLR Setup: FAIL", LogLevel.ERROR);
            }
        }
        string[] tmp1 = new string[2];
        public void IMDACLR(string offSet, out double IMDACLR, string TM, int port)
        {
            IMDACLR = 0;
            string[] data = new string[] { "TXPOWER", "LOWER ABS ADJ", "UPPER ABS ADJ", "LOWER ADJ", "UPPER ADJ", "ACLR", "Worst Abs Power" };
            string mode = "IMD_ACLR_" + offSet + "Mhz";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //    fsw.WriteString(":SENS:ADJ:LEV;*WAI", true);
                System.Threading.Thread.Sleep(5000);

                //    TakePhoto(mode); // luu anh

                //    System.Threading.Thread.Sleep(500);
                fsw.WriteString("CALC:MARK:FUNC:POW:RES? ACP", true);
                object[] fsw26_aclr;
                fsw26_aclr = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");

                double tmp = Math.Abs(double.Parse(fsw26_aclr[1].ToString())); // doi dau , ep kieu du lieu sang double
                double tmp1 = Math.Abs(double.Parse(fsw26_aclr[2].ToString()));
                IMDACLR = Math.Min(tmp, tmp1);
                Log("ADJ: " + tmp + "\n" + "ALT: " + tmp1);
                string[] result = new string[] { Math.Max(tmp, tmp1).ToString() };
                mainForm.DLCsvRecord(data, fsw26_aclr, result, "ACLR", TM, fsw26_aclr.Length, result.Length, port);
            }
            catch
            {
                Log("FSW26 ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadIMDSEMSetup(string offSet, string Freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsw.WriteString("INST:DEL '5G NR'");
                fsw.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/IMD/IMD_SEM/SEM_" + Freq + "_IMD_OFF" + offSet + "MHz" + "'", true);
                Log("fsw26 load IMD SEM measurement: SEM " + offSet + "Mhz");
            }
            catch
            {
                Log("FSW26 Load SEM Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void IMDSEMMeas(string offSet, out string semLimit, string TM, int port)
        {
            semLimit = "ERROR";
            string mode = "IMD_SEM_" + offSet + "Mhz";
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            try
            {
                string[] limit = new string[7];
                string[] data = new string[] {
            "RANGE", "START FREQ 1", "STOP FREQ 1", "RBW 1", "FREQ 1", "ABS POWER 1", "RELATIVE POWER 1", "DELTA LIMIT 1", "WORST POWER 1","WORST LIMIT 1","FREQUENCY 1",
            "RANGE", "START FREQ 2", "STOP FREQ 2", "RBW 2", "FREQ 2", "ABS POWER 2", "RELATIVE POWER 2", "DELTA LIMIT 2", "WORST POWER 2","WORST LIMIT 2","FREQUENCY 2",
            "RANGE", "START FREQ 3", "STOP FREQ 3", "RBW 3", "FREQ 3", "ABS POWER 3", "RELATIVE POWER 3", "DELTA LIMIT 3", "WORST POWER 3","WORST LIMIT 3","FREQUENCY 3",
            "RANGE", "START FREQ 4", "STOP FREQ 4", "RBW 4", "FREQ 4", "ABS POWER 4", "RELATIVE POWER 4", "DELTA LIMIT 4", "WORST POWER 4","WORST LIMIT 4","FREQUENCY 4",
            "RANGE", "START FREQ 5", "STOP FREQ 5", "RBW 5", "FREQ 5", "ABS POWER 5", "RELATIVE POWER 5", "DELTA LIMIT 5", "WORST POWER 5","WORST LIMIT 5","FREQUENCY 5",
            "RANGE", "START FREQ 6", "STOP FREQ 6", "RBW 6", "FREQ 6", "ABS POWER 6", "RELATIVE POWER 6", "DELTA LIMIT 6", "WORST POWER 6","WORST LIMIT 6","FREQUENCY 6",
            "RANGE", "START FREQ 7", "STOP FREQ 7", "RBW 7", "FREQ 7", "ABS POWER 7", "RELATIVE POWER 7", "DELTA LIMIT 7", "WORST POWER 7","WORST LIMIT 7","FREQUENCY 7"};
                System.Threading.Thread.Sleep(2000);
                // luu anh 
                //TakePhoto(mode);
                //
                fsw.WriteString("TRAC:DATA? LIST", true);
                System.Threading.Thread.Sleep(2000);
                object[] fsw26_SEM;
                fsw26_SEM = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                if (offSet == "-25" || offSet == "25")
                {
                    System.Threading.Thread.Sleep(2000);
                    limit[0] = fsw26_SEM[7].ToString();
                    limit[1] = fsw26_SEM[18].ToString();
                    limit[2] = fsw26_SEM[29].ToString();
                    limit[3] = fsw26_SEM[40].ToString();
                    limit[4] = fsw26_SEM[51].ToString();
                    limit[5] = fsw26_SEM[62].ToString();
                    limit[6] = fsw26_SEM[73].ToString();
                    Log("IMD SEM limit: " + "\n" + limit[0] + "\n" + limit[1] + "\n" + limit[2] + "\n" + limit[3] + "\n" + limit[4] + "\n" + limit[5] + "\n" + limit[6]);
                    double max = -100;
                    for (int i = 0; i < 7; i++)
                    {
                        if (double.Parse(limit[i]) > max)
                        {
                            max = double.Parse(limit[i]);
                        }
                    }
                    semLimit = max.ToString();
                }
                else if (offSet == "-15" || offSet == "15")
                {
                    System.Threading.Thread.Sleep(2000);
                    limit[0] = fsw26_SEM[7].ToString();
                    limit[1] = fsw26_SEM[18].ToString();
                    limit[2] = fsw26_SEM[29].ToString();
                    limit[3] = fsw26_SEM[40].ToString();
                    limit[4] = fsw26_SEM[51].ToString();
                    limit[5] = fsw26_SEM[62].ToString();
                    Log("IMD SEM limit: " + "\n" + limit[0] + "\n" + limit[1] + "\n" + limit[2] + "\n" + limit[3] + "\n" + limit[4] + "\n" + limit[5]);
                    double max = -100;
                    for (int i = 0; i < 6; i++)
                    {
                        if (double.Parse(limit[i]) > max)
                        {
                            max = double.Parse(limit[i]);
                        }
                    }
                    semLimit = max.ToString();
                }
                else if (offSet == "-5" || offSet == "5")
                {
                    System.Threading.Thread.Sleep(2000);
                    limit[0] = fsw26_SEM[7].ToString();
                    limit[1] = fsw26_SEM[18].ToString();
                    limit[2] = fsw26_SEM[29].ToString();
                    limit[3] = fsw26_SEM[40].ToString();
                    limit[4] = fsw26_SEM[51].ToString();
                    Log("IMD SEM limit: " + "\n" + limit[0] + "\n" + limit[1] + "\n" + limit[2] + "\n" + limit[3] + "\n" + limit[4]);
                    double max = -100;
                    for (int i = 0; i < 5; i++)
                    {
                        if (double.Parse(limit[i]) > max)
                        {
                            max = double.Parse(limit[i]);
                        }
                    }
                    semLimit = max.ToString();
                }
                string[] result = new string[] { };
                mainForm.DLCsvRecord(data, fsw26_SEM, result, "IMDSEM", TM, fsw26_SEM.Length, result.Length, port);
            }
            catch
            {
                Log("FSW26 IMD SEM measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadIMDTSESetup(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                string filepath = $"'C:/R_S/Instr/user/NR5G/IMD/IMD_TSE/IMD_Spurious'";
                fsw.WriteString($"MMEM:LOAD:STAT 1, {filepath}", true);
                Log($"fsw26 load IMD TSE measurement: {filepath}");
            }
            catch
            {
                Log("FSW26 Load Transmitter Spurious Emission Setup: FAIL", LogLevel.ERROR);
            }
        }
        double?[] tmp5 = new double?[20];
        public void IMDTSEMeas(out double?[] imdTSE, int port)
        {
            imdTSE = tmp5;
            string[] data = new string[] { "RANGE", "RANGE LOW", "RANGE HIGH", "RBW", "FREQ", "ABS POWER", "delta Limit" };
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(4000);
                fsw.WriteString("CALC:PSE:SUBR 1", true); //Sets 1 peaks per range to be stored in the list.
                System.Threading.Thread.Sleep(2000);
                fsw.WriteString("INIT:SPUR; *WAI", true); //Performs a spurious emission measurement and waits until the sweep has finished.
                System.Threading.Thread.Sleep(2000);
                fsw.WriteString("TRAC:DATA? LIST", true);// Queries the peak list of the spurious emission measurement 
                System.Threading.Thread.Sleep(5000);
                //convert data
                object[] Spur;
                Spur = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ","); //44
                while (Spur.Length % 11 == 0)
                {
                    int n = Spur.Length / 11;
                    string[] range = new string[n];
                    string[] rangelow = new string[n];
                    string[] rangehigh = new string[n];
                    string[] rbw = new string[n];
                    string[] freq = new string[n];
                    double[] abs = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        range[i] = Spur[i * 11].ToString();
                        rangelow[i] = Spur[i * 11 + 1].ToString();
                        rangehigh[i] = Spur[i * 11 + 2].ToString();
                        rbw[i] = Spur[i * 11 + 3].ToString();
                        freq[i] = Spur[i * 11 + 4].ToString();
                        abs[i] = double.Parse(Spur[i * 11 + 5].ToString());
                        imdTSE[i] = double.Parse(Spur[i * 11 + 7].ToString());
                        Log($"Delta limit range {i + 1} : { imdTSE[i]}");
                        object[] value = new object[] { range[i], rangelow[i], rangehigh[i], rbw[i], freq[i], abs[i], imdTSE[i] };
                        mainForm.DLCsvRecord(data, value, null, "IMDSpurious", "TM1.1", value.Length, 0, port);
                    }
                    break;
                }
            }
            catch
            {
                Log("FSW26 TSE Measurement: FAIL", LogLevel.ERROR);
            }
        }

        //19.2.2024
        public void TakeScreenshot(string filePathVSA)
        {
            try
            {
                fsw.WriteString("HCOP:DEST 'MMEM'");
                fsw.WriteString($"MMEM:NAME \"{filePathVSA}\"");
                fsw.WriteString("HCOP:IMM");

                Log($"FSW26 Screenshot saved to: {filePathVSA}");
            }
            catch (Exception ex)
            {
                Log("[Error]Save Screen Shot: " + ex.Message, LogLevel.ERROR);
            }
        }
        public void SingleSweepOnOff(int onOff)
        {
            try
            {
                if (onOff == 0)
                {
                    fsw.WriteString("INIT:CONT OFF");
                    System.Threading.Thread.Sleep(5000); //time for VSA change to Ready
                    Log("FSW26: Changed to Single Sweep");
                }
                else if (onOff == 1)
                {
                    fsw.WriteString($"INIT:CONT ON");
                    //System.Threading.Thread.Sleep(2000); //time for VSA change to Ready
                    Log("FSW26: Changed to Continous Sweep");
                }
                else
                {
                    Log("Error: Init SingleSweep/ContinousSweep parameter invalid", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log("[Error] Single Sweep: " + ex.Message, LogLevel.ERROR);
            }
        }

        internal void ACLRPower(int port, string TM, out double intraACLR, string mode)
        {
            intraACLR = 0;
            string[] data = new string[] { "TXPOWER", "LOWER ABS ADJ", "UPPER ABS ADJ", "LOWER ADJ", "UPPER ADJ", "LOWER ABS ALT", "UPPER ABS ALT", "LOWER ALT", "UPPER ALT", "ACLR", "Worst Abs Power" };
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //    TakePhoto(mode); // luu anh
                //   System.Threading.Thread.Sleep(500);
                fsw.WriteString("CALC:MARK:FUNC:POW:RES:DET? ACP", true);
                object[] fsw26_aclr;
                fsw26_aclr = (object[])fsw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");

                string power = fsw26_aclr[0].ToString();
                // Gia tri ACLR ABS 
                string aclrAdjabsLower = fsw26_aclr[1].ToString();
                string aclrAdjabsUpper = fsw26_aclr[2].ToString();
                string aclrAdtabsLower = fsw26_aclr[5].ToString();
                string aclrAdtabsUpper = fsw26_aclr[6].ToString();
                // Gia tri ACLR RELATIVE
                string aclrAdjLower = fsw26_aclr[3].ToString();
                string aclrAdjUpper = fsw26_aclr[4].ToString();
                string aclrAlt1Lower = fsw26_aclr[7].ToString();
                string aclrAlt1Upper = fsw26_aclr[8].ToString();
                Log("FSW26: ACLR measurement" + "\n"
                    + "ADJ lower: " + aclrAdjLower + "\n"
                    + "ADJ upper: " + aclrAdjUpper + "\n"
                    + "ALT lower: " + aclrAlt1Lower + "\n"
                    + "ALT upper: " + aclrAlt1Upper + "\n");
                double WorstabsACLR = CompareACLR(aclrAdjabsLower, aclrAdjabsUpper, aclrAdtabsLower, aclrAdtabsUpper);
                intraACLR = CompareACLR(aclrAdjLower, aclrAdjUpper, aclrAlt1Lower, aclrAlt1Upper);
                string[] result = new string[] { intraACLR.ToString(), WorstabsACLR.ToString() };
                mainForm.DLCsvRecord(data, fsw26_aclr, result, mode, TM, fsw26_aclr.Length, result.Length, port);
            }
            catch
            {
                Log("FSW26 ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        private double CompareACLR(string param1, string param2, string param3, string param4)
        {
            double tmp1 = Math.Max(double.Parse(param1), double.Parse(param2));
            double tmp2 = Math.Max(double.Parse(param3), double.Parse(param4));
            double tmp3 = Math.Max(tmp1, tmp2);
            return tmp3;
        }

        public void SaveCSVFile(string filePath)
        {
            try
            {
                fsw.WriteString($"FORM:DEXP:FORM CSV"); // Determines the format of the exported ASCII file is CSV
                fsw.WriteString($"MMEM:STOR1:TRAC 1, \"{filePath}\""); // Save file
                System.Threading.Thread.Sleep(1000);
                Log($"FSW26 CSV trace saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Log("[Error]Save CSV File: " + ex.Message, LogLevel.ERROR);
            }
        }
        public void MakeDir(string dir)
        {
            try
            {
                fsw.WriteString($"MMEM:MDIR '{dir}'");
                System.Threading.Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Log("[Error]: " + ex.Message, LogLevel.ERROR);
            }
        }
        public void AdjustTiming()
        {
            System.Threading.Thread.Sleep(2000);
            fsw.WriteString(":INIT:CONT ON");
            Log("FSW26: Changed to Cont Sweep");
            System.Threading.Thread.Sleep(2000);
            fsw.WriteString(":SENS:NR5G:OOP:ATIM");
            Log("FSW26: Adjust Timing successful");
            System.Threading.Thread.Sleep(3000);
            fsw.WriteString(":INIT:CONT OFF");
            fsw.WriteString(":INIT:IMM;* WAI");

            Log("FSW26: Changed to Single Sweep");
        }
    }
}
