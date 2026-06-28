using Keysight.SignalStudio.N7631;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _5GAutoTool
{
    public class SMW200A
    {
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 smw = new Ivi.Visa.Interop.FormattedIO488();
        public Form5GAT mainForm;
        public string Alias;
        public string Name = "SMW200A";
        public double TriggerDelay = 0;
        private bool rfON, iqON;

        public SMW200A(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        object[] tmp;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "SMW200A");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Connection(string IP, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            try
            {
                smw.IO = (Ivi.Visa.Interop.IMessage)rm.Open($"TCPIP::{IP}::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                smw.WriteString(Cmd, true);
                smw.IO.Timeout = 2000;
                tmp = (object[])smw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Manufacturer = tmp[0].ToString();
                Model = tmp[1].ToString();
                Serial = tmp[2].ToString();
                if (Model == "SMW200A")
                {                    
                    //Thread.Sleep(500);
                    smw.WriteString("*RST", true);
                    smw.WriteString("*CLS", true); // *CLS; *OPC?
                    Log($"{Model}: Connected IP: {IP}", LogLevel.SUCCESS);
                    Name += $"-IP:{IP}";
                }
                else if (Model!="ERROR")
                {
                    Log($"WARN: SMW model is not compatible, {Model} is in connection ", LogLevel.WARN);
                }
                else
                {
                    Log($"WARN: SMW Unknown Model", LogLevel.WARN);
                }

            }
            catch (Exception e)
            {
                Log("ERROR: SMW200A Connection fail " + e.Message, LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (smw != null)
                {
                    smw.IO.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                smw.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])smw.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception e)
            {
                Log($"[SMW200A] {Alias} send command Fail! {e.Message}", LogLevel.ERROR);
            }
        }
        //09.4.2024 - send command and querry instrument status (short command)
        public bool SendCmdStatus(string Cmd)
        {
            bool isDone = true;
            try
            {
                smw.WriteString(Cmd, true);
                System.Threading.Thread.Sleep(10);
                smw.WriteString("*OPC?", true);
                string tmp = smw.ReadString();
                if(tmp=="1")
                {
                    Log($"{Alias}: SMW send command: {Cmd}");
                    isDone = false;
                }
                else
                {
                    Log($"{Alias}: SMW is still busy!", LogLevel.WARN);
                }
            }
            catch (Exception e)
            {
                Log($"[SMW200A] {Alias} send command Fail! {e.Message}", LogLevel.ERROR);
            }

            return isDone;
        }

        public void SetFreq(int sour, string freqSet)
        {
            try
            {
                //==== 09.4.2024 - tam comment lai ====
                //smw.WriteString($":SOUR{sour}:FREQ:CW {freqSet}", true);
                //System.Threading.Thread.Sleep(1000);
                //========================
                //smw.WriteString($":SOUR{sour}:BB:NR5G:TCW:WS:RFFR {freqSet}", true);
                //System.Threading.Thread.Sleep(1000);
                string cmd = $":SOUR{sour}:FREQ:CW {freqSet}";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} Source{sour} set frequency: {double.Parse(freqSet) / 1e6} MHz.");
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine($"{Alias} Source{sour} set frequencye Fail! {e.Message}");
            }
        }
        public void SetPower(int sour, string setPower)
        {
            try
            {
                //====09.4.2024========
                //smw.WriteString($":SOUR{sour}:POW:LEV:IMM:AMPL {setPower}", true);
                //System.Threading.Thread.Sleep(1000);
                //=====================
                //smw.WriteString("*OPC?", true);
                //double response = smw.ReadNumber();
                //if (response == 1)
                //{
                //    Log($"SMW200A Souce{sour} power level: {setPower} dBm");
                //}
                //else
                //{
                //    Log($"SMW200A is busy");
                //}
                //Log($"SMW200A Source{sour} power level: {setPower} dBm");
                string cmd = $":SOUR{sour}:POW:LEV:IMM:AMPL {setPower}";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    SendCmd($"SOUR{sour}:FREQ?", out string respond);
                    Log($"{Alias} SMW Source{sour} power level: {respond} dBm");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR: {Alias} SMW Source{sour} set power Level Fail! " + e.Message);
            }
        }
        public void LoadGFR1A15(string freq)
        {
            try
            {
                //=====09.4.2024================
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/FRC-A1-5-51rb_7d1s2u_oran.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/FRC-A1-5-51rb_7d1s2u_oran.savrcltxt'");
                //========================
                string path = "/var/user/test3gpp/FRC-A1-5-51rb_7d1s2u_oran.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch
            {
                Log($"ERROR: {Alias} SMW can not load file", LogLevel.ERROR);
            }
        }
        // Tín hiệu nhiêu AWGN signal
        public void LoadGFR1A25(string freq)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/FRC-A1-5-51rb_7d1s2u_oran.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/FRC-A1-5-51rb_7d1s2u_oran.savrcltxt'");
                string path = "/var/user/test3gpp/FRC-A1-5-51rb_7d1s2u_oran.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }
        // Tín hiệu nhiêu ICS DFT-s-OFDM NR signal, SCS 30 kHz,50 RB
        public void SetICSInterferenceSignal()
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM.savrcltxt'");
                //====================
                string path = "/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }

        // Tín hiệu nhiêu ICS DFT-s-OFDM NR signal, SCS 30 kHz,50 RB
        public void SetICSInterferenceSignalLower(string rbOffset)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM_lower_edge.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM_lower_edge.savrcltxt'");
                ////smw.WriteString($":SOUR2:BB:NR5G:SCH:CELL0:SUBF0:USER0:BWP0:ALL0:RBOF {rbOffset}; *OPC?", true);
                //Log($"SMW200A Set RB Offset at: {rbOffset}");
                //====================
                string path = "/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM_lower_edge.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }
        public void SetICSInterferenceSignalUpper(string rbOffset)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM_upper_edge.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM_upper_edge.savrcltxt'");
                ////smw.WriteString($":SOUR2:BB:NR5G:SCH:CELL0:SUBF0:USER0:BWP0:ALL0:RBOF {rbOffset}; *OPC?", true);
                //Log($"SMW200A Set RB Offset at: {rbOffset}");
                //====================
                string path = "/var/user/test3gpp/ICS_DFT-s-OFDM_50rb_30kHz_16QAM_upper_edge.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }

        public void SetACSInterferenceSignal()
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/ACS_DFT-s-OFDM_100rb_15kHz_QPSK.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/ACS_DFT-s-OFDM_100rb_15kHz_QPSK.savrcltxt'");
                //====================
                string path = "/var/user/test3gpp/ACS_DFT-s-OFDM_100rb_15kHz_QPSK.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }
        // Inband Blocking signal OFDM 100RB - SCS 15Khz
        public void InbandBlockingSignal()
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/INB_DFT-s-OFDM_100rb_15kHz_QPSK.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/INB_DFT-s-OFDM_100rb_15kHz_QPSK.savrcltxt'");
                //====================
                string path = "/var/user/test3gpp/INB_DFT-s-OFDM_100rb_15kHz_QPSK.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }

        public void NarrowbandLowerBlockingSignal()
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //smw.WriteString(":SYST:RCL '/var/user/test3gpp/NAB_DFT-s-OFDM_1rb_15kHz_QPSK_lower_edge.savrcltxt'");
                //Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/NAB_DFT-s-OFDM_1rb_15kHz_QPSK_lower_edge.savrcltxt'");
                //====================
                string path = "/var/user/test3gpp/NAB_DFT-s-OFDM_1rb_15kHz_QPSK_lower_edge.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
            }
            catch (Exception)
            {
                Log($"ERROR: {Alias} SMW load file fail", LogLevel.ERROR);
            }
        }


        public void NarrowbandLowerBlockingSignal(string rbOffset)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //System.Threading.Thread.Sleep(100);
                //string path = @"/var/user/test3gpp/NAB_DFT-s-OFDM_1rb_15kHz_QPSK_lower_edge.savrcltxt";
                //smw.WriteString($":SYST:RCL '{path}'");
                //Log($"SMW200A Recall File: {path}");
                ////Set RB position
                ////smw.WriteString(":SOUR2:BB:NR5G:UBWP:USER0:CELL0:UL:BWP0:FRC:STAT 0");
                //smw.WriteString($":SOUR2:BB:NR5G:SCH:CELL0:SUBF0:USER0:BWP0:ALL0:RBOF {rbOffset}");
                //Log($"SMW200A Set RB Offset at: {rbOffset}");
                //====================
                string path = @"/var/user/test3gpp/NAB_DFT-s-OFDM_1rb_15kHz_QPSK_lower_edge.savrcltxt";
                string cmd = $":SYST:RCL '{path}'";
                bool tmp1 = SendCmdStatus(cmd);
                if (tmp1)
                {
                    Log($"{Alias} SMW Recall File: {path}");
                }
                //Set RB position

            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }
        }

        // NarrowBand Blocking Signal Upper edger OFDM 1RB 15kHz QPSK upper edge
        public void NarrowbandUpperBlockingSignal(string rbOffset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
               
                string path = @"/var/user/test3gpp/NAB_DFT-s-OFDM_1rb_15kHz_QPSK_upper_edge.savrcltxt";
                smw.WriteString($":SYST:RCL '{path}'");
                Log($"SMW200A Recall File: {path}");
                //Set RB position
                //smw.WriteString(":SOUR2:BB:NR5G:UBWP:USER0:CELL0:UL:BWP0:FRC:STAT 0");
                smw.WriteString($":SOUR2:BB:NR5G:SCH:CELL0:SUBF0:USER0:BWP0:ALL0:RBOF {rbOffset}");
                Log($"SMW200A Set RB Offset at: {rbOffset}");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }
        }

        //Set rb offset 
        public void SetRbOffset(string sour, string rbOffset)
        {
            try
            {
                string cmd = $":SOUR{sour}:BB:NR5G:SCH:CELL0:SUBF0:USER0:BWP0:ALL0:RBOF {rbOffset}";
                bool tmp = SendCmdStatus(cmd);
                if (tmp)
                {
                    Log($"{Alias} SMW Set RBOffset at: {rbOffset}");
                }
            }
            catch (Exception)
            {
                Log($"{Alias} SMW Set RBOffset Fail");
            }
        }
        // Phat song sin
        public void CWSignal(string CwOffset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                smw.WriteString($":SOUR2:AWGN:FREQ:TARG {CwOffset} ");
                Log($"SMW200A setup CW signal at {CwOffset} ");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A set up CW signal fail", LogLevel.ERROR);
            }
        }

        // phát tín hiệu General IMD
        public void GeneralIMDSignalLower()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                string path = @"/var/user/test3gpp/IMD_Gen_DFT-s-OFDM_50rb_30kHz_QPSK.savrcltxt";
                smw.WriteString($":SYST:RCL '{path}'");
                Log($"SMW200A Recell File: :SYST:RCL '{path}'");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }

        }
        public void GeneralIMDSignalUpper()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                smw.WriteString(":SYST:RCL '/var/user/test3gpp/IMD_Gen_DFT-s-OFDM_50rb_30kHz_QPSK.savrcltxt'");
                Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/IMD_Gen_DFT-s-OFDM_50rb_30kHz_QPSK_upper_edge.savrcltxt'");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }

        }

        // phát tín hiệu Narrowband IMD upper edger
        public void NarrowBandUpperIMDSignal()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                smw.WriteString(":SYST:RCL '/var/user/test3gpp/IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_upper_edge.savrcltxt'");
                Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_upper_edge.savrcltxt'");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }
        }

        // phát tín hiệu Narrowband IMD lower edger
        public void NarrowBandLowerIMDSignal()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                smw.WriteString(":SYST:RCL '/var/user/test3gpp/IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_lower_edge.savrcltxt'");
                Log("SMW200A Recell File: :SYST:RCL '/var/user/test3gpp/IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_lower_edge.savrcltxt'");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }
        }

        // phat tin hieu TX IMD NRTM1.1 BW 10Mhz 
        public void TXIMDSignal()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                string path = @"/var/user/test3gpp/TXIMDSignal.savrcltxt";
                smw.WriteString($":SYST:RCL '{path}'");
                Log($"SMW200A Recell File: :SYST:RCL '{path}'");
            }
            catch (Exception)
            {
                Log("ERROR: SMW200A load file fail", LogLevel.ERROR);
            }
        }

        public void FR1_NRTM11_FDD()
        {
            try
            {
                smw.WriteString("MMEM:RCL 'C:/ProgramData/Rohde-Schwarz/SMA200A/Data/Save/FRT_TM1.1_FDD.dfl'", true);
                Console.WriteLine("SMA200A Recell File: FRT_TM1.1_FDD.dfl");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void RFOnOff(int sour, int ON_OFF)
        {
            try
            {
                smw.WriteString($"SOUR{sour}:BB:NR5G:STAT {ON_OFF}", true);
                System.Threading.Thread.Sleep(500);
                smw.WriteString($"OUTP{sour}:STAT {ON_OFF}", true);
                Log($"SMW200A RF{sour} State: {ON_OFF}");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void IQOnOff(int sour, int ON_OFF)
        {
            try
            {
                smw.WriteString($":SOUR{sour}:BB:DM:STAT {ON_OFF}");
                smw.WriteString($"SOUR{sour}:IQ:STAT {ON_OFF}", true);
                //Log($"SMW200A IQ{sour} State: {ON_OFF}");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
