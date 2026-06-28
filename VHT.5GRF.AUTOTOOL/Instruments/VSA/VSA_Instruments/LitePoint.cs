using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.Office.Interop.Excel;
using System.Web;
using System.IO;
using Renci.SshNet.Sftp;
using Microsoft.Office.Interop.Word;

namespace _5GAutoTool
{
    public class LitePoint
    {
        IPAddress ipa;
        string manu;
        string name;
        int port;

        public TcpClient tcpClient;
        private int numberOfBytesRead = 0;
        bool bLineFeed = true;
        bool bCarriageReturn = true;

        public Form5GAT mainForm;
        public LitePoint(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, "IQFR1-RU");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        public void Connect(string ipa, int port, out string manu, out string model, out string serial)
        {
            manu = "ERROR";
            model = "ERROR";
            serial = "ERROR";
            String[] response;
            if (ipa != null)
            {
                this.ipa = IPAddress.Parse(ipa);
                this.port = port;

                try
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipa, port);
                    if (tcpClient.Connected)
                    {
                        SendCmd("*IDN?");
                        response = ReadResponse(1000).Split(',');
                        manu = response[0];
                        model = response[1];
                        serial = response[2];
                    }

                }
                catch (Exception ex)
                {
                    log.Log($"{ex}", LogLevel.ERROR);
                }
            }
            else { log.Log($"IP Address can not be null!"); }
        }
        public void Disconnect()
        {
            try
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                }
            }
            catch (Exception)
            {

            }
        }
        public string ReadFileSetup(string path)
        {
            StringBuilder sbFileContent = new StringBuilder();
            log.Log($"[IQFR1-RU] Reading setup file: {path} . . .");
            foreach (string line in File.ReadLines(path))
            {
                if(!line.StartsWith("#"))
                {
                    
                    sbFileContent.Append(line);
                }
            }
            return sbFileContent.ToString();
        }
        public void SendCmd(string sb)
        {
            if (tcpClient.Connected)
            {
                NetworkStream ns = this.tcpClient.GetStream();
                StreamWriter writer = new StreamWriter(ns);
                if (ns.CanWrite)
                {
                    writer.WriteLine($"{sb};");
                    log.Log($"[IQFR1-RU] sending command >> {sb}; {"\r\n"}");
                    writer.Flush();
                }
            }
            else
            {
                Log("Not connected to the device.", LogLevel.ERROR);
            }
        }
        public String ReadResponse(int iTimeOutMs)
        {
            string strResult = "";
            try
            {   
                NetworkStream ns = this.tcpClient.GetStream();
                StreamReader reader = new StreamReader(ns);
                
                ns.ReadTimeout = iTimeOutMs;
                if (ns.CanRead)
                {
                    strResult += reader.ReadLine();
                    log.Log($"[IQFR1-RU] response >> {strResult}", LogLevel.INFO);
                    numberOfBytesRead = strResult.Length;
                }
            }
            catch(Exception ex)
            {
                log.Log(ex.ToString(), LogLevel.ERROR);
                return "";
            }

            return strResult;
        }

        public void TXPower(int[] port, out string[] power)
        {
            power = new string[8];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\OutputPower.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    System.Threading.Thread.Sleep(1000);
                    String[] reponse = ReadResponse(2000).Split(',');
                    power[i] = reponse.Length > 1 ? reponse[1] : "ERROR";
                    log.Log($"[IQFR1-RU] [PORT{port[i]}] response >> {power[i]}");
                }
            }
        }

        public void ACLRPower(int[] port, string tM, out string[] aclr)
        {
            aclr = new string[8];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\ACLRPower.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    double alt1, alt2, adj1, adj2;
                    String[] reponse = ReadResponse(1000).Split(',');
                    alt1 = double.Parse(reponse[1]);
                    alt2 = double.Parse(reponse[5]);
                    adj1 = double.Parse(reponse[2]);
                    adj2 = double.Parse(reponse[4]);
                    double alt = Math.Min(Math.Abs(alt1), Math.Abs(alt2));
                    double adj = Math.Min(Math.Abs(adj1), Math.Abs(adj2));
                    aclr[i] = Math.Min(Math.Abs(alt), Math.Abs(adj)).ToString();
                    log.Log($"[IQFR1-RU] [PORT{port[i]}] response >> {aclr[i]}");
                }
            }
        }

        public void EVMFreqErr(int[] port, out string[] evm, out string[] Freq)
        {
            evm = new string[8];
            Freq = new string[8];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\EVMFreqErr.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            for(int i =0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    String[] reponse = ReadResponse(1000).Split(',');
                    evm[i] = reponse.Length > 0 ? reponse[5] : "ERROR";
                    Freq[i] = reponse.Length > 0 ? reponse[3] : "ERROR";
                    log.Log($"[IQFR1-RU] [PORT{port[i]}] response >> {evm[i]} , {Freq[i]}");
                }
            }
        }

        public void SEMMeas(int[] port, string TM, out string[] semLimit)
        {
            semLimit = new string[8];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\SEMMeas.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    double l1, l2, l3, r1, r2, r3;
                    String[] reponse = ReadResponse(1000).Split(',');
                    l1 = double.Parse(reponse[1]);
                    l2 = double.Parse(reponse[2]);
                    l3 = double.Parse(reponse[3]);
                    r1 = double.Parse(reponse[4]);
                    r2 = double.Parse(reponse[5]);
                    r3 = double.Parse(reponse[6]);
                    double l = Math.Min(Math.Min(l1, l2),l3);
                    double r = Math.Min(Math.Min(r1, r2), r3);
                    semLimit[i] = Math.Min(l,r).ToString();
                    log.Log($"[IQFR1-RU] [PORT{port[i]}] response >> {semLimit[i]}");
                }
            }
        }

        public void LoadMeasurementSetup(string NRTM, string freq)
        {
            string path = $"E:\\hieuvn1\\Docs\\LitePoint\\State\\MeasurementSetup_{NRTM}_2550MHz.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                SendCmd(line);
                if (line.Contains("?"))
                {
                    System.Threading.Thread.Sleep(3000);
                    string reponse = ReadResponse(1000);
                    log.Log($"[IQFR1-RU] response >> {reponse}");
                }
            }
        }

        public void OBWMeas(int[] port, string TM, out string[] obwMeas)
        {
            obwMeas = new string[8];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\OBWMeas.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    System.Threading.Thread.Sleep(1000);
                    String[] reponse = ReadResponse(2000).Split(',');
                    obwMeas[i] = reponse.Length > 1 ? reponse[1] : "ERROR";
                    log.Log($"[IQFR1-RU] [PORT{port[i]}] response >> {obwMeas[i]}");
                }
            }
        }
    }
}
