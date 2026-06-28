using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Renci.SshNet;

namespace _5GAutoTool
{
    class Server
    {
        public Form5GAT mainForm;
        public string Name = "Server";
        public Server(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        bool IsConnected = false;
        TcpClient client;
        private const int BUFFER_SIZE = 1024;
        public class Data
        {
            public string sn_trx { get; set; }
            public string sn_pa { get; set; }
            public string sn_fil { get; set; }
            public string sn_pwr { get; set; }
            public string sn_ant { get; set; }
        }
        string[] tmp = new string[5];

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "Server");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Connect(string sn_rru, out string[] sn)
        {
            sn = tmp;
            try
            {
                client = new TcpClient();

                // 1. connect
                client = new TcpClient("127.0.0.1", 1308);
                Stream stream = client.GetStream();

                // 2. send
                string message = "{sn_rru:" + "\"" + sn_rru + "\"" + "}";
                Byte[] data = Encoding.ASCII.GetBytes(message);
                // send to connected
                stream.Write(data, 0, data.Length);

                // 3. receive
                data = new byte[BUFFER_SIZE];
                stream.Read(data, 0, BUFFER_SIZE);
                string response = Encoding.ASCII.GetString(data);
                Data json = JsonConvert.DeserializeObject<Data>(response);
                dynamic js = JsonConvert.DeserializeObject(response);
                Console.WriteLine(js);
                tmp[0] = json.sn_trx;
                tmp[1] = json.sn_pa;
                tmp[2] = json.sn_fil;
                tmp[3] = json.sn_pwr;
                tmp[4] = json.sn_ant;
                // 4. Close
                stream.Close();
                client.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
            private string host = "192.168.61.100";
            private string user = "tester";
            private string password = "password";

            string directoryPath = @"C:\Data OQC\32T32R\logs\";
            string fileSendPass = @"C:\Data OQC\32T32R\logs\SendtoServerPass.txt";
            string fileSendFail = @"C:\Data OQC\32T32R\logs\SendtoServerFail.txt";
            DirectoryInfo directory;
            FileStream fs;
            StreamWriter sWriter;
            StreamReader sReader;
            string[] arr = new string[200];
            public void Send(string mode, string sourceFile)
            {

                using (SftpClient client = new SftpClient(new PasswordConnectionInfo(host, user, password)))
                {
                    try
                    {
                        client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(3);
                        client.Connect();
                        if (mode == "TX")
                        {
                            client.ChangeDirectory("/sftp/report/3.2 Do kiem Tx"); //data\sftp\report\3.2 Do kiem Tx, Rx);
                        }
                        else if (mode == "RX")
                        {
                            client.ChangeDirectory("/sftp/report/3.3 Do kiem Rx"); //data\sftp\report\3.2 Do kiem Tx, Rx);
                        }

                        if (client.IsConnected)
                        {
                            // Gửi file vừa đo
                            if (sourceFile != null)
                            {
                                using (FileStream fs = new FileStream(sourceFile, FileMode.Open))
                                {
                                    //client.BufferSize = 1024;
                                    client.UploadFile(fs, Path.GetFileName(sourceFile));
                                }
                                txtSendPass(sourceFile);
                                Console.WriteLine("Send " + sourceFile + " to SFTP Sever: " + host);
                            }
                            // Gửi file chưa gửi được do lỗi mạng
                            if (File.Exists(fileSendFail))
                            {
                                ReadLogFileSendFileFail();
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (arr[i].Trim() != string.Empty)
                                    {
                                        using (FileStream fs = new FileStream(arr[i].Trim(), FileMode.Open))
                                        {
                                            //client.BufferSize = 1024;
                                            client.UploadFile(fs, Path.GetFileName(arr[i].Trim()));
                                        }
                                        txtSendPass(arr[i].Trim());
                                        Console.WriteLine("Send " + arr[i].Trim() + " to SFTP Sever: " + host);
                                    }
                                }
                                DeleteLogFileSendFileFail();
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Send file to SFTP server: Fail" + "\n" + e.Message);
                        txtSendFail(sourceFile);
                    }
                }
            }
            private void txtSendFail(string file)
            {
                directory = new DirectoryInfo(directoryPath);
                directory.Create();
                fs = new FileStream(fileSendFail, FileMode.Append);
                sWriter = new StreamWriter(fs, Encoding.UTF8);
                sWriter.WriteLine(file);
                sWriter.Flush();
                //sWriter.Close();
                sWriter.Dispose();
            }
            private void txtSendPass(string file)
            {
                directory = new DirectoryInfo(directoryPath);
                directory.Create();
                fs = new FileStream(fileSendPass, FileMode.Append);
                sWriter = new StreamWriter(fs, Encoding.UTF8);
                sWriter.WriteLine(file);
                sWriter.Flush();
                sWriter.Dispose();
            }

            private void ReadLogFileSendFileFail()
            {
                try
                {
                    string content = null;
                    sReader = new StreamReader(fileSendFail);
                    content = sReader.ReadToEnd();
                    sReader.Dispose();
                    arr = content.Split('\n');
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("không thể đọc file:");
                    Console.WriteLine(e.Message);
                }
            }
            private void DeleteLogFileSendFileFail()
            {
                try
                {
                    File.Delete(fileSendFail);
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("không thể xóa file:");
                    Console.WriteLine(e.Message);
                }
            }

        
    }
}
