using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using CsvHelper.Configuration;

namespace _5GAutoTool
{
    public class N9020A
    {
        public Form5GAT mainForm;
        public string Name = "N9020A";
        public N9020A(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 n9020a = new Ivi.Visa.Interop.FormattedIO488();
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
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "N9020A");
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
                //    Disconnect();
                n9020a.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                n9020a.IO.Timeout = 2000;
                n9020a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                manufacturer = tmp[0].ToString();
                model = tmp[1].ToString();
                serial = tmp[2].ToString();
                if (tmp[1].ToString() == Instrument)
                {
                    Log($"IP: {IP} connected successfully! ", LogLevel.SUCCESS);
                }
            }
            catch (Exception e)
            {
                Log("Connection: FAIL! " + e.Message, LogLevel.ERROR);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (n9020a != null)
                {
                    n9020a.IO.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {

                n9020a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception)
            {
                Log("Connection: FAIL! ", LogLevel.ERROR);
            }
        }
        public void SendCommand(string cmd)
        {
            try
            {
                cmd = cmd.Trim();
                n9020a.WriteString("*CLS", true);
                Log($"[ClearErrors] SEND: *CLS");

                if(!cmd.Contains("*OPC?") && !cmd.Contains("?"))
                {
                    cmd += ";*OPC?";                    
                }
                n9020a.WriteString(cmd, true);
                Log($"[Querry] SEND: {cmd}");

                object[] tmp;
                tmp = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Log($"[Querry] RECV: {string.Join(",",tmp)}");

                n9020a.WriteString("*ESR?", true);
                Log($"[CheckErrors] SEND: *ESR?");
                tmp = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Log($"[CheckErrors] RECV: {string.Join(",", tmp)}");
                //if(error != "0")
                //{
                //    Log($"[N9020A]: [CheckErrors] Error Message: ", LogLevel.ERROR);
                //}
            }
            catch(Exception ex)
            {
                Log($"Send command:{cmd} FAIL! "+ ex.Message, LogLevel.ERROR);
            }
        }

        public void SetFreqCent(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                SendCommand("CCAR:REF " + freq + "Hz");
                Log("Set center frequency: " + freq);
            }
            catch (Exception ex)
            {
                Log($"Set center frequency: {freq} => FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void SetTriggerSource(string TriggerSoure)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //n9020a.WriteString("CCAR:REF " + freq + "Hz", true);
                //Log("N9020A select center frequency: " + freq);
            }
            catch (Exception)
            {
                Log("Set trigger source: FAIL", LogLevel.ERROR);
            }
        }

        public void SetEnableGate(int ONOFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //n9020a.WriteString("CCAR:REF " + freq + "Hz", true);
                //Log("N9020A select center frequency: " + freq);
            }
            catch (Exception)
            {
                Log("Set Gate ONOFF: FAIL", LogLevel.ERROR);
            }
        }

        public void SetGateDelay(double GateDelay)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //n9020a.WriteString("CCAR:REF " + freq + "Hz", true);
                //Log("N9020A select center frequency: " + freq);
            }
            catch (Exception)
            {
                Log("Set Gate delay: FAIL", LogLevel.ERROR);
            }
        }

        public void SetGateLength(double GateLength)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //n9020a.WriteString("CCAR:REF " + freq + "Hz", true);
                //Log("N9020A select center frequency: " + freq);
            }
            catch (Exception)
            {
                Log("Set Gate Length: FAIL", LogLevel.ERROR);
            }
        }

        public void SetAtt(string att)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $"CORR:BTS:GAIN -{att} dB";
                SendCommand(cmd);
                Log("Set External Gain: " + att);
                System.Threading.Thread.Sleep(200);
                SendCommand("DISP:CHP:WIND:TRAC:Y:RLEV 50 dBm");
                Log("Set Reference Level Offset: 50 dBm");
            }
            catch (Exception ex)
            {
                Log("Set External Gain:  FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void AutoScale()
        {
            SendCommand("POW:RANG:OPT IMM");
            //System.Threading.Thread.Sleep(7000);
        }
        public void LoadACLRSetup(string BW, string TDDFDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(1000);
                SendCommand($@":MMEM:LOAD:STAT 'D:\Users\Instrument\Documents\NR5G\state\ACLR\ACLR_{BW}Mhz_{TDDFDD}.state'");
                Log("Load ACLR Measurement State: ACLR_" + BW + "_" + TDDFDD);
            }
            catch (Exception e)
            {
                Log("Load ACLR Measurement State: FAIL! " + e.Message, LogLevel.ERROR);
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
                n9020a.WriteString("READ:ACP?", true);
                System.Threading.Thread.Sleep(2000);
                object[] aclr;
                aclr = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = aclr[1].ToString();
                aclrAdjLower = aclr[4].ToString();
                aclrAdjUpper = aclr[6].ToString();
                aclrAlt1Lower = aclr[8].ToString();
                aclrAlt1Upper = aclr[10].ToString();
                string aclrAdjabsLower = aclr[5].ToString();
                string aclrAdjabsUpper = aclr[7].ToString();
                string aclrAdtabsLower = aclr[9].ToString();
                string aclrAdtabsUpper = aclr[11].ToString();
                Log("N9020A: ACLR measurement" + "\n"
                                + "ADJ lower: " + aclrAdjLower + "\n"
                                + "ADJ upper: " + aclrAdjUpper + "\n"
                                + "ALT lower: " + aclrAlt1Lower + "\n"
                                + "ALT upper: " + aclrAlt1Upper + "\n");
                double tmp1 = Math.Max(double.Parse(aclrAdjLower), double.Parse(aclrAdjUpper));
                double tmp2 = Math.Max(double.Parse(aclrAlt1Lower), double.Parse(aclrAlt1Upper));
                double tmp3 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp4 = Math.Max(double.Parse(aclrAdtabsLower), double.Parse(aclrAdtabsUpper));
                string[] result = new string[] { Math.Max(tmp1, tmp2).ToString(), Math.Max(tmp3, tmp4).ToString() };
                object[] n9020a_aclr = new object[] { aclr[1], aclr[5], aclr[7], aclr[4], aclr[6], aclr[9], aclr[11], aclr[8], aclr[10] };
                mainForm.DLCsvRecord(data, n9020a_aclr, result, "ACLR", TM, n9020a_aclr.Length, result.Length, port);
                Log(aclrAdjLower + "\n" + aclrAdjUpper + "\n" + aclrAlt1Lower + "\n" + aclrAlt1Upper);
            }
            catch
            {
                Log("ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadOBWSetup(string BW, string TDD_or_FDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(3000);
                SendCommand($@":MMEM:LOAD:STAT 'D:\Users\Instrument\Documents\NR5G\state\OBW\OBW_{BW}Mhz.state'");
                Log("load OBW measurement: OBW_" + BW + "Mhz");
            }
            catch
            {
                Log("Load OBW Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void OBWMeas(int port, out string OBWMeas)
        {
            OBWMeas = "ERROR";
            string[] data = new string[] { "Occupied Bandwidth" };
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(2000);
                n9020a.WriteString("READ:OBW?", true);
                object[] obw;
                obw = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                OBWMeas = obw[0].ToString();
                object[] n9020b_obw = new object[] { obw[0] };
                mainForm.DLCsvRecord(data, n9020b_obw, null, "OBW", "TM1.1", n9020b_obw.Length, 0, port);
                Log(OBWMeas);
            }
            catch
            {
                Log("Load OBW measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadNRTMSetup(string NRTMxx)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                SendCommand(":MMEM:LOAD:STAT " + "\"" + "D:/Users/Instrument/Documents/NR5G/state/NRTM/" + NRTMxx + "_7D1S2U" + ".state" + "\"");
                Log("Load NRTM measurement: " + NRTMxx);
            }
            catch
            {
                Log("Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }
        string[] evmN9020 = new string[13];
        public void EVMFreqErr(int port, string NRTM, out string[] evm, out string freqErr)
        {
            string[] data = new string[] { "Channel Power (dBm)", "Channel Power Active (dBm)", "EVM (%rms)", "EVM Peak (%)", "Freq Error (Hz)", "Symbol Clock Error (ppm)", "IQ Offset (dB)", "Time offset (s)" };

            evm = evmN9020;
            freqErr = "";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                n9020a.WriteString("READ:EVM?", true);                
                System.Threading.Thread.Sleep(4000);
                object[] n9020A_EVM;
                n9020A_EVM = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                string[] evmN9020A = new string[]
                {
                    n9020A_EVM[0].ToString(), 
                    n9020A_EVM[22].ToString(), 
                    n9020A_EVM[1].ToString(),
                    n9020A_EVM[2].ToString(),
                    n9020A_EVM[3].ToString(),
                    n9020A_EVM[4].ToString(),
                    n9020A_EVM[5].ToString(),
                    n9020A_EVM[6].ToString(),
                };
                evmN9020[10] = n9020A_EVM[0].ToString();
                if (NRTM == "NRTM_20")
                {
                    evmN9020[2] = n9020A_EVM[1].ToString(); //64QAM
                }
                else if (NRTM == "NRTM_20a")
                {
                    evmN9020[3] = n9020A_EVM[1].ToString(); //256QAM
                }
                else if (NRTM == "NRTM_31")
                {
                    evmN9020[2] = n9020A_EVM[1].ToString(); //64QAM
                }
                else if (NRTM == "NRTM_31a")
                {
                    evmN9020[3] = n9020A_EVM[1].ToString(); //256QAM
                }
                else if (NRTM == "NRTM_32")
                {
                    evmN9020[1] = n9020A_EVM[1].ToString(); //16QAM
                }
                else if (NRTM == "NRTM_33")
                {
                    evmN9020[0] = n9020A_EVM[1].ToString(); //QPSK
                }
                freqErr = n9020A_EVM[3].ToString();
                mainForm.DLCsvRecord(data, evmN9020A, null, "EVM_FreqError", NRTM, evmN9020A.Length, 0, port);
                Log("OFDM Power: " + evmN9020[10] + "\n" + "Freq " + freqErr + "\n" + "EVM: " + n9020A_EVM[1].ToString());
            }
            catch
            {
                Log("EVM Measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void OFDMPower(out string power)
        {
            power = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                n9020a.WriteString("READ:EVM?", true);
                System.Threading.Thread.Sleep(5000);
                object[] n9020A_Power;
                n9020A_Power = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = n9020A_Power[22].ToString();
            }
            catch
            {
                Log("EVM Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadTXPower()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                SendCommand(":MMEM:LOAD:STAT " + "\"" + "D:/Users/Instrument/Documents/NR5G/state/OSTP_POWER.state" + "\"");
                Log("Chanel Power measurement: ");
            }
            catch
            {
                Log("Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }

        internal void LoadTransONOFFSetup(string v1, string v2)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                SendCommand(":MMEM:LOAD:STAT " + "\"" + "D:/Users/Instrument/Documents/NR5G/state/ONOFFPOWER/TransmitONOFFpower.state" + "\"");
                Log("ONOFF Power measurement: ");
            }
            catch
            {
                Log("Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }

        internal void TransONOFFMeas(string mode, string TM, out string offPower, out string transPeriod, int port)
        {
            offPower = "";
            transPeriod = "";
            string[] data = new string[] { "ON Power", "OFF Power", "Max Power", "Min Power", "Ramp up time", "Ramp down time" };
            try
            {
                string OFF_Power = "";
                string Falling_Period = "";
                string Rising_Period = "";
                System.Threading.Thread.Sleep(5000);
                AutoScale();
                n9020a.WriteString("MEAS:PVT?", true);
                object[] n9020a_TransONOFF;
                n9020a_TransONOFF = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                offPower = Math.Round(double.Parse(n9020a_TransONOFF[7].ToString()), 3).ToString();
                Rising_Period = Math.Round(double.Parse(n9020a_TransONOFF[5].ToString())*1e6, 3).ToString();
                Falling_Period = Math.Round(double.Parse(n9020a_TransONOFF[6].ToString())*1e6, 3).ToString();
                string[] OnOff9020a = new string[]
                {
                    n9020a_TransONOFF[2].ToString(),
                    n9020a_TransONOFF[7].ToString(), 
                    n9020a_TransONOFF[8].ToString(),
                    n9020a_TransONOFF[9].ToString(),
                    n9020a_TransONOFF[5].ToString(),
                    n9020a_TransONOFF[6].ToString(),
                };
                double tmpTransperiod = -1;
                if(double.TryParse(Rising_Period,out double on_period))
                {
                    tmpTransperiod = Math.Max(on_period,tmpTransperiod);
                }
                if (double.TryParse(Falling_Period, out double off_period))
                {
                    if(off_period < 9.91e+37)
                    {
                        tmpTransperiod = Math.Max(off_period, tmpTransperiod);
                    }                    
                }
                //transPeriod = Math.Max(int.Parse(Rising_Period), int.Parse(Falling_Period)).ToString();
                transPeriod = (tmpTransperiod >= 0) ? tmpTransperiod.ToString() : "ERROR";

                string[] result = new string[] {offPower, transPeriod };
                mainForm.DLCsvRecord(data, OnOff9020a, result, mode, TM, OnOff9020a.Length, result.Length, port);
            }
            catch
            {
                Log("TransON/OFF measurement: FAIL", LogLevel.ERROR);
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
                System.Threading.Thread.Sleep(1);
                n9020a.WriteString("READ:EVM?", true);
                System.Threading.Thread.Sleep(5000);
                object[] n9020A_Power;
                n9020A_Power = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = n9020A_Power[22].ToString();
                string[] powerresult = new string[] { power, bbu.SHW_TX_GAIN(port.ToString(), out gain) };
                mainForm.DLCsvRecord(data, powerresult, null, "POWER", "NRTM1.1", powerresult.Length, 0, port);
                Log("OFDM Power: " + power);
            }
            catch
            {
                Log("OFDM Power measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadSEMSetup(string BW, string TDDFDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                SendCommand(":MMEM:LOAD:STAT " + "\"" + "D:/Users/Instrument/Documents/NR5G/state/SEM/SEM_" + BW + "Mhz.state" + "\"");
                Log("Load SEM measurement: SEM_" + BW + "_" + TDDFDD);
            }
            catch
            {
                Log("Load SEM Setup: FAIL", LogLevel.ERROR);

            }
        }
        public void SEMMeas(int port, string TM, out double absPwr1, out double absPwr2, out double absPwr3, out double semLimit)
        {
            absPwr1 = 0;
            absPwr2 = 0;
            absPwr3 = 0;
            semLimit = 1;
            string[] data = new string[] { "Start Freq", "Stop Freq", "Integ BW", "Lower Power (dBm)", "delta Limit(dB)", "Freq (Hz)", "Upper Power (dBm)", "delta Limit(dB)", "Freq (Hz)", "Note", "Worst Power", "Worst delta", "Freq (Hz)" };
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            try
            {
                n9020a.WriteString("READ:SEM?", true);
                System.Threading.Thread.Sleep(10000);
                object[] n9020a_SEM;
                #region Convertdata
                n9020a_SEM = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                string limit1 = n9020a_SEM[70].ToString();  //Minimum margin from limit line on the negative offset A
                string limit2 = n9020a_SEM[71].ToString();  //Minimum margin from limit line on the positive offset A
                string limit3 = n9020a_SEM[72].ToString();  //Minimum margin from limit line on the negative offset B
                string limit4 = n9020a_SEM[73].ToString();  //Minimum margin from limit line on the positive offset B
                string limit5 = n9020a_SEM[74].ToString();
                string limit6 = n9020a_SEM[75].ToString();
                // limitmax
                string absPower1 = n9020a_SEM[13].ToString(); //Abs peak power on the negative offset A(dBm)
                string absPower2 = n9020a_SEM[18].ToString(); //Abs peak power on the positive offset A (dBm)
                string absPower3 = n9020a_SEM[23].ToString();
                string absPower4 = n9020a_SEM[28].ToString();
                string absPower5 = n9020a_SEM[33].ToString();
                string absPower6 = n9020a_SEM[38].ToString();
                //Freq
                string FreqNegative1 = n9020a_SEM[14].ToString();
                string FreqPositive1 = n9020a_SEM[19].ToString();
                int Freq1 = Math.Max(Math.Abs(int.Parse(FreqNegative1)), Math.Abs(int.Parse(FreqPositive1)));
                string FreqNegative2 = n9020a_SEM[24].ToString();
                string FreqPositive2 = n9020a_SEM[29].ToString();
                int Freq2 = Math.Max(Math.Abs(int.Parse(FreqNegative2)), Math.Abs(int.Parse(FreqPositive2)));
                string FreqNegative3 = n9020a_SEM[34].ToString();
                string FreqPositive3 = n9020a_SEM[39].ToString();
                int Freq3 = Math.Max(Math.Abs(int.Parse(FreqNegative3)), Math.Abs(int.Parse(FreqPositive3)));
                int[] Freq = new int[] { Freq1, Freq2, Freq3 };
                #endregion
                Log(absPower1 + "\n" + absPower2 + "\n" + absPower3 + "\n" + absPower4 + "\n" + absPower5 + "\n" + absPower6 + "\n");
                double tmp1 = Math.Max(double.Parse(limit1), double.Parse(limit2));
                double tmp2 = Math.Max(double.Parse(limit3), double.Parse(limit4));
                double tmp3 = Math.Max(double.Parse(limit5), double.Parse(limit6));
                absPwr1 = Math.Max(double.Parse(absPower1), double.Parse(absPower2));
                absPwr2 = Math.Max(double.Parse(absPower3), double.Parse(absPower4));
                absPwr3 = Math.Max(double.Parse(absPower5), double.Parse(absPower6));
                double tmp5 = Math.Max(absPwr1, absPwr2);
                semLimit = Math.Max(tmp5, absPwr3);


                for (int i = 0; i < Freq.Length; i++)
                {
                    int startFreq = 50000;
                    int stopFreq = 5050000;
                    int IntegBW = 100000;
                    string worstabs = "";
                    string worstdelta = "";
                    string worstfreq = "";
                    string Note;
                    if ((50000 <= Math.Abs(Freq[i])) && Math.Abs(Freq[i]) <= 5050000)
                    {
                        Note = "OBUE_1";
                        worstabs = absPwr1.ToString();
                        worstdelta = tmp1.ToString();
                        if (Math.Abs(double.Parse(absPower1)) > Math.Abs(double.Parse(absPower2)))
                        {
                            worstfreq = FreqNegative1;
                        }
                        else
                        {
                            worstfreq = FreqPositive1;
                        }
                        string[] result = new string[] { worstabs, worstdelta, worstfreq };
                        string[] SEMvalue = new string[] { startFreq.ToString(), stopFreq.ToString(), IntegBW.ToString(), absPower1, limit1, FreqNegative1, absPower2, limit2, FreqPositive1, Note };
                        mainForm.DLCsvRecord(data, SEMvalue, result, "OBUE", TM, SEMvalue.Length, result.Length, port);
                    }
                    else if (5050000 < Math.Abs(Freq[i]) && Math.Abs(Freq[i]) < 10500000)
                    {
                        startFreq = 5050000;
                        stopFreq = 10050000;
                        Note = "OBUE_2";
                        worstabs = absPwr2.ToString();
                        worstdelta = tmp2.ToString();
                        if (Math.Abs(double.Parse(absPower3)) > Math.Abs(double.Parse(absPower4)))
                        {
                            worstfreq = FreqNegative2;
                        }
                        else
                        {
                            worstfreq = FreqPositive2;
                        }
                        string[] result = new string[] { worstabs, worstdelta, worstfreq };
                        string[] SEMvalue = new string[] { startFreq.ToString(), stopFreq.ToString(), IntegBW.ToString(), absPower3, limit3, FreqNegative2, absPower4, limit4, FreqPositive2, Note };
                        mainForm.DLCsvRecord(data, SEMvalue, result, "OBUE", TM, SEMvalue.Length, result.Length, port);
                    }
                    else if (10500000 <= Math.Abs(Freq[i]))
                    {
                        startFreq = 10500000;
                        stopFreq = 100000000;
                        IntegBW = 1000000;
                        Note = "OBUE_3";
                        worstabs = absPwr3.ToString();
                        worstdelta = tmp1.ToString();
                        if (Math.Abs(double.Parse(absPower5)) > Math.Abs(double.Parse(absPower6)))
                        {
                            worstfreq = FreqNegative3;
                        }
                        else
                        {
                            worstfreq = FreqPositive3;
                        }
                        string[] result = new string[] { worstabs, worstdelta, worstfreq };
                        string[] SEMvalue = new string[] { startFreq.ToString(), stopFreq.ToString(), IntegBW.ToString(), absPower5, limit5, FreqNegative3, absPower6, limit6, FreqPositive3, Note };
                        mainForm.DLCsvRecord(data, SEMvalue, result, "OBUE", TM, SEMvalue.Length, result.Length, port);
                    }
                }

            }
            catch
            {
                Log("SEM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadIMDACLR(string offset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                SendCommand(":MMEM:LOAD:STAT " + "\"" + "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_ACLR/IMD_ACLR_" + offset + "MHz.state" + "\"");
                Log("D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_ACLR/IMD_ACLR_" + offset + "MHz.state");
            }
            catch
            {
                Log("Load ACLR Setup: FAIL", LogLevel.ERROR);
            }
        }

        public void IMDACLR(string offset, out double aclr)
        {
            aclr = 0;
            double tmp, tmp1;
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                n9020a.WriteString("READ:ACP?", true);
                object[] obj;
                obj = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                if (offset == "-25" || offset == "-15" || offset == "-5")
                {
                    aclr = Math.Min(Math.Abs(double.Parse(obj[4].ToString())), Math.Abs(double.Parse(obj[14].ToString())));
                    Log(obj[4] + "\n" + obj[14]);
                }
                else if (offset == "25" || offset == "15" || offset == "5")
                {
                    aclr = Math.Min(Math.Abs(double.Parse(obj[4].ToString())), Math.Abs(double.Parse(obj[14].ToString())));
                    Log(obj[4] + "\n" + obj[14]);
                }
            }
            catch (Exception)
            {
                Log("IMD ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadIMDSEM(string offset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                SendCommand(":MMEM:LOAD:STAT " + "\"" + "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_SEM/IMD_SEM_OFF" + offset + "MHz.state" + "\"");
                Log("Load File: " + "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_SEM/IMD_SEM_OFF" + offset + "MHz.state");
            }
            catch
            {
                Log("Load Transmitter Spurious Emission Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void IMDSEM(string offset, out string semLimit)
        {
            semLimit = "ERROR";
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            try
            {
                string limit1;
                string limit2;
                string limit3;
                string limit4;
                string limit5;
                string limit6;
                string limit7;
                string limit8;
                System.Threading.Thread.Sleep(3000);
                n9020a.WriteString("READ:SEM?", true);
                System.Threading.Thread.Sleep(4000);
                object[] n9020a_SEM;
                n9020a_SEM = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                limit1 = n9020a_SEM[70].ToString();
                limit2 = n9020a_SEM[71].ToString();
                limit3 = n9020a_SEM[72].ToString();
                limit4 = n9020a_SEM[73].ToString();
                limit5 = n9020a_SEM[74].ToString();
                limit6 = n9020a_SEM[75].ToString();
                limit7 = n9020a_SEM[76].ToString();
                limit8 = n9020a_SEM[77].ToString();
                Log(limit1 + "\n" + limit2 + "\n" + limit3 + "\n" + limit4 + "\n" + limit5 + "\n" + limit6 + "\n" + limit7 + "\n" + limit8);
                string tmp1 = (Math.Max(double.Parse(limit1), double.Parse(limit2))).ToString();
                string tmp2 = (Math.Max(double.Parse(limit3), double.Parse(limit4))).ToString();
                string tmp3 = (Math.Max(double.Parse(limit5), double.Parse(limit6))).ToString();
                string tmp4 = (Math.Max(double.Parse(limit7), double.Parse(limit8))).ToString();
                string tmp5 = (Math.Max(double.Parse(tmp1), double.Parse(tmp2))).ToString();
                string tmp6 = (Math.Max(double.Parse(tmp3), double.Parse(tmp4))).ToString();
                semLimit = (Math.Max(double.Parse(tmp5), double.Parse(tmp6))).ToString();
            }
            catch
            {
                Log("SEM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadIMDTSE(string mode, string Freq)
        {
            string filename = "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_TSE/IMD_TSE_DC_COUPLE";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                if (mode == "DC")
                {
                    filename = "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_TSE/IMD_TSE_DC_COUPLE";
                }
                else if (mode == "AC")
                {
                    if (2496000000 <= double.Parse(Freq) && double.Parse(Freq) <= 2690000000)
                    {
                        filename = "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_TSE/IMD_TSE_AC_COUPLE_n41_DL_1H";
                    }
                    else if (3300000000 <= double.Parse(Freq) && double.Parse(Freq) <= 3800000000)
                    {
                        filename = "D:/Users/Instrument/Documents/NR5G/state/IMD/IMD_TSE/IMD_TSE_AC_COUPLE_n78_DL_1H";
                    }
                }
                SendCommand(":MMEM:LOAD:STAT " + "\"" + filename + "\"");
                Log("Load: " + filename);
            }
            catch
            {
                Log("Load Transmitter Spurious Emission Setup: FAIL", LogLevel.ERROR);
            }
        }

        //12.6.2024 - test Setup State bai do TSE
        public void SetupIMDTSE(SpuriousRange rng)
        {
            try
            {
                //mode preset
                SendCommand(":SYST:PRES");
                //Set Spectrum Swept SA 
                SendCommand(":INST:CONF:SA:SAN");
                //set parameters                
                SendCommand($":SENS:BAND {rng.ResBW}");           //set ResBW            
                SendCommand($":SENS:BAND:VID {rng.VidBW}");     //set VidBW            
                SendCommand($"FREQ:STOP {rng.StopFreq}");          //set stop freq                
                SendCommand($":FREQ:STAR {rng.StartFreq}");    //set start freq
                // set RF Coupling
                SendCommand($":INP:COUP {rng.RFCoupling}");
                // set trace1 type as average 
                SendCommand($":TRAC1:TYPE {rng.TraceType}");
                // set detector as RMS
                SendCommand($":DET:TRAC1 {rng.Detector}");
                //set limit value
                SendCommand(":CALC:LLIN1:DISP ON"); //display limit line 1
                SendCommand(":CALC:LLIN:ALL:DEL"); //delete all previous limit first
                SendCommand($":CALC:LLIN1:DATA {rng.StartFreq}, {rng.ABSStartLim}, 0, {rng.StopFreq}, {rng.ABSStopLim}, 1");

                //load complex correction
                SendCommand($":MMEM:LOAD:CCOR 1, {rng.CorrectionFile}");
                SendCommand(":CCOR:CSET:SEL 1");
                SendCommand(":CCOR:CSET ON"); //turn on correction
                SendCommand("SENS:CORR:CSET:ALL ON"); //apply correction
                //turn on marker table 
                System.Threading.Thread.Sleep(1000); //can thiet
                SendCommand("CALC:MARK:TABL ON");
                SendCommand(":CALC:MARK1:MODE POS");

                //set Average/hold number
                System.Threading.Thread.Sleep(1000); //can thiet
                SendCommand($":SENS:AVER:COUN {rng.AveNum}");
                SendCommand(":SENS:AVER:CLE"); //start new counting

                //waiting for measurement stable
                System.Threading.Thread.Sleep(5000); //waiting for counting up to AverNum
                SendCommand("CALC:MARK1:MAX"); //set marker 1 peak search
                System.Threading.Thread.Sleep(1000);
                //change to single sweep
                SingleSweepOnOff(0);
                //get result                
                n9020a.WriteString("CALC:MARK1:X? ", true);
                object[] tseX = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                n9020a.WriteString("CALC:MARK1:Y? ", true);
                object[] tseY = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                n9020a.WriteString(":CALC:TRAC1:FAIL? ", true);
                object[] tsePF = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                rng.ResultFreq = tseX[0].ToString();
                rng.ResultLevel = tseY[0].ToString();
                rng.ResultPassFail = tsePF[0].ToString() =="0" ? "PASS" : "FAIL";

                //save screenshoot + data
                string folder = $@"D:\VHTSavedData\{DateTime.Now.ToString("ddMMyyyy")}\Spurious"; //test- update later
                string dataPath = $@"{folder}\Suprious_2500MHz_P1_{rng.Name}_{DateTime.Now.ToString("HHmmss")}.csv";
                string imagePath = $@"{folder}\Suprious_2500MHz_P1_{rng.Name}_{DateTime.Now.ToString("HHmmss")}.png";
                SaveCSVFile(dataPath);
                TakeScreenshot(imagePath);
                //change to continuos sweep
                SingleSweepOnOff(1);
            }
            catch
            {
                Log("Load Transmitter Spurious Emission Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void IMDTSEMMeas(string mode, out double amp1, out double amp2, out double amp3)
        {
            amp1 = 0;
            amp2 = 0;
            amp3 = 0;
            //amp4 = 0;
            try
            {
                double tmp5;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                n9020a.WriteString(":READ:SPUR?", true);
                System.Threading.Thread.Sleep(5000);
                object[] transmitterSpuriousEmission;
                transmitterSpuriousEmission = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                if (mode == "AC")
                {
                    amp1 = double.Parse(transmitterSpuriousEmission[4].ToString());
                    amp2 = double.Parse(transmitterSpuriousEmission[10].ToString());
                    amp3 = double.Parse(transmitterSpuriousEmission[16].ToString());
                    //amp4 = double.Parse(transmitterSpuriousEmission[22].ToString()); 
                    Log(transmitterSpuriousEmission[4].ToString() + "\n" + transmitterSpuriousEmission[10].ToString() + "\n" + transmitterSpuriousEmission[16].ToString());
                }
                else if (mode == "DC")
                {
                    amp1 = double.Parse(transmitterSpuriousEmission[4].ToString());
                    amp2 = double.Parse(transmitterSpuriousEmission[10].ToString());
                    Log(transmitterSpuriousEmission[4].ToString() + "\n" + transmitterSpuriousEmission[10].ToString());
                }
            }
            catch
            {
                Log("Spurious emission measurement: FAIL", LogLevel.ERROR);
            }
        }

        internal void ACLRPower(int port, string TM, out double intraACLR, string mode)
        {
            string[] data = new string[] { "TXPOWER", "LOWER ABS ADJ", "UPPER ABS ADJ", "LOWER ADJ", "UPPER ADJ", "LOWER ABS ALT", "UPPER ABS ALT", "LOWER ALT", "UPPER ALT", "ACLR", "Worst Abs Power" };
            intraACLR = 0;
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                n9020a.WriteString("READ:ACP?", true);
                System.Threading.Thread.Sleep(2000);
                object[] aclr;
                aclr = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                string power = aclr[1].ToString();
                string aclrAdjLower = aclr[4].ToString();
                string aclrAdjUpper = aclr[6].ToString();
                string aclrAlt1Lower = aclr[8].ToString();
                string aclrAlt1Upper = aclr[10].ToString();
                string aclrAdjabsLower = aclr[5].ToString();
                string aclrAdjabsUpper = aclr[7].ToString();
                string aclrAdtabsLower = aclr[9].ToString();
                string aclrAdtabsUpper = aclr[11].ToString();
                Log("ACLR measurement" + "\n"
                                + "ADJ lower: " + aclrAdjLower + "\n"
                                + "ADJ upper: " + aclrAdjUpper + "\n"
                                + "ALT lower: " + aclrAlt1Lower + "\n"
                                + "ALT upper: " + aclrAlt1Upper + "\n");
                double WorstabsACLR = CompareACLR(aclrAdjabsLower, aclrAdjabsUpper, aclrAdtabsLower, aclrAdtabsUpper);
                intraACLR = CompareACLR(aclrAdjLower, aclrAdjUpper, aclrAlt1Lower, aclrAlt1Upper);
                string[] result = new string[] { intraACLR.ToString(), WorstabsACLR.ToString() };
                object[] n9020a_aclr = new object[] { aclr[1], aclr[5], aclr[7], aclr[4], aclr[6], aclr[9], aclr[11], aclr[8], aclr[10] };
                mainForm.DLCsvRecord(data, n9020a_aclr, result, mode, TM, n9020a_aclr.Length, result.Length, port);
                Log(aclrAdjLower + "\n" + aclrAdjUpper + "\n" + aclrAlt1Lower + "\n" + aclrAlt1Upper);
            }
            catch
            {
                Log("ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        private double CompareACLR(string param1, string param2, string param3, string param4)
        {
            double tmp1 = Math.Max(double.Parse(param1), double.Parse(param2));
            double tmp2 = Math.Max(double.Parse(param3), double.Parse(param4));
            double tmp3 = Math.Max(tmp1, tmp2);
            return tmp3;
        }
        
        public void SingleSweepOnOff(int onOff)
        {
            try
            {
                if (onOff == 0)
                {
                    SendCommand("INIT:CONT OFF");
                    System.Threading.Thread.Sleep(5000); //time for VSA change to Ready
                    Log("Changed to Single Sweep");
                }
                else if (onOff == 1)
                {
                    SendCommand($"INIT:CONT ON");
                    //System.Threading.Thread.Sleep(2000); //time for VSA change to Ready
                    Log("Changed to Continous Sweep");
                }
                else
                {
                    Log("Init SingleSweep/ContinousSweep parameter invalid", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log("Change Single Sweep mode got an error! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void TakeScreenshot(string filePathVSA)
        {
            try
            {
                SendCommand($":MMEM:STOR:SCR \"{filePathVSA}\"");

                Log($"Screenshot saved to: {filePathVSA}");
                //save to remote PC
                TransferFileToPC(filePathVSA);
            }
            catch (Exception ex)
            {
                Log("Save Screenshot  => FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void SaveCSVFile(string filePathVSA)
        {
            try
            {
                SendCommand($":MMEM:STOR:TRAC:DATA TRACE1, \"{filePathVSA}\""); //save the marker table
                Log($"Saved measurement data to: {filePathVSA}");
                //save to remote PC
                TransferFileToPC(filePathVSA);
            }
            catch (Exception ex)
            {
                Log("Save data => FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void TransferFileToPC(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            n9020a.WriteString($":MMEM:DATA? \"{filename}\"");
            byte[] data = (byte[])n9020a.ReadIEEEBlock(Ivi.Visa.Interop.IEEEBinaryType.BinaryType_UI1);
            File.WriteAllBytes(filename, data);

        }

        public void LoadSpurSetup(string coupling, string mode, string Freq, string RBW)
        {
            try
            {
                string filename = "";
                switch (mode)
                {
                    case "General":
                        if (double.Parse(Freq) < 2600000000)
                        {
                            if (coupling == "AC")
                            {
                                filename = "D:/Users/Instrument/Documents/NR5G/state/Spurious/General/SPUR_CatA_N41_1C_AC_B";
                            }
                            else
                            {
                                filename = "D:/Users/Instrument/Documents/NR5G/state/Spurious/General/SPUR_CatA_N41_1C_DC_B";
                            }
                        }
                        else
                        {
                            filename = "D:/Users/Instrument/Documents/NR5G/state/General/SPUR_CatA_N41_1C_DC_T";
                        }
                        break;
                    case "Protection":
                        filename = "D:/Users/Instrument/Documents/NR5G/state/Protection/SpurProtection";
                        break;
                    case "Co-existance":
                        filename = "D:/Users/Instrument/Documents/NR5G/state/SpurCoex/SpurCoex_RBW" + RBW;
                        break;
                    case "Co-location":
                        if (double.Parse(Freq) < 2600000000)
                        {
                            filename = "D:/Users/Instrument/Documents/NR5G/state/Co_location/SpurColo_B";
                        }
                        else
                        {
                            filename = "D:/Users/Instrument/Documents/NR5G/state/Co_location/SpurColo_T";
                        }
                        break;
                    case "Rx-Spurious":
                        filename = "D:/Users/Instrument/Documents/NR5G/state/RSE/Rx_Spurious";
                        break;
                }
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1000);
                n9020a.WriteString($"MMEM:LOAD:STAT 1,'{filename}'", true);
                log.Log($"N9020A load Spurious measurement: {filename}", LogLevel.INFO);
            }
            catch(Exception ex) 
            {
                log.Log(ex.Message, LogLevel.ERROR);
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
                System.Threading.Thread.Sleep(2000);
                n9020a.WriteString("READ:SPUR?", true);// Queries the peak list of the spurious emission measurement 
                //convert data
                object[] Spur;
                Spur = (object[])n9020a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                while (Spur.Length % 7 == 0)
                {
                    int n = Spur.Length / 7;
                    string[] range = new string[n];
                    string[] rangelow = new string[n];
                    string[] rangehigh = new string[n];
                    string[] rbw = new string[n];
                    string[] freq = new string[n];
                    double[] abs = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        range[i] = Spur[i * 6 + 1].ToString();
                        //rbw[i] = Spur[i * 7 + 3].ToString();
                        freq[i] = Spur[i * 6 + 4].ToString();
                        //abs[i] = double.Parse(Spur[i * 7 + 5].ToString());
                        spur[i] = double.Parse(Spur[i * 6 + 4].ToString());
                        log.Log($"Delta limit range {i + 1} : {spur[i]}");
                        object[] value = new object[] { range[i], freq[i], spur[i] };
                        mainForm.DLCsvRecord(data, value, null, "Spurious", "TM1.1", value.Length, 0, port);
                    }
                    break;
                }
            }
            catch
            {
                log.Log("N9020A Spurious emission measurement: FAIL", LogLevel.ERROR);
            }
        }

        internal void Restart()
        {
            try
            {
                n9020a.WriteString($":INIT:IMM", true);
                n9020a.WriteString($":INIT:REST", true);
            }
            catch (Exception ex)
            {
                log.Log(ex.Message, LogLevel.ERROR);
            }
        }
    }
}
