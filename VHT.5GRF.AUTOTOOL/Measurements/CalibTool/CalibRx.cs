using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    class CalibRX
    {
        public Form5GAT mainForm;
        public CalibRX(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel);
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        // TODO: CalibRx function to calibrate Rx gain
        public void Calibrx(BBU bbu, List<int> testingportList, VSG vsg1, string freqSet, string power, out double rxDeviation, out bool flagrx)
        {
            double[] RXpower = new double[testingportList.Count()];
            string respond = null;
            bool flagStart = false;
            bool flag = true;
            flagrx = true;
            rxDeviation = 0;
            double attSetVal = 0;
            double rxPowerMean = 0;
            double rxMinPower = 0;
            double[] attVal = new double[testingportList.Count()];

            foreach (int i in testingportList)
            {
                bbu.CHG_RX_GAIN(i.ToString(), "0");
                attVal[testingportList.IndexOf(i)] = mainForm.GeneralDataSetupRX("RX", i , out attVal[testingportList.IndexOf(i)]);
                attVal.Append(attVal[testingportList.IndexOf(i)]);             
            }
            bbu.StartRXCalib(out flagStart);
            vsg1.SetCWWaveform(power, freqSet);
            while (flag)
            {
                if (flagStart)
                {
                    foreach(int i in testingportList)
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        System.Threading.Thread.Sleep(1);
                        //attSetVal = mainForm.GeneralDataSetupRX("RX", i ,out attSetVal); // Set suy hao va switch tung port RX
                        string portpower = (attVal[testingportList.IndexOf(i)] - attVal[0] + double.Parse(power)).ToString();
                        //vsg1.setParameter(1,portpower,freqSet);
                        vsg1.SetPower(portpower);
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        bbu.SampleRXCalib(i, out rxPowerMean);
                    }
                    bbu.FinishRXCalib(out rxMinPower);
                    if (rxMinPower > (-50))
                    {
                        //
                        foreach (int i in testingportList)
                        {
                            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                            System.Threading.Thread.Sleep(1);
                            string portpower = (attVal[testingportList.IndexOf(i)] - attVal[0] + double.Parse(power)).ToString();
                            //vsg1.setParameter(1, portpower, freqSet);
                            vsg1.SetPower(portpower);
                            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                            bbu.SampleRXCalib(i, out rxPowerMean);
                            Log("RSSI : Port" + i + ": " + rxPowerMean + "dbfs");
                            RXpower[testingportList.IndexOf(i)] = rxPowerMean;
                        }
                        rxDeviation = Math.Round(Math.Abs(RXpower.Max() - RXpower.Min()), 2);
                        Log("Do lech RX Power: " + RXpower.Max() + " - " + RXpower.Min() + " = " + rxDeviation + "dB");
                        flag = false;
                        flagrx = true;
                    }
                    else
                    {
                        flag = false;
                        flagrx = false;
                    }
                }
                else
                {
                    flag = false;
                    flagrx = false;
                }
            }
        }
    }
}
