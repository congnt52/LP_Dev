using Keysight.SignalStudio.N7631;
using System;

namespace _5GAutoTool
{
    public class M9410a
    {
        public Form5GAT mainForm;
        public string Alias;
        public string Name = "M9410A";
        public double TriggerDelay = 0;

        public M9410a(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        public Keysight.SignalStudio.N7631.Api api;
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 m9410a = new Ivi.Visa.Interop.FormattedIO488();

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "M9410a");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        //// TODO: Define a new api() using for N7631C.dll
        public void Connect(string IP, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            api = new Api();
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            string ConnectCmd = "TCPIP0::localhost::hislip" + int.Parse(IP) + "::INSTR";
            api.New();
            try
            {
                m9410a.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP0::localhost::hislip" + int.Parse(IP) + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                //System.Threading.Thread.Sleep(3000);
                m9410a.WriteString(Cmd, true);
                //System.Threading.Thread.Sleep(1000);

                if (api.ConnectInstrument(ConnectCmd))
                {
                    Model = api.SignalGenerator.InstrumentModelNumber.ToString();
                    Manufacturer = "Keysight";
                    //Serial = api.SignalGenerator;
                    Log("LOG: M9410a using N7631C API Connected!");
                }
                else
                {
                    Log("ERROR: N7631C Connection fail");
                }
            }
            catch (Exception e)
            {
                Log("ERROR: M9410a Connection fail " + e);
            }
        }
        public void Disconnect()
        {
            try
            {
                //api.Close();
                Log("LOG: M9410a preset N7631C API!");
                api.Preset();
                //N5182b.IO.Close();
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
                m9410a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])m9410a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception)
            {
                Log("N5182B connect: fail");
            }
        }
        public void SetFreq(string freqSet)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString("FREQ " + freqSet + "Hz", true);
                //Log("N5182B frequency: " + freqSet);
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"LOG: M9410A set frequency to {freqSet}");
                api.SignalGenerator.Frequency = double.Parse(freqSet);
                //api.Download();
                api.UpdateToInstrument();
            }
            catch (Exception)
            {
                Log("ERROR: M9410A setup frequency fail");
            }
        }
        public void SetPower(string setPower)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString(":POW " + setPower + "DBM", true);
                //Log("N5182B setup power: " + setPower + "dBm");
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"LOG: M9410A setup power {setPower} dBm");
                api.SignalGenerator.Amplitude = double.Parse(setPower);
                api.UpdateToInstrument();
            }
            catch (Exception)
            {
                Log("M9410A setup power: Fail");
            }
        }

        // TODO: Load G-FRC-A1-5 reference signal at freqSet Hz
        public void LoadGFR1A15(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A1-5.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: M9410A open file G-FRC-A1-5.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: M9410A load file G-FRC-A1-5.scp");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch
            {
                Log("ERROR: M9410A can not load file");
            }
        }

        // TODO: Load G-FRC-A1-5 reference signal at freqSet Hz and RBOffset
        public void LoadGFR1A15(string freqSet, string NRULSCHRBOffset)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A1-5.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file G-FRC-A1-5.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log($"LOG: N5182B load file FRC at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG: FRC at RBoffset {NRULSCHRBOffset}");
                    SetRBOffset(NRULSCHRBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch
            {
                Log("ERROR: N5182B can not load file");
            }
        }
        // TODO: Tín hiệu nhiêu AWGN signal
        public void SetAWGNSignal()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                m9410a.WriteString(":RAD:ARB:WAV " + "\"" + "FRC-A2-5-51rb_DDDSU.wfm" + "\"");
                Log("N5182B: Load file: FRC-A2-5-51rb_DDDSU.wfm");
                m9410a.WriteString(":SOUR:RAD:ARB:STAT ON");
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }

        public void LoadGRFA25(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A2-5.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file G-FRC-A2-5.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file G-FRC-A2-5.scp");
                    EnableAWGNSignal(true, "11.0", "18360000", "98280000");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }

        public void LoadGRFA25(string freqSet, string NRULSCHRBOffset)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A2-5.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file G-FRC-A2-5.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file G-FRC-A2-5.scp");
                    EnableAWGNSignal(true, "10.0", "18360000", "98280000");
                    Log($"LOG: N5182B set FRC at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG: FRC at RB Offset {NRULSCHRBOffset}");
                    SetRBOffset(NRULSCHRBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }
        //TODO: Set RBOffset for 5G NR Signal
        public void SetRBOffset(string NRULSCHRBOffset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"LOG: N5182B setup RB offset {NRULSCHRBOffset} for ULSCH channel");
                api.NR5GWaveformSettings.NRCarriers[0].Uplink.ULSCH[0].RBOffset = int.Parse(NRULSCHRBOffset);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //TODO: Enable and set SNR for AWGND signale
        public void EnableAWGNSignal(bool AwgnEnable, string CarriertoNoiseRatio, string CarrierBandwidth, string NoiseBandwidth)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5180B enables AWGN signal.");
                api.NR5GWaveformSettings.AwgnEnabled = AwgnEnable;
                Log($"LOG: N5182B setups carrier to noise ratio {CarriertoNoiseRatio} dB.");
                api.NR5GWaveformSettings.CarrierToNoiseRatio = double.Parse(CarriertoNoiseRatio);
                Log($"LOG: N5182B setups carrier bandwidth {CarrierBandwidth} Hz.");
                api.NR5GWaveformSettings.CarrierBandwidth = double.Parse(CarrierBandwidth);
                Log($"LOG: N5182B setups noise bandwidth {NoiseBandwidth} Hz.");
                api.NR5GWaveformSettings.NoiseBandwidth = double.Parse(NoiseBandwidth);
            }
            catch (Exception)
            {
                Log("ERROR: N5182B setups AWGN fail");
                throw;
            }
        }
        // TODO: Tín hiệu nhiêu ICS DFT-s-OFDM NR signal, SCS 30 kHz,50 RB
        public void SetICSInterferenceSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\ICS_DFT-s-OFDM_50RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file ICS_DFT-s-OFDM_50RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file ICS_DFT-s-OFDM_50RB.scp");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }
        // TODO: Load ICS DFT-s-OFDM NR signal, SCS 30 kHz,50 RB at RB Offset
        public void SetICSInterferenceSignal(string freqSet, string RBOffset)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\ICS_DFT-s-OFDM_50RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file ICS_DFT-s-OFDM_50RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file ICS_DFT-s-OFDM_50RB.scp");
                    Log($"LOG: Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG: Intefering signal at RBoffset {RBOffset}");
                    SetRBOffset(RBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }

        // TODO: Load Intefering signal 5G NR in ACS at freqSet Hz
        public void SetACSInterferenceSignal(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\ACS_DFT-s-OFDM_100RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file ACS_DFT-s-OFDM_100RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file ACS_DFT-s-OFDM_100RB.scp");
                    Log($"LOG: Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }
        // TODO: Load Intefering signal In-band blocking OFDM 100RB - SCS 15Khz
        public void InbandBlockingSignal(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\INB_DFT-s-OFDM_100RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file INB_DFT-s-OFDM_100RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file INB_DFT-s-OFDM_100RB.scp");
                    Log($"LOG: Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }
        public void InbandBlockingSignal(string freqSet, string RBOffset)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\INB_DFT-s-OFDM_100RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file INB_DFT-s-OFDM_100RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file INB_DFT-s-OFDM_100RB.scp");
                    Log($"LOG: Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG: Set RB Offset at {RBOffset}");
                    SetRBOffset(RBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }

        // TODO: Load Narrow-band Blocking Signal 
        public void NarrowbandBlockingSignal(string freqSet, string RBOffset)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NAB_DFT-s-OFDM_1RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file NAB_DFT-s-OFDM_1RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file NAB_DFT-s-OFDM_1RB.scp");
                    Log($"LOG: Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG: Set RB Offset at {RBOffset}");
                    SetRBOffset(RBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }
        // NarrowBand Blocking Signal Upper edger OFDM 1RB 15kHz QPSK upper edge
        public void NarrowbandUpperBlockingSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NAB_DFT-s-OFDM_1RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file NAB_DFT-s-OFDM_1RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file NAB_DFT-s-OFDM_1RB.scp");
                    Log("LOG: Set RB Offset at 0");
                    // Set RB Offset at 1
                    SetRBOffset("0");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }
        // TODO: Phat song sin
        public void CWSignal()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                api.NR5GWaveformSettings.NRCarriers[0].CarrierType = CarrierType.CW;
                api.Generate();
                api.Download();
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }

        // TODO: Phát tín hiệu General IMD
        public void GeneralIMDSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\IMD_GEN_DFT-s-OFDM_50RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file IMD_GEN_DFT-s-OFDM_50RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file IMD_GEN_DFT-s-OFDM_50RB.scp");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }

        }

        // phát tín hiệu Narrowband IMD upper edger
        public void NarrowBandUpperIMDSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\IMD_NAB_DFT-s-OFDM_1RB.scp";
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString(":RAD:ARB:WAV " + "\"" + "IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_upper_edge.wfm" + "\"");
                //Log("N5182B: Load file: IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_upper_edge.wfm");
                //N5182b.WriteString(":SOUR:RAD:ARB:STAT ON");
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file IMD_NAB_DFT-s-OFDM_1RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file IMD_NAB_DFT-s-OFDM_1RB.scp");
                    SetRBOffset("0");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }

        }

        // phát tín hiệu Narrowband IMD lower edger
        public void NarrowBandLowerIMDSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\IMD_NAB_DFT-s-OFDM_1RB.scp";
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString(":RAD:ARB:WAV " + "\"" + "IMD_Nar_DFT-s-OFDM_1rb_30kHz_QPSK_lower_edge.wfm" + "\"");
                //Log("N5182B: Load file: IMD_Nar_DFT -s-OFDM_1rb_30kHz_QPSK_lower_edge.wfm");
                //N5182b.WriteString(":SOUR:RAD:ARB:STAT ON");
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log("LOG: N5182B open file IMD_NAB_DFT-s-OFDM_1RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: N5182B load file IMD_NAB_DFT-s-OFDM_1RB.scp");
                    SetRBOffset("50");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }
        }

        //TODO: Phat tin hieu TX IMD NRTM1.1 BW 10Mhz 
        public void TXIMDSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NRTM11_10MHz_FDD.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"LOG: M9410a set TX IMD NRTM1.1 10 MHz");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG: M910a load file NRTM11_10MHz_FDD.scp");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: M9410a load file fail");
            }
        }

        //TODO: Load NR_TM1.1 signal 100 MHz FDD

        public void FR1_NRTM11_FDD(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NRTM11_FDD.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"LOG: M9410a load NR_TM1.1 signal");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log($"LOG: M9410a setups frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log("LOG: M9410a load file NRTM11_FDD.scp");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5182B load file fail");
            }

        }
        public void RFOnOff(int ON_OFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString(":OUTP " + ON_OFF);
                if (ON_OFF == 1)
                {
                    api.SignalGenerator.RFOutputEnabled = true;
                }
                else
                {
                    api.SignalGenerator.RFOutputEnabled = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void MODOnOff(int ON_OFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                if (ON_OFF == 1)
                {
                    api.SignalGenerator.RFOutputEnabled = true;
                }
                else
                {
                    api.SignalGenerator.RFOutputEnabled = false;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
