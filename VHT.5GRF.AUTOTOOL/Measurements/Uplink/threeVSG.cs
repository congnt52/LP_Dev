using _5GAutoTool.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _5GAutoTool
{
    class threeVSG
    {
        public Form5GAT mainForm;
        public threeVSG(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "";
        public void Reset()
        {
            Mode = "";
        }
        string[] tmpArr = null;
        List<string> csvHeader = new List<string> { "DATE", "TIME", "TYPE", "SPEC", "PORT", "WS FREQ", "WS PWR LVL", "IS1 FREQ", "IS1 PWR LVL", "IS2 FREQ", "IS2 PWR LVL", "THROUGHPUT" };
        PortMapping currPort;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "threeVSG");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Measurement(VSG vsg1, VSG vsg2, VSG vsg3, BBU bbu, string mode, string WsRBOffset, string IsRBOffset, double step, string setAtt, int setPort, string wsPowerStr, string wsFreq, string isPowerStr, string isFreq, string cwFreq, out double TP)

        {
            TP = 0;
            string throughput;

            //ServerRedhat - finding PortMapping
            currPort = mainForm.PortMappings.FirstOrDefault(item => item.Port == setPort);
            bbu.SwitchGroup(currPort.XRanIndex.ToString());

            //double _setAtt = double.Parse(setAtt);
            double _wsPower = double.Parse(wsPowerStr);
            double _wsFreq = double.Parse(wsFreq);

            double _isPower = double.Parse(isPowerStr);
            double _isFreq = double.Parse(isFreq);
            // Cw signal power equal Interferting signal power
            double _cwPower = double.Parse(isPowerStr);
            double _cwFreq = double.Parse(cwFreq);

            //read ATT
            //double _wsATT = vsg1.Attenuators[setPort - 1];
            //double _isATT = vsg2.Attenuators[setPort - 1];
            //double _cwATT = vsg3.Attenuators[setPort - 1];
            double _wsATT = vsg1.ReadATTfromCSVFile($@"{mainForm.wsCSVFilePathRX}/RX{setPort}.csv", _wsFreq);
            double _isATT = vsg2.ReadATTfromCSVFile($@"{mainForm.isCSVFilePathRX}/ISRX{setPort}.csv", _isFreq);
            double _cwATT = vsg3.ReadATTfromCSVFile($@"{mainForm.cwCSVFilePathRX}/CWRX{setPort}.csv", _cwFreq);

            Log($"{vsg1.Name}: Expected Power Level={_wsPower} dBm\tATT Level={_wsATT} dB");
            Log($"{vsg2.Name}: Expected Power Level={_isPower} dBm\tATT Level={_isATT} dB");
            Log($"{vsg3.Name}: Expected Power Level={_cwPower} dBm\tATT Level={_cwATT} dB");

            string wsPower = (_wsPower + _wsATT).ToString(); //công suất tai cong output cua VSG1 WS
            string isPower = (_isPower + _isATT).ToString(); //công suất tai cong output cua VSG2 IS
            // Set cw power to VSG3
            string cwPower = (_cwPower + _cwATT).ToString();

            try
            {
                //int port = 1;
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //doc gia tri suy hao cua VSGs

                vsg1.SetPsenWaveform(wsPower, wsFreq, WsRBOffset);
                if (mode == "GenIMD" && double.Parse(isFreq) < double.Parse(wsFreq))
                {
                    vsg2.SetGeneralIMDWaveformLower(isPower, isFreq);
                }
                else if (mode == "GenIMD" && double.Parse(isFreq) > double.Parse(wsFreq))
                {
                    vsg2.SetGeneralIMDWaveformUpper(isPower, isFreq);
                }
                else if (mode == "NBIMD" && double.Parse(isFreq) > double.Parse(wsFreq))
                {
                    vsg2.SetNBUpperIMDWaveform(isPower, isFreq, IsRBOffset);
                }
                else if (mode == "NBIMD" && double.Parse(isFreq) < double.Parse(wsFreq))
                {
                    vsg2.SetNBLowerIMDWaveform(isPower, isFreq, IsRBOffset);
                }
                vsg1.setParameter(1, wsPower, wsFreq);
                vsg2.setParameter(2, isPower, isFreq);
                vsg3.SetCWWaveform(cwPower, cwFreq);
                //bbu.NEW_RXSENS(out throughput);
                bbu.NEW_RXSENS(WsRBOffset, out throughput);
                writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, isFreq, isPowerStr, cwFreq, isPowerStr, throughput, out TP);
                //tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //int index = (setPort-1) % tmpArr.Length;
                //TP=double.Parse(tmpArr[index]);
                ////if (setPort < 9)
                ////{
                ////    TP = double.Parse(tmpArr[setPort - 1]);
                ////}
                ////else if (setPort >= 9 && setPort < 17)
                ////{
                ////    TP = double.Parse(tmpArr[setPort - 9]);
                ////}
                ////else if (setPort >= 17 && setPort < 25)
                ////{
                ////    TP = double.Parse(tmpArr[setPort - 17]);
                ////}
                ////else
                ////{
                ////    TP = double.Parse(tmpArr[setPort - 25]);
                ////}
                //string tmp1 = (double.Parse(wsPower) - double.Parse(setAtt)).ToString();
                //string tmp2 = (double.Parse(isPower) - double.Parse(setAtt)).ToString();
                //Log("LOG: Thoughtput = " + TP + "%" + " WS Power =" + tmp1 + " IS Power =" + tmp2);
                //if (TP < 95) Log("LOG: Throughput < 95% => FAILED!");
                //else Log("LOG: Throughput > 95% => PASSED!");
                //Log("========================================");
                //mainForm.csvRecord(mode, setPort.ToString(), wsFreq, tmp1, isFreq, tmp2, cwFreq, tmp2, TP.ToString());
            }
            catch (Exception ex)
            {
                Log("Cannot find throughput! " + ex.Message, LogLevel.ERROR);
            }
        }

        private void writeCSVThroughputValue(string mode, int port, string wsFreq, string wsPower, string wsRBOffset, string isFreq, string isPower, string isRBOffset, string throughput, out double TP)
        {
            int index;
            tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //========== 16.2.2024 - sua lai doan code ======
            //index = (port - 1) % tmpArr.Length;
            //double.TryParse(tmpArr[index], out TP);

            //Redhat - get throughput
            double.TryParse(tmpArr[(currPort.StreamIndex - 1) % tmpArr.Length], out TP);

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
        }
    }
}