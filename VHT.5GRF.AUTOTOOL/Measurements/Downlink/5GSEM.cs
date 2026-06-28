using System;

namespace _5GAutoTool
{
    class _5GSEM
    {
        public Form5GAT mainForm;
        public _5GSEM(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        public void Reset()
        {
            sameMode = "";
            mainForm.currNRTM = "";
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, "", "_5GSEM");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void SEMMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setPower, string setBW, out double absP1, out double absP2, out double absP3, out string[] semLimit)
        {
            semLimit = null;
            absP1 = 0;
            absP2 = 0;
            absP3 = 0;
            Mode = "SEM_100MHz_TDD";
            try
            {
                if (NRTM != mainForm.currNRTM)
                {
                    
                    bbu.Generate_NRTM(NRTM);
                    mainForm.currNRTM = NRTM;
                }
                if(Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup(Mode, setFreq);
                    sameMode = Mode;
                }
                //vsa.SetParameter(setFreq, setAtt);
                //vsa.AutoScale();
                //vsa.SingleSweepOnOff(0);
                vsa.SEMMeasurement(port, NRTM, out absP1, out absP2, out absP3, out semLimit);

            }
            catch (Exception ex)
            {
                Log("SEM Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}