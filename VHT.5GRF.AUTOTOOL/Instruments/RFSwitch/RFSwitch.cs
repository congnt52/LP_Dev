using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;

namespace _5GAutoTool
{
    public class RFSwitch
    {
        public string Name = "ZTVX";
        public string IpAddress;
        public int Port = 23;
        public string Serial;
        public string Model;
        public string Manufacturer = "Mini-Circuits";
        public string Status;
        public string CurrentNodeA;
        public string CurrentNodeN;
        Telnet rfs;
        private Form5GAT mainForm;
        public RFSwitch(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        //public RFSwitch() { }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "RFSwitch");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Connection(string ip, int port, out bool flagcon)
        {
            Status = "No Connection";
            int connReturn = 0; // initial
            flagcon = false;
            try
            {
                rfs = new Telnet(ip, port);
                rfs.WriteLine(":MN?");
                Log($"[RFS] [Querry] SEND::MN?");
                string s = rfs.Read();
                connReturn = s.IndexOf("MN=ZTVX-16-18-S") + s.IndexOf("MN=ZTVX-8-18-S");
                if (s.Contains("MN=ZTVX")) //Model ZTVX
                {
                    flagcon = true;
                    Log($"[RFSwitch] IP:{ip} is connected successfully", LogLevel.SUCCESS);


                    Status = "Connected";
                    IpAddress = ip;
                    Port = port;
                    Model = s.IndexOf("MN=ZTVX-16-18-S") > 0 ? "ZTVX-16-18-S" : Model;
                    Model = s.IndexOf("MN=ZTVX-8-18-S") > 0 ? "ZTVX-8-18-S" : Model;
                    Name = $"ZTVX-IP:{IpAddress}";
                }
                else if (s.Contains("MN=RC")) //MOdel RC
                {
                    flagcon = true;
                    Log($"[RFSwitch] IP:{ip} is connected successfully", LogLevel.SUCCESS);


                    Status = "Connected";
                    IpAddress = ip;
                    Port = port;
                    Model = s.IndexOf("MN=RC-2SPDT-A18") > 0 ? "MN=RC-2SPDT-A18" : Model;
                    Name = $"RC-IP:{IpAddress}";
                }
                else
                {
                    Log($"[RFSwitch] IP:{ip} is no connection", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log($"[RFSwitch] IP:{ip} connection FAIL!" + ex.Message, LogLevel.ERROR);
            }
        }

        public void SetSwitch(string portA, string portN, out string Respond)
        {
            Respond = "No Connection";
            switch (Model)
            {
                case "MN=RC-2SPDT-A18":
                    {
                        try
                        {
                            if (rfs != null)
                            {
                                string sw = (portA == "A1" || portA == "A") ? "A" : "B"; //model 2SPDT only has SwitchA and SwitchB
                                string port = (portN == "N1" || portN == "0") ? "0" : "1"; //port A1->0, A2->1
                                rfs.WriteLine($"SET{sw}={port}");
                                Respond = (rfs.Read().Trim() == "1") ? "1 - Success" : rfs.Read().Trim();
                                Log($"[RFS][{IpAddress}]: switched port  {portA} : {portN}");
                                CurrentNodeA = portA;
                                CurrentNodeN = portN;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"[RFS][{IpAddress}]: switch port FAIL!" + ex.Message, LogLevel.ERROR);
                        }
                    }
                    break;
                default:
                    {
                        try
                        {
                            if (rfs != null)
                            {
                                rfs.WriteLine(":PATH:" + portA + ":" + portN);
                                Respond = rfs.Read().Trim();
                                Log($"[RFS][{IpAddress}]: switched port  {portA} : {portN}");
                                CurrentNodeA = portA;
                                CurrentNodeN = portN;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"[RFS][{IpAddress}]: switch port FAIL!" + ex.Message, LogLevel.ERROR);
                        }
                    }
                    break;
            }


        }

        public void ShowInfoCommand()
        {
            Log($"RF Switch information: {Name}");
            Log($"Model:{Model}\tS/N:{Serial}\tManufacturer:{Manufacturer}");
            Log($"IP:{IpAddress}\tStatus:{Status}");
            Log($"Current Path: {CurrentNodeA} - {CurrentNodeN}");
        }
    }
}