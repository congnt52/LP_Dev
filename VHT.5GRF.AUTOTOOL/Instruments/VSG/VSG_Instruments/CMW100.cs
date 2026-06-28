using System;
using System.IO;
using System.Threading;

namespace _5GAutoTool
{
    public class CMW100
    {
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 cmw100 = new Ivi.Visa.Interop.FormattedIO488();
        public Form5GAT mainForm;
        public string Alias;
        public string Name="CMW100A";
        public double TriggerDelay = 0;
        string currWaveform = "";
        public CMW100(Form5GAT mMainForm)
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
            log.Log(text, logLevel, Name, "CMW100");
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
                cmw100.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                cmw100.WriteString(Cmd, true);
                cmw100.IO.Timeout = 2000;
                tmp = (object[])cmw100.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Manufacturer = tmp[0].ToString();
                Model = tmp[1].ToString();
                Serial = tmp[2].ToString();
                if (tmp[1].ToString() == "CMW")
                {
                    Log("Connected IP: " + IP, LogLevel.SUCCESS);
                    currWaveform = "";
                    Name = "CMW100";
                }
                else
                {
                    Log("Connection FAIL!", LogLevel.ERROR);
                }
                Thread.Sleep(500);
                cmw100.WriteString("*RST;", true);
                cmw100.WriteString("*CLS;", true); // *CLS; *OPC?
            }
            catch (Exception e)
            {
                Log("Connection FAIL! " + e.Message, LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (cmw100 != null)
                {
                    cmw100.IO.Close();
                    currWaveform = "";
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
                cmw100.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])cmw100.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception)
            {
                Log($"Send command:{Cmd} => FAIL", LogLevel.ERROR);
            }
        }
        public void SetFreq(string freqSet)
        {
            try
            {
                cmw100.WriteString("SOUR:GPRF:GEN:RFS:FREQ " + freqSet, true);
                Log("Set frequency: " + freqSet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"CMW100 set Frequency Fail! {e.Message}");
            }
        }
        public void SetPower(string setPower)
        {
            try
            {
                cmw100.WriteString("SOUR:GPRF:GEN:RFS:LEV " + setPower, true);
                Log("CMW100 power level: " + setPower);
            }
            catch (Exception e)
            {
                Console.WriteLine($"CMW100 Set Power Fail! {e.Message}");
            }
        }
        //====== MinhNT sua 06.08.2024 ===================
        //thay doi: load theo waveform .wv, khong load theo state .dfl
        //public void LoadGFR1A15(string freq)
        //{
        //    LoadGFR1A15(freq, "0");
        //}

        public void LoadGFR1A15(string freq, string NRULSCHRBOffset = "0")
        {
            try
            {
                //path
                double.TryParse(freq, out double tmpFreq);
                string path = $@"C:\test3gpp\waveforms\VSG_RohdeSchwarz\G_FRC_A1-5_offset{NRULSCHRBOffset}_{tmpFreq / 1e6}.wv";   //change freq to MHz
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                if (path != currWaveform)
                {
                    // select ARB mode
                    cmw100.WriteString("SOUR:GPRF:GEN:BBM ARB", true);
                    Console.WriteLine($"{Alias}: CMW100 Select ARB mode");

                    //cmw100.WriteString($"SOUR:GPRF:GEN:ARB:FILE '{path}';*OPC?");
                    //Log($"{Alias} CMW100 Load waveform: {path}");

                    //set trigger - tam thoi ngat bo
                    //string trigName = "Base1: External TRIG IN";
                    //double trigDelay = 9.986e-3; // 9.9883e-3;
                    //cmw100.WriteString($"TRIG:GPRF:GEN:ARB:SOUR '{trigName}'", true); // TRIG:​GPRF:​GEN:​ARB:​​SOUR 'Base1: External TRIG IN'
                    //Log($"{Alias}: CMW100 Set trigger input: External TRIG IN ");
                    //cmw100.WriteString($"TRIG:GPRF:GEN:ARB:​DEL {trigDelay}");
                    //Log($"{Alias}: CMW100 Set trigger delay: {trigDelay} s ");
                    SetTrigger("Base1: External TRIG IN", TriggerDelay);
                    currWaveform = path;

                    //test
                    cmw100.WriteString($"SOUR:GPRF:GEN:ARB:FILE '{path}'", true);
                    Log($"{Alias} CMW100 Load waveform: {path}");

                }
                else
                {
                    Log($"{Alias}: CMW100 Select ARB mode");
                }

            }
            catch (Exception e)
            {
                Log($"ERROR: {Alias} CMW100 load file FAIL! {e.Message}", LogLevel.ERROR);
            }
        }

        public void SetTrigger(string triggerName, double delay)
        {
            try
            {
                cmw100.WriteString($"TRIG:GPRF:GEN:ARB:SOUR '{triggerName}'", true);
                Log($"{Alias}: CMW100 Set trigger input: External TRIG IN ");

                cmw100.WriteString($"TRIG:GPRF:GEN:ARB:DEL {delay}", true);
                Log($"{Alias}: Set trigger delay: {delay} s");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Set trigger delay => FAIL! {e.Message}");
            }
        }

        ////============ DInhNH sua 05.08.2024 ============
        //public void LoadGFR1A15(string freq)
        //{
        //    try
        //    {
        //        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
        //        System.Threading.Thread.Sleep(100);
        //        string path = "C:/ProgramData/Rohde-Schwarz/CMW/Data/Save/FRC-A1-5.dfl";
        //        cmw100.WriteString($"MMEM:RCL '{path}'");
        //        Log($"CMW100 Recall File: {path}");

        //        //// select ARB mode
        //        //cmw100.WriteString("SOUR:GPRF:GEN:BBM ARB");
        //        ////load waveform
        //        //string path = @"C:\test3gpp\waveforms\VSG_RohdeSchwarz\G_FRC_A1-5_offset0.wv";                
        //        //cmw100.WriteString($"SOUR:GPRF:GEN:ARB:FILE '{path}'");

        //        //Log($"CMW Recall File: {path}");
        //        ////set external trigger input
        //        //cmw100.WriteString("SOUR:GPRF:GEN:BBM ARB");
        //        ////set trigger in
        //        //cmw100.WriteString("TRIG:​GPRF:​GEN<i>:​ARB:​CAT:​SOUR 'Base1: External TRIG A'");
        //    }
        //    catch (Exception e)
        //    {
        //        Log($"ERROR: CMW100 load file FAIL! {e.Message}");
        //    }
        //}

        //public void LoadGFR1A15(string freq, string NRULSCHRBOffset)
        //{
        //    try
        //    {
        //        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
        //        System.Threading.Thread.Sleep(100);
        //        string path = $"C:/ProgramData/Rohde-Schwarz/CMW/Data/Save/FRC-A1-5_offset{NRULSCHRBOffset}.dfl";
        //        cmw100.WriteString($"MMEM:RCL '{path}'");
        //        Log($"CMW100 Recall File: {path}");

        //        //// select ARB mode
        //        //cmw100.WriteString("SOUR:GPRF:GEN:BBM ARB");
        //        ////load waveform
        //        //string path = @"C:\test3gpp\waveforms\VSG_RohdeSchwarz\G_FRC_A1-5_offset0.wv";                
        //        //cmw100.WriteString($"SOUR:GPRF:GEN:ARB:FILE '{path}'");

        //        //Log($"CMW Recall File: {path}");
        //        ////set external trigger input
        //        //cmw100.WriteString("SOUR:GPRF:GEN:BBM ARB");
        //        ////set trigger in
        //        //cmw100.WriteString("TRIG:​GPRF:​GEN<i>:​ARB:​CAT:​SOUR 'Base1: External TRIG A'");
        //    }
        //    catch (Exception e)
        //    {
        //        Log($"ERROR: CMW100 load file FAIL! {e.Message}");
        //    }
        //}
        ////============================================================================
        // Tín hiệu nhiêu AWGN signal
        public void SetAWGNSignal()
        {
            try
            {

            }
            catch (Exception e)
            {
                Log($"ERROR: CMW100 load file fail! {e.Message}", LogLevel.ERROR);
            }
        }
        // Tín hiệu nhiêu ICS DFT-s-OFDM NR signal, SCS 30 kHz,50 RB
        public void SetICSInterferenceSignal()
        {
            try
            {

            }
            catch (Exception e)
            {

            }
        }

        public void SetACSInterferenceSignal()
        {
            try
            {

            }
            catch (Exception)
            {
                Log("ERROR: CMW100 load file fail", LogLevel.ERROR);
            }
        }
        // Inband Blocking signal OFDM 100RB - SCS 15Khz
        public void InbandBlockingSignal()
        {
            try
            {

            }
            catch (Exception)
            {
                Log("ERROR: CMW100 load file fail", LogLevel.ERROR);
            }
        }


        public void NarrowbandLowerBlockingSignal()
        {
            try
            {

            }
            catch (Exception)
            {
                Log("ERROR: CMW100 load file fail", LogLevel.ERROR);
            }
        }
        // NarrowBand Blocking Signal Upper edger OFDM 1RB 15kHz QPSK upper edge
        public void NarrowbandUpperBlockingSignal()
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }
        // Phat song sin
        public void CWSignal(string freq, string power)
        {
            try
            {
                //path
                double.TryParse(freq, out double tmpFreq);
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(10);
                cmw100.WriteString("SOUR:GPRF:GEN:BBM CW", true);
                Log($"{Alias}: Select CW mode");

                SetFreq(freq);
                SetPower(power);
            }
            catch (Exception)
            {
                Log($"{Alias}: Cannot config CW mode");
            }
        }

        // phát tín hiệu General IMD
        public void GeneralIMDSignal()
        {
            try
            {

            }
            catch (Exception)
            {

            }

        }

        // phát tín hiệu Narrowband IMD upper edger
        public void NarrowBandUpperIMDSignal()
        {
            try
            {

            }
            catch (Exception)
            {

            }

        }

        // phát tín hiệu Narrowband IMD lower edger
        public void NarrowBandLowerIMDSignal()
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

        // phat tin hieu TX IMD NRTM1.1 BW 10Mhz 
        public void TXIMDSignal()
        {
            try
            {


            }
            catch (Exception)
            {

            }
        }

        public void FR1_NRTM11_FDD()
        {
            try
            {
                cmw100.WriteString("MMEM:RCL 'C:/ProgramData/Rohde-Schwarz/CMW/Data/Save/FRT_TM1.1_FDD.dfl'", true);
                Console.WriteLine("CMW Recall File: FRT_TM1.1_FDD.dfl");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
        public void RFOnOff(string ON_OFF)
        {
            try
            {
                cmw100.WriteString("SOUR:GPRF:GEN:STAT " + ON_OFF, true);
                cmw100.WriteString("*OPC?", true);
                tmp = (object[])cmw100.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Log($"{Alias} RF State: " + ON_OFF + " " + tmp[0].ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void MODOnOff(int ON_OFF)
        {

        }
    }
}