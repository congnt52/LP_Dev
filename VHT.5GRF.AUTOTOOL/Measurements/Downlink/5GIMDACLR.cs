using System;

namespace _5GAutoTool
{
    class _5GIMDACLR
    {
        public Form5GAT mainForm;
        public _5GIMDACLR(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        string sameOffset = "";
        public void Reset()
        {
            sameMode = "";
            sameOffset = "";
        }
        string[] tmp = new string[2];
        string vsgFreq;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GIMDACLR");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void IMDACLRMeasurement(VSA vsa, VSG vsg1, BBU bbu, int port, string NRTM, string setAtt, string setFreq, string setBW, string offSet, string isPower, out double IMDACLR)
        {
            Mode = NRTM;
            IMDACLR = 0;
            try
            {
                double offsetVal = double.Parse(offSet) * 1e6;
                double factor = (offsetVal > 0) ? 1 : -1;
                vsgFreq = Convert.ToString((double.Parse(setFreq)) + factor * ((double.Parse(setBW)) / 2) * 1e6 + offsetVal);

                if (Mode != sameMode || offSet != sameOffset)
                {
                    if (Mode != sameMode)
                    {
                        bbu.Generate_NRTM(NRTM);
                    }

                    vsa.LoadIMDSetup(offSet, "ACLR", setFreq);
                    sameMode = Mode;
                    sameOffset = offSet;
                }

                vsg1.SetTXIMDWaveform(isPower, vsgFreq);
                vsa.SetParameter(setFreq, setAtt);

                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.IMDACLRMeasurement(offSet, out IMDACLR, NRTM, port);

            }
            catch (Exception ex)
            {
                Log("IMD/ACLR Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void IntraACLRMeasurement(VSA vsa, VSG vsg1, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower,
            string setBW, string isPower, ref double intraACLR)
        {
            Mode = "ACLR_" + NRTM + "_" + setBW + "MHz_TDD";
            try
            {
                if (Mode != sameMode)
                {
                    vsg1.SetNRTM11FDD(isPower, setFreq);
                    vsa.LoadMeasurementSetup("ACLR", setFreq);
                    bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                }

                vsa.SetParameter(setFreq, setAtt);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.ACLRPowerMeasurement(port, NRTM, ref intraACLR, "INTRAACLR");
            }
            catch (Exception ex)
            {
                Log("IMD/ACLR(Intra System) Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}