using System;
using System.ComponentModel.Design;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms.DataVisualization.Charting;

namespace _5GAutoTool
{
    public class M9410A
    {

        public Form5GAT mainForm;
        public string Name = "M9410";
        public M9410A(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        int rm = 0;
        int xApp;
        //Ivi.Visa.Interop.ResourceManager rm1 = new Ivi.Visa.Interop.ResourceManager();
        //Ivi.Visa.Interop.FormattedIO488 m9410a = new Ivi.Visa.Interop.FormattedIO488();
        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "M9410A");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }

        public void Connect(string hislipaddr, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            try
            {
                Manufacturer = "ERROR";
                Model = "ERROR";
                Serial = "ERROR";
                string queryResult;
                //Below two commands are used to open connection to the M941x initialized X-series SA application display
                //Need to use "TCPIP0::localhost::hislip{0}::INSTR" address. This is a Hislip address, not the chassis address.
                //Address related explanation please refer to M941x's programmer's guide.
                AgVisa32.viOpenDefaultRM(out rm);
                Log(rm.ToString());
                AgVisa32.viOpen(rm, "TCPIP0::localhost::hislip" + int.Parse(hislipaddr) + "::INSTR", 0, 0, out xApp);
                AgVisa32.viPrintf(xApp, ":SYST:ERR:VERB ON;\n");
                AgVisa32.viPrintf(xApp, Cmd + "\n");
                AgVisa32.viRead(xApp, out queryResult, 1024);
                Log(queryResult);
                string[] str = new string[3];
                str = queryResult.Split(',');
                Manufacturer = str[0];
                Model = str[1];
                Serial = str[2];
                Preset();
                Log("Connection: SUCCESS! ", LogLevel.SUCCESS);                
            }
            catch (Exception ex)
            {
                Log("Connection: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (rm == 256)
                {
                    AgVisa32.viClose(rm);
                    rm = 0;
                    Log("Disconnected! " , LogLevel.WARN);
                }
            }
            catch (Exception)
            {

            }
        }
        public void Preset()
        {
            string cmd = "SYST:PRES;*OPC?";
            AgVisa32.viPrintf(xApp, $"{cmd}\n");
            Log($"[Preset] SEND: {cmd}");
            //AgVisa32.viPrintf(xApp, "*OPC?\n");
        }
        public void ResetStatus()
        {
            AgVisa32.viPrintf(xApp, "*CLS\n");
            Log($"[ClearStatus] SEND: *CLS");
        }

        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                string queryResult;
                AgVisa32.viPrintf(xApp, Cmd);
                object[] tmp;
                AgVisa32.viRead(xApp, out queryResult, 1024);
                tmp = queryResult.Split(',');
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception ex)
            {
                Log("M9410A connect: FAIl!" + ex.Message, LogLevel.ERROR);
            }
        }
        /// <summary>
        /// USE to wait for the completion of commands that take little or moderate time to execute
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="response"></param>
        public void SendCommand(string cmd, out string response)
        {
            response = null;
            try
            {
                cmd = cmd.Trim();

                //AgVisa32.viPrintf(xApp, "*CLS\n");
                //Log($"[ClearErrors] SEND: *CLS");

                if (!cmd.Contains("*OPC?") && !cmd.Contains("?"))
                {
                    cmd += ";*OPC?";
                }
                AgVisa32.viPrintf(xApp, $"{cmd}\n");
                Log($"[M9410A]: [Querry] SEND: {cmd}");
                AgVisa32.viRead(xApp, out response, 1024);
                Log($"[M9410A]: [Querry] RECV: {response}");

                //AgVisa32.viPrintf(xApp, "*ESR?\n");
                //Log($"[M9410A]: [CheckErrors] SEND: *ESR?");
                //Log($"[M9410A]: [CheckErrors] RECV: {AgVisa32.viRead(xApp, out response, 1024)}");
                //if(error != "0")
                //{
                //    Log($"[N9020A]: [CheckErrors] Error Message: ", LogLevel.ERROR);
                //}
            }
            catch (Exception ex)
            {
                Log($"Send command:{cmd} FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void LoadCommand(string cmd)
        {
            try
            {

                cmd = cmd.Trim();
                string ESRvalue="";
                AgVisa32.viPrintf(xApp, "*CLS\n");
                Log($"[M9410A]: [ClearErrors] SEND: *CLS");            

                AgVisa32.viPrintf(xApp, $"{cmd}\n");
                Log($"[Command] SEND: {cmd}");

                //waiting
                AgVisa32.viPrintf(xApp, $"*OPC\n");
                Log($"[Command] SEND: *OPC");

                AgVisa32.viPrintf(xApp, "*ESR?\n");
                Log($"[M9410A]: [CheckErrors] SEND: *ESR?");

                while (ESRvalue != "0\n")
                {
                    AgVisa32.viPrintf(xApp, "*ESR?\n");
                    AgVisa32.viRead(xApp, out ESRvalue, 1024);
                    System.Threading.Thread.Sleep(100);
                }
                
                Log($"[M9410A]: [CheckErrors] RECV: {ESRvalue}");

                
                //if(error != "0")
                //{
                //    Log($"[N9020A]: [CheckErrors] Error Message: ", LogLevel.ERROR);
                //}
            }
            catch (Exception ex)
            {
                Log($"Send command:{cmd} FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        // This d=function for RF input frequency
        public void SetFreqCent(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":CCAR:REF {freq} Hz";
                SendCommand(cmd, out string response);
                //AgVisa32.viPrintf(xApp, ":CCAR:REF " + freq + " Hz \n");//Set Frequency
                //Log("M9410A select center frequency: " + freq);
            }
            catch (Exception ex)
            {
                Log("M9410A select center frequency: FAIL! " + ex.Message);
            }
        }
        // This function for RF output frequency
        public void SetFreqOut(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":SOUR:FREQ {freq}";
                SendCommand(cmd, out string response);
                //AgVisa32.viPrintf(xApp, ":SOUR:FREQ " + freq + "\n");//Set Frequency
                //Log("M9410A setup output frequency: " + freq);
            }
            catch (Exception ex)
            {
                Log("M9410A setup output frequency: FAIL!" + ex.Message, LogLevel.ERROR);
            }
        }
        public void SetAtt(string att)
        {
            try
            {
                string queryResult;
                string cmd = $":CORR:BTS:GAIN -{att}";
                SendCommand(cmd, out string response);
                //AgVisa32.viPrintf(xApp, ":CORR:BTS:GAIN -" + att + " \n");  //:CORRection:BTS[:RF]:GAIN <rel_ampl>
                //System.Threading.Thread.Sleep(200);
                //Log("M9410A ref lever Offset: " + att);
            }
            catch (Exception ex)
            {
                Log("M9410A sets an external gain: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void AutoScale()
        {
            try
            {
                string queryResult;
                string cmd = $":POW:RANG:OPT IMM";
                LoadCommand(cmd);
                //System.Threading.Thread.Sleep(500);
                //AgVisa32.viPrintf(xApp, ":POW:RANG:OPT IMM \n");  //[:SENSe]:POWer[:RF]:RANGe:OPTimize IMMediate
                //System.Threading.Thread.Sleep(500);
                //Log("M9410A Scaled");
            }
            catch (Exception ex)
            {
                Log("M9410A AutoScaled: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void LoadACLRSetup(string BW, string TDDFDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/ACLR/ACLR_{BW}Mhz_{TDDFDD}.state'";
                SendCommand(cmd, out string response);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/ACLR/ACLR_" + BW + "Mhz_" + TDDFDD + ".state" + "' \n ");
                //Log("M9410A  load ACLR measurement: ACLR_" + BW + "_" + TDDFDD);
            }
            catch (Exception ex)
            {
                Log("M9410A Load ACLR Setup: FAIL! " +ex.Message, LogLevel.ERROR);
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
                string queryResult;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = "READ:ACP?";
                SendCommand(cmd, out queryResult);
                //AgVisa32.viPrintf(xApp, "READ:ACP? \n");                
                //System.Threading.Thread.Sleep(8000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] aclr = queryResult.Split(',');
                power = aclr[1].ToString();
                aclrAdjLower = aclr[4].ToString();
                aclrAdjUpper = aclr[6].ToString();
                aclrAlt1Lower = aclr[8].ToString();
                aclrAlt1Upper = aclr[10].ToString();
                string aclrAdjabsLower = aclr[5].ToString();
                string aclrAdjabsUpper = aclr[7].ToString();
                string aclrAdtabsLower = aclr[9].ToString();
                string aclrAdtabsUpper = aclr[11].ToString();
                Log("M9410A: ACLR measurement" + "\n"
                                + "ADJ lower: " + aclrAdjLower + "\n"
                                + "ADJ upper: " + aclrAdjUpper + "\n"
                                + "ALT lower: " + aclrAlt1Lower + "\n"
                                + "ALT upper: " + aclrAlt1Upper + "\n");
                double tmp1 = Math.Max(double.Parse(aclrAdjLower), double.Parse(aclrAdjUpper));
                double tmp2 = Math.Max(double.Parse(aclrAlt1Lower), double.Parse(aclrAlt1Upper));
                double tmp3 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp4 = Math.Max(double.Parse(aclrAdtabsLower), double.Parse(aclrAdtabsUpper));
                string[] result = new string[] { Math.Max(tmp1, tmp2).ToString(), Math.Max(tmp3, tmp4).ToString() };
                object[] m9410a_aclr = new object[] { aclr[1], aclr[5], aclr[7], aclr[4], aclr[6], aclr[9], aclr[11], aclr[8], aclr[10] };
                mainForm.DLCsvRecord(data, m9410a_aclr, result, "ACLR", TM, m9410a_aclr.Length, result.Length, port);
                Log(aclrAdjLower + "\n" + aclrAdjUpper + "\n" + aclrAlt1Lower + "\n" + aclrAlt1Upper);
            }
            catch (Exception e)
            {
                Log("M9410A ACLR Measurement: FAIL " + e, LogLevel.ERROR);
            }
        }

        public void LoadOBWSetup(string BW, string TDD_or_FDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/OBW/OBW_{BW}Mhz.state'";
                LoadCommand(cmd);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "'" + "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/OBW_" + BW + "Mhz.state" + "' \n");
                //Log("M9410A  load OBW measurement: OBW_" + BW + "Mhz");
                //bool Flag = true;
            }
            catch (Exception)
            {
                Log("M9410A Load OBW Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void OBWMeas(int port, out string OBWMeas)
        {
            OBWMeas = "ERROR";
            string[] data = new string[] { "Occupied Bandwidth" };
            try
            {
                string queryResult;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $"READ:OBW?";
                SendCommand(cmd, out queryResult);
                //AgVisa32.viPrintf(xApp, "READ:OBW? \n");
                //System.Threading.Thread.Sleep(5000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] n9020b_obw = queryResult.Split(',');
                OBWMeas = n9020b_obw[0].ToString();
                mainForm.DLCsvRecord(data, n9020b_obw, null, "OBW", "TM1.1", n9020b_obw.Length, 0, port);
                Log(OBWMeas);
            }
            catch (Exception)
            {
                Log("M9410A reading OBW measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadNRTMSetup(string NRTMxx)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/NRTM/{NRTMxx}_7D1S2U.state'";
                LoadCommand(cmd);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/NRTM/" + NRTMxx + "_7D1S2U" + ".state" + "'\n");
                //System.Threading.Thread.Sleep(10000);
                //Log("M9410A load " + NRTMxx + " SUCCESSFULLY");
            }
            catch (Exception e)
            {
                Log("M9410A load NRTM measurement: FAIL" + e.Message, LogLevel.ERROR);
            }
        }
        string[] evmM9410a = new string[13];
        public void EVMFreqErr(int port, string NRTM, out string[] evm, out string freqErr)
        {
            string[] data = new string[] { "Channel Power (dBm)", "Channel Power Active (dBm)", "EVM (%rms)", "EVM Peak (%)", "Freq Error (Hz)", "Symbol Clock Error (ppm)", "IQ Offset (dB)", "Time offset (s)" };
            evm = evmM9410a;
            freqErr = "";
            try
            {
                string queryResult;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(1);
                string cmd = $"READ:EVM?";
                SendCommand(cmd, out queryResult);
                //AgVisa32.viPrintf(xApp, "READ:EVM? \n");
                //System.Threading.Thread.Sleep(5000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] m9410A_evm = queryResult.Split(',');
                evmM9410a[10] = m9410A_evm[0].ToString();
                string[] evmM9410A = new string[]
                {
                    m9410A_evm[0].ToString(),
                    m9410A_evm[22].ToString(),
                    m9410A_evm[1].ToString(),
                    m9410A_evm[2].ToString(),
                    m9410A_evm[3].ToString(),
                    m9410A_evm[4].ToString(),
                    m9410A_evm[5].ToString(),
                    m9410A_evm[6].ToString(),
                };
                if (NRTM == "NRTM_20")
                {
                    evmM9410a[2] = m9410A_evm[1].ToString(); //64QAM
                }
                else if (NRTM == "NRTM_20a")
                {
                    evmM9410a[3] = m9410A_evm[1].ToString(); //256QAM
                }
                else if (NRTM == "NRTM_31")
                {
                    evmM9410a[2] = m9410A_evm[1].ToString(); //64QAM
                }
                else if (NRTM == "NRTM_31a")
                {
                    evmM9410a[3] = m9410A_evm[1].ToString(); //256QAM
                }
                else if (NRTM == "NRTM_32")
                {
                    evmM9410a[1] = m9410A_evm[1].ToString(); //16QAM
                }
                else if (NRTM == "NRTM_33")
                {
                    evmM9410a[0] = m9410A_evm[1].ToString(); //QPSK
                }
                freqErr = m9410A_evm[3].ToString();
                mainForm.DLCsvRecord(data, evmM9410A, null, "EVM_FreqError", NRTM, evmM9410A.Length, 0, port);
                Log("OFDM Power: " + evmM9410a[10] + "\n" + "Freq " + freqErr + "\n" + "EVM: " + m9410A_evm[1].ToString());
            }
            catch (Exception)
            {
                Log("M9410A EVM Measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void OFDMPower(out string power)
        {
            power = "ERROR";
            try
            {
                string queryResult;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $"READ:EVM?";
                SendCommand(cmd, out queryResult);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, "READ:EVM? \n");
                //System.Threading.Thread.Sleep(4000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] m9410A_Power = queryResult.Split(',');
                power = m9410A_Power[22].ToString();
            }
            catch (Exception)
            {
                Log("M9410A OFDM Power: FAIL");
            }
        }

        public void LoadTXPower()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/OSTP_POWER.state'";
                LoadCommand(cmd);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "\"" + "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/OSTP_POWER.state" + "\" \n");
                //Log("M9410A Channel Power measurement: ");
            }
            catch (Exception)
            {
                Log("M9410A load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void TXPower(int port, BBU bbu, out string power)
        {
            string[] data = new string[] { "OUTPUT POWER", "GAIN" };
            string gain = "";
            power = "ERROR";
            try
            {
                string queryResult;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $"READ:EVM?";
                SendCommand(cmd, out queryResult);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, "READ:EVM? \n");
                //System.Threading.Thread.Sleep(4000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] m9410A_Power = queryResult.Split(',');
                power = m9410A_Power[22].ToString();
                string[] powerresult = new string[] { power, bbu.SHW_TX_GAIN(port.ToString(), out gain) };
                mainForm.DLCsvRecord(data, powerresult, null, "POWER", "NRTM1.1", powerresult.Length, 0, port);
                Log("OFDM Power: " + power);
            }
            catch (Exception e)
            {
                Log("M9410A OFDM Power measurement: fail" + e.Message, LogLevel.ERROR);
            }
        }
        public void LoadSEMSetup(string BW, string TDDFDD)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/SEM/SEM_{BW}Mhz.state'";
                LoadCommand(cmd);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "'" + "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/SEM/SEM_" + BW + "Mhz.state" + "' \n");
                //Log("M9410A load SEM measurement: SEM_" + BW + "_" + TDDFDD);
            }
            catch (Exception)
            {
                Log("M9410A Load SEM Setup: FAIL", LogLevel.ERROR);

            }
        }

        public void SEMMeas(int port, string TM, out double deltaPwr1, out double deltaPwr2, out double deltaPwr3, out double semLimit)
        {
            deltaPwr1 = 0;
            deltaPwr2 = 0;
            deltaPwr3 = 0;
            semLimit = 1;
            string[] data = new string[] { "Start Freq", "Stop Freq", "Integ BW", "Lower Power (dBm)", "delta Limit(dB)", "Freq (Hz)", "Upper Power (dBm)", "delta Limit(dB)", "Freq (Hz)", "Note", "Worst Power", "Worst delta", "Freq (Hz)" };
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            //System.Threading.Thread.Sleep(1);
            try
            {
                string queryResult;
                string cmd = $":SEM:AVER:COUN 10";
                SendCommand(cmd, out string response);
                //AgVisa32.viPrintf(xApp, ":SEM:AVER:COUN 10 \n");
                //System.Threading.Thread.Sleep(5000);
                cmd = $"READ:SEM?";
                SendCommand(cmd, out queryResult);
                //AgVisa32.viPrintf(xApp, "READ:SEM? \n");
                //System.Threading.Thread.Sleep(15000);
                //AgVisa32.viRead(xApp, out queryResult, 2048);
                string[] m9410A_SEM = queryResult.Split(',');
                string limit1 = m9410A_SEM[70].ToString();
                string limit2 = m9410A_SEM[71].ToString();
                string limit3 = m9410A_SEM[72].ToString();
                string limit4 = m9410A_SEM[73].ToString();
                string limit5 = m9410A_SEM[74].ToString();
                string limit6 = m9410A_SEM[75].ToString();
                // limitmax
                string absPower1 = m9410A_SEM[13].ToString();
                string absPower2 = m9410A_SEM[18].ToString();
                string absPower3 = m9410A_SEM[23].ToString();
                string absPower4 = m9410A_SEM[28].ToString();
                string absPower5 = m9410A_SEM[33].ToString();
                string absPower6 = m9410A_SEM[38].ToString();

                string FreqNegative1 = m9410A_SEM[14].ToString();
                string FreqPositive1 = m9410A_SEM[19].ToString();
                double Freq1 = Math.Max(Math.Abs(double.Parse(FreqNegative1)), Math.Abs(double.Parse(FreqPositive1)));
                string FreqNegative2 = m9410A_SEM[24].ToString();
                string FreqPositive2 = m9410A_SEM[29].ToString();
                double Freq2 = Math.Max(Math.Abs(double.Parse(FreqNegative2)), Math.Abs(double.Parse(FreqPositive2)));
                string FreqNegative3 = m9410A_SEM[34].ToString();
                string FreqPositive3 = m9410A_SEM[39].ToString();
                double Freq3 = Math.Max(Math.Abs(double.Parse(FreqNegative3)), Math.Abs(double.Parse(FreqPositive3)));
                double[] Freq = new double[] { Freq1, Freq2, Freq3 };
                Log(absPower1 + "\n" + absPower2 + "\n" + absPower3 + "\n" + absPower4 + "\n" + absPower5 + "\n" + absPower6 + "\n");
                double tmp1 = Math.Max(double.Parse(absPower1), double.Parse(absPower2));
                double tmp2 = Math.Max(double.Parse(absPower3), double.Parse(absPower4));
                double tmp3 = Math.Max(double.Parse(absPower5), double.Parse(absPower6));
                deltaPwr1 = Math.Max(double.Parse(limit1), double.Parse(limit2));
                deltaPwr2 = Math.Max(double.Parse(limit3), double.Parse(limit4));
                deltaPwr3 = Math.Max(double.Parse(limit5), double.Parse(limit6));
                double tmp5 = Math.Max(deltaPwr1, deltaPwr2);
                semLimit = Math.Max(tmp5, deltaPwr3);


                for (int i = 0; i < Freq.Length; i++)
                {
                    double startFreq = 50000;
                    double stopFreq = 5050000;
                    double IntegBW = 100000;
                    string worstabs = "";
                    string worstdelta = "";
                    string worstfreq = "";
                    string Note;
                    if ((50000 <= Math.Abs(Freq[i])) && Math.Abs(Freq[i]) <= 5050000)
                    {
                        Note = "OBUE_1";
                        worstabs = tmp1.ToString();
                        worstdelta = deltaPwr1.ToString();
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
                        worstabs = tmp2.ToString();
                        worstdelta = deltaPwr2.ToString();
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
                        worstabs = tmp3.ToString();
                        worstdelta = deltaPwr3.ToString();
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
            catch (Exception e)
            {
                Log("M9410A reading SEM : FAIL " + "\n" + e.Message, LogLevel.ERROR);
            }
        }

        public void LoadIMDACLR(string offset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_ACLR/IMD_ACLR_{offset}MHz.state'";
                LoadCommand(cmd);
                //System.Threading.Thread.Sleep(1000);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "\"" + "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_ACLR/IMD_ACLR_" + offset + "MHz.state" + "\" \n");
                //Log("C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_ACLR/IMD_ACLR_" + offset + "MHz.state");
            }
            catch (Exception)
            {
                Log("M9410A load ACLR Setup: FAIL", LogLevel.ERROR);
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
                string queryResult;
                string cmd = $"READ:ACP?";
                SendCommand(cmd, out queryResult);
                //AgVisa32.viPrintf(xApp, "READ:ACP? \n");
                //System.Threading.Thread.Sleep(3000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] obj = queryResult.Split(',');

                if (offset == "25" || offset == "-15" || offset == "-5")
                {
                    aclr = Math.Min(Math.Abs(double.Parse(obj[6].ToString())), Math.Abs(double.Parse(obj[8].ToString())));
                    Log(obj[6] + "\n" + obj[8]);
                }
                else if (offset == "-25" || offset == "15" || offset == "5")
                {
                    aclr = Math.Min(Math.Abs(double.Parse(obj[4].ToString())), Math.Abs(double.Parse(obj[10].ToString())));
                    Log(obj[4] + "\n" + obj[10]);
                }
            }
            catch (Exception)
            {
                Log("M9410A IMD ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void LoadIMDSEM(string offset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":MMEM:LOAD:STAT 'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_SEM/IMD_SEM_OFF{offset}MHz.state'";
                LoadCommand(cmd);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "\"" + "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_SEM/IMD_SEM_OFF" + offset + "MHz.state" + "\" \n");
                //Log("Load File: " + "\"C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_SEM/IMD_SEM_OFF" + offset + "MHz.state");
                bool Flag = true;
            }
            catch (Exception)
            {
                Log("M9410A Load Transmitter OBUE Setup: FAIL", LogLevel.ERROR);
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
                string queryResult;
                string cmd = $"READ:SEM?";
                SendCommand(cmd, out queryResult);
                //AgVisa32.viPrintf(xApp, "READ:SEM? \n");
                //System.Threading.Thread.Sleep(7000);
                //AgVisa32.viRead(xApp, out queryResult, 2048);
                Console.WriteLine(queryResult);
                string[] m9410A_SEM = queryResult.Split(',');
                limit1 = m9410A_SEM[70].ToString();
                limit2 = m9410A_SEM[71].ToString();
                limit3 = m9410A_SEM[72].ToString();
                limit4 = m9410A_SEM[73].ToString();
                limit5 = m9410A_SEM[74].ToString();
                limit6 = m9410A_SEM[75].ToString();
                limit7 = m9410A_SEM[76].ToString();
                limit8 = m9410A_SEM[77].ToString();
                Log(limit1 + "\n" + limit2 + "\n" + limit3 + "\n" + limit4 + "\n" + limit5 + "\n" + limit6 + "\n" + limit7 + "\n" + limit8);
                string tmp1 = (Math.Max(double.Parse(limit1), double.Parse(limit2))).ToString();
                string tmp2 = (Math.Max(double.Parse(limit3), double.Parse(limit4))).ToString();
                string tmp3 = (Math.Max(double.Parse(limit5), double.Parse(limit6))).ToString();
                string tmp4 = (Math.Max(double.Parse(limit7), double.Parse(limit8))).ToString();
                string tmp5 = (Math.Max(double.Parse(tmp1), double.Parse(tmp2))).ToString();
                string tmp6 = (Math.Max(double.Parse(tmp3), double.Parse(tmp4))).ToString();
                semLimit = (Math.Max(double.Parse(tmp5), double.Parse(tmp6))).ToString();
            }
            catch (Exception)
            {
                Log("M9410A SEM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadIMDTSE(string mode, string Freq)
        {
            string filename = "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_TSE/IMD_TSE_DC_COUPLE";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                if (mode == "DC")
                {
                    filename = "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_TSE/IMD_TSE_DC_COUPLE";
                }
                else if (mode == "AC")
                {
                    if (2496000000 <= double.Parse(Freq) && double.Parse(Freq) <= 2690000000)
                    {
                        filename = "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_TSE/IMD_TSE_AC_COUPLE_n41_DL_1C";
                    }
                    else if (3300000000 <= double.Parse(Freq) && double.Parse(Freq) <= 3800000000)
                    {
                        filename = "C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/IMD/IMD_TSE/IMD_TSE_AC_COUPLE_n78_DL_1C";
                    }
                }
                string cmd = $":MMEM:LOAD:STAT '{filename}'";
                LoadCommand(cmd);
                //AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "\"" + filename + "\" \n");
                //Log("M9410A load: " + filename);
            }
            catch (Exception)
            {
                Log("M9410A Load Transmitter Spurious Emission Setup: FAIL", LogLevel.ERROR);
            }
        }
        public void IMDTSEMMeas(string mode, out double amp1, out double amp2, out double amp3, out double amp4)
        {
            amp1 = 0;
            amp2 = 0;
            amp3 = 0;
            amp4 = 0;
            try
            {
                double tmp5;
                string queryResult;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":READ:SPUR?";
                SendCommand(cmd, out queryResult);
                //System.Threading.Thread.Sleep(1);                
                //AgVisa32.viPrintf(xApp, ":READ:SPUR? \n");
                //System.Threading.Thread.Sleep(2000);
                //AgVisa32.viRead(xApp, out queryResult, 1024);
                string[] transmitterSpuriousEmission = queryResult.Split(',');
                if (mode == "AC")
                {
                    amp1 = double.Parse(transmitterSpuriousEmission[4].ToString());
                    amp2 = double.Parse(transmitterSpuriousEmission[10].ToString());
                    amp3 = double.Parse(transmitterSpuriousEmission[16].ToString());
                    amp4 = double.Parse(transmitterSpuriousEmission[22].ToString());
                    Log(transmitterSpuriousEmission[4].ToString() + "\n" + transmitterSpuriousEmission[10].ToString() + "\n" + transmitterSpuriousEmission[16].ToString() + "\n" + transmitterSpuriousEmission[22].ToString());
                }
                if (mode == "DC")
                {
                    amp1 = double.Parse(transmitterSpuriousEmission[4].ToString());
                    amp2 = double.Parse(transmitterSpuriousEmission[10].ToString());
                    Log(transmitterSpuriousEmission[4].ToString() + "\n" + transmitterSpuriousEmission[10].ToString());
                }
            }
            catch (Exception)
            {
                Log("M9410A Spurious emission measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void SetOutputPower(string power)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":SOUR:POW {power}";
                SendCommand(cmd, out string response);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, ":SOUR:POW " + power + "\n");
                Log("M9410A set output power: " + power);
            }
            catch (Exception e)
            {
                Log("M9410A set output power: FAIL" + e.Message, LogLevel.ERROR);
            }
        }

        public void RFOnOff(int ON_OFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                string cmd = $":OUTP {ON_OFF}";
                SendCommand(cmd, out string response);
                //System.Threading.Thread.Sleep(1);
                //AgVisa32.viPrintf(xApp, ":OUTP " + ON_OFF + "\n");
                //Log("M9410A set RFONOff: OK" + ON_OFF);
            }
            catch (Exception e)
            {
                Log("M9410A set RFOnOff: FAIL" + e.Message, LogLevel.ERROR);
            }
        }
        public void LoadFileSetup(string filename)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                AgVisa32.viPrintf(xApp, ":MMEM:LOAD:STAT " + "'C:/Users/Administrator/Documents/Keysight/Instrument/NR5G/state/RXWaveform/" + filename + ".state" + "'\n");
                System.Threading.Thread.Sleep(5000);
                Log("M9410A load " + filename + " SUCCESSFULLY");
            }
            catch (Exception e)
            {
                Log("M9410A load " + filename + " : FAIL" + e.Message, LogLevel.ERROR);
            }
        }

        public void TakeScreenshot(string filePathVSA)
        {
            try
            {
                string cmd = $":MMEM:STOR:SCR '{filePathVSA}'";
                LoadCommand(cmd);
                //AgVisa32.viPrintf(xApp, $":MMEM:STOR:SCR \"{filePathVSA}\"\n");
                Log($"[M9410A] Screenshot saved to: {filePathVSA}");
                //save to remote PC
                //TransferFileToPC(filePathVSA);
            }
            catch (Exception ex)
            {
                Log("Save Screenshot got an error! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void SaveCSVFile(string filePathVSA)
        {
            try
            {
                string cmd = $":MMEM:STOR:TRAC:DATA TRACE1, '{filePathVSA}'";
                LoadCommand(cmd);
                //AgVisa32.viPrintf(xApp, $":MMEM:STOR:TRAC:DATA TRACE1, \"{filePathVSA}\"\n");
                Log($"Saved measurement data to: {filePathVSA}");
                //save to remote PC
                TransferFileToPC(filePathVSA);
            }
            catch (Exception ex)
            {
                Log("Save data fail! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void TransferFileToPC(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            AgVisa32.viPrintf(xApp, $":MMEM:DATA? \"{filename}\"\n");
            byte[] queryResult = new byte[1024]; // Buffer for reading data
            int bytesRead = 0;
            int status = AgVisa32.viRead(xApp, queryResult, queryResult.Length, out bytesRead);
            File.WriteAllBytes(filename, queryResult);

        }
    }
}

