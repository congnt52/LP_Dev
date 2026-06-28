using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _5GAutoTool
{
    class CalibTXPower
    {
        public Form5GAT mainForm;
        public CalibTXPower(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        public void Reset()
        {
            sameMode = "";
        }
        // Tool calib
        string mode = "NRTM_11", sameMode = "";
        string powerPreCalib = "0";
        double lowRange, highRange, lowPower, highPower, gainLimit, lowGainLimit;
        double lowGain, highGain;
        int dfe = 1;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel);
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        public void calibTX(VSA vsa, BBU bbu, RRU rru, string totalchn, string channarr, string NRTM, string setAtt, string setFreq, string gainDefault, string powerExpected, out string powerMeas, out string rruReadingPower, out string gainTX)
        {
            rruReadingPower = "";
            gainTX = ""; 
            powerMeas = "";
            bool flagCalib = true;
            double rsPower;
            double pwrMeas = 0;
            double lowPower = 30; //30dBm (1W) tuong ung voi gia tri gainDefaut trong cau hinh
            double.TryParse(powerExpected, out double pwrExpected);
            double.TryParse(gainDefault, out double gainDefaultValue);
            //if (sameMode != mode)
            //{
            //    vsa.LoadMeasurementSetup(NRTM, setFreq);
            //    vsa.SetParameter(setFreq, setAtt);
            //    bbu.Generate_NRTM(NRTM);
            //    //pwrMeas = ReadAndValidateCalibPower(bbu, vsa, port, gainDefaultValue, lowPower, 5); //limit = 5 dBm, low =30dBm
            //    //VSAPower(bbu, vsa, port, gainDefaultValue, out pwrMeas);
            //}
            ////Set gain at low power (1W)
            //lowGain = gainDefaultValue;
            //double deltaLow = Math.Round((pwrMeas - lowPower),1)*1e3; //lam tron toi hang tram            
            //while (Math.Abs(deltaLow) > 2000)
            //{                
            //    pwrMeas = ReadAndValidateCalibPower(bbu, vsa, port, lowGain, lowPower, 5);
            //    deltaLow = Math.Round((pwrMeas - lowPower), 1)*1e3; //lam tron toi hang tram
            //    lowGain += deltaLow; //giam them 100 nua                
            //}

            //Console.WriteLine($"Set Low Gain = {lowGain-100}");
            //bbu.CAL_RRU_PWR(port, "low", pwrMeas.ToString());

            ////Set gain at expected power 
            //highGain = lowGain - (pwrExpected-lowPower)*1e3;
            //double deltaHigh = Math.Round((pwrMeas - pwrExpected), 1)*1e3; //lam tron toi hang tram            
            //while (Math.Abs(deltaHigh) > 2000)
            //{
            //    pwrMeas = ReadAndValidateCalibPower(bbu, vsa, port, highGain, pwrExpected, 5);
            //    deltaHigh = Math.Round((pwrMeas - pwrExpected), 1)*1e3; //lam tron toi hang tram
            //    highGain = gainDefaultValue + deltaLow + 100; //giam them 100 nua                
            //}

            //Console.WriteLine($"Set Low Gain = {lowGain}");
            //bbu.CAL_RRU_PWR(port, "low", pwrMeas.ToString());





            //while (Math.Abs(deltaLow) > 2000)
            //{
            //    pwrMeas = ReadAndValidateCalibPower(bbu, vsa, port, lowGain, lowPower, 5);
            //    deltaLow = Math.Round((pwrMeas - lowPower), 1)*1e3; //lam tron toi hang tram
            //    lowGain = gainDefaultValue + deltaLow + 100; //giam them 100 nua                
            //}
            //if (highGain <= 2000)
            //{
            //    pwrMeas = ReadAndValidateCalibPower(bbu, vsa, port, gain, pwrExpected, 5); //limit = 5 dBm
            //    VSAPower(bbu, vsa, port, highGain, out pwrMeas);
            //    double deltaHigh = Math.Round((pwrMeas - pwrExpected), 1)*1e3; //lam tron toi hang tram
            //    if (Math.Abs(deltaHigh)<=2000)
            //    {
            //        lowGain = gainDefaultValue + deltaLow + 100; //giam them 100 nua
            //        Console.WriteLine($"Set Low Gain = {lowGain}");
            //        bbu.CAL_RRU_PWR(port, "low", pwrMeas.ToString());
            //    }
            //    bbu.CAL_RRU_PWR(port, "high", pwrMeas.ToString());
            //}
            //else
            //{
            //    Log("Gain Value has been out of limitation!");
            //}







            //code cu, tam thoi bo 
            if (double.Parse(powerExpected) >= 44 && double.Parse(powerExpected) <= 47)
            {
                lowRange = 37000;
                gainLimit = 2000;
                lowGainLimit = 40000;

            }
            else if (double.Parse(powerExpected) >= 35 && double.Parse(powerExpected) <= 37)
            {
                lowRange = 30000;
                gainLimit = 2000;
                lowGainLimit = 40000;
            }
            else if (double.Parse(powerExpected) >= 37 && double.Parse(powerExpected) <= 40)
            {
                lowRange = 30000;
                gainLimit = 2000;
                lowGainLimit = 40000;
            }
            else 
            {
                lowRange = 30000;
                gainLimit = 2000;
                lowGainLimit = 40000;
            }
            highRange = double.Parse(powerExpected) * 1000;
            bbu.Calib_RRU("STR");
            if (sameMode != mode)
            {
                vsa.LoadMeasurementSetup(NRTM, setFreq);
                vsa.SetParameter(setFreq, setAtt);
                bbu.Generate_NRTM(NRTM);
                
                VSAPower(bbu, vsa, totalchn, channarr, double.Parse(gainDefault), out lowPower);
                if (lowPower <= ((double.Parse(powerExpected)*1000) + gainLimit - double.Parse(gainDefault)) || lowPower >= highRange)
                {
                    powerMeas = (lowPower / 1000).ToString();
                }
                else if (lowPower >= (lowRange - 1000) && lowPower <= (lowRange + 1000))
                {
                    bbu.CAL_RRU_PWR(totalchn, channarr, "LOW", lowPower.ToString());
                    double highGain = double.Parse(gainDefault) - (highRange - lowPower);
                    VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                    while (true)
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        Thread.Sleep(1);
                        if (highPower >= (highRange - 2000) && highPower <= highRange)
                        {
                            break;
                        }
                        else
                        {
                            highGain = highGain - (highRange - highPower);
                            if (highGain <= gainLimit || highGain >= lowGainLimit)
                            {
                                flagCalib = false;
                                break;
                            }
                            VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                        }
                    }
                    if (flagCalib)
                    {
                        bbu.CAL_RRU_PWR(totalchn, channarr, "HIGH", highPower.ToString());
                        Calib(bbu, vsa, totalchn, channarr, lowRange.ToString(), highRange.ToString(), out powerMeas);
                    }
                    else
                    {
                        powerMeas = (highPower / 1000).ToString();
                    }
                }
                else
                {
                    double newGain = double.Parse(gainDefault);
                    while (true)
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        Thread.Sleep(1);
                        newGain = newGain - (30000 - lowPower);
                        if (newGain <= gainLimit || newGain >= lowGainLimit)
                        {
                            flagCalib = false;
                            break;
                        }
                        if (lowPower >= (lowRange - 1000) && lowPower <= (lowRange + 1000))
                        {
                            break;
                        }
                        VSAPower(bbu, vsa, totalchn, channarr, newGain, out lowPower);
                    }
                    if (flagCalib)
                    {
                        bbu.CAL_RRU_PWR(totalchn, channarr, "LOW", lowPower.ToString());
                        double highGain = double.Parse(gainDefault) - (highRange - lowPower);
                        VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                        while (true)
                        {
                            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                            Thread.Sleep(1);
                            if (highPower >= (highRange - 2000) && highPower <= highRange)
                            {
                                break;
                            }
                            else
                            {
                                highGain = highGain - (highRange - highPower);
                                if (highGain <= gainLimit || highGain >= lowGainLimit )
                                {
                                    flagCalib = false;
                                    break;
                                }
                                VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                            }
                        }
                        if (flagCalib)
                        {
                            bbu.CAL_RRU_PWR(totalchn, channarr, "HIGH", highPower.ToString());
                            Calib(bbu, vsa, totalchn, channarr, lowRange.ToString(), highRange.ToString(), out powerMeas);
                        }
                        else
                        {
                            powerMeas = (highPower / 1000).ToString();
                        }
                    }
                    else
                    {
                        powerMeas = (lowPower / 1000).ToString();
                    }
                }
                sameMode = mode;
            }
            else
            {
                vsa.SetParameter(setFreq, setAtt);
                VSAPower(bbu, vsa, totalchn, channarr, double.Parse(gainDefault), out lowPower);
                if (lowPower <= ((double.Parse(powerExpected) * 1000) + gainLimit - double.Parse(gainDefault)) || lowPower >= highRange)
                {
                    powerMeas = (lowPower / 1000).ToString();
                }
                else if (lowPower >= (lowRange - 1000) && lowPower <= (lowRange + 1000))
                {
                    bbu.CAL_RRU_PWR(totalchn, channarr, "LOW", lowPower.ToString());
                    double highGain = double.Parse(gainDefault) - (highRange - lowPower);
                    VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                    while (true)
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        Thread.Sleep(1);
                        if (highPower >= (highRange - 2000) && highPower <= highRange)
                        {
                            break;
                        }
                        else
                        {
                            highGain = highGain - (highRange - highPower);
                            if (highGain <= gainLimit || highGain >= lowGainLimit)
                            {
                                flagCalib = false;
                                break;
                            }
                            VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                        }
                    }
                    if (flagCalib)
                    {
                        bbu.CAL_RRU_PWR(totalchn, channarr, "HIGH", highPower.ToString());
                        Calib(bbu, vsa, totalchn, channarr, lowRange.ToString(), highRange.ToString(), out powerMeas);
                    }
                    else
                    {
                        powerMeas = (highPower / 1000).ToString();
                    }
                }
                else
                {
                    double newGain = double.Parse(gainDefault);
                    while (true)
                    {
                        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                        Thread.Sleep(1);
                        newGain = newGain - (30000 - lowPower);
                        if (newGain <= gainLimit || newGain >= lowGainLimit)
                        {
                            flagCalib = false;
                            break;
                        }
                        if (lowPower >= (lowRange - 1000) && lowPower <= (lowRange + 1000))
                        {
                            break;
                        }
                        VSAPower(bbu, vsa, totalchn, channarr, newGain, out lowPower);
                    }
                    if (flagCalib)
                    {
                        bbu.CAL_RRU_PWR(totalchn, channarr, "LOW", lowPower.ToString());
                        double highGain = double.Parse(gainDefault) - (highRange - lowPower);
                        VSAPower(bbu, vsa, totalchn, channarr, highGain, out highPower);
                        while (true)
                        {
                            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                            Thread.Sleep(1);
                            if (highPower >= (highRange - 2000) && highPower <= highRange)
                            {
                                break;
                            }
                            else
                            {
                                highGain = highGain - (highRange - highPower);
                                if (highGain <= gainLimit || highGain >= lowGainLimit)
                                {
                                    flagCalib = false;
                                    break;
                                }
                                VSAPower(bbu, vsa, totalchn, channarr, highGain,out highPower);
                            }
                        }
                        if (flagCalib)
                        {
                            bbu.CAL_RRU_PWR(totalchn, channarr, "high", highPower.ToString());
                            Calib(bbu, vsa,totalchn, channarr, lowRange.ToString(), highRange.ToString(), out powerMeas);
                        }
                        else
                        {
                            powerMeas = (highPower / 1000).ToString();
                        }
                    }
                    else
                    {
                        powerMeas = (lowPower / 1000).ToString();
                    }
                }
                bbu.Calib_RRU("STP");
            }
        }
        public void VSAPower(BBU bbu, VSA vsa, string totalchn, string channarr, double gain, out double power)
        {
            bbu.CHG_TX_GAIN(channarr, gain.ToString());
            string tmp;
            vsa.AutoScale();
            System.Threading.Thread.Sleep(3000);
            vsa.OutputPowerMeasurement(int.Parse(channarr), bbu, out tmp);
            double.TryParse(tmp, out power);
            power = Math.Round(power,1) * 1000;
            Console.WriteLine(power);
        }
        //private double ReadAndValidateCalibPower(BBU bbu, VSA vsa, string port, double gain, double pwrExpect, double limit, double acceptRange)
        //{
        //    double retVal = 0;
        //    VSAPower(bbu, vsa, port, gain, out retVal);
        //    double delta = pwrExpect-retVal;
        //    while (Math.Abs(delta) > limit)
        //    {
        //        Console.WriteLine($"Power = {retVal} dBm from VSA is an Abnormal Value, reading again!");
        //        VSAPower(bbu, vsa, port, gain, out retVal);
        //        if(retVal)
        //    }

        //    return retVal;
        //}
        public void Calib(BBU bbu, VSA vsa, string totalchn, string channarr, string lowRange, string highRange, out string powerMeas)
        {
            powerMeas = "";
            bbu.CLB_XADC(totalchn, channarr);
            bbu.CALIB_RRU(totalchn, channarr, lowRange, highRange);
            vsa.AutoScale();
            vsa.OutputPowerMeasurement(int.Parse(channarr), bbu, out powerMeas);
        }
    }
}
