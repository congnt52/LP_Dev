using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Agilent.SA.Vsa;
using SSH = Renci.SshNet;
using Renci.SshNet.Common;

namespace _5GAutoTool
{
    class XeonD
    {
        public Form5GAT mainForm;
        public string Name = "XeonD";
        public XeonD(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }

        SSH.SshClient xeonD;
        BackgroundWorker backgroundWorker_NRTM, backgroundWorker_RX;
        string ThroughputValue = "0";

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "XeonD");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Connection(string IP, string Username, string Password, out bool statusConnection)
        {
            statusConnection = false;
            try
            {
                xeonD = new SSH.SshClient(IP, Username, Password);
                xeonD.Connect();
                xeonD.ConnectionInfo.Timeout = TimeSpan.FromSeconds(2);
                statusConnection = xeonD.IsConnected;
                if (statusConnection == true)
                {
                    Log("XEON D: Connected: " + IP, LogLevel.SUCCESS);
                }
                else
                {
                    Log("ERROR: XEON connection fail", LogLevel.ERROR);
                }

            }
            catch
            {
                Log("ERROR: XEON Dconnection fail", LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (xeonD.IsConnected)
                {
                    xeonD.Disconnect();
                    Log("XEON D Disconnected", LogLevel.WARN);
                }
            }
            catch
            {

            }
        }
        #region hardcode
        public void GenerateNRTM(string NRTM_xx)
        {
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            backgroundWorker_NRTM = new BackgroundWorker();
            backgroundWorker_NRTM.WorkerSupportsCancellation = true;
            if (NRTM_xx == "NRTM_11")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM11;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM11");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_12")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM12;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM12");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_20")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM20;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM20");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_20a")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM20a;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM20a");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_31a")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM31a;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM31a");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_31")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM31;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM31");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_32")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM32;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM32");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if (NRTM_xx == "NRTM_33")
            {
                backgroundWorker_NRTM.DoWork += GenerateNRTM33;
                if (!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running NR-TM33");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else if(NRTM_xx == "TAE")
            {
                backgroundWorker_NRTM.DoWork += GenerateTAEdata;
                if(!backgroundWorker_NRTM.IsBusy)
                {
                    backgroundWorker_NRTM.RunWorkerAsync();
                    Log("XEON D: Running TAE data");
                    System.Threading.Thread.Sleep(20000);
                }
            }
            else
            {
                Log("ERROR: Generate NRTM: FAIL", LogLevel.ERROR);
            }
        }
        public void GenerateTAEdata(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G;" +
                " ./run_o_du_4t4r_TAE.sh");
                Log("XEON D: Run TAE data");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run TAE data", LogLevel.ERROR);
            }
        }
        public void GenerateNRTM11(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G;" +
                "./run_o_du_4t4r_TM1.1.sh");
                Log("XEON D: Run NR-TM11");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM11", LogLevel.ERROR);
            }
        }

        public void GenerateNRTM12(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM1.2.sh");
                Log("XEON D: Run NR-TM12");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                string data = GenNRTM.Result;
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM12", LogLevel.ERROR);
            }
        }
        public void GenerateNRTM20(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM2.0.sh");
                Log("XEON D: Run NR-TM20");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM20", LogLevel.ERROR);
            }
        }

        public void GenerateNRTM20a(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM2.0a.sh");
                Log("XEON D: Run NR-TM20a");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM20a", LogLevel.ERROR);
            }
        }

        public void GenerateNRTM31(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM3.1.sh");
                Log("XEON D: Run NR-TM31");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM31", LogLevel.ERROR);
            }
        }

        public void GenerateNRTM31a(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM3.1a.sh");
                Log("XEON D: Run NR-TM31a");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM31a", LogLevel.ERROR);
            }
        }

        public void GenerateNRTM32(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM3.2.sh");
                Log("XEON D: Run NR-TM32");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
                if (backgroundWorker_NRTM.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM32", LogLevel.ERROR);
            }
        }

        public void GenerateNRTM33(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (backgroundWorker_NRTM.CancellationPending) return;
                string sGeneratNRTestModel = string.Format
                ("cd /home/test3gpprru/oran/downlinkc6/oran_hardcode_25G/;" +
                "./run_o_du_4t4r_TM3.3.sh");
                Log("XEON D: Run NR-TM33");
                var GenNRTM = xeonD.CreateCommand(sGeneratNRTestModel);
                GenNRTM.Execute();
            }
            catch (Exception)
            {
                Log("ERROR: can not run NR-TM33", LogLevel.ERROR);
            }
        }

        public void KillHardcode()
        {
            try
            {
                if (backgroundWorker_NRTM != null)
                {
                    backgroundWorker_NRTM.CancelAsync();
                    var checkNRTMCmd = xeonD.CreateCommand("kill -9 $(pidof sample-app)");
                    checkNRTMCmd.Execute();
                    Log("Xeon D : kill -9 `pgrep pcie`");
                }
            }
            catch (Exception)
            {
                Log("ERROR: XEON kill pcie fail", LogLevel.ERROR);
            }
        }
        #endregion
        /*.............................................Uplink......................................................................................*/


        public void RXSetup(string Mode)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                backgroundWorker_RX = new BackgroundWorker();
                backgroundWorker_RX.WorkerSupportsCancellation = true;

                if (Mode == "L1sh_Port0")
                {
                    backgroundWorker_RX.DoWork += L1sh_Port1;
                    if (!backgroundWorker_RX.IsBusy)
                        backgroundWorker_RX.RunWorkerAsync();
                }
                else if (Mode == "L1sh_Port1")
                {
                    backgroundWorker_RX.DoWork += L1sh_Port2;
                    if (!backgroundWorker_RX.IsBusy)
                        backgroundWorker_RX.RunWorkerAsync();
                }
                else if (Mode == "L1sh_Port2")
                {
                    backgroundWorker_RX.DoWork += L1sh_Port3;
                    if (!backgroundWorker_RX.IsBusy)
                        backgroundWorker_RX.RunWorkerAsync();
                }
                else if (Mode == "L1sh_Port3")
                {
                    backgroundWorker_RX.DoWork += L1sh_Port4;
                    if (!backgroundWorker_RX.IsBusy)
                        backgroundWorker_RX.RunWorkerAsync();
                }
                else if (Mode == "L1Tae")
                {
                    backgroundWorker_RX.DoWork += L1Tae;
                    if (!backgroundWorker_RX.IsBusy)
                        backgroundWorker_RX.RunWorkerAsync();
                }
                else
                {
                    Log("Mode Error", LogLevel.ERROR);
                }

            }
            catch (Exception)
            {
            }
        }

        public void L1sh_Port1(object sender, DoWorkEventArgs e)
        {
            string Command = string.Format("cd /home/test3gpprru/oran/uplinkc7_25G;" +
            "./pre_run_l1.sh port1");
            var TransferCardCmd = xeonD.CreateCommand(Command);
            TransferCardCmd.CommandTimeout = TimeSpan.FromSeconds(25);
            try
            {
                TransferCardCmd.Execute();
                string data = TransferCardCmd.Result;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                if (backgroundWorker_RX.CancellationPending) return;
            }
            catch (Exception ex)
            {
                Log("ERROR: can not run L1.sh" + "\n" + ex.Message, LogLevel.ERROR);
            }
        }
        public void L1sh_Port2(object sender, DoWorkEventArgs e)
        {
            string Command = string.Format("cd /home/test3gpprru/oran/uplinkc7_25G;" +
            "./pre_run_l1.sh port2");
            var TransferCardCmd = xeonD.CreateCommand(Command);
            TransferCardCmd.CommandTimeout = TimeSpan.FromSeconds(25);
            try
            {
                TransferCardCmd.Execute();
                string data = TransferCardCmd.Result;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                if (backgroundWorker_RX.CancellationPending) return;
            }
            catch (Exception ex)
            {
                Log("ERROR: can not run L1.sh", LogLevel.ERROR);
            }
        }
        public void L1sh_Port3(object sender, DoWorkEventArgs e)
        {
            string Command = string.Format("cd /home/test3gpprru/oran/uplinkc7_25G;" +
            "./pre_run_l1.sh port3");
            var TransferCardCmd = xeonD.CreateCommand(Command);
            TransferCardCmd.CommandTimeout = TimeSpan.FromSeconds(25);
            try
            {
                TransferCardCmd.Execute();
                string data = TransferCardCmd.Result;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Thread.Sleep(1);
                if (backgroundWorker_RX.CancellationPending) return;
            }
            catch (Exception ex)
            {
                Log("ERROR: can not run L1.sh", LogLevel.ERROR);
            }
        }
        public void L1sh_Port4(object sender, DoWorkEventArgs e)
        {
            string Command = string.Format("cd /home/test3gpprru/oran/uplinkc7_25G;" +
            "./pre_run_l1.sh port4");
            var TransferCardCmd = xeonD.CreateCommand(Command);
            TransferCardCmd.CommandTimeout = TimeSpan.FromSeconds(25);
            try
            {
                TransferCardCmd.Execute();
                string data = TransferCardCmd.Result;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Thread.Sleep(1);
                if (backgroundWorker_RX.CancellationPending) return;
            }
            catch (Exception ex)
            {
                Log("ERROR: can not run L1.sh", LogLevel.ERROR);
            }
        }

        public void ThroughPut(string mode, out string through_put_value)
        {
            through_put_value = "";
            string Command = string.Format("cd /home/test3gpprru/oran/uplinkc7_25G/testmac;" +
            "./l2.sh nSlot=2000 testfile=" + "\"" + "ul_c7/c7_" + mode + ".cfg" + "\"");
            var TransferCardCmd = xeonD.CreateCommand(Command);
            TransferCardCmd.CommandTimeout = TimeSpan.FromSeconds(25);
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                if (backgroundWorker_RX.CancellationPending) return;
                Log("Xeon D: Run L2.sh");
                TransferCardCmd.Execute();
                Thread.Sleep(20000);
                ThroughputValue = TransferCardCmd.Result;
                Console.WriteLine(ThroughputValue);
                int start = ThroughputValue.IndexOf("TP = ");
                through_put_value = ThroughputValue.Substring(start + 5, 4);
                Log("TP: " + through_put_value);
            }
            catch(Exception ex)
            {
                Log("ERROR: Xeon D: Can not find throughput: " + ex.Message, LogLevel.ERROR);
            }
        }

        public void CloseL1sh()
        {
            try
            {
                if (backgroundWorker_RX != null)
                {
                    System.Threading.Thread.Sleep(1);
                    if (backgroundWorker_RX.CancellationPending) return;
                    var checkNRTMCmd = xeonD.CreateCommand("ps -a");
                    checkNRTMCmd.Execute();
                    string data = checkNRTMCmd.Result;
                    checkNRTMCmd = xeonD.CreateCommand("pkill -9 testmac");
                    checkNRTMCmd.Execute();
                    checkNRTMCmd = xeonD.CreateCommand("pkill -9 nr_du_l1");
                    checkNRTMCmd.Execute();
                    Log("Xeon D: pkill testmac, l1");
                }
                else
                {
                    var checkNRTMCmd = xeonD.CreateCommand("ps -a");
                    checkNRTMCmd.Execute();
                    string data = checkNRTMCmd.Result;
                    checkNRTMCmd = xeonD.CreateCommand("pkill -9 testmac");
                    checkNRTMCmd.Execute();
                    checkNRTMCmd = xeonD.CreateCommand("pkill -9 nr_du_l1");
                    checkNRTMCmd.Execute();
                    Log("Xeon D: pkill testmac, l1");
                }

            }
            catch (Exception)
            {
                Log("ERROR: XEON pkill fail", LogLevel.ERROR);
            }
        }

        public void SwitchPort(string port)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                string Command = string.Format
                ("tf_fpgatool 0x41034 0x" + port);
                var SwitchPortCmd = xeonD.CreateCommand(Command);
                SwitchPortCmd.Execute();
                Log("Xeon D: tf_fpgatool 0x41034 0x" + port);

            }
            catch
            {
                Log("ERROR: Xeon D switch port fail", LogLevel.ERROR);
            }
        }

        public void L1Tae(object sender, DoWorkEventArgs e)
        {
            try
            {
                string Command = string.Format
                ("cd /home/test3gpprru/uplinkc7/bin/l1/;" +
                "./l1.sh -rfmode");
                var TransferCardCmd = xeonD.CreateCommand(Command);
                TransferCardCmd.Execute();
                string data = TransferCardCmd.Result;
                Log("Xeon D: Run L1Tae");
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                if (backgroundWorker_RX.CancellationPending) return;
            }
            catch (Exception)
            {
                Log("ERROR: Xeon D config L1.sh Tae fail", LogLevel.ERROR);
            }
        }
        public void L2Tae()
        {
            try
            {
                string Command = string.Format
                ("cd /home/test3gpprru/uplinkc7/bin/testmac/;" +
                "./l2.sh --testfile=testmac_tae.cfg");
                var TransferCardCmd = xeonD.CreateCommand(Command);
                TransferCardCmd.Execute();
                string data = TransferCardCmd.Result;
                Log("Run L2Tae");
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
            }
            catch (Exception)
            {
                Log("ERROR: Xeon D Config L2.sh Tae fail", LogLevel.ERROR);
            }
        }

        public void loginRRU()
        {
            try
            {
                xeonD = new SSH.SshClient("192.168.120.3", "root", "1");
                xeonD.Connect();
                var port = new SSH.ForwardedPortLocal("127.0.0.1", "172.168.14.2", 22);
                xeonD.AddForwardedPort(port);
                port.Start();
                //DriverLoad();
            }
            catch
            {
                Log("BBU and RRU are out of sync", LogLevel.WARN);
            }
        }
        public void cmd(string cmd)
        {
            try
            {
        
            }
            catch
            {

            }

        }

        public void test()
        {
            string Command = string.Format("pwd");
            var TransferCardCmd = xeonD.CreateCommand(Command);
            TransferCardCmd.CommandTimeout = TimeSpan.FromSeconds(5);
            try
            {
                Log("Xeon D: Run L2.sh");
                Thread.Sleep(20000);
                TransferCardCmd.Execute();
                ThroughputValue = TransferCardCmd.Result;
                Console.WriteLine(ThroughputValue);
            }
            catch (Exception e)
            {
                Log( e.Message, LogLevel.ERROR);
            }
        }
    }
}
