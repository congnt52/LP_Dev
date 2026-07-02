using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

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
            power = new string[port.Length];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\OutputPower.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int resultIndex = 0;

            for(int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    System.Threading.Thread.Sleep(1000);
                    String[] reponse = ReadResponse(2000).Split(',');
                    if (resultIndex < port.Length)
                    {
                        power[resultIndex] = reponse.Length > 1 ? reponse[1] : "ERROR";
                        log.Log($"[IQFR1-RU] [PORT{port[resultIndex]}] response >> {power[resultIndex]}");
                    }
                    resultIndex++;
                }
            }
        }

        public void ACLRPower(int[] port, string tM, out string[] aclr)
        {
            aclr = new string[port.Length];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\ACLRPower.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int resultIndex = 0;

            for(int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    double alt1, alt2, adj1, adj2;
                    String[] reponse = ReadResponse(1000).Split(',');
                    if (resultIndex < port.Length)
                    {
                        alt1 = double.Parse(reponse[1]);
                        alt2 = double.Parse(reponse[5]);
                        adj1 = double.Parse(reponse[2]);
                        adj2 = double.Parse(reponse[4]);
                        double alt = Math.Min(Math.Abs(alt1), Math.Abs(alt2));
                        double adj = Math.Min(Math.Abs(adj1), Math.Abs(adj2));
                        aclr[resultIndex] = Math.Min(Math.Abs(alt), Math.Abs(adj)).ToString();
                        log.Log($"[IQFR1-RU] [PORT{port[resultIndex]}] response >> {aclr[resultIndex]}");
                    }
                    resultIndex++;
                }
            }
        }

        public void EVMFreqErr(int[] port, out string[] evm, out string[] Freq)
        {
            evm = new string[port.Length];
            Freq = new string[port.Length];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\EVMFreqErr.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            int resultIndex = 0;

            for(int i =0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    String[] reponse = ReadResponse(1000).Split(',');
                    if (resultIndex < port.Length)
                    {
                        evm[resultIndex] = reponse.Length > 5 ? reponse[5] : "ERROR";
                        Freq[resultIndex] = reponse.Length > 3 ? reponse[3] : "ERROR";
                        log.Log($"[IQFR1-RU] [PORT{port[resultIndex]}] response >> {evm[resultIndex]} , {Freq[resultIndex]}");
                    }
                    resultIndex++;
                }
            }
        }

        public void SEMMeas(int[] port, string TM, out string[] semLimit)
        {
            semLimit = new string[port.Length];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\SEMMeas.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int resultIndex = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    double l1, l2, l3, r1, r2, r3;
                    String[] reponse = ReadResponse(1000).Split(',');
                    if (resultIndex < port.Length)
                    {
                        l1 = double.Parse(reponse[1]);
                        l2 = double.Parse(reponse[2]);
                        l3 = double.Parse(reponse[3]);
                        r1 = double.Parse(reponse[4]);
                        r2 = double.Parse(reponse[5]);
                        r3 = double.Parse(reponse[6]);
                        double l = Math.Min(Math.Min(l1, l2),l3);
                        double r = Math.Min(Math.Min(r1, r2), r3);
                        semLimit[resultIndex] = Math.Min(l,r).ToString();
                        log.Log($"[IQFR1-RU] [PORT{port[resultIndex]}] response >> {semLimit[resultIndex]}");
                    }
                    resultIndex++;
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
        // Thêm hàm LoadMeasurementSetup nhận tham số pathIndex (1 -> 4)
        public void LoadMeasurementSetup(string NRTM, string setFreq, int pathIndex)
        {
            try
            {
                // Định nghĩa đường dẫn động dựa trên pathIndex được truyền vào
                string basePath = @"E:\hieuvn1\Docs\LitePoint\State";
                string path = Path.Combine(basePath, $"Setup_Path{pathIndex}.txt");

                if (!File.Exists(path))
                {
                    Log($"Setup file not found: {path}", LogLevel.ERROR);
                    return;
                }

                string Cmd = ReadFileSetup(path);
                string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                Log($"[IQFR1-RU] Loading config for Path {pathIndex} from: {path}", LogLevel.INFO);

                foreach (string line in lines)
                {
                    string trimLine = line.Trim();
                    if (string.IsNullOrEmpty(trimLine)) continue;

                    // Thay thế tần số động nếu file config chứa tham số tần số
                    if (trimLine.Contains("[FREQ]"))
                    {
                        trimLine = trimLine.Replace("[FREQ]", setFreq);
                    }

                    SendCmd(trimLine);
                    if (trimLine.Contains("?"))
                    {
                        System.Threading.Thread.Sleep(500);
                        string response = ReadResponse(1000);
                        log.Log($"[IQFR1-RU] Path{pathIndex} response >> {response}");
                    }
                }
                Log($"[IQFR1-RU] Load setup for Path {pathIndex}: DONE", LogLevel.SUCCESS);
            }
            catch (Exception ex)
            {
                Log($"LoadMeasurementSetup Path {pathIndex} FAIL: " + ex.Message, LogLevel.ERROR);
            }
        }
        public void LoadMeasurementSetup(string NRTM, string freq, int[] port)
        {
            int pathIndex = GetPathIndex(port);
            if (pathIndex !=0)
            {
                //string path = $"E:\\hieuvn1\\Docs\\LitePoint\\State\\Path{pathIndex}\\MeasurementSetup_{NRTM}_2550MHz.txt";
                string path = $"E:\\hieuvn1\\Docs\\LitePoint\\State\\MeasurementSetup_{NRTM}_2550MHz_Path{pathIndex}.txt";
                log.Log($"[IQFR1-RU] Loading measurement setup (Path{pathIndex}): {path}");
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
            else
                Log($"Chua tich chon port", LogLevel.ERROR);

        }

        public void LoadMeasurementSetup_SEM(string NRTM, string freq, int[] port)
        {
            int pathIndex = GetPathIndex(port);
            if (pathIndex != 0)
            {
                //string path = $"E:\\hieuvn1\\Docs\\LitePoint\\State\\Path{pathIndex}\\MeasurementSetup_{NRTM}_2550MHz.txt";
                string path = $"E:\\hieuvn1\\Docs\\LitePoint\\State\\SEM_Path{pathIndex}.txt";
                log.Log($"[IQFR1-RU] Loading measurement setup (Path{pathIndex}): {path}");
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
            else
                Log($"Chua tich chon port", LogLevel.ERROR);

        }


        private int GetPathIndex(int[] port)
        {
            if (port == null || port.Length == 0) return 1; int idx = ((port[0] - 1) / 8) + 1; if (idx < 1) idx = 1; if (idx > 4) idx = 4; return idx;
        }


        /// <summary>
        /// Maps the first RRU port of a measurement group to its physical path (1..4).
        /// Path1 = P1-P8, Path2 = P9-P16, Path3 = P17-P24, Path4 = P25-P32.
        /// Falls back to Path1 when no port info is available.
        /// </summary>
        public void OBWMeas(int[] port, string TM, out string[] obwMeas)
        {
            obwMeas = new string[port.Length];
            string path = @"E:\hieuvn1\Docs\LitePoint\State\OBWMeas.txt";
            string Cmd = ReadFileSetup(path);
            string[] lines = Cmd.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int resultIndex = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                SendCmd(lines[i]);
                if (lines[i].Contains("?"))
                {
                    System.Threading.Thread.Sleep(1000);
                    String[] reponse = ReadResponse(2000).Split(',');
                    if (resultIndex < port.Length)
                    {
                        obwMeas[resultIndex] = reponse.Length > 1 ? reponse[1] : "ERROR";
                        log.Log($"[IQFR1-RU] [PORT{port[resultIndex]}] response >> {obwMeas[resultIndex]}");
                    }
                    resultIndex++;
                }
            }
        }
    }
}
