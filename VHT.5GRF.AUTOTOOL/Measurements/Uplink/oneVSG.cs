using _5GAutoTool.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _5GAutoTool
{
    public enum L1TestMacMode
    {
        RXSEN,
        RXSENS_V2,
        RXSENS_V3,
    }

    public class oneVSG
    {
        public Form5GAT mainForm;
        public string AttenuatorPath;
        private static bool flagStep = false;
        public oneVSG(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
            AttenuatorPath = mainForm.isCSVFilePathRX;
        }
        string Mode = "";
        public void Reset()
        {
            Mode = "";
        }
        //string[] tmpArr = new string[4];
        string[] tmpArr = null; //16.2.2024 - revise
        //26.2.2024 - khai bao header csv log
        List<string> csvHeader = new List<string> { "DATE", "TIME", "TYPE", "SPEC", "PORT", "WS FREQ", "WS PWR LVL", "THROUGHPUT" };
        PortMapping currPort;

        //TODO: Define Measurement function for SEN and DYN items
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "oneVSG");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public double Measurement(VSG vsg1, BBU bbu, string mode, int repeatNumber, string spec, double step, string setAtt, int setPort, string setPower, string setFreq,bool isMultiMeas, out double TP)
        {
            TP = -1;
            string throughput = "";
            //ServerRedhat - finding PortMapping
            currPort = mainForm.PortMappings.FirstOrDefault(item => item.Port == setPort);
            bbu.SwitchGroup(currPort.XRanIndex.ToString());
            //for debugging
            Log($"Current Testing Port = {currPort.Port} \tXran = {currPort.XRanIndex} \tStream = {currPort.StreamIndex}");
            //doc gia tri suy hao cua VSG
            double.TryParse(setPower, out double wantedSignalLevel);
            //double tmpPowerThreshold = wantedSignalLevel + vsg1.Attenuators[setPort - 1];  //check laij xem la cong hay tru
            double tmpPowerThreshold = wantedSignalLevel + double.Parse(setAtt);  //check laij xem la cong hay tru

            //09.10.2024 - case do dong thoi 8 kenh
            if(isMultiMeas && currPort.IsPassed)
            {
                writeCSVThroughputValue(mode, setPort, setFreq, wantedSignalLevel.ToString(), currPort.Result, out TP);
                return tmpPowerThreshold - double.Parse(setAtt); ;
            }

            try
            {
                Thread.Sleep(1);
                {
                    if (mode == "SEN")
                    {
                        vsg1.SetPsenWaveform(tmpPowerThreshold.ToString(), setFreq);
                    }
                    else if (mode == "DR")
                    {
                        vsg1.SetDRWaveform(tmpPowerThreshold.ToString(), setFreq);
                    }
                }

                Log($"Expected Power Level: {wantedSignalLevel} dBm");
                if (step > 0.0) // Do throughput theo step
                {
                    double initPwr = double.Parse(spec);
                    double freq = double.Parse(setFreq);
                    double threshold = FindThresholdLevel(bbu, vsg1, setPort, freq, initPwr, step, out throughput);
                    writeCSVThroughputValue(mode, setPort, setFreq, threshold.ToString(), throughput, out TP);
                }
                else
                {
                    throughput = GetThroughput(bbu, mode, currPort.StreamIndex);
                    writeCSVThroughputValue(mode, setPort, setFreq, wantedSignalLevel.ToString(), throughput, out TP);
                    if(isMultiMeas)
                    {
                        WriteThroughputToPortGroup(throughput,currPort.XRanIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Cannot get throughtput! {ex.Message}", LogLevel.ERROR);
            }

            return tmpPowerThreshold - double.Parse(setAtt);
        }

        public double FindThresholdLevel(BBU bbu, VSG vsg, int port, double freq, double initLevel, double step, out string throughput)
        {
            throughput = "";
            double tmpThroughput = -1;
            double threshold = 0;
            bool firstStep = true;
            bool currentTestPass;
            bool previousTestPass = false;
            int repeatCheck = 1;
            double factor = 0.0; // -1 => TP at initial Power is Pass, decrease Power Level / 1 => TP at initial Power is Fail, increase Power Level;  

            // Read attenuation and set VSG power level
            string path = $@"{AttenuatorPath}/ISRX{port}.csv";
            double attVal = vsg.ReadATTfromCSVFile(path, freq);
            Log($"[{vsg.Alias}] Set ATT_Port{port} = {attVal} dB at frequency = {freq / 1e6}MHz");
            double currentPowerLevel = initLevel - attVal;
            vsg.SetPsenWaveform(currentPowerLevel.ToString(), freq.ToString());

            double limLow = currentPowerLevel - 0.1 * Math.Abs(initLevel);
            double limHigh = currentPowerLevel + 0.1 * Math.Abs(initLevel);

            // Start TestMac
            bool isRunTM = StartL1TestMac(L1TestMacMode.RXSENS_V2, bbu, port);
            DashedLineLog("--- Start PSens Threshold Investigation ---");
            System.Threading.Timer timerSequence = new System.Threading.Timer(TimeCycle, null, 0, 800);
            while (isRunTM && currentPowerLevel >= limLow && currentPowerLevel <= limHigh)
            {
                if (flagStep)
                {
                    // Check if the background worker has been canceled
                    if (mainForm.measurementBackgroundWorker.CancellationPending)
                    {
                        Log("Measurement Background worker has been stopped!", LogLevel.WARN);
                        break;
                    }

                    //print log
                    DashedLineLog("-----");
                    Log($"Expected Power Level = {currentPowerLevel + attVal} dBm");
                    Log($"Set VSG Output Power = {currentPowerLevel}dBm [ATT:{attVal}dB] at frequency = {freq / 1e6}MHz");

                    //get throughput
                    bbu.RXSENS_V2(out string throughputString);
                    tmpThroughput = GetthroughputFromString(throughputString, port);
                    string tmpLog = $"Throughput[Port{port}] = {throughput}% at Expected Power Level = {currentPowerLevel + attVal}dBm / frequency = {freq / 1e6} MHz";

                    currentTestPass = tmpThroughput >= 95;

                    if (currentTestPass)
                    {
                        Log($"{tmpLog} => PASS!  [Test Count = {repeatCheck}]", LogLevel.SUCCESS);
                    }
                    else
                    {
                        Log($"{tmpLog} => FAIL!  [Test Count = {repeatCheck}]", LogLevel.FAILED);
                    }

                    //DETECT THRESHOLD========
                    if (firstStep)
                    {
                        previousTestPass = currentTestPass;
                        factor = (currentTestPass) ? -1.0 : 1.0;
                        // Adjust sweep level
                        currentPowerLevel += (factor * step);
                        firstStep = false;
                    }

                    else
                    {
                        if (currentTestPass != previousTestPass)
                        {
                            repeatCheck += 1;
                            if (repeatCheck == 5)
                            {
                                if (currentTestPass)
                                {
                                    Log($"At Threshold[Port{port}] = {currentPowerLevel + attVal}dBm, throughput = {throughput}%");
                                    Log("Threshold Investigation done!", LogLevel.SUCCESS);
                                    threshold = currentPowerLevel + attVal;
                                    break;
                                }
                                else
                                {
                                    //check previous point
                                    currentPowerLevel -= (factor * step);
                                    previousTestPass = currentTestPass;
                                    repeatCheck = 1;
                                }
                            }
                        }
                        else
                        {
                            previousTestPass = currentTestPass;
                            currentPowerLevel += (factor * step);
                            repeatCheck = 1;
                        }
                    }

                    //set VSG level
                    vsg.SetPower(currentPowerLevel.ToString());
                    // Reset flag
                    flagStep = false;
                }
            }
            throughput = tmpThroughput.ToString();
            return threshold;
        }

        //+++++++++++++ moi ++++++++++++++++++
        // TODO: GetThroughput without RBOffset
        public string GetThroughput(BBU bbu, string mode, int setPort)
        {
            string throughput = "";
            try
            {
                if (mode == "SEN")
                {
                    bbu.NEW_RXSENS(out throughput);
                }
                else if (mode == "DR")
                {
                    bbu.NEW_RXDYN(out throughput);
                }

            }
            catch (Exception ex)
            {
                Log($"Cannot get throughput! {ex.Message}", LogLevel.ERROR);
                throw;
            }
            return throughput;
        }

        // TODO: GetThroughput with RBOffset
        public string GetThroughput(BBU bbu, string mode, string RBoffset, int setPort)
        {
            string throughput = "";
            try
            {
                if (mode == "SEN")
                {
                    bbu.NEW_RXSENS(RBoffset, out throughput);
                }
                else if (mode == "DR")
                {
                    bbu.NEW_RXDYN(RBoffset, out throughput);
                }
            }
            catch (Exception ex)
            {
                Log($"Cannot get throughput! {ex.Message}", LogLevel.ERROR);
                throw;
            }
            return throughput;
        }

        // TODO: Get throughput many times with RBOffset
        public void GetThroughputNtime(BBU bbu, string mode, string RBoffset, int setPort, int repeatNumber, out double TP)
        {
            TP = -1;
            double tmpTP, minTP = 100.0; // 100%            
            //try
            //{
            //    Log($"LOG: Get throughput in {repeatNumber} times");
            //    for (int iCount = 0; iCount < repeatNumber; iCount++)
            //    {
            //        Log($"Get throughput in {iCount + 1} times.");
            //        GetThroughput(bbu, mode, RBoffset, setPort, out tmpTP);
            //        // Find the lowest throughput in N times
            //        if (tmpTP < minTP)
            //        {
            //            minTP = tmpTP;
            //        }
            //    }
            //    Log($"LOG: Minimum throughput in {repeatNumber} times: {minTP}%");
            //    TP = minTP;
            //}
            //catch (Exception ex)
            //{
            //    Log($"ERROR: Can't get throughput. {ex.Message}");
            //    throw;
            //}
        }
        // TODO: Khao sat dac tinh do nhay SEN/DYN tuyen thu
        // Cong suat đặt bằng PowerSpecification = -95.6 dBm (SEN), -69.1 dBm (DYN)
        // Voi step = 0.1 dB, doc gia tri thong luong
        // Vong lặp chỉ dừng lại đến khi thông lượng đạt giá trị bằng 0%
        public void InvestigateReciver5GCharacteristic(VSG vsg1, BBU bbu, string mode, double step, string setAtt, int setPort, string setPower, string setFreq, out double TP)
        {
            TP = -1;
            bool flagStopProcess = false;
            double tmpPower = double.Parse(setPower);
            int gainRx = 5000;
            //try
            //{
            //    if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            //    // Phat tin hieu mong muon tai vsg theo gia tri dăt
            //    if (setPort == 1) //vi sao lai chi dung cho setport = 1 ?????
            //    {
            //        if (mode == "SEN")
            //        {
            //            vsg1.SetPsenWaveform(setPower, setFreq);
            //        }
            //        else if (mode == "DR")
            //        {
            //            vsg1.SetDRWaveform(setPower, setFreq);
            //        }
            //    }
            //    else
            //    {
            //        vsg1.SetPower(setPower);
            //    }
            //    do
            //    {
            //        Log($"Setting gain Rx to {gainRx} dB");
            //        while (!flagStopProcess)
            //        {
            //            GetThroughput(bbu, mode, setPort, out TP);
            //            Log($"INFO: Throughput in {mode} is: {TP}% at {tmpPower - double.Parse(setAtt)} dBm.");
            //            Log("====================================================");
            //            if (TP > 0.0)
            //            {
            //                // flagStopProcess = true;
            //                tmpPower -= step;
            //                Log($"INFO: Change power to {tmpPower - double.Parse(setAtt)} dBm with step {step} dB");
            //                vsg1.SetPower(tmpPower.ToString());
            //            }
            //            else
            //            {
            //                // Stop process and change other port
            //                Log($"INFOR: Investigating process is stopped at {tmpPower} dBm.");
            //                flagStopProcess = true;
            //            }
            //            //mainForm.csvRecord(mode, setPort.ToString(), setFreq, (tmpPower - double.Parse(setAtt)).ToString(), null, null, null, null, TP.ToString());

            //        }

            //        // Increase gainRx with step 1000 mdB (1 dB)
            //        gainRx += 3000;
            //    } while (gainRx <= 20000);
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        private void writeCSVThroughputValue(string mode, int port, string wsFreq, string wsPower, string throughput, out double TP)
        {
            tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double.TryParse(tmpArr[(currPort.StreamIndex - 1) % tmpArr.Length], out TP);
            //Data collection
            List<string> data = new List<string> { DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"), mainForm.RruType, mode, port.ToString(), wsFreq, wsPower, TP.ToString() };
            if (TP < 95)
            {
                Log($"LOG: Throughput = {TP}% < 95% => FAILED!", LogLevel.FAILED);
            }
            else
            {
                Log($"LOG: Throughput = {TP}% > 95% => PASSED!", LogLevel.SUCCESS);
            }
            //write to CSV
            mainForm.ULCsvRecord(mode, csvHeader, data);
        }

        public bool StartL1TestMac(L1TestMacMode tm, BBU bbu, int port)
        {
            bool isStart;
            bool startL1;

            switch (tm)
            {
                case L1TestMacMode.RXSEN:
                    return true;
                case L1TestMacMode.RXSENS_V2:
                    startL1 = bbu.STR_RXSENS_V2(port);
                    if (!startL1)
                    {
                        bbu.STOP_L1TESTMAC();
                        //waiting for 30s to restart L1TestMac again
                        countdownTimerInSecond(30, "Waiting for L1TestMac restart again");
                        Log($"Waiting for {tm} restart again");
                        startL1 = bbu.STR_RXSENS_V2(port);
                    }

                    if (startL1)
                    {
                        Log("Setting Measurement Environment...");
                        //waiting for 25s to restart L1TestMac again
                        countdownTimerInSecond(20, $"Waiting for preseting {tm}");
                        while (!bbu.VALIDATEFILEOUT_RXSENS_V2())
                        {
                            if (mainForm.measurementBackgroundWorker.CancellationPending)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(1);
                        }
                        Log("Start Measurement!");
                        isStart = true;
                    }
                    else
                    {
                        Log($"Cannot start {tm}", LogLevel.ERROR);
                        isStart = false;
                    }
                    return isStart;
                case L1TestMacMode.RXSENS_V3:
                    startL1 = bbu.STR_RXSENS_V3(port);
                    if (!startL1)
                    {
                        bbu.STOP_L1TESTMAC_V3();
                        //waiting for 30s to restart L1TestMac again
                        countdownTimerInSecond(30, "Waiting for L1TestMac restart again");
                        Log($"Waiting for {tm} restart again");
                        startL1 = bbu.STR_RXSENS_V3(port);
                    }

                    if (startL1)
                    {
                        Log("Setting Measurement Environment...");
                        //waiting for 25s to restart L1TestMac again
                        countdownTimerInSecond(20, $"Waiting for preseting {tm}");
                        while (!bbu.VALIDATEFILEOUT_RXSENS_V3())
                        {
                            if (mainForm.measurementBackgroundWorker.CancellationPending)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(1);
                        }
                        Log("Start Measurement!");
                        isStart = true;
                    }
                    else
                    {
                        Log($"Cannot start {tm}", LogLevel.ERROR);
                        isStart = false;
                    }
                    return isStart;
                default:
                    return false;
            }
        }
        private double GetthroughputFromString(string tp, int port)
        {
            double tmpTP = -1;
            tmpArr = tp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //28.3.2024 - sua lai code xu ly neu tra ve chuoi null
            if (tmpArr != null && tmpArr.Length > 0)
            {
                //Redhat - get throughput
                double.TryParse(tmpArr[(currPort.StreamIndex - 1) % tmpArr.Length], out tmpTP);
            }
            return tmpTP;
        }
        private void WriteThroughputToPortGroup(string throughput, int group)
        {
            //split throughput string
            tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 1; i <= tmpArr.Length; i++)
            {
                foreach (PortMapping port in mainForm.PortMappings)
                {
                    if (port.XRanIndex == group && port.StreamIndex == i)
                    {
                        port.Result = tmpArr[i-1];
                        //test only
                        //port.Result = tmpArr[0]; //only test voi he cu

                        double.TryParse(port.Result, out double tp);
                        port.IsPassed = (tp >= 95);
                    }
                }
            }           
        }
        private void countdownTimerInSecond(int iSec, string message)
        {
            TimeSpan remainingTime;
            DateTime delay = DateTime.Now.AddSeconds(iSec);
            while (DateTime.Now <= delay)
            {
                remainingTime = delay - DateTime.Now;
                System.Threading.Thread.Sleep(10);
            }
        }

        private static void TimeCycle(object state)
        {
            flagStep = true;
        }

    }
}