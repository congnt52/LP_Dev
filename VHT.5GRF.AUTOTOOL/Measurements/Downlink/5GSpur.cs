using System;

namespace _5GAutoTool
{
    class _5GSpur
    {
        public Form5GAT mainForm;
        public _5GSpur(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "", RBW = "", NRTMcheck = "", setcoupling ="";
        public void Reset()
        {
            sameMode = "";
            RBW = "";
            NRTMcheck = "";
        }
        double?[] tmp = new double?[20];

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GSpur");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void SpurMeasurement(VSA vsa, BBU bbu, string mode, string coupling, string NRTM, string setAtt, int port, string setFreq, string setRBW, out double?[] spur)
        {
            spur = tmp;
            try
            {
                if (mode == sameMode && setRBW == RBW && coupling == setcoupling)
                {
                    if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                    System.Threading.Thread.Sleep(3000);
                    vsa.SetParameter(setFreq, setAtt);
                    //===19.2.2024 change VSA to single mode ===
                    vsa.SingleSweepOnOff(0);
                    vsa.Restart();
                    System.Threading.Thread.Sleep(2000);
                    //++++++++++++++++++++++++++++++++++++++++++
                    vsa.SpurMeasurement(port, setAtt, setFreq, out spur);
                }
                else
                {
                    if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                    System.Threading.Thread.Sleep(3000);
                    bbu.Generate_NRTM("NRTM_11");
                    vsa.LoadMeasurementSetup("Spur_100MHz_TDD", setFreq, mode, setRBW, coupling);
                    vsa.SetParameter(setFreq, setAtt);
                    //===19.2.2024 change VSA to single mode ===
                    vsa.SingleSweepOnOff(0);
                    vsa.Restart();
                    System.Threading.Thread.Sleep(2000);
                    //++++++++++++++++++++++++++++++++++++++++++
                    vsa.SpurMeasurement(port, setAtt, setFreq, out spur);
                    sameMode = mode;
                    RBW = setRBW;
                }
            }
            catch
            {
                Console.WriteLine("Spurious measurement: FAIL");
            }
        }
    }
}