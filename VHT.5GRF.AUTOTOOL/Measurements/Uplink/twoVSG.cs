using _5GAutoTool.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Timer = System.Threading.Timer;

namespace _5GAutoTool
{
    class twoVSG
    {
        public Form5GAT mainForm;
        private static Timer timerSequence;
        private static bool flagStep = false;
        private List<string[]> oobCoData = new List<string[]>();
        private List<string[]> oobCoISsweepRanges = new List<string[]>();
        private bool L1TestMacV2Start = false;
        private bool L1TestMacV3Start = false;
        private int portIndex;
        public twoVSG(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string mode = "", old_mode = "";
        public void Reset()
        {
            mode = "";
            old_mode = "";
        }
        //string[] tmpArr = new string[4]; // 16.2.2024 - bo
        string[] tmpArr = null; //16.2.2024 - them moi
        List<string> csvHeader = new List<string> { "DATE", "TIME", "TYPE", "SPEC", "PORT", "WS FREQ", "WS PWR LVL", "WS RBOFS", "IS FREQ", "IS PWR LVL", "IS RBOFS", "THROUGHPUT" };
        PortMapping currPort;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "twoVSG");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        /// <summary>
        /// THỰC HIỆN CÁC BÀI ĐO CHƯỜNG 7 SỬ DỤNG 1 VSG TẠO TÍN HIỆU WANTED VÀ 1 VSG TẠO TÍN HIỆU NHIỄU
        /// Bao gồm các bài đo: ACS, ICS, NBB, INB, OOB
        /// </summary>   
        public void Measurement(VSG vsg1, VSG vsg2, BBU bbu, string mode, string WsRBOffset, string IsRBOffset, double step, string setAtt, int setPort, string wsPowerStr, string wsFreq, string isPowerStr, string isFreq, string isLowerFreqRange, string isUpperFreqRange, out double TP)
        {
            TP = -1;
            string throughput = "ERROR";
            string isPower = "";
            //ServerRedhat - finding PortMapping and switch group port
            currPort = mainForm.PortMappings.FirstOrDefault(item => item.Port == setPort);
            bbu.SwitchGroup(currPort.XRanIndex.ToString());
            //for debugging
            Log($"Current Testing Port = {currPort.Port}\tXran = {currPort.XRanIndex}\tStream = {currPort.StreamIndex}");

            //double _setAtt = double.Parse(setAtt);
            double _wsPower = double.Parse(wsPowerStr);
            double _isPower = double.Parse(isPowerStr);
            //double _wsATT = vsg1.Attenuators[setPort - 1];
            double _wsATT = double.Parse(setAtt);
            Log($"{vsg1.Name}: Expected Power Level={_wsPower}dBm, ATT_Port{setPort} Level={_wsATT}dB");
            string wsPower = (_wsPower + _wsATT).ToString("N3"); //công suất tai cong output cua VSG1 WS

            if (mode != "OOB2" && mode != "OOB1" && mode != "INB2")
            {
                //double _isATT = vsg2.Attenuators[setPort - 1];
                double _isATT = vsg2.AttenuatorLevel;
                Log($"{vsg2.Alias}: Expected Power Level={_isPower}dBm, ATT_Port{setPort} Level={_isATT}dB");
                isPower = (_isPower + _isATT).ToString("N3"); //công suất tai cong output cua VSG2 IS - 3 numerics
            }
            else
            {
                isPower = isPowerStr;
            }

            //doc gia tri suy hao cua VSGs
            //vsg1.ReadATTfromCSVdir();
            //vsg2.ReadAttennuatorVSGfromCSV();



            L1TestMacV2Start = false;
            L1TestMacV3Start = false;

            string isType = null;
            double _isFreqLimitLeft = mainForm.channelBWLower - mainForm.deltaFoobValue;
            double _isFreqLimitRight = mainForm.channelBWUpper + mainForm.deltaFoobValue;
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //Generate wanted signal_Chung cho tất cả các bài
                //vsg1.SetPsenWaveform(wsPower, wsFreq);
                vsg1.SetPsenWaveform(wsPower, wsFreq, WsRBOffset);
                vsg1.OnOffSignal(1);
                //vsg2.OnOffSignal(1);
                //Generate inferrence signal
                switch (mode)
                {
                    case "ACS":
                        vsg2.SetACSWaveform(isPower, isFreq);
                        vsg2.TypeOfSignal = "DFT-s-OFDM_100RBs";
                        if(vsg1.IpAddress==vsg2.IpAddress)  //SMW200A-2port
                        {
                            vsg1.setParameter(1, wsPower, wsFreq);
                            vsg2.setParameter(2, isPower, isFreq);
                        }
                        
                        bbu.NEW_RXSENS(WsRBOffset, out throughput);
                        //26.2.2024 - sua code
                        writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                        break;
                    case "ICS":
                        vsg2.SetICSWaveform(isPower, isFreq, IsRBOffset);
                        vsg2.TypeOfSignal = "DFT-s-OFDM_50RBs";
                        if (vsg1.IpAddress == vsg2.IpAddress)  //SMW200A-2port
                        {
                            vsg1.setParameter(1, wsPower, wsFreq);
                            vsg2.setParameter(2, isPower, isFreq);
                        }
                        bbu.NEW_RXSENS(WsRBOffset, out throughput);
                        //26.2.2024 - sua code
                        writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                        break;
                    case "INB":
                        //vsg2.SetIBBWaveform(isPower, isFreq);
                        vsg2.TypeOfSignal = "DFT-s-OFDM_100RBs";
                        vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset);
                        if (vsg1.IpAddress == vsg2.IpAddress)  //SMW200A-2port
                        {
                            vsg1.setParameter(1, wsPower, wsFreq);
                            vsg2.setParameter(2, isPower, isFreq);
                        }
                        bbu.NEW_RXSENS(WsRBOffset, out throughput);
                        //26.2.2024 - sua code
                        writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                        break;
                    case "INB2":
                        //measuring INB with TM run continuosly in xx (s)
                        vsg2.TypeOfSignal = "DFT-s-OFDM_100RBs";
                        vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset);
                        if (vsg1.IpAddress == vsg2.IpAddress)  //SMW200A-2port
                        {
                            vsg1.setParameter(1, wsPower, wsFreq);
                            vsg2.setParameter(2, isPower, isFreq);
                        }
                        //inbMeasurementInSequence(bbu, vsg2, mode, setPort, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, _isFreqLimitLeft.ToString(), isLowerFreqRange, isUpperFreqRange, _isFreqLimitRight.ToString(), out TP);
                        inbMeasurementInSequence(bbu, vsg2, mode, setPort, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, vsg2.SweepFrequencyFrom.ToString(), isLowerFreqRange, isUpperFreqRange, vsg2.SweepFrequencyTo.ToString(), out TP);
                        break;
                    case "NBB":
                        if (vsg2.Model != "SMW200A")
                        {
                            vsg2.SetNBBWaveform(isPower, isFreq, IsRBOffset);
                        }
                        else
                        {
                            if (double.Parse(wsFreq) < double.Parse(isFreq)) //Nhieu ben phai
                            {
                                vsg2.SetNBBUpperWaveform(isPower, isFreq, IsRBOffset);
                            }
                            else
                            {
                                vsg2.SetNBBLowerWaveform(isPower, isFreq, IsRBOffset); //nhieu ben trai
                            }
                        }
                        //vsg2.SetNBBWaveform(isPower, isFreq, IsRBOffset);
                        if (vsg1.IpAddress == vsg2.IpAddress)  //SMW200A-2port
                        {
                            vsg1.setParameter(1, wsPower, wsFreq);
                            vsg2.setParameter(2, isPower, isFreq);
                        }
                        vsg2.TypeOfSignal = "DFT-s-OFDM_1RB";
                        bbu.NEW_RXSENS(WsRBOffset, out throughput);
                        //26.2.2024 - sua code
                        writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                        break;
                    case "OOB2":
                        //25.3.2024 - viet lai bai do oob                        
                        //neu su dung smw200a la nguon phat ws => tat bo nguon nhieu o port 2
                        if (vsg1.Model == "SMW200A")
                        {
                            vsg1.OffSignal(2); //off nguon 2
                        }
                        //measRange
                        List<string[]> oobMeasRange = new List<string[]>();
                        oobMeasRange.Add(new string[] { "", isLowerFreqRange, isUpperFreqRange });
                        //exceptRange
                        List<string[]> oobExRange = new List<string[]>();
                        //add Co-location bands to exception range if check option
                        if (mainForm.OOBexcludeColocationRange)
                        {
                            getISCoLocationRangeFromExcel();
                            foreach (var item in oobCoISsweepRanges)
                            {
                                oobExRange.Add(new string[] { item[1].ToString(), item[2].ToString() });
                            }
                        }
                        // add Operating band with delta_fOOB to exception range
                        oobExRange.Add(new string[] { (_isFreqLimitLeft / 1e6).ToString(), (_isFreqLimitRight / 1e6).ToString() });
                        //for debugging
                        Log("[Out-of-band Measurement] Reading Excluding Range:");
                        foreach (var range in oobExRange)
                        {
                            Log($"[ {range[0]} MHz, {range[1]} MHz]");
                        }
                        //============
                        oobMeasurementInSequence(bbu, vsg2, mode, setPort, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, oobMeasRange, oobExRange, out TP);
                        break;

                    case "OOB1": //oob co-location
                        //neu su dung smw200a la nguon phat ws => tat bo nguon nhieu o port 2
                        if (vsg1.Model == "SMW200A")
                        {
                            vsg1.OffSignal(2);
                        }
                        getISCoLocationRangeFromExcel();
                        oobMeasurementInSequence(bbu, vsg2, mode, portIndex, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, oobCoISsweepRanges, null, out TP);
                        break;

                    // Add cases for other modes

                    default:
                        // Handle unsupported mode here.
                        break;
                }
                vsg2.OnOffSignal(0);
            }
            catch (Exception ex)
            {
                Log("Cannot find throughput! " + ex.Message, LogLevel.ERROR);
                //stop cacs testmac
                if (L1TestMacV2Start) bbu.STOP_L1TESTMAC();
                if (L1TestMacV3Start) bbu.STOP_L1TESTMAC_V3();

            }

        }
        //bai do inband theo phuong an chay TM lien tuc - 01.03.2024
        private void inbMeasurementInSequence(BBU bbu, VSG vsg2, string mode, int setPort, double step, int timeSequence, string wsFreq, string wsPower, string WsRBOffset, string isFreq, string isPower, string IsRBOffset, string isLowerFreqFrom, string isLowerFreqTo, string isUpperFreqFrom, string isUpperFreqTo, out double TP)
        {
            TP = -1;
            double minTP = -1; //out of range 0-100            
            double.TryParse(isLowerFreqFrom, out double isFreqFromL);
            double.TryParse(isLowerFreqTo, out double isFreqToL);
            double.TryParse(isUpperFreqFrom, out double isFreqFromR);
            double.TryParse(isUpperFreqTo, out double isFreqToR);
            string throughput;
            int testCount = 0;

            double freqIS = 0;
            //read att of IS:
            double sFreqChange = isFreqFromL;
            string path = $@"{mainForm.isCSVFilePathRX}/ISRX{setPort}.csv";
            double isATT = vsg2.ReadATTfromCSVFile(path, sFreqChange);
            Log($"{vsg2.Alias}: Set ATT_Port{setPort}={isATT} dB at f={sFreqChange / 1e6}MHz");
            Log($"{vsg2.Alias}: CSV Path:{vsg2.AttenuatorCSVpath}");
            //setup IS 
            //vsg2.TypeOfSignal = "DFT-s-OFDM_100RBs";
            //vsg2.SetIBBWaveform((double.Parse(isPower)+isATT).ToString("N3"), isFreq, IsRBOffset);

            bool runningMode = true; // mode 
            if (runningMode)
            {
                //L1TestMacV3Start = bbu.STR_RXSENS_V3(setPort);  //sua thanh V3 sau khi update OAM
                L1TestMacV3Start = bbu.STR_RXSENS_V3(currPort.StreamIndex);
                //01.4.2024 - neu start RxSen_V2 fail => chay STP_RXSEN roi cho khoang 30s roi chay lai 1 lan nua, neu khong duoc thi thoat
                if (!L1TestMacV3Start)
                {
                    bbu.STOP_L1TESTMAC_V3();
                    //waiting for 30s to restart L1TestMac again
                    countdownTimerInSecond(30, "Waiting for L1TestMac restart again");
                    L1TestMacV3Start = bbu.STR_RXSENS_V3(currPort.StreamIndex);
                }
                //start RxSen_V2 fail => Dung do
                if (L1TestMacV3Start)
                {
                    Log("Setting Measurement Environment...");
                    //waiting for 25s to restart L1TestMac again
                    countdownTimerInSecond(25, "Waiting for preseting Testmac");
                    while (!bbu.VALIDATEFILEOUT_RXSENS_V3())  //sua thanh V3 sau khi update OAM
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending)
                        {
                            TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=-1;
                            //bbu.STOP_L1TESTMAC_V3(); //sua sau khi update OAM
                            break;
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                    Log("Synchronized Time Period completed: " + DateTime.Now.ToString()); //for debugging                        
                    Log("Start Measurement!");

                    //start loop                
                    DashedLineLog();
                    //======== Setup Cycle ======
                    //int stepTime = 10000; // 10s
                    System.Threading.Thread.Sleep(10);
                    timerSequence = new Timer(NRFrequencyLoop, null, 0, timeSequence);
                    //sFreqChange = isFreqFrom;
                    bool isOAMsyncFail = false; //28.3.2024 - co bao Fail OAM
                    bool firstStep = true; //28.3.2024 - check if first step
                    int errCount = 0;
                    while (sFreqChange <= isFreqToR)
                    {
                        if (flagStep)
                        {
                            if (!firstStep)
                            {
                                //08.01.2024 - chuyen thong bao len dau vong lap
                                Log($"Set frequency of IS to: {sFreqChange / 1e6} MHz");
                                //tao chu ky 10s

                                //delay = DateTime.Now.AddSeconds(0.05); // 08.01.2024 - test voi chu ky 100ms ???can trong vong lap nay khong???
                                isFreq = sFreqChange.ToString();

                                //check background worker
                                if (mainForm.measurementBackgroundWorker.CancellationPending)
                                {
                                    TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
                                    break; //nhay khoi vong lap
                                }
                                System.Threading.Thread.Sleep(1);

                                //Đọc throughput về
                                bbu.RXSENS_V3(out throughput); //sua sau khi update OAM
                                System.Threading.Thread.Sleep(1);
                                double tempTP = GetthroughputFromString(throughput, setPort);
                                if ((tempTP >= 95 && tempTP <= 100) || testCount == 3)
                                {
                                    //28.3.2024 - Truong hop van doc dc ve mang throughput
                                    if (tempTP > 0)
                                    {
                                        writeCSVThroughputValue(mode, setPort, wsFreq, wsPower, WsRBOffset, isFreq, isPower, IsRBOffset, throughput, out TP);
                                        //luu lại giá trị TP nhỏ nhất
                                        if (minTP >= TP || minTP == -1)
                                        {
                                            minTP = TP;
                                            freqIS = sFreqChange;
                                        }
                                        Log($"Min throughput = {minTP}% @ NR Interfering Freq. = {freqIS / 1e6} MHz");
                                        DashedLineLog("-------");
                                        //09.01.2024 - plot chart
                                        mainForm.PopulateMeasurementChart($"Inband-Blocking: Port {setPort}", (sFreqChange / 1e6).ToString(), TP.ToString(), "95", "100");
                                        //==================
                                        //Change CW frequency for next step                            
                                        sFreqChange += step;
                                        //kiem tra tan so nam trong dai tan sweep
                                        if (sFreqChange > isFreqToL & sFreqChange < isFreqFromR)
                                        {
                                            sFreqChange = isFreqFromR;
                                        }
                                        //end loop when freq is up to UpperFreq
                                        if (sFreqChange > isFreqToR)
                                        {
                                            break;
                                        }
                                        isATT = vsg2.ReadATTfromCSVFile(path, sFreqChange);
                                        Log($"{vsg2.Alias}: Set ATT_Port{setPort}={isATT} dB at f={sFreqChange / 1e6}MHz");
                                        //Phat nhieu                             
                                        isFreq = sFreqChange.ToString();
                                        //vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset);
                                        //19.3.2024
                                        if (vsg2.Model == "SMW200A")
                                        {
                                            vsg2.setParameter(2, (double.Parse(isPower) + isATT).ToString("N3"), isFreq);
                                        }
                                        else
                                        {
                                            vsg2.SetFreq(isFreq);
                                            vsg2.SetPower((double.Parse(isPower) + isATT).ToString("N3"));
                                        }
                                        //reset testCount ve 0
                                        testCount = 0;
                                        errCount = 0;
                                        isOAMsyncFail = false;
                                    }
                                    else   // truong hop khong doc duoc ve mang throughput do loi mat OAM sau khi da test lai 3 lan
                                    {
                                        errCount += 1;
                                        //Console.WriteLine($"\rWaiting for synchronization.....{errCount}");
                                        if (errCount >= 1000)  //28.3.2024 - Sau 1000 vong lap, neu van loi thi bao loi
                                        {
                                            isOAMsyncFail = true;
                                            Log("Server is not responding, measurement stop!");
                                            errCount = 0;
                                            break;
                                        }
                                    }

                                }
                                else
                                {
                                    Log($"TP = {tempTP}% => Retest: {testCount}");
                                    testCount += 1;
                                }
                            }
                            else
                            {
                                firstStep = false;
                            }
                            //xoa flag
                            flagStep = false;
                        }
                        if (isOAMsyncFail)  //28.3.2024 - Sau 100 vong lap, neu van loi thi bao loi va thoat bai do
                        {
                            break;
                        }
                    }

                    //Ket thuc chay vong lap
                    timerSequence.Dispose();  //Stop timer
                    bbu.STOP_L1TESTMAC_V3();
                    L1TestMacV3Start = false;
                    vsg2.OnOffSignal(0);
                    //gan gia tri throughput nho nhat de in ra ngoai ListView
                    DashedLineLog("---");
                    Log("Measurement finished!");
                    TP = minTP;
                    if (TP < 95) Log("LOG: Throughput < 95% => FAILED!", LogLevel.ERROR);
                    else Log("LOG: Throughput > 95% => PASSED!", LogLevel.SUCCESS);
                    Log("========================================");

                    if (isOAMsyncFail)
                    {
                        MessageBox.Show("OAM is not responding, measurement stop!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                else
                {
                    bbu.STOP_L1TESTMAC_V3();
                    L1TestMacV3Start = false;
                    Log("Start RXSENS_V3 Fail! Wait for a while before next measurement!", LogLevel.ERROR);
                }
            }
        }
        private void oobMeasurementInSequence(BBU bbu, VSG vsg2, string mode, int setPort, double step, int timeSequence, string wsFreq, string wsPower, string WsRBOffset, string isFreq, string isPower, string IsRBOffset, List<string[]> measRanges, List<string[]> exceptRanges, out double TP)
        {
            TP = -1;
            vsg2.TypeOfSignal = "CW";
            double minTP = -1; //out of range 0-100
            double sFreqChange = 0;
            double freqIS = 0;
            int testCount = 0;
            int errCount = 0; // 28.3.2024 - counting loop mất đồng bộ
            string throughput = "ERROR";
            // Get the sweep range
            List<string[]> sweepRange = GetSweepRangeWithID(measRanges, exceptRanges);
            //printing sweepRange
            Log($"Sweeping Frequency Range [{sweepRange.Count}]:");
            foreach (var item in sweepRange)
            {
                Log($"[{item[0]}, {item[1]}, {item[2]}]");
            }
            //Load Ucorr SGS100A
            string ucorrPath = @"/var/user/RxBlocking";
            vsg2.LoadUserCorrDataSGS100A(ucorrPath);
            //initial CW signal
            if (sweepRange == null || sweepRange.Count == 0)
            {
                Log($"No frequency value sweep, Measurement finish!");
            }
            else
            {
                sFreqChange = double.Parse(sweepRange[0][1]) * 1e6; //in Hz
                vsg2.SetCWWaveform(isPower, sFreqChange.ToString());
                //loop
                bool oobMode = true; // mode OOB
                if (oobMode)
                {
                    //Start RXSens_V2
                    //bbu.STR_RXSENS_V2();
                    L1TestMacV2Start = bbu.STR_RXSENS_V2(currPort.StreamIndex);
                    //01.4.2024 - neu start RxSen_V2 fail => chay STP_RXSEN roi cho khoang 30s roi chay lai 1 lan nua, neu khong duoc thi thoat
                    if (!L1TestMacV2Start)
                    {
                        bbu.STOP_L1TESTMAC();
                        //waiting for 30s to restart L1TestMac again
                        countdownTimerInSecond(30, "Waiting for L1TestMac restart again");
                        L1TestMacV2Start = bbu.STR_RXSENS_V2(currPort.StreamIndex);
                    }

                    if (L1TestMacV2Start)
                    {
                        //DateTime delay;
                        //TimeSpan remainingTime;
                        //delay = DateTime.Now.AddSeconds(25); //waiting 25s
                        Log("Setting Measurement Environment...");
                        //while (DateTime.Now <= delay)
                        //{
                        //    remainingTime = delay - DateTime.Now;
                        //    Console.Write($"\rWaiting for preseting Testmac: {remainingTime.ToString("ss")}s..."); //for debugging                            
                        //    System.Threading.Thread.Sleep(10); //de do phan giai 10ms
                        //}
                        //waiting for 30s to restart L1TestMac again
                        countdownTimerInSecond(30, "Waiting for preseting Testmac");

                        //Check if throughut Fileout is existing
                        Log("Start Synchronizating Time period with Testmac: " + DateTime.Now.ToString()); //for debugging
                        while (!bbu.VALIDATEFILEOUT_RXSENS_V2())
                        {
                            if (mainForm.measurementBackgroundWorker.CancellationPending)
                            {
                                TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
                                break;
                            }
                            System.Threading.Thread.Sleep(10);
                        }
                        Log("Synchronized Time Period is ready. Start Measurement!");

                        //start loop                
                        Log("------------------------------------------------------------");
                        //======== Setup Cycle ======
                        int stepTime = timeSequence; // (in ms) - 21.2.2024
                        bool firstStep = true; //28.3.2024 - check if first step
                        timerSequence = new Timer(CWFrequencyLoop, null, 0, stepTime);
                        //sweep Freq in sweepRange
                        bool isOAMsyncFail = false; //28.3.2024 - using to detect OAM not responding
                        int listIndex = 0; //28.3.2024 - store index of range in sweepRang
                        foreach (var range in sweepRange)
                        {
                            listIndex += 1;
                            double sweepFrom = double.Parse(range[1]) * 1e6;
                            double sweepTo = double.Parse(range[2]) * 1e6;
                            sFreqChange = sweepFrom;
                            string isFreqSweep = sFreqChange.ToString();
                            errCount = 0;
                            //break foreach if measurementBackgroundWorker is canceled
                            if (mainForm.measurementBackgroundWorker.CancellationPending)
                            {
                                TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
                                break;
                            }
                            while (sFreqChange <= sweepTo)
                            {
                                if (flagStep)
                                {
                                    if (!firstStep) //28.3.2024 - bo first step
                                    {
                                        //check background worker Cancelled
                                        if (mainForm.measurementBackgroundWorker.CancellationPending)
                                        {
                                            TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
                                            break;
                                        }
                                        Log($"[{DateTime.Now.ToString("hh:mm:ss:fff")}] LOG: Set frequency of CW signal to: {sFreqChange / 1e6} MHz");
                                        //Đọc throughput về
                                        bbu.RXSENS_V2(out throughput);
                                        System.Threading.Thread.Sleep(1);
                                        double tempTP = GetthroughputFromString(throughput, setPort);
                                        if ((tempTP >= 95 && tempTP <= 100) || testCount == 3)
                                        {
                                            //28.3.2024 - Truong hop van doc dc ve mang throughput
                                            //5.4.2024 - tạm thời tách TH throughput = 0 coi như là lỗi mất đồng bộ
                                            if (tempTP > 0)
                                            {
                                                string subName = (range[0] == null || range[0] == "") ? "" : ("_" + range[0]);
                                                writeCSVThroughputValue($"{mode}_P{setPort}{subName}", setPort, wsFreq, wsPower, WsRBOffset, isFreqSweep, isPower, IsRBOffset, throughput, out TP);
                                                //luu lại giá trị TP nhỏ nhất
                                                if (minTP >= TP || minTP == -1)
                                                {
                                                    minTP = TP;
                                                    freqIS = sFreqChange;
                                                }
                                                Log($"[OVERALL] Min throughput = {minTP}% @ CW Freq. = {freqIS / 1e6} MHz");
                                                DashedLineLog("----");
                                                mainForm.PopulateMeasurementChart($"Out-of-band Blocking: Port {setPort}", (sFreqChange / 1e6).ToString(), TP.ToString(), "95", "100");
                                                //Change CW frequency for next step                            
                                                sFreqChange += step;
                                                //Kiem tra lai gia tri tan so va Phat nhieu
                                                if (sFreqChange <= sweepTo)
                                                {
                                                    isFreqSweep = sFreqChange.ToString();
                                                    vsg2.SetCWWaveform(isPower, isFreqSweep);
                                                }
                                                else if (listIndex < sweepRange.Count())//neu gia tri Freq la cuoi cung cua range => Set sang range tiep theo
                                                {
                                                    sFreqChange = double.Parse(sweepRange[listIndex][1]) * 1e6; //nhay toi gia tri tiep theo
                                                    isFreqSweep = sFreqChange.ToString();
                                                    vsg2.SetCWWaveform(isPower, isFreqSweep);
                                                }
                                                //reset testCount ve 0
                                                testCount = 0;
                                                errCount = 0;
                                                isOAMsyncFail = false;
                                            }
                                            else   // truong hop khong doc duoc ve mang throughput do loi mat OAM sau khi da test lai 3 lan
                                            {
                                                errCount += 1;
                                                //Console.WriteLine($"\rWaiting for synchronization.....{errCount}");
                                                if (errCount >= 1000)  //28.3.2024 - Sau 1000 vong lap, neu van loi thi bao loi
                                                {
                                                    isOAMsyncFail = true;
                                                    Log("Server is not responding, measurement stop!");
                                                    errCount = 0;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Log($"TP = {tempTP}% => Retest: {testCount}");
                                            testCount += 1;
                                        }
                                    }
                                    else
                                    {
                                        firstStep = false;
                                    }
                                    //xoa flag
                                    flagStep = false;
                                }
                            }
                            if (isOAMsyncFail)  //28.3.2024 - Sau 100 vong lap, neu van loi thi bao loi va thoat bai do
                            {
                                break;
                            }
                        }
                        //Ket thuc chay vong lap
                        timerSequence.Dispose();  //Stop timer
                        bbu.STOP_L1TESTMAC();
                        L1TestMacV2Start = false;
                        vsg2.OnOffSignal(0);
                        //gan gia tri throughput nho nhat de in ra ngoai ListView
                        DashedLineLog("---");
                        Log("Measurement finished!");
                        TP = minTP;
                        if (TP < 95) Log("LOG: Throughput < 95% => FAILED!", LogLevel.FAILED);
                        else Log("LOG: Throughput > 95% => PASSED!", LogLevel.SUCCESS);
                        DashedLineLog("---");
                        //28.3.2024
                        if (isOAMsyncFail)
                        {
                            MessageBox.Show("OAM is not responding, measurement stop!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        bbu.STOP_L1TESTMAC();
                        L1TestMacV2Start = false;
                        Log("Start RXSENS_V2 Fail! Wait for a while before next measurement!");
                    }
                }
            }
        }

        //26.2.2024 - Viet lại hàm
        private void writeCSVThroughputValue(string mode, int port, string wsFreq, string wsPower, string wsRBOffset, string isFreq, string isPower, string isRBOffset, string throughput, out double TP)
        {
            //int index;
            //tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ////========== 16.2.2024 - sua lai doan code ======
            //index = (port - 1) % tmpArr.Length;
            //double.TryParse(tmpArr[index], out TP);
            TP = GetthroughputFromString(throughput, port);
            //Data collection
            List<string> data = new List<string> { DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"), mainForm.RruType, mode, port.ToString(), wsFreq, wsPower, wsRBOffset, isFreq, isPower, isRBOffset, TP.ToString() };

            if (TP < 95)
            {
                Log($"Throughput = {TP}% < 95% => FAILED!", LogLevel.FAILED);
            }
            else
            {
                Log($"Throughput = {TP}% > 95% => PASSED!", LogLevel.SUCCESS);
            }

            //Write to CSV
            mainForm.ULCsvRecord(mode, csvHeader, data);
            //Log("-----------------------------------------------------------");
        }
        private double GetthroughputFromString(string tp, int port)
        {
            double tmpTP = -1;
            tmpArr = tp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //28.3.2024 - sua lai code xu ly neu tra ve chuoi null
            if (tmpArr != null && tmpArr.Length > 0)
            {
                //int index = (port - 1) % tmpArr.Length;
                //double.TryParse(tmpArr[index], out tmpTP);
                //Redhat - get throughput
                double.TryParse(tmpArr[(currPort.StreamIndex - 1) % tmpArr.Length], out tmpTP);
            }
            return tmpTP;
        }
        private void getISCoLocationRangeFromExcel()
        {
            //xoa list
            oobCoISsweepRanges.Clear();

            string filename = mainForm.directionPathTemplate + "OQC_UL_SETUP_3GPP.xlsx";
            Log($"Read IS Co-location bands information from path:{filename}");
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filename);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[2]; //out-of-band: NR co-location bands are defined in sheet2
            Excel.Range xlRange = xlWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            for (int i = 2; i <= rowCount; i++)
            {
                if (xlRange.Cells[i, 1].Value != null)
                {
                    //Log($"[from Excel]{xlRange.Cells[i, 1].Value}: From {xlRange.Cells[i, 2].Value} MHz to {xlRange.Cells[i, 3].Value} MHz");
                    oobCoISsweepRanges.Add(new string[] { xlRange.Cells[i, 1].Value.ToString(), xlRange.Cells[i, 2].Value.ToString(), xlRange.Cells[i, 3].Value.ToString() });
                }
            }
            //for debugging
            Log("[Out-of-band Measurement] NR bands Co-location:");
            foreach (var range in oobCoISsweepRanges)
            {
                Log($"{range[0]}: From {range[1]} MHz To {range[2]} MHz");
            }
        }

        public List<double[]> GetSweepRange(List<string[]> measRange, List<string[]> exceptRange) //note: measRange element has 3 dimensions, exceptRange's has 2 dimension
        {
            // Sort the ranges
            measRange = measRange.OrderBy(r => r[1]).ToList();
            exceptRange = exceptRange.OrderBy(r => r[0]).ToList();

            //ranges after sorting
            Log("Meas Range after sorting:");
            foreach (var range in measRange)
            {
                Log($"[{range[1]}, {range[2]}]");
            }
            // Print the Except range
            Log("Except Range after sorting:");
            foreach (var range in exceptRange)
            {
                Log($"[{range[0]}, {range[1]}]");
            }

            List<double[]> sweepRange = new List<double[]>();

            if (exceptRange == null || exceptRange.Count == 0)
            {
                // If exceptRange is null or empty, sweep range is the same as measRange
                foreach (var mRange in measRange)
                {
                    sweepRange.Add(new double[] { double.Parse(mRange[1]), double.Parse(mRange[2]) });
                }
                return sweepRange;
            }
            bool resetLoop = false;
            foreach (var eRange in exceptRange)
            {
                bool overlapped = false;

                foreach (var mRange in measRange)
                {
                    if (double.Parse(mRange[1]) <= double.Parse(eRange[1]) && double.Parse(mRange[2]) >= double.Parse(eRange[0]))
                    {
                        // There's an overlap, adjust the range
                        overlapped = true;
                        if (double.Parse(mRange[1]) < double.Parse(eRange[0]))
                            sweepRange.Add(new double[] { double.Parse(mRange[1]), double.Parse(eRange[0]) });
                        if (double.Parse(mRange[2]) > double.Parse(eRange[1]))
                            sweepRange.Add(new double[] { double.Parse(eRange[1]), double.Parse(mRange[2]) });
                    }
                    //if eRange is overlap of all mRange
                    if (sweepRange == null || sweepRange.Count == 0)
                    {
                        Log("excepRange overlapped measRange");
                        resetLoop = true;
                        break;
                    }
                }

                if (!overlapped)
                {
                    // If no overlap, add the entire range to the sweep range
                    sweepRange.Add(new double[] { double.Parse(eRange[0]), double.Parse(eRange[1]) }); //kiem tra code nay
                }
                if (resetLoop) break;
            }

            //debugging - print sweepRange
            Log("Sweep Range:");
            foreach (var range in sweepRange)
            {
                Log($"{range[0]}: From {range[1]} MHz To {range[2]} MHz");
            }

            return sweepRange;
        }


        //get sweeprange with ID beyond ID of measRange
        public List<string[]> GetSweepRangeWithID(List<string[]> measRange, List<string[]> exceptRange) //note: measRange element has 3 dimensions, exceptRange's has 2 dimension
        {
            List<string[]> sweepRange = new List<string[]>();

            if (exceptRange == null || exceptRange.Count == 0)
            {
                // If exceptRange is null or empty, sweep range is the same as measRange
                foreach (var mRange in measRange)
                {
                    sweepRange.Add(new string[] { mRange[0], mRange[1], mRange[2] });
                }
            }
            else
            {
                foreach (var meas in measRange)
                {
                    sweepRange.Add(new string[] { meas[0], meas[1], meas[2] });
                    //Console.WriteLine($"Add to sweep range: [{meas[0]}, {meas[1]}, {meas[2]}]");
                    foreach (var except in exceptRange)
                    {
                        PopulateRange(sweepRange, except);
                    }
                }
            }
            return sweepRange;
        }
        private void PopulateRange(List<string[]> sourceRange, string[] subtract)
        {
            List<string[]> temp = new List<string[]>();
            foreach (var item in sourceRange)
            {
                int startMeas = int.Parse(item[1]);
                int endMeas = int.Parse(item[2]);
                int startExcept = int.Parse(subtract[0]);
                int endExcept = int.Parse(subtract[1]);

                if (startMeas >= endExcept || endMeas <= startExcept)
                {
                    temp.Add(new string[] { item[0], item[1], item[2] });
                }
                else
                {
                    if (startMeas < startExcept)
                    {
                        temp.Add(new string[] { item[0], $"{startMeas}", $"{startExcept}" });
                    }
                    if (endMeas > endExcept)
                    {
                        temp.Add(new string[] { item[0], $"{endExcept}", $"{endMeas}" });
                    }
                }
            }
            //renew sourceRange
            sourceRange.Clear();
            foreach (var mRange in temp)
            {
                sourceRange.Add(new string[] { mRange[0], mRange[1], mRange[2] });
            }
            ////debugging
            //foreach (var xRange in sourceRange)
            //{
            //    Console.WriteLine($"read sourceRange: [{xRange[0]}, {xRange[1]}, {xRange[2]}]");
            //}
            //Console.WriteLine("end round \n");
        }

        private static void CWFrequencyLoop(object state)
        {
            flagStep = true;
        }
        private static void NRFrequencyLoop(object state)
        {
            flagStep = true;
        }

        private void countdownTimerInSecond(int iSec, string message)
        {
            TimeSpan remainingTime;
            DateTime delay = DateTime.Now.AddSeconds(iSec);
            while (DateTime.Now <= delay)
            {
                remainingTime = delay - DateTime.Now;
                Console.Write($"\r{message} in {remainingTime.ToString("ss")}s...");
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}