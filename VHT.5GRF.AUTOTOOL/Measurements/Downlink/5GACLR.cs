namespace _5GAutoTool
{
    class _5GACLR
    {
        public Form5GAT mainForm;
        public _5GACLR(Form5GAT mMainForm)
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
            log.Log(text, logLevel,"", "_5GACLR");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void ACLRMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setPower, string setBW, out string[] ACLRMeas,
            out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            aclrAdjLower = "ERROR";
            aclrAdjUpper = "ERROR";
            aclrAlt1Lower = "ERROR";
            aclrAlt1Upper = "ERROR";
            ACLRMeas = null;
            Mode = "ACLR_" + NRTM + "_" + setBW + "Mhz_TDD";
            try
            {
                System.Threading.Thread.Sleep(1000);
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;

                if (NRTM != mainForm.currNRTM)
                {
                    //vsa.LoadMeasurementSetup("ACLR_100Mhz_TDD", setFreq);
                    vsa.LoadMeasurementSetup(NRTM, setFreq);
                    bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                    mainForm.currNRTM = NRTM;
                }

                //vsa.SetParameter(setFreq, setAtt);
                //vsa.AutoScale();
                //vsa.SingleSweepOnOff(0);
                vsa.ACLRPowerMeasurement(port, NRTM, out ACLRMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            catch
            {
                Log("ACLR Measurement: FAIL!", LogLevel.ERROR);
            }
        }
    }
}