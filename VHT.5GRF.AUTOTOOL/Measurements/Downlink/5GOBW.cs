using System;

namespace _5GAutoTool
{
    class _5GOBW
    {
        public Form5GAT mainForm;
        public _5GOBW(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        public void Reset()
        {
            sameMode = "";
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GOBW");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        public void OBWMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int []port, string setFreq, string setBW, out string[] obwMeas)
        {
            obwMeas = null;
            Mode = "OBW_" + setBW + "MHz_TDD";
            
            try
            {
                if (Mode != sameMode)
                {
                    //vsa.LoadMeasurementSetup(Mode, setFreq);
                    //vsa.LoadMeasurementSetup(NRTM, setFreq);
                    //bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                }

                //vsa.SetParameter(setFreq, setAtt);
                //vsa.AutoScale();
                //System.Threading.Thread.Sleep(5000); //on dinh may do
                //vsa.SingleSweepOnOff(0);
                vsa.OBWMeasurement(port, NRTM, out obwMeas);

            }
            catch (Exception ex)
            {
                obwMeas = null;
                Log("OBW Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}