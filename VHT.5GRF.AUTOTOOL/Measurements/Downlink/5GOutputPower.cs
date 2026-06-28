using System;

namespace _5GAutoTool
{
    class _5GOutputPower
    {
        public Form5GAT mainForm;
        string Name = "Base station output power";
        LogAdapter log = LogAdapter.Instance;
        public _5GOutputPower(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string oldNRTM = "";
        public void Reset()
        {
            oldNRTM = "";
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GOutputPower");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============


        public void OutputPowerMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, out string TXPower)
        {
            TXPower = "ERROR";
            try
            {
                if (NRTM != mainForm.currNRTM)
                {
                    log.Log("Setting up measurement...");
                    vsa.LoadMeasurementSetup(NRTM, setFreq);
                    //bbu.Generate_NRTM(NRTM);
                    oldNRTM = NRTM;
                }
                //vsa.SetParameter(setFreq, setAtt);
                //vsa.AutoScale();
                //vsa.SingleSweepOnOff(0);
                vsa.OutputPowerMeasurement(port, bbu, out TXPower);
            }
            catch (Exception ex)
            {
                Log("Channel power Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        public void OutputPowerMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setPower, string setBW, out string[] TXPower)
        {
            TXPower = null;
            try
            {
                if (NRTM != mainForm.currNRTM)
                {
                    bbu.Generate_NRTM(NRTM);
                    log.Log("Setting up measurement...");
                    vsa.LoadMeasurementSetup(NRTM, setFreq);
                    
                    mainForm.currNRTM = NRTM;
                }
                //vsa.SetParameter(setFreq, setAtt);
                //vsa.AutoScale();
                //vsa.SingleSweepOnOff(0);
                vsa.OutputPowerMeasurement(port, bbu, out TXPower);
            }
            catch (Exception ex)
            {
                Log("Channel power Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}
