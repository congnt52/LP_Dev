using System;
using System.Threading;

namespace _5GAutoTool
{
    class _5GIMDTSE
    {
        public Form5GAT mainForm;
        public _5GIMDTSE(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        string vsgFreq;
        public void Reset()
        {
            sameMode = "";
        }
        double?[] tmp = new double?[100];

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GIMDTSE");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void IMDTSEmissionMeasurement(VSA vsa, VSG vsg1, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, string offSet, string isPower, out double?[] imdTSE)
        {
            imdTSE = tmp;
            Mode = "TSE_" + setBW + "MHz_TDD";
            try
            {
                double offsetVal = double.Parse(offSet) * 1e6;
                double factor = (offsetVal > 0) ? 1 : -1;
                vsgFreq = Convert.ToString((double.Parse(setFreq)) + factor * ((double.Parse(setBW)) / 2) * 1e6 + offsetVal);

                if (Mode != sameMode)
                {
                    bbu.Generate_NRTM(NRTM);
                    vsa.LoadIMDSetup(offSet, "TSE", setFreq);
                    sameMode = Mode;
                }

                vsg1.SetTXIMDWaveform(isPower, vsgFreq);
                Thread.Sleep(3000);
                vsa.SingleSweepOnOff(0);
                vsa.IMDTSEMeasurement(port, setFreq, setAtt, out imdTSE);

            }
            catch (Exception ex)
            {
                Log("IMD/TSE Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}