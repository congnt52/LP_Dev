using System;
using System.Threading;

namespace _5GAutoTool
{
    class _5GIMDSEM
    {
        public Form5GAT mainForm;
        public _5GIMDSEM(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        string vsgFreq;
        string sameOffset = "";
        public void Reset()
        {
            sameMode = "";
            sameOffset = "";
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GIMDSEM");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void IMDSEMMeasurement(VSA vsa, VSG vsg1, BBU bbu, int port, string NRTM, string setAtt, string setFreq, string setPower, string setBW, string offSet, string isPower, out string semLimit)
        {
            semLimit = "ERROR";
            Mode = NRTM;
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

                    vsa.LoadIMDSetup(offSet, "SEM", setFreq);
                    sameMode = Mode;
                    sameOffset = offSet;
                }

                vsg1.SetTXIMDWaveform(isPower, vsgFreq);
                vsa.SetParameter(setFreq, setAtt);
                vsa.AutoScale();
                Thread.Sleep(2000);
                vsa.SingleSweepOnOff(0);
                vsa.IMDSEMMeasurement(setAtt, setFreq, offSet, out semLimit, NRTM, port);

            }
            catch (Exception ex)
            {
                Log("IMD/SEM Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
        internal void IntraSEMMeasurement(VSA vsa, VSG vsg1, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, string isPower, out double absP1, out double absP2, out double absP3, ref double intraSem)
        {
            absP1 = 0;
            absP2 = 0;
            absP3 = 0;
            Mode = "SEM_" + NRTM + "_" + setBW + "MHz_TDD";
            try
            {
                if (Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup("SEM_100MHz_TDD", setFreq);
                    bbu.Generate_NRTM(NRTM);
                    sameMode = Mode;
                }

                vsg1.SetNRTM11FDD(isPower, setFreq);
                vsa.SetParameter(setFreq, setAtt);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.SEMMeasurement(port, NRTM, out absP1, out absP2, out absP3, out intraSem);
            }
            catch (Exception ex)
            {
                Log("IMD/SEM(Intra System) Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}