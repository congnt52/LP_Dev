using System;

namespace _5GAutoTool
{
    class _5GOnOffPower
    {
        public Form5GAT mainForm;
        public _5GOnOffPower(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        public void Reset()
        {
            sameMode = "";
        }
        double[] tmp = new double[14];

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GOnOffPower");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        public void OnOffPowerMeasurement(VSA vsa, BBU bbu, string NRTM, int port, string setAtt, string setFreq, out string OffPower, out string TransPeriod)
        {
            OffPower = "ERROR";
            TransPeriod = "ERROR";
            Mode = "ONOFFPower_100Mhz_3D";
            try
            {
                if (Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup("ONOFFPower_100Mhz_3D", setFreq);
                    bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                }

                vsa.SetParameter(setFreq, setAtt);
                System.Threading.Thread.Sleep(3000);
                vsa.AdjustTiming();
                System.Threading.Thread.Sleep(5000);
                vsa.SingleSweepOnOff(0);
                vsa.OnOffMeasurement(Mode, NRTM, out OffPower, out TransPeriod, port);

            }
            catch (Exception ex)
            {
                Log("ON/OFF Power Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}