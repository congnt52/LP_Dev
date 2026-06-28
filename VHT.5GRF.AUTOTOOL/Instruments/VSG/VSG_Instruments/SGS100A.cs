using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Agilent.SA.Vsa;
using Newtonsoft.Json.Linq;

namespace _5GAutoTool
{
    public class SGS100A
    {
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 sgs100a = new Ivi.Visa.Interop.FormattedIO488();

        public Form5GAT mainForm;
        public string Alias;
        public string Name = "SGS100A";
        private double currFreq = 0;
        private double currPowerLevel = 0;
        public SGS100A(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        object[] tmp;
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "SGS100A");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        public void Connection(string IP, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            try
            {
                sgs100a.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                sgs100a.WriteString(Cmd, true);
                sgs100a.IO.Timeout = 2000;
                tmp = (object[])sgs100a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Manufacturer = tmp[0].ToString();
                Model = tmp[1].ToString();
                Serial = tmp[2].ToString();
                if (tmp[1].ToString() == "SGS100A")
                {
                    Log("SGS100A: Connected IP: " + IP, LogLevel.SUCCESS);
                }
                else
                {
                    Log("ERROR: SGS100A Connection fail", LogLevel.ERROR);
                }
                Thread.Sleep(500);
                sgs100a.WriteString("*RST;", true);
                sgs100a.WriteString("*CLS;", true); // *CLS; *OPC?
                Name += $"-IP:{IP}";
            }
            catch (Exception e)
            {
                Log("ERROR:<SGS100A> Connection Request error! " + e.Message, LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (sgs100a != null)
                {
                    sgs100a.IO.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SGS100A> Disconnection Request error! " + e.Message);
            }

        }
        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                sgs100a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])sgs100a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception e)
            {
                Log("ERROR:<SGS100A> Cannot send command to device! " + e.Message, LogLevel.ERROR);
            }
        }
        public void SetFreq(string freqSet)
        {
            try
            {
                double.TryParse(freqSet, out double tmp);
                if(tmp!=currFreq)
                {
                    //send command to setup frequency value and get response
                    sgs100a.WriteString($":SOUR:FREQ:CW {freqSet}; *OPC?", true);
                    double response = sgs100a.ReadNumber();

                    if (response == 1)
                    {
                        Log($"SGS100A: Source frequency= {tmp / 1e6} MHz");
                        currFreq = tmp;
                    }
                    else
                    {
                        Log($"SGS100A: isBusy!", LogLevel.WARN);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SGS100A> cannot set frequency! " + e.Message);
            }
        }
        public void SetPower(string setPower)
        {
            try
            {
                double.TryParse(setPower, out double tmp);
                if (tmp != currPowerLevel)
                {
                    //send command to setup frequency value and get response
                    sgs100a.WriteString($":SOUR:POW:LEV:IMM:AMPL {setPower}; *OPC?", true);
                    double response = sgs100a.ReadNumber();

                    if (response == 1)
                    {
                        Log($"SGS100A: Source power level = {setPower} dBm");
                        currPowerLevel = tmp;
                    }
                    else
                    {
                        Log($"SGS100A: isBusy!", LogLevel.WARN);
                    }
                }                    
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SGS100A> cannot set power level! " + e.Message);
            }
        }
       
        public void RFOnOff(int ON_OFF)
        {
            try
            {
                sgs100a.WriteString($":OUTP:STAT {ON_OFF}; *OPC?", true);
                double response = sgs100a.ReadNumber();
                if (response == 1)
                {
                    Log($"SGS100A: RF State = {ON_OFF}");
                }
                else
                {
                    Log($"SGS100A: isBusy!", LogLevel.WARN);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SGS100A> cannot set output port ON/OFF! " + e.Message);
            }
        }
        public void LoadUserCorrectionSettingFile(string corrPath)
        {
            try                
            {                
                sgs100a.WriteString($":SOUR:CORR:CSET '{corrPath}'", true);
                Log($"SGS100A: Load User Correction Data from Path: {corrPath}");
                //Activates user correction
                sgs100a.WriteString($":SOUR:CORR:STAT ON; *OPC?", true);

                double response = sgs100a.ReadNumber();
                if (response == 1)
                {
                    Log($"SGS100A: UCOR State is: ON ");
                }
                else
                {
                    Log($"SGS100A: isBusy!", LogLevel.WARN);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SGS100A> cannot call User Correction Setting! "+ e.Message);
            }
        }
    }
}
