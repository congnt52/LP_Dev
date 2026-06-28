using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace _5GAutoTool
{
    public enum VSGAlias : uint
    {
        WS,
        NRIS,
        CW,
        AWGN
    }
public class VSG:IInstrument
    {
        //TODO: MINHNT59 - define VSG's Properties
        public string Name;  //Wanted Signal, Interference Signal, CW Signal
        public string IpAddress;
        public string Model;
        public int Port;
        public string Manufacturer;
        public string Serial;
        public bool IsConnected;
        public string Status;
        public double SetFrequencyValue;
        public double SetPowerValue;
        public string VSGConfigurationPath;
        public string AttenuatorCSVpath;
        public double AttenuatorLevel;
        public List<double> Attenuators = new List<double>();
        public string TypeOfSignal;
        private double currentSetFrequency;
        public string Alias;
        public double TriggerDelay = 0;
        public double SweepFrequencyFrom = 0;
        public double SweepFrequencyTo = 0;


        public N5182B N5182b;
        public CMW100 cmw100;
        private SMW200A smw200a;
        private SGS100A sgs100a;
        public M9410a m9410a;
        string instrumentVSG = "0";
        public Form5GAT mainForm;
        public VSG(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
            N5182b = new N5182B(mainForm);
            cmw100 = new CMW100(mainForm);
            sgs100a = new SGS100A(mainForm);
            smw200a = new SMW200A(mainForm);
            m9410a = new M9410a(mainForm);
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "VSG");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        // Lua chon loai may VSG (Bat buoc)
        /*
            - N5182B : Agilent Technologies N5182B 
        */
        public void Instrument(string instrument)
        {
            instrumentVSG = instrument;
        }
        public void Instr(out string instrument)
        {
            instrument = instrumentVSG;
        }
        public void Connection(string ip, string cmd, out string manufacturer, out string model, out string serial)
        {
            manufacturer = "ERROR";
            model = "ERROR";
            serial = "ERROR";

            if (instrumentVSG == "N5182B")
            {
                N5182b.Connection(ip, cmd, out manufacturer, out model, out serial);
                Model = "N5182B";
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.Connection(ip, cmd, out manufacturer, out model, out serial);
                Model = "CMW100";
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.Connection(ip, cmd, out manufacturer, out model, out serial);
                Model = "SMW200A";
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.Connection(ip, cmd, out manufacturer, out model, out serial);
                Model = "SGS100A";
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.Connect(ip, cmd, out manufacturer, out model, out serial);
                Model = "M9410A";
            }
            //TODO: Minhnt59 - VSG Connection 
            IpAddress = ip;
            Manufacturer = manufacturer;
            Model = model;
            Serial = serial;
            Status = "Connected";
            IsConnected = true;
            Name += $"-IP:{IpAddress}";

        }
        public string GetIDNumber()
        {
            string tmp="";
            if (instrumentVSG == "N5182B")
            {
                N5182b.SendCmd("*IDN?", out tmp);
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.SendCmd("*IDN?", out tmp);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SendCmd("*IDN?", out tmp);
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.SendCmd("*IDN?", out tmp);
            }
            else if (instrumentVSG == "SGS100A")
            {
                m9410a.SendCmd("*IDN?", out tmp);
            }

            return tmp;
        }
        public void Disconnect()
        {
            if (instrumentVSG == "N5182B")
            {
                N5182b.Disconnect();
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.Disconnect();
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.Disconnect();
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.Disconnect();
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.Disconnect();
            }
            //TODO: Minhnt59 - VSG Disconnect
            Status = "No connection";
            IsConnected = false;
        }

        public void Reset()
        {
            //RF, IQ out change to OFF            

        }
        public void SendCmd(string Cmd, out string respond)
        {
            respond = "";
            if (instrumentVSG == "N5182B")
            {
                N5182b.SendCmd(Cmd, out respond);
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.SendCmd(Cmd, out respond);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SendCmd(Cmd, out respond);
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.SendCmd(Cmd, out respond);
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.SendCmd(Cmd, out respond);
            }
        }
        public void SetFreq(string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                N5182b.SetFreq(freqSet);
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.SetFreq(freqSet);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SetFreq(1, freqSet);
                smw200a.SetFreq(2, freqSet);
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.SetFreq(freqSet);
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.SetFreq(freqSet);
            }
            //TODO: Minhnt59 - setfrequency
            double.TryParse(freqSet, out SetFrequencyValue);
        }


        public void SetPower(string powerSet)
        {

            if (instrumentVSG == "N5182B")
            {
                N5182b.SetPower(powerSet);
                N5182b.Alias = Name;
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.SetPower(powerSet);
                cmw100.Alias = Name;
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SetPower(1, powerSet);
                smw200a.Alias = Name;
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.SetPower(powerSet);
                sgs100a.Alias = Name;
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.SetPower(powerSet);
            }
        }


        public void setParameter(int source, string powerSet, string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "CMW100")
            {
                //cmw100.RFOnOff("OFF");
                //System.Threading.Thread.Sleep(500);
                cmw100.SetFreq(freqSet);
                cmw100.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                cmw100.RFOnOff("ON");
            }
            else if (instrumentVSG == "SMW200A")
            {
                //smw200a.IQOnOff(source, 0);
                //smw200a.RFOnOff(source, 0);
                //System.Threading.Thread.Sleep(500);
                smw200a.SetFreq(source, freqSet);
                smw200a.SetPower(source, powerSet);
                System.Threading.Thread.Sleep(2000);  //8.4.2024 - tam thoi de thoi gian tang len
                smw200a.IQOnOff(source, 1);
                smw200a.RFOnOff(source, 1);
            }
            else if (instrumentVSG == "SGS100A")
            {
                //sgs100a.RFOnOff(0);
                System.Threading.Thread.Sleep(500);
                sgs100a.SetFreq(freqSet);
                sgs100a.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                sgs100a.RFOnOff(1);
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.RFOnOff(0);
                System.Threading.Thread.Sleep(2000);
                m9410a.SetFreq(freqSet);
                System.Threading.Thread.Sleep(2000);
                m9410a.SetPower(powerSet);
                System.Threading.Thread.Sleep(2000);
                m9410a.RFOnOff(1);
            }
        }
        /// <summary>
        /// Setup bài đo Reference sensitivity
        /// </summary>
        /// <param name="powerSet"></param>
        /// <param name="freqSet"></param>
        //public void SetPsenWaveform(string powerSet, string freqSet)
        //{
        //    if (instrumentVSG == "N5182B")
        //    {
        //        //N5182b.MODOnOff(0);
        //        //N5182b.RFOnOff(0);
        //        //System.Threading.Thread.Sleep(1000);
        //        //N5182b.SetFreq(freqSet);
        //        N5182b.LoadGFR1A15(freqSet);
        //        N5182b.SetPower(powerSet);

        //        System.Threading.Thread.Sleep(1000);
        //        N5182b.MODOnOff(1);
        //        N5182b.RFOnOff(1);
        //    }
        //    else if (instrumentVSG == "CMW100")
        //    {
        //        cmw100.LoadGFR1A15(freqSet);
        //        cmw100.SetFreq(freqSet);
        //        cmw100.SetPower(powerSet);
        //        System.Threading.Thread.Sleep(500);
        //        cmw100.RFOnOff("ON");
        //    }
        //    else if (instrumentVSG == "SMW200A")
        //    {
        //        smw200a.LoadGFR1A15(freqSet);
        //        System.Threading.Thread.Sleep(2000);
        //        smw200a.SetFreq(1, freqSet);
        //        System.Threading.Thread.Sleep(2000);
        //        smw200a.SetPower(1, powerSet);
        //        System.Threading.Thread.Sleep(500);
        //        smw200a.RFOnOff(1, 1);
        //    }
        //    else if (instrumentVSG == "M9410A")
        //    {
        //        string filename = "FRC-A1-5-51rb_7d1s2u_oran";
        //        m9410a.LoadGFR1A15(freqSet);
        //        System.Threading.Thread.Sleep(2000);
        //        m9410a.SetFreq(freqSet);
        //        System.Threading.Thread.Sleep(2000);
        //        m9410a.SetPower(powerSet);
        //        System.Threading.Thread.Sleep(2000);
        //        m9410a.RFOnOff(1);
        //    }
        //}

        public void SetTriggerDelay(double delay)
        {
            if (instrumentVSG == "N5182B")
            {
                N5182b.TriggerDelay = delay;
            }
            else if (instrumentVSG == "CMW100")
            {


                cmw100.TriggerDelay = delay;
            }
        }

        public void SetPsenWaveform(string powerSet, string freqSet, string RBOffset = "0")
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(1000);                               
                N5182b.LoadGFR1A15(freqSet, RBOffset);
                //N5182b.SetFreq(freqSet); 
                N5182b.SetPower(powerSet);

                System.Threading.Thread.Sleep(1000);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.LoadGFR1A15(freqSet, RBOffset);
                cmw100.SetFreq(freqSet);
                cmw100.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                cmw100.RFOnOff("ON");
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.LoadGFR1A15(freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(1, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(1, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(1, 1);
            }
            else if (instrumentVSG == "M9410A")
            {
                string filename = "FRC-A1-5-51rb_7d1s2u_oran.state";
                m9410a.LoadGFR1A15(freqSet);
                m9410a.SetFreq(freqSet);
                m9410a.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                m9410a.RFOnOff(1);
            }
        }
        // TODO: Load test vector Dynamic Range
        public void SetDRWaveform(string powerSet, string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.LoadGRFA25(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "CMW100")
            {
                Log($"CMW100 not supported", LogLevel.WARN);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.LoadGFR1A25(freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(1, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(1, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(1, 1);
            }
        }

        public void SetDRWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //ystem.Threading.Thread.Sleep(500);
                N5182b.LoadGRFA25(freqSet, RBOffset);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "CMW100")
            {
                Log($"CMW100 not supported", LogLevel.WARN);
            }
        }
        // Load test vector In-channel selectivity
        public void SetICSLowerWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.SetICSInterferenceSignal(freqSet, RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SetICSInterferenceSignalLower(RBOffset);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(2, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(2, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(2, 1);
            }
        }
        public void SetICSUpperWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.SetICSInterferenceSignal(freqSet, RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SetICSInterferenceSignalUpper(RBOffset);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(2, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(2, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(2, 1);
            }
        }
        // Load test vector In-channel selectivity
        public void SetICSWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.SetICSInterferenceSignal(freqSet, RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                int.TryParse(RBOffset, out int intRBOffset );
                if(intRBOffset < 136)
                {
                    smw200a.SetICSInterferenceSignalLower(RBOffset);
                }
                else
                {
                    smw200a.SetICSInterferenceSignalUpper(RBOffset);
                }                
            }
        }
        // Load test vector Adjacent Channel Selectivity
        public void SetACSWaveform(string powerSet, string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.SetACSInterferenceSignal(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.SetACSInterferenceSignal();
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(2, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(2, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(2, 1);
            }
        }
        // Load test vector inband blocking
        public void SetIBBWaveform(string powerSet, string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.InbandBlockingSignal(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.InbandBlockingSignal();
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(2, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(2, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(2, 1);
            }
        }
        public void SetIBBWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.InbandBlockingSignal(freqSet, RBOffset);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.InbandBlockingSignal();
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(2, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(2, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(2, 1);
            }
        }
        public void SetNBBLowerWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.NarrowbandBlockingSignal(freqSet, RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.NarrowbandLowerBlockingSignal(RBOffset);
                //System.Threading.Thread.Sleep(5000);
                //smw200a.SetFreq(2, freqSet);
                //System.Threading.Thread.Sleep(2000);
                //smw200a.SetPower(2, powerSet);
                //System.Threading.Thread.Sleep(500);
                //smw200a.RFOnOff(2, 1);
            }
        }
        public void SetNBBUpperWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.NarrowbandBlockingSignal(freqSet, RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.NarrowbandUpperBlockingSignal(RBOffset);
                //System.Threading.Thread.Sleep(5000);
                //smw200a.SetFreq(2, freqSet);
                //System.Threading.Thread.Sleep(2000);
                //smw200a.SetPower(2, powerSet);
                //System.Threading.Thread.Sleep(500);
                //smw200a.RFOnOff(2, 1);
            }
        }

        public void SetNBBWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.NarrowbandBlockingSignal(freqSet, RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                //N5182b.MODOnOff(1);
                //N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.NarrowbandLowerBlockingSignal();
            }
        }

        public void SetCWWaveform(string powerSet, string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.CWSignal();
                N5182b.SetFreq(freqSet);
                //N5182b.SetPower(powerSet);
                //System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(0);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SGS100A")
            {
                //sgs100a.RFOnOff(0);
                System.Threading.Thread.Sleep(1); //08.01.2024 -test chu ky 100ms
                sgs100a.SetFreq(freqSet);
                sgs100a.SetPower(powerSet);
                sgs100a.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                //smw200a.CWSignal(freqSet);
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.CWSignal(freqSet, powerSet);
            }
        }


        public void SetGeneralIMDWaveformLower(string powerSet, string freqSet)
        {
            if(instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.GeneralIMDSignal();
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.GeneralIMDSignalLower();
                System.Threading.Thread.Sleep(2000);
                //smw200a.SetFreq(2, freqSet);
                //System.Threading.Thread.Sleep(2000);
                //smw200a.SetPower(2, powerSet);
                //System.Threading.Thread.Sleep(500);
                //smw200a.RFOnOff(2, 1);
            }
        }
        public void SetGeneralIMDWaveformUpper(string powerSet, string freqSet)
        {
            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.GeneralIMDSignal();
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.GeneralIMDSignalUpper();
                System.Threading.Thread.Sleep(2000);
                //smw200a.SetFreq(2, freqSet);
                //System.Threading.Thread.Sleep(2000);
                //smw200a.SetPower(2, powerSet);
                //System.Threading.Thread.Sleep(500);
                //smw200a.RFOnOff(2, 1);
            }
        }

        public void SetNBUpperIMDWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if(instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.NarrowBandUpperIMDSignal(RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if(instrumentVSG == "SMW200A")
            {
                smw200a.NarrowBandUpperIMDSignal();
                System.Threading.Thread.Sleep(2000);
                //smw200a.SetFreq(2, freqSet);
                //System.Threading.Thread.Sleep(2000);
                //smw200a.SetPower(2, powerSet);
                //System.Threading.Thread.Sleep(500);
                //smw200a.RFOnOff(2, 1);
            }
        }

        public void SetNBLowerIMDWaveform(string powerSet, string freqSet, string RBOffset)
        {
            if(instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.NarrowBandLowerIMDSignal(RBOffset);
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if(instrumentVSG == "SMW200A")
            {
                smw200a.NarrowBandLowerIMDSignal();
                System.Threading.Thread.Sleep(2000);
                //smw200a.SetFreq(2, freqSet);
                //System.Threading.Thread.Sleep(2000);
                //smw200a.SetPower(2, powerSet);
                //System.Threading.Thread.Sleep(500);
                //smw200a.RFOnOff(2, 1);
            }
        }
        // tin hieu TX IMD 10Mhz
        public void SetTXIMDWaveform(string powerSet, string freqSet)
        {
            if(instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.TXIMDSignal();
                N5182b.SetFreq(freqSet);
                N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.RFOnOff(1, 0);
                smw200a.TXIMDSignal();
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(1, freqSet);
                smw200a.SetPower(1, powerSet);
                smw200a.RFOnOff(1, 1);
                System.Threading.Thread.Sleep(500);
            }
            else if (instrumentVSG == "M9410A")
            {
                m9410a.TXIMDSignal();
                m9410a.SetFreq(freqSet);
                m9410a.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                m9410a.MODOnOff(1);
                m9410a.RFOnOff(1);
            }
        }

        public void SetNRTM11FDD(string powerSet, string freqSet)
        {


            if (instrumentVSG == "N5182B")
            {
                //N5182b.MODOnOff(0);
                //N5182b.RFOnOff(0);
                //System.Threading.Thread.Sleep(500);
                N5182b.FR1_NRTM11_FDD(freqSet);
                //N5182b.SetFreq(freqSet);
                //N5182b.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "CMW100")
            {
                cmw100.FR1_NRTM11_FDD();
                cmw100.SetFreq(freqSet);
                cmw100.SetPower(powerSet);
                System.Threading.Thread.Sleep(500);
                cmw100.RFOnOff("ON");
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.FR1_NRTM11_FDD();
                System.Threading.Thread.Sleep(2000);
                smw200a.SetFreq(1, freqSet);
                System.Threading.Thread.Sleep(2000);
                smw200a.SetPower(1, powerSet);
                System.Threading.Thread.Sleep(500);
                smw200a.RFOnOff(1, 1);
            }
        }

        public void OffSignal(int index =1)
        {
            if (instrumentVSG == "N5182B")
            {
                N5182b.MODOnOff(0);
                N5182b.RFOnOff(0);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.IQOnOff(index, 0);
                smw200a.RFOnOff(index, 0);
            }
        }

        public void OnSignal(int index = 1)
        {
            if (instrumentVSG == "N5182B")
            {
                N5182b.MODOnOff(1);
                N5182b.RFOnOff(1);
            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.IQOnOff(index, 1);
                smw200a.RFOnOff(index, 1);
            }
        }
        public void OnOffSignal(int OnOff)
        {
            if (instrumentVSG == "N5182B")
            {
                N5182b.RFOnOff(OnOff);
                N5182b.MODOnOff(OnOff);                
            }
            else if (instrumentVSG == "CMW100")
            {
                if(OnOff==1)
                {
                    cmw100.RFOnOff("ON");
                }
                else
                    cmw100.RFOnOff("OFF");

            }
            else if (instrumentVSG == "SMW200A")
            {
                smw200a.RFOnOff(1, OnOff);
                smw200a.RFOnOff(2, OnOff);
                smw200a.IQOnOff(1, OnOff);
                smw200a.IQOnOff(2, OnOff);                
            }
            else if (instrumentVSG == "SGS100A")
            {
                sgs100a.RFOnOff(OnOff);
            }
        }

        public void LoadUserCorrDataSGS100A(string ucorrPath)
        {
            if (instrumentVSG == "SGS100A")
            {
                sgs100a.LoadUserCorrectionSettingFile(ucorrPath);
            }
        }

        public void ReadATTfromCSVdir(string dir, string format, double FindFreq)
        {
            if (AttenuatorCSVpath != dir || currentSetFrequency != FindFreq)
            {
                //LogDashedLine($"RRU ATTENUATORS");
                //Log($"Reading RRU attenuators: Set frequency = {FindFreq}\tSource = {dir}");
                var list = Directory.GetFiles(dir, "*.csv");
                Attenuators.Clear();
                for (int j = 1; j <= list.Count(); j++)
                {
                    string path = $@"{dir}\{format}{j}.csv";
                    Attenuators.Add(ReadATTfromCSVFile(path, FindFreq));
                }
                //update properties
                AttenuatorCSVpath = dir;
                currentSetFrequency = FindFreq;
                //SetFrequencyValue = FindFreq;
            }
        }

        public double ReadATTfromCSVFile(string path, double FindFreq)
        {
            double tempVal = 0.0;
            if (File.Exists(path))
            {
                try
                {
                    var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        HasHeaderRecord = true,
                        Comment = '#',
                        AllowComments = true,
                        Delimiter = ",",
                    };
                    var streamReader = new StreamReader(path, Encoding.UTF8);
                    var CSV = new CsvReader(streamReader, csvConfig);
                    double min = 1.0, tmp = 10.0, max = 100.0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (!CSV.Read()) break;
                    }

                    while (CSV.Read())
                    {
                        double.TryParse(CSV.GetField(0), out double freq);  //first column contains range of Frequency
                        tmp = Math.Abs((FindFreq - freq) / 1000000);
                        if (tmp < min)
                        {
                            double.TryParse(CSV.GetField(1), out tempVal);
                            tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
                            break;
                        }
                        else if (tmp < max)
                        {
                            double.TryParse(CSV.GetField(1), out tempVal);
                            tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
                            max = tmp;
                        }
                    }
                    CSV.Dispose();
                }
                catch (Exception ex)
                {
                    Log($"[{Name}] Read ATT from  => ERROR! CSVFile:{path}");
                }
            }
            else Log($"[{Name} ReadATTfromCSVFile] CSVFile:{path} not exist!");

            return tempVal;
        }

        public bool IsSweepingFreqRangeValidated(string range)
        {
            bool tmp = false;
            SweepFrequencyFrom = 0;
            SweepFrequencyTo = 0;
            try
            {
                //xu ly chuoi nhap dang x-y
                if (range.Contains("-"))
                {
                    string[] tmp1 = range.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp1.Length == 2)
                    {
                        int a = int.Parse(tmp1[0]);
                        int b = int.Parse(tmp1[1]);
                        SweepFrequencyFrom = Math.Min(a, b);
                        SweepFrequencyTo = Math.Max(a, b);
                        //Log($"{Alias}:Sweeping Range [{SweepFrequencyFrom} - {SweepFrequencyTo}]");
                        tmp = true;
                    }
                }
            }
            catch(Exception e)
            {
                Log("Reading Sweeping Frequancy Range => ERROR! " + e.Message, LogLevel.ERROR);
            }
            return tmp;
        }

        public void AssignAlias(string name)
        {
            switch (instrumentVSG)
            {
                case "N5182B":
                    N5182b.Alias = name;
                    break;
                case "CMW100A":
                    cmw100.Alias = name;
                    break;
                case "SGS100A":
                    sgs100a.Alias = name;
                    break;
                case "SMW200A":
                    smw200a.Alias = name;
                    break;
                default:
                    break;
            }
            Alias = name;
        }



        public async Task<bool> IsConnectedAsync()
        {
            bool tmp = false;
            string id = GetIDNumber();
            if(id!=null || id!="")
            {
                tmp = true;
            }
            return tmp;
        }
    }
}
