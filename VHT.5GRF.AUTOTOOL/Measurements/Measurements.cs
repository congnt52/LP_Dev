using Keysight.SignalStudio.N7631;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace _5GAutoTool
{
    class Measurements1
    {
        public Form5GAT mainForm;
        public Measurements1(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
            meas5GEVMFreqErr = new _5GEVMFreqErr(mainForm);
            meas5GACRLPower = new _5GACLR(mainForm);
            meas5GOBW = new _5GOBW(mainForm);
            meas5GSEM = new _5GSEM(mainForm);
            meas5GOutputPower = new _5GOutputPower(mainForm);
            meas5GIMDACLR = new _5GIMDACLR(mainForm);
            meas5GIMDSEM = new _5GIMDSEM(mainForm);
            meas5GIMDTSE = new _5GIMDTSE(mainForm);
            meas5GSpur = new _5GSpur(mainForm);
            meas5GOnOff = new _5GOnOffPower(mainForm);
            meas5GTAE = new _5GTAE(mainForm);


            meas5GoneVSG = new oneVSG(mainForm);
            meas5GtwoVSG = new twoVSG(mainForm);
            meas5GthreeVSG = new threeVSG(mainForm);
            calibTXPower = new CalibTXPower(mainForm);
            calibrx = new CalibRX(mainForm);
        }
        CalibRX calibrx;
        CalibTXPower calibTXPower;
        _5GEVMFreqErr meas5GEVMFreqErr;
        _5GACLR meas5GACRLPower;
        _5GOBW meas5GOBW;
        _5GSEM meas5GSEM;
        _5GSpur meas5GSpur;
        _5GOutputPower meas5GOutputPower;
        _5GOnOffPower meas5GOnOff;
        _5GTAE meas5GTAE;
        _5GIMDACLR meas5GIMDACLR;
        _5GIMDSEM meas5GIMDSEM;
        _5GIMDTSE meas5GIMDTSE;
        oneVSG meas5GoneVSG;
        twoVSG meas5GtwoVSG;
        threeVSG meas5GthreeVSG;
        public void Output5G(VSA vsa, BBU bbu, string NRTM, string setAtt, int [] port, string setFreq, string setPower, string setBW, out string[] Power)
        {
            meas5GOutputPower.OutputPowerMeasurement(vsa, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, out Power);
        }
        public void EVMFreqErr5G(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setPower, string setBW, out string[] evm, out string[] freqErr)
        {
            meas5GEVMFreqErr.EVMFreqErrMeasurement(vsa, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, out evm, out freqErr);
        }

        public void ACLRPower5G(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setPower, string setBW, out string[] ACLRMeas,
            out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            meas5GACRLPower.ACLRMeasurement(vsa, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, out ACLRMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
        }

        public void OBW5G(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setBW, out string[] obwMeas)
        {
            meas5GOBW.OBWMeasurement(vsa, bbu, NRTM, setAtt,port, setFreq, setBW, out obwMeas);
        }

        public void SEM5G(VSA vsa, BBU bbu, string NRTM, string setAtt, int[] port, string setFreq, string setPower, string setBW, out double absP1, out double absP2, out double absP3, out string[] semLimit)
        {
            meas5GSEM.SEMMeasurement(vsa, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, out absP1, out absP2, out absP3, out semLimit);
        }
        public void Spur5G(VSA vsa, BBU bbu, string mode, string coupling, string NRTM, string setAtt, int port, string setFreq, string setRBW, out double?[] spur)
        {
            //meas5GSpur.SpurTest(vsa, mode, NRTM, setAtt, port, setFreq, setRBW, out spur);
            meas5GSpur.SpurMeasurement(vsa, bbu, mode, coupling, NRTM, setAtt, port, setFreq, setRBW, out spur);
        }
        public void IMDACLR5G(VSA vsa, VSG vsg1, BBU bbu, int port, string NRTM, string setAtt, string setFreq, string setBW, string offSet, string isPower, out double IMDACLR)
        {
            meas5GIMDACLR.IMDACLRMeasurement(vsa, vsg1, bbu, port, NRTM, setAtt, setFreq, setBW, offSet, isPower, out IMDACLR);
        }

        public void IMDSEM5G(VSA vsa, VSG vsg1, BBU bbu, int port, string NRTM, string setAtt, string setFreq, string setPower, string setBW, string offSet, string isPower, out string semLimit)
        {
            meas5GIMDSEM.IMDSEMMeasurement(vsa, vsg1, bbu, port, NRTM, setAtt, setFreq, setPower, setBW, offSet, isPower, out semLimit);
        }

        public void IMDTSE5G(VSA vsa, VSG vsg1, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, string offSet, string isPower, out double?[] imdTSE)
        {
            meas5GIMDTSE.IMDTSEmissionMeasurement(vsa, vsg1, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, offSet, isPower, out imdTSE);
        }
        public void OnOffPower5G(VSA vsa, BBU bbu, string NRTM, int j, string setAtt, string setFreq, out string OnOffPower, out string TransPeriod)
        {
            meas5GOnOff.OnOffPowerMeasurement(vsa, bbu, NRTM, j, setAtt, setFreq, out OnOffPower, out TransPeriod);
        }
        public void TAEMeasurement(VSA vsa, BBU bbu, string NRTM, int port, string Att, string setFreq, ref string taeref, ref string[] TAE)
        {
            meas5GTAE.NewTAEMeasurement(vsa, bbu, NRTM, port, Att, setFreq, ref taeref, ref TAE);
            //meas5GTAE.TAEMeasurement(vsa, bbu, NRTM, Att, setFreq, out TAE);
        }
        public void IntraACLR5G(VSA vsa, VSG vsg1, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, string isPower, ref double intraACLR)
        {
            meas5GIMDACLR.IntraACLRMeasurement(vsa, vsg1, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, isPower, ref intraACLR);
        }
        public void IntraSem(VSA vsa, VSG vsg1, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, string isPower, out double absP1, out double absP2, out double absP3, ref double intraSem)
        {
            meas5GIMDSEM.IntraSEMMeasurement(vsa, vsg1, bbu, NRTM, setAtt, port, setFreq, setPower, setBW, isPower, out absP1, out absP2, out absP3, ref intraSem);
        }
        public void Reset()
        {
            meas5GACRLPower.Reset();
            meas5GEVMFreqErr.Reset();
            meas5GOBW.Reset();
            meas5GOutputPower.Reset();
            meas5GSEM.Reset();
            meas5GSpur.Reset();
            calibTXPower.Reset();
            meas5GIMDACLR.Reset();
            meas5GIMDSEM.Reset();
            meas5GoneVSG.Reset();
            meas5GtwoVSG.Reset();
            meas5GthreeVSG.Reset();
            calibTXPower.Reset();
        }

        #region Properties
        //Devices properties
        //[CategoryAttribute("RRU Properties")]
        //private RRUType rruType=RRUType.RRU32T32R10W;
        //public RRUType RRUType { get { return rruType; } set { rruType = value; } }


        //Input Properties

        //Result properties

        #endregion


        #region Methods
        //uplink
        public double oneVSGMode(VSG vsg1, BBU bbu, string mode, int repeatNumber, string spec, double step, string setAtt, int setPort, string setPower, string setFreq, bool isMultiMeas, out double TP)
        {
            return meas5GoneVSG.Measurement(vsg1, bbu, mode, repeatNumber, spec, step, setAtt, setPort, setPower, setFreq, isMultiMeas, out TP);
        }

        public void oneVSGModeCharacterise(VSG vsg1, BBU bbu, string mode, double step, string setAtt, int setPort, string setPower, string setFreq, out double TP)
        {
            meas5GoneVSG.InvestigateReciver5GCharacteristic(vsg1, bbu, mode, step, setAtt, setPort, setPower, setFreq, out TP);
        }

        public void twoVSGMode(VSG vsg1, VSG vsg2, BBU bbu, string mode, string WsRBOffset, string IsRBOffset, double step, string setAtt, int setPort, string wsPower, string wsFreq, string isPower, string isFreq, string isLowerFreqRange, string isUpperFreqRange, out double TP)
        {
            meas5GtwoVSG.Measurement(vsg1, vsg2, bbu, mode, WsRBOffset, IsRBOffset, step, setAtt, setPort, wsPower, wsFreq, isPower, isFreq, isLowerFreqRange, isUpperFreqRange, out TP);
        }

        public void threeVSGMode(VSG vsg1, VSG vsg2, VSG vsg3, BBU bbu, string mode, string WsRBOffset, string IsRBOffset, double step, string setAtt, int setPort, string wsPower, string wsFreq, string isPower, string is1Freq, string is2Freq, out double TP)
        {
            meas5GthreeVSG.Measurement(vsg1, vsg2, vsg3, bbu, mode, WsRBOffset, IsRBOffset, step, setAtt, setPort, wsPower, wsFreq, isPower, is1Freq, is2Freq, out TP);
        }

        // calib tool tx
        public void calibTX(VSA vsa, BBU bbu, RRU rru, string totalchn, string channarr, string NRTM, string setAtt, string setFreq, string gainDefault, string powerExpected, out string powerMeas, out string rruReadingPower, out string gainTX)
        {
            calibTXPower.calibTX(vsa, bbu, rru, totalchn, channarr, NRTM, setAtt, setFreq, gainDefault, powerExpected, out powerMeas, out rruReadingPower, out gainTX);
        }
        public void calibRX(BBU bbu, List<int> testingportList, VSG vsg1, string setFreq, string power, out double rxDeviation, out bool flag)
        {
            
            calibrx.Calibrx(bbu, testingportList, vsg1, setFreq, power, out rxDeviation, out flag);
        }
        #endregion
    }

    //TODO: Add class SpuriousRange - 13.6.2024
    public class SpuriousRange
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartFreq { get; set; } //Hz
        public string StopFreq { get; set; }  //Hz
        public string ResBW { get; set; }
        public string VidBW { get; set; }
        public string RFCoupling { get; set; }
        public string SweepTime { get; set; }
        public string TraceType { get; set; }
        public string Detector { get; set; }
        public int AveNum { get; set; }
        public string Category { get; set; }
        public string ABSStartLim { get; set; } //dBm
        public string ABSStopLim { get; set; } //dBm
        public string CorrectionFile { get; set; }
        public string ResultFreq { get; set; }
        public string ResultLevel { get; set; }
        public string ResultPassFail { get; set; }
        //public string RruFreq { get;set; }
        //public string RruPower { get; set; }
        //public string RruNRTM { get; set; }
    }
}