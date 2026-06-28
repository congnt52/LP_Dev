using System;

namespace _5GAutoTool
{
    class _5GTAE
    {
        public Form5GAT mainForm;
        public _5GTAE(Form5GAT mMainForm)
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
            log.Log(text, logLevel,"", "_5GTAE");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void TAEMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, string setFreq, out string tae)
        {
            tae = "ERROR";
            Mode = "TAE_7D1S2U_100MHz";
            try
            {
                if (Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup("TAE_7D1S2U_100MHz", setFreq);
                    bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                }

                vsa.SetParameter(setFreq, setAtt);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.TAEMeasurement(setFreq, out tae);
            }
            catch (Exception ex)
            {
                Log("TAE Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void NewTAEMeasurement(VSA vsa, BBU bbu, string NRTM, int port, string att, string setFreq, ref string taeref, ref string[] tae)
        {
            //taeref = "0";
            tae = new string[] { };
            Mode = "new_Time Aligment Error";
            try
            {
                if (Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup(NRTM, setFreq);
                    bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                }

                vsa.SetParameter(setFreq, att);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.NewTAEMeasurement(Mode, NRTM, port, ref taeref, ref tae);

            }
            catch (Exception ex)
            {
                Log("TAE Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}