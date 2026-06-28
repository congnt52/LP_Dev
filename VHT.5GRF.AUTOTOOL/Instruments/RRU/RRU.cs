using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SSH = Renci.SshNet;

namespace _5GAutoTool
{
    //TODO: MINHNT59 - define RRU's Properties
    public enum RRUType
    {
        Hera,
        Pluto
    }
    public enum RFChannels
    {
        BOT,    //bottom
        MID,    //middle
        TOP,    //top
        SET,    //User set
    }
    public enum TestModel
    {
        NRTM11,
        NRTM12,
        NRTM2,
        NRTM2a,
        NRTM31,
        NRTM31a,
        NRTM32,
        NRTM33,
    }
    public enum TestCase
    {
        TX,
        RX,
    }
    public enum DownlinkSpec
    {

    }

    public enum UplinkSpec
    {

    }


    public class RRU
    {
        private Form5GAT mainForm;

        public RRUType Type = RRUType.Pluto;
        public string IpAddress = "192.168.120.2";
        public string Serial = "MRUxxxxxx";
        public string Status = "No connection";
        public string User = "root";
        public string Password = "root";
        public string Version = "";
        public string Name = "RRU";

        public double SetPowerLevel;
        public double SetBandWidth;
        public RFChannels SetRFChannel;
        public double ChannelBWBottom;
        public double ChannelBWTop;
        public double SetFrequencyValue;
        public TestCase Testcase;
        public string TestModel;
        public string CurrentTestingSpec;
        public int CurrentTestingPort;
        public double SetAttValue;

        public string AttenuatorCSVpath = "";
        //public List<double> AttennuatorsTX = new List<double>();
        //public List<double> AttennuatorsRX = new List<double>();
        public List<double> Attenuators = new List<double>();
        public RRU(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }

        SSH.SshClient evt1;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,Name,"RRU");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        string path = "export PATH=\"/usr/local/bin:/usr/bin:/bin:/usr/local/sbin:/usr/sbin:/sbin:/nr_sw/rru/running/bin:/nr_sw/rru/running/cfg:/nr_sw/rru/running/scripts:/nr_sw/rru/running/lib:/nr_sw/rru/running/inc:/tmp\"";
        public void RRUVersion(string Version_RRU)
        {
        }

        public void Connection(string ip, string user, string password, out bool flag)
        {
            //Minhnt59-test
            IpAddress = ip;
            User = user;
            Password = password;

            flag = false;
            try
            {
                evt1 = new SSH.SshClient(IpAddress, User, Password);
                evt1.Connect();
                evt1.ConnectionInfo.Timeout = TimeSpan.FromSeconds(2);
                flag = evt1.IsConnected;
                if (flag)
                {
                    Log($"{IpAddress} connection: SUCCESS", LogLevel.SUCCESS);
                    var Cmd = evt1.CreateCommand(path + "&&" + " echo $PATH");                    
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Log($"Data: {data}");

                    Cmd = evt1.CreateCommand(path + "&&" + "rftool freq");
                    Cmd.Execute();
                    data = Cmd.Result;
                    Log($"Data: {data}");
                    Status = "Connected";
                }
                else
                {
                    Log($"{IpAddress} connection: FAIL", LogLevel.ERROR);
                }
            }
            catch
            {
                Log($"{IpAddress} connection: FAIL", LogLevel.ERROR);
            }
        }
        //==========================================
        //TODO: MINHNT59 - rewrite RRU connection
        public void Connection(out bool flag)
        {
            flag = false;
            try
            {
                evt1 = new SSH.SshClient(IpAddress, User, Password);
                evt1.Connect();
                evt1.ConnectionInfo.Timeout = TimeSpan.FromSeconds(2);
                flag = evt1.IsConnected;
                if (flag)
                {
                    Log($"{IpAddress} connection: SUCCESS", LogLevel.SUCCESS);
                    var Cmd = evt1.CreateCommand(path + "&&" + " echo $PATH");
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Log($"Data: {data}");

                    Cmd = evt1.CreateCommand(path + "&&" + "rftool freq");
                    Cmd.Execute();
                    data = Cmd.Result;
                    Log($"Data: {data}");
                    Status = "Connected";
                }
                else
                {
                    Log($"{IpAddress} connection: FAIL", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log($"{IpAddress} connection: FAIL " +ex.Message, LogLevel.ERROR);
            }
        }
        //=====================================================================
        public void DownlinkConfig()
        {
            string Command = string.Format("cd /nr_sw/rru/running/scripts;" + "./fronthaul_reset_nocom;" + "./fpgaconfig_nocom;" + "./switch_calib.sh");
            var RunConfig = evt1.CreateCommand(Command);
            RunConfig.Execute();
        }
        public void UplinkConfig()
        {
            string Command = string.Format("cd /nr_sw/rru/running/scripts;" + "./fronthaul_reset;" + "./fpgaconfig;" + "./switch_calib.sh");
            var RunConfig = evt1.CreateCommand(Command);
            RunConfig.Execute();
        }
        public void Setfreq(string freq)
        {
            //Minhnt59-test
            //double.TryParse(freq, out double freqSet);
            //SetFrequencyValue = double.Parse(freq);
            try
            {
                if (evt1.IsConnected)
                {
                    var Cmd = evt1.CreateCommand(path + "&&" + "rftool freq " + freq);
                    Log("rftool freq " + freq);
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Log($"Data: {data}");
                    SetFrequencyValue = double.Parse(freq);
                }
                else
                {
                    Log("No connection", LogLevel.WARN);
                }
            }
            catch (Exception e)
            {
                Log("Set frequency: FAIL " + e.Message, LogLevel.ERROR);
            }
        }
        //===================================
        ////TODO: MINHNT59 - rewrite RRU SetFreq
        public void Setfreq()
        {
            try
            {
                if (evt1.IsConnected)
                {
                    var Cmd = evt1.CreateCommand(path + "&&" + "rftool freq " + SetFrequencyValue.ToString());
                    Log("rftool freq " + SetFrequencyValue.ToString());
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Log(data);

                }
                else
                {
                    Log("No connection", LogLevel.WARN);
                }
            }
            catch (Exception e)
            {
                Log("Set frequency: FAIL " + e.Message, LogLevel.ERROR);
            }
        }

        //===================================
        //TODO: MINHNT59 - Read Attenuator from CSV File (separated CSVfile)
        public void ReadATTfromCSVdir(string dir, string format, double FindFreq)
        {
            if (AttenuatorCSVpath != dir || SetFrequencyValue != FindFreq)
            {
                //LogDashedLine($"RRU ATTENUATORS");
                //Log($"Reading RRU attenuators: Set frequency = {FindFreq}\tSource = {dir}");
                var list = Directory.GetFiles(dir, "*.csv");
                for (int j = 1; j <= list.Count(); j++)
                {
                    string path = $@"{dir}\{format}{j}.csv";
                    Attenuators.Add(ReadATTfromCSVFile(path, FindFreq));
                }
                //LogDashedLine();

                //update properties
                AttenuatorCSVpath = dir;
                SetFrequencyValue = FindFreq;
            }
        }
        //=====================================
        //TODO: 24.07.2024 - MINHNT59 - update function read att from csv with variable delimiters
        public double ReadATTfromCSVFile(string path, double FindFreq)
        {
            double tempVal = 0.0;
            double min = 1.0, max = 100.0;
            if (File.Exists(path))
            {
                try
                {
                    double tmp = 10.0;
                    var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        HasHeaderRecord = true,
                        Comment = '#',
                        AllowComments = true,
                        //Delimiter = ",",
                        DetectDelimiter = true,
                        DetectDelimiterValues = new[] { ",", ";", "\t", "|" },
                    };
                    using (var streamReader = new StreamReader(path, Encoding.UTF8))
                    using (var csv = new CsvReader(streamReader, csvConfig))
                    {

                        var records = csv.GetRecords<dynamic>();
                        foreach (var record in records)
                        {
                            if (double.TryParse(csv.GetField(0), out double freq))
                            {
                                tmp = Math.Abs((FindFreq - freq) / 1000000);
                                if (tmp < min)
                                {
                                    double.TryParse(csv.GetField(1), out tempVal);
                                    tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
                                    break;
                                }
                                else if (tmp <= max)
                                {
                                    double.TryParse(csv.GetField(1), out tempVal);
                                    tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
                                    max = tmp;
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"Read data file:{path} => ERROR " + ex.Message, LogLevel.ERROR);
                }
            }
            else Log($"[RRU ReadATTfromCSVFile] CSVFile:{path} not exist!", LogLevel.WARN);

            return tempVal;
        }
        //=====================================
        ////=====================================
        ////TODO: MINHNT59 - Read Attenuator of specific port from CSV File (separated CSVfile)
        //public double ReadATTfromCSVFile(string path, double FindFreq)
        //{
        //    double tempVal = 0.0;
        //    if (File.Exists(path))
        //    {                
        //        try
        //        {                   
        //            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        //            {
        //                HasHeaderRecord = true,
        //                Comment = '#',
        //                AllowComments = true,
        //                //Delimiter = ",",
        //            };
        //            var streamReader = new StreamReader(path, Encoding.UTF8);
        //            var CSV = new CsvReader(streamReader, csvConfig);
        //            var delimiters = new List<string> { ",", ";", "\t", "|"};
        //            double min = 1.0, tmp = 10.0, max=100.0;

        //            for (int i = 0; i < 3; i++)
        //            {
        //                if (!CSV.Read()) break;
        //            }

        //            while (CSV.Read())
        //            {
        //                double.TryParse(CSV.GetField(0), out double freq); //first column contains range of Frequency
        //                tmp = Math.Abs((FindFreq - freq) / 1000000);
        //                if (tmp < min)
        //                {
        //                    double.TryParse(CSV.GetField(1), out tempVal);
        //                    tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
        //                    break;
        //                }
        //                else if(tmp<=max)
        //                {
        //                    double.TryParse(CSV.GetField(1), out tempVal);
        //                    tempVal = Math.Abs(Math.Round(tempVal, 3));  //lay ve gia tri duong
        //                    max = tmp;
        //                }
        //            }
        //            CSV.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            Error(ex, $"[RRU ReadATTfromCSVFile] Read ATT from CSVFile:{path} got error!");
        //        }
        //    }
        //    else Log($"[RRU ReadATTfromCSVFile] CSVFile:{path} not exist!");

        //    return tempVal;
        //}
        ////=====================================
        public void SetGainTX(string port, double gain)
        {
            try
            {
                if (evt1.IsConnected)
                {
                    var Cmd = evt1.CreateCommand(path + "&&" + "rftool gain tx " + port + " " + gain);
                    Log("rftool gain tx " + port + " " + gain);
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Log(data);
                }
                else
                {
                    Console.WriteLine("Khong co ket noi");
                }
            }
            catch (Exception e)
            {
                Log("ERROR: RRU Set Gain Fail" + "\n" + e.Message, LogLevel.ERROR);
            }
        }

        public void sendMeasValue(string port, string mode, double measValue)
        {
            try
            {
                if (evt1.IsConnected)
                {
                    var Cmd = evt1.CreateCommand(path + "&&" + "calibtool tx meas " + mode + " " + port + " " + measValue);
                    Log("calibtool tx meas " + mode + " " + port + " " + measValue);
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Log(data);
                }
                else
                {
                    Console.WriteLine("Khong co ket noi");
                }
            }
            catch
            {
                Log("ERROR: send measurement power Fail", LogLevel.ERROR);
            }
        }
        public void calib(string port, string lowRange, string highRange)
        {
            try
            {
                if (evt1.IsConnected)
                {
                    var Cmd = evt1.CreateCommand(path + "&&" + "calibtool tx calib " + port + " " + lowRange + " " + highRange);
                    Log("calibtool tx calib " + port + " " + lowRange + " " + highRange);
                    Cmd.Execute();
                    string data = Cmd.Result;
                    Console.WriteLine(data);
                }
                else
                {
                    Console.WriteLine("Khong co ket noi");
                }

            }
            catch
            {
                Log("ERROR: RRU calib Fail", LogLevel.ERROR);
            }
        }

        public void ShowInfoCommand()
        {
            Log("RRU information:");
            Log($"S/N:{Serial}\t| Type:{Type}\t | Version:{Version}");
            Log($"IP:{IpAddress}\tStatus:{Status}");
            Log($"Set Power Level:{SetPowerLevel}\tSet Frequency:{SetFrequencyValue / 1000000}\tChannel BW:{ChannelBWBottom / 1000000} - {ChannelBWTop / 1000000}MHz");
            if (Testcase == TestCase.TX) Log($"Test Model: {TestModel}");
            Log($"Current Testing Specification: {CurrentTestingSpec}");
            Log($"RF Port: {CurrentTestingPort}\tRF Loss (Attenuator):{Attenuators[CurrentTestingPort - 1]}");
            Log($"RF Loss CSV File path:{AttenuatorCSVpath}");

        }
    }
}
