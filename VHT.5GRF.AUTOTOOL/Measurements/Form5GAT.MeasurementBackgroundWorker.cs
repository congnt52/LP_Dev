using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _5GAutoTool
{
    public partial class Form5GAT
    {
        //++++++++++++++++++ MEASUREMENT BACKGROUND WORKER +++++++++++++++++++++++++++++++++++++
        //TODO:measurementBackgroundWorker_DoWork
        #region measurementBackgroundWorker_DoWork
        public void measurementBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProgressLabel($"Staring Measurement Background Worker...");
            // in put
            string PassFail = fail;
            bool isTestcasePass = true;
            string[] Result= new string[8];
            Dictionary<int, string> dict;
            double freqHz = double.Parse(setFrequency);
            double freqMHz = freqHz / 1e6;
            double[] dbRS = new double[20];
            double?[] tseRS = new double?[20];
            double limitMin = 0;
            double limitMax = 0;
            string result;
            double tp = 0;
            double RS = 0;
            double? nRS;
            string NRTM = "";
            string note = "";
            int rruPower = int.Parse(setPower);
            double attSetVal = 0;


            //process variable
            bool reTest = false;
            TestcaseNode testcaseInTesting;
            int completedTask = 0;
            bool interEVM20 = false; //use for testcase EVM & FreqErr TM2.0
            bool interEVM20a = false; //use for testcase EVM & FreqErr TM2.0a
            bool interEVM31 = false; //use for testcase EVM & FreqErr TM3.1
            bool interEVM31a = false; //use for testcase EVM & FreqErr TM3.1a
            bool interEVM32 = false; //use for testcase EVM & FreqErr TM3.2
            bool interEVM33 = false; //use for testcase EVM & FreqErr TM3.3



            // out put
            string[] transPeriod = new string[32];
            string aclrAdjLower = "";
            string aclrAdjUpper = "";
            string aclrAlt1Lower = "";
            string aclrAlt1Upper = "";
            string[] evm = new string[32];
            string[] freq = new string[32];
            string[] taeMeas = new string[32];
            string[] tse = new string[9];
            double[] imdACLR = new double[6];
            double[] imdSEM = new double[6];
            double?[,] TSE = new double?[20, 20];
            double[] intraACLR = new double[4];
            double[] intraSEM = new double[4];
            string[,] OFDM = new string[32, 6];
            string[,] Transient = new string[32, 6];
            string[] noise = new string[32];
            int loopTime = 1;
            int timeout = 0;

            List<string> failedTestcases = new List<string>();  //create a List to store Failed Testcase

            // Store failed test results
            Dictionary<string, List<int>> failedTestList = new Dictionary<string, List<int>>();


            //OOBexcludeColocationRange = chckExcludeCoLoRange.Checked; //27.3.2024 - OOB option - include/exclude Co-location Bands
            //deltaFoobValue = double.Parse(deltaFoob.Text) * 1e6; //in Hz

            // Set frequency for RRU
            Log($"Set frequency to RRU: {freqMHz} MHz");
            //set tan so cho thiet bi
            //BBU.CHG_Freq(setFrequency);

            //BBU.TDDMODE("CALIB");  //kiem tra lai xem co dung khong, khong thi xoa

            //TODO: Assign Role
            ProgressLabel($"Assigning Devices...");
            VSG1.Name = "Wanted Signal";
            VSG2.Name = "Interference Signal";
            VSG3.Name = "CW Signal";
            rfSwitch1.Name = "RFS-Master";
            rfSwitch2.Name = "RFS-D1";
            rfSwitch3.Name = "RFS-D2";

            //Declare factory info
            userID = txbUser.Text;
            processValue = txbStation.Text + "_" + freqMHz.ToString();
            productValue = "";

            //10.1.2024 - clear Chart
            //chart1.Series.Clear(); //tam thoi comment lai
            enRecordLog = true; //start writing log to file
            PrintMeasurementInfo();
            ProgressLabel($"Measurement Start!");
            LogDashedLine($"=============== MEASUREMENT START: {DateTime.Now.ToString("f")} ===============");
            //TODO: ========= 18.9.2024 - Background worker define new loop ===========
            TestcasePending = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Pending);
            if (userID != "Administrator")
            {
                //try
                //{
                //    int apiret = api.ScanAutoTestIn(RruSerial, processValue, productValue, userID);
                //    if (apiret == 1)
                //    {
                //        log.Log("Scanin thành công !", LogLevel.INFO);
                //    }
                //    else if (apiret == -1)
                //    {
                //        log.Log("Serial đã scanin", LogLevel.WARN);
                //        MessageBox.Show("Serial đã Scanin", "WARNING", MessageBoxButtons.OK);
                //        if (measurementBackgroundWorker.IsBusy == true)
                //        {
                //            measurementBackgroundWorker.CancelAsync();
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                //    measurementBackgroundWorker.CancelAsync();
                //}
                while (TestcasePending.Count > 0)
                {
                    if (measurementBackgroundWorker.CancellationPending) return;
                    System.Threading.Thread.Sleep(1);
                    //test: Pending List to Log
                    LogDashedLine("--------- Remaining Testcases in Testing ----------------");
                    foreach (TestcaseNode node in TestcasePending)
                    {
                        Log($"+ {node.Text}");
                    }
                    LogDashedLine("-------------------------");

                    testcaseInTesting = TestcasePending[0];
                    if (testcaseInTesting != null)
                    {
                        if (testingMode == "TX")
                        {
                            //BBU.TDDMODE("TXRX");
                            //BBU.FACTORY_ANT_CALIB("DISABLE");
                            //BBU.CHG_RRU_POWER(rruPower);
                        }
                        if (testingMode == "RX")
                        {
                            BBU.TDDMODE("RX_ONLY");
                        }
                        //change node to Run Status
                        testcaseInTesting.Status = TestcaseStatus.Running;
                        ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Running);
                        //UpdateNodeStatusToTreeView(testcaseInTesting, trvTestCases.Nodes[0]);

                        //create a List to store Fail Ports
                        List<int> failedPorts = new List<int>();

                        //create Testing port list after each Testcase
                        ProgressLabel($"Collecting List of Testing Port...");
                        PopulateTestingPortList();
                        //Re-arrange Testing port list by MeasMode
                        string strMeasMode = ReadValueByHeader(testcaseInTesting.Parameters, "MultiMeas");
                        bool isMultiMeas = (strMeasMode.ToUpper() == "TRUE") ? true : false;
                        if (testingMode == "RX" && isMultiMeas)
                        {
                            //re-arrange testingPortList in groups
                            ReArrangeTestingPortList(PortMappings);
                        }
                        Log($"Testing Ports List: {string.Join(", ", testingPortList)}");

                        //Clear data MappingPort List
                        ProgressLabel($"Clearing List of Testing Port...");
                        PortListClearData(PortMappings);
                        //TODO: run sequence testing port in list
                        if (ckbMuitlMeas.Checked)
                        {
                            try
                            {
                                int temp = testingPortList.Count % 8 != 0 ? numbArr = testingPortList.Count / 8 + 1 : numbArr = testingPortList.Count / 8;
                                Multiport = new int[numbArr][];
                                for (int i = 0; i < numbArr; i++)
                                {
                                    // Determine the size of the current subarray (handle the last chunk carefully)
                                    int currentSize = Math.Min(8, testingPortList.Count - i * 8);
                                    Multiport[i] = new int[currentSize];
                                    for (int j = 0; j < currentSize; j++)
                                    {
                                        Multiport[i][j] = testingPortList[8 * i + j];
                                    }

                                    // Logging array content as string
                                    log.Log($"Array {i}: [{string.Join(", ", Multiport[i])}]");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Log($"{ex}", LogLevel.ERROR);
                            }
                        }
                        //foreach (int testingPort in testingPortList)
                        foreach (int[] mtlport in Multiport)
                        {
                            if (measurementBackgroundWorker.CancellationPending) return;
                            System.Threading.Thread.Sleep(1);
                            while (timeout < loopTime)
                            {
                                ProgressLabel($"Measuring Port:{mtlport.ToString()}");
                                reTest = false;
                                #region TXcalib
                                //if (testcaseInTesting.Text.Contains("TX Calibration"))
                                //{
                                //    mode = "NRTM_11";
                                //    GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode);
                                //    double.TryParse(measClause[i, 3], out limitMax);
                                //    double.TryParse(measClause[i, 2], out limitMin);
                                //    string gainDefault = measClause[i, 5];
                                //    string powerExpected = txtPower.Text;
                                //    limitMax = double.Parse(powerExpected) + 0.5;
                                //    limitMin = double.Parse(powerExpected) - 0.5;
                                //    Measurement.calibTX(VSA, BBU, RRU, "1", (j + 1).ToString(), mode, arrAttDL[j], setFrequency, gainDefault, powerExpected, out Result[0], out Result[1], out Result[2]);
                                //    bool isSuccess = double.TryParse(Result[0], out RS);
                                //    string gain;
                                //    BBU.SHW_TX_GAIN((j + 1).ToString(), out gain);
                                //    if (isSuccess)
                                //    {
                                //        RS = Math.Round(RS, 1);
                                //        if ((RS < limitMin || RS > limitMax))
                                //        {
                                //            PassFail = fail;
                                //            reTest = true;
                                //        }
                                //        else
                                //        {
                                //            PassFail = pass;
                                //        }
                                //        result = RS.ToString();
                                //    }
                                //    else
                                //    {
                                //        result = error;
                                //        PassFail = fail;
                                //        reTest = true;
                                //        BBU.PREPARE_CALIB_RRU((j + 1).ToString(), gainDefault);
                                //    }
                                //    BBU.CHG_RRU_POWER(1000);
                                //    Log("TRX" + (j + 1) + " Power : vsa reading " + RS + "dBm"); //+ " rru reading " + antPower + "dBm"
                                //    dataCalibTx[j, 0] = measClause[i, 2]; // 0 Min limit, 1 Reading Value, 2 Max limit, 3 Testing Time, 4 Result. 
                                //    dataCalibTx[j, 1] = result;     // 1 Reading Value
                                //    dataCalibTx[j, 2] = measClause[i, 3]; // 2 Max limit
                                //    dataCalibTx[j, 3] = DateTime.Now.ToString("hh:mm:ss");
                                //    dataCalibTx[j, 4] = PassFail;
                                //}
                                #endregion
                                #region Pout
                                //TODO: TX - Base station output power                            
                                if (testcaseInTesting.Text == "Base station output power")
                                {
                                    NRTM = "NRTM_31a";

                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);
                                    Measurement.Output5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setPower, setBandwidth, out Result); //arrAttDL duoc update tu ham doc csv                                                                                                                                                        //==============19.2.2024 save screen shot =====================
                                    currNRTM = NRTM;
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < Result.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], Result[j]);
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Round(RS, 1);
                                            if (RS < limitMin || RS > limitMax)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }

                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataPout[dt.Key - 1, 0] = PassFail;  // 4 Result
                                        dataPout[dt.Key - 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataPout[dt.Key - 1, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    //dict.Clear();
                                }
                                #endregion
                                #region OCB
                                //TODO: TX - Occupied bandwidth 
                                if (testcaseInTesting.Text == "Occupied bandwidth")
                                {
                                    NRTM = "NRTM_31a";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    double.TryParse(strLimMax, out limitMax);
                                    limitMin = 0;
                                    Measurement.OBW5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setBandwidth, out Result);
                                    currNRTM = NRTM;
                                    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    double tmp;
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < Result.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], Result[j]);
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out tmp))
                                        {
                                            RS = Math.Abs(tmp / 1e6);
                                            RS = Math.Round(RS, 1);
                                            if (RS > limitMax)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, "40", "", RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataOBW[dt.Key - 1, 0] = PassFail;  // 4 Result
                                        dataOBW[dt.Key - 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataOBW[dt.Key - 1, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    dict.Clear();
                                }

                                #endregion
                                #region ACLR1.1
                                //TODO: TX - Adjacent channel leakage power TM1.1
                                if (testcaseInTesting.Text == "Adjacent channel leakage power TM1.1")
                                {
                                    NRTM = "NRTM_31a";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);

                                    Measurement.ACLRPower5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setPower, setBandwidth, out Result, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
                                    currNRTM = NRTM;
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < Result.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], Result[j]);
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Abs(Math.Round(RS, 1));
                                            if (RS < limitMin)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataACLR[dt.Key - 1, 0, 0] = PassFail;  // 4 Result
                                        dataACLR[dt.Key - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataACLR[dt.Key - 1, 0, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    dict.Clear();
                                }
                                #endregion
                                #region ACLR1.2
                                //TODO: TX - Adjacent channel leakage power TM1.2
                                if (testcaseInTesting.Text == "Adjacent channel leakage power TM1.2")
                                {
                                    NRTM = "NRTM_12";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);

                                    Measurement.ACLRPower5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setPower, setBandwidth, out Result, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < Result.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], Result[j]);
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Abs(Math.Round(RS, 1));
                                            if (RS < limitMin)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, dt.Value, strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataACLR[dt.Key - 1, 1, 0] = PassFail;  // 4 Result
                                        dataACLR[dt.Key - 1, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataACLR[dt.Key - 1, 1, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    dict.Clear();
                                }
                                #endregion
                                #region OBUE-1.1
                                //TODO: TX - Operating band Unwanted emissions TM1.1
                                if (testcaseInTesting.Text == "Operating band Unwanted emissions TM1.1")
                                {
                                    NRTM = "NRTM_31a";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);

                                    Measurement.SEM5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setPower, setBandwidth, out dbRS[1], out dbRS[2], out dbRS[3], out Result);
                                    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    currNRTM = NRTM;
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < Result.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], Result[j]);
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Abs(Math.Round(RS, 1));
                                            if (RS < limitMin)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataSEM[dt.Key - 1, 0, 0] = PassFail;
                                        dataSEM[dt.Key - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataSEM[dt.Key - 1, 0, 2] = RS.ToString();
                                        //dataSEM[dt.Key - 1, 0, 3] = Math.Round(dbRS[2], 1).ToString();
                                        //dataSEM[dt.Key - 1, 0, 4] = Math.Round(dbRS[3], 1).ToString();
                                    }
                                    dict.Clear();


                                }
                                #endregion
                                #region OBUE-1.2
                                //TODO: TX - Operating band Unwanted emissions TM1.2
                                if (testcaseInTesting.Text == "Operating band Unwanted emissions TM1.2")
                                {
                                    NRTM = "NRTM_12";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);

                                    //Measurement.SEM5G(VSA, BBU, NRTM, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out dbRS[1], out dbRS[2], out dbRS[3], out dbRS[0]);
                                    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                    RS = Math.Round(dbRS[0], 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        ////TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
                                        PassFail = pass;
                                        //update TestcaseNode status

                                    }
                                    result = RS.ToString();
                                    //dataSEM[testingPort - 1, 1, 0] = PassFail;
                                    //dataSEM[testingPort - 1, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                    //dataSEM[testingPort - 1, 1, 2] = dbRS[1].ToString();
                                    //dataSEM[testingPort - 1, 1, 3] = dbRS[2].ToString();
                                    //dataSEM[testingPort - 1, 1, 4] = dbRS[3].ToString();
                                }
                                #endregion
                                #region EVM-2.0
                                //TODO: TX - Modulation quality TM2.0 
                                if (testcaseInTesting.Text == "Modulation quality TM2.0")
                                {
                                    NRTM = "NRTM_20";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);

                                    //Measurement.EVMFreqErr5G(VSA, BBU, NRTM, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    bool isSuccess = double.TryParse(evm[2], out RS); //evm[3] giá trị EVM 256QAM
                                    if (isSuccess)
                                    {
                                        RS = Math.Round(RS, 1);
                                        if (RS > limitMax)
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        else
                                        {
                                            PassFail = pass;

                                        }
                                        result = RS.ToString();
                                    }
                                    else
                                    {
                                        result = error;
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    //dataEVM[testingPort - 1, 0, 0] = PassFail;  // 4 Result
                                    //dataEVM[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                    //dataEVM[testingPort - 1, 0, 2] = result;     // 1 Reading Value
                                }
                                #endregion
                                #region EVM-2.0a
                                //TODO: TX - Modulation quality TM2.0a
                                if (testcaseInTesting.Text == "Modulation quality TM2.0a")
                                {
                                    NRTM = "NRTM_20a";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);

                                    Measurement.EVMFreqErr5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setPower, setBandwidth, out evm, out freq);
                                    currNRTM = NRTM;
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < evm.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], evm[j]);
                                                dataFreqErr[mtlport[i] - 1, 3, 2] = freq[j];
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Abs(Math.Round(RS, 1));
                                            if (RS < limitMin)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataEVM[dt.Key - 1, 1, 0] = PassFail;  // 4 Result
                                        dataEVM[dt.Key - 1, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataEVM[dt.Key - 1, 1, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    dict.Clear();
                                    // OFDM[testingPort - 1, 0] = evm[10]; // giá trị OFDM power 2.0a
                                    // dataFreqErr[testingPort - 1, 1, 2] = Result[0];
                                }
                                #endregion
                                #region EVM-3.1
                                //TODO: TX - Modulation quality TM3.1
                                //if (testcaseInTesting.Text == "Modulation quality TM3.1")
                                //{
                                //    mode = "NRTM_31";
                                //    GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                //    if (measurementBackgroundWorker.CancellationPending) return;
                                //    System.Threading.Thread.Sleep(1);
                                //    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //    double.TryParse(strLimMax, out limitMax);
                                //    double.TryParse(strLimMin, out limitMin);

                                //    Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                //    dataFreqErr[testingPort - 1, 2, 2] = Result[0];
                                //    bool isSuccess = double.TryParse(evm[2], out RS); //evm[3] giá trị EVM 64QAM
                                //    if (isSuccess)
                                //    {
                                //        RS = Math.Round(RS, 1);
                                //        if (RS > limitMax)
                                //        {
                                //            PassFail = fail;
                                //            reTest = true;
                                //        }
                                //        else
                                //        {
                                //            PassFail = pass;

                                //        }
                                //        result = RS.ToString();
                                //    }
                                //    else
                                //    {
                                //        result = error;
                                //        PassFail = fail;
                                //        reTest = true;
                                //    }
                                //    dataEVM[testingPort - 1, 2, 0] = PassFail;  // 4 Result
                                //    dataEVM[testingPort - 1, 2, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                //    dataEVM[testingPort - 1, 2, 2] = result;     // 1 Reading Value

                                //}
                                #endregion
                                #region EVM-3.1a
                                //TODO: TX - Modulation quality TM3.1a
                                if (testcaseInTesting.Text == "Modulation quality TM3.1a")
                                {
                                    NRTM = "NRTM_31a";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);
                                    Measurement.EVMFreqErr5G(VSA, BBU, NRTM, attSetVal.ToString(), mtlport, setFrequency, setPower, setBandwidth, out evm, out freq);
                                    currNRTM = NRTM;
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        for (int j = 0; j < evm.Length; j++)
                                        {
                                            if (i == j)
                                            {
                                                dict.Add(mtlport[i], evm[j]);
                                                dataFreqErr[mtlport[i] - 1, 3, 2] = freq[j];
                                            }
                                        }
                                    }
                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Abs(Math.Round(RS, 1));
                                            if (RS < limitMin)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                        dataEVM[dt.Key - 1, 3, 0] = PassFail;  // 4 Result
                                        dataEVM[dt.Key - 1, 3, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataEVM[dt.Key - 1, 3, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    dict.Clear();
                                    //OFDM[testingPort - 1, 1] = evm[10]; // giá trị OFDM power 2.0a
                                    //dataFreqErr[testingPort - 1, 3, 2] = Result[0];
                                }
                                #endregion
                                #region EVM-3.2
                                //TODO: TX - Modulation quality TM3.2
                                //if (testcaseInTesting.Text == "Modulation quality TM3.2")
                                //{
                                //    mode = "NRTM_32";
                                //    GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                //    if (measurementBackgroundWorker.CancellationPending) return;
                                //    System.Threading.Thread.Sleep(1);
                                //    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //    double.TryParse(strLimMax, out limitMax);
                                //    double.TryParse(strLimMin, out limitMin);

                                //    Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //    dataFreqErr[testingPort - 1, 4, 2] = Result[0];
                                //    bool isSuccess = double.TryParse(evm[1], out RS); //giá trị EVM 16QAM
                                //    if (isSuccess)
                                //    {
                                //        RS = Math.Round(RS, 1);
                                //        if (RS > limitMax)
                                //        {
                                //            PassFail = fail;
                                //            reTest = true;
                                //        }
                                //        else
                                //        {
                                //            PassFail = pass;
                                //        }
                                //        result = RS.ToString();
                                //    }
                                //    else
                                //    {
                                //        result = error;
                                //        PassFail = fail;
                                //        reTest = true;
                                //    }
                                //    dataEVM[testingPort - 1, 4, 0] = PassFail;  // 4 Result
                                //    dataEVM[testingPort - 1, 4, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                //    dataEVM[testingPort - 1, 4, 2] = result;     // 1 Reading Value
                                //}
                                #endregion
                                #region EVM-3.3
                                //TODO: TX - Modulation quality TM3.3
                                //if (testcaseInTesting.Text == "Modulation quality TM3.3")
                                //{
                                //    mode = "NRTM_33";
                                //    GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                //    if (measurementBackgroundWorker.CancellationPending) return;
                                //    System.Threading.Thread.Sleep(1);
                                //    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //    double.TryParse(strLimMax, out limitMax);
                                //    double.TryParse(strLimMin, out limitMin);

                                //    Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);


                                //    dataFreqErr[testingPort - 1, 5, 2] = Result[0];
                                //    bool isSuccess = double.TryParse(evm[0], out RS); //giá trị EVM QPSK
                                //    if (isSuccess)
                                //    {
                                //        RS = Math.Round(RS, 1);
                                //        if (RS > limitMax)
                                //        {
                                //            PassFail = fail;
                                //            reTest = true;
                                //        }
                                //        else
                                //        {
                                //            PassFail = pass;
                                //        }
                                //        result = RS.ToString();
                                //    }
                                //    else
                                //    {
                                //        result = error;
                                //        PassFail = fail;
                                //        reTest = true;
                                //    }
                                //    dataEVM[testingPort - 1, 5, 0] = PassFail;  // 4 Result
                                //    dataEVM[testingPort - 1, 5, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                //    dataEVM[testingPort - 1, 5, 2] = result;     // 1 Reading Value

                                //}
                                #endregion
                                #region FRQERR-2.0
                                //TODO: TX - Frequency error TM2.0
                                //if (testcaseInTesting.Text == "Frequency error TM2.0")
                                //{
                                //    mode = "NRTM_20";
                                //    GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                //    if (measurementBackgroundWorker.CancellationPending) return;
                                //    System.Threading.Thread.Sleep(1);
                                //    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //    double.TryParse(strLimMax, out limitMax);
                                //    double.TryParse(strLimMin, out limitMin);

                                //    bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 0, 2], out RS);
                                //    if (isSuccess)
                                //    {
                                //        RS = Math.Abs(Math.Round(RS, 1));
                                //        if (RS > limitMax)
                                //        {
                                //            PassFail = fail;
                                //            reTest = true;
                                //        }
                                //        else
                                //        {
                                //            PassFail = pass;

                                //        }
                                //        result = RS.ToString();
                                //    }
                                //    else
                                //    {
                                //        result = error;
                                //        PassFail = fail;
                                //        reTest = true;
                                //    }
                                //    dataFreqErr[testingPort - 1, 0, 0] = PassFail;  // 4 Result
                                //    dataFreqErr[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                //    dataFreqErr[testingPort - 1, 0, 2] = result;     // 1 Reading Value

                                //}
                                #endregion
                                #region FRQERR-3.1a
                                //TODO: TX - Frequency error TM3.1a
                                if (testcaseInTesting.Text == "Frequency error TM3.1a")
                                {
                                    NRTM = "NRTM_31a";
                                    //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    string Spec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    double.TryParse(strLimMax, out limitMax);
                                    double.TryParse(strLimMin, out limitMin);
                                    dict = new Dictionary<int, string>();
                                    //Dictionary luu so port va mang ket qua tra ve
                                    for (int i = 0; i < mtlport.Length; i++)
                                    {
                                        dict.Add(mtlport[i], dataFreqErr[mtlport[i] - 1, 3 ,2]);
                                    }

                                    foreach (var dt in dict)
                                    {
                                        if (double.TryParse(dt.Value, out RS))
                                        {
                                            RS = Math.Abs(Math.Round(RS, 1));
                                            if (RS < limitMin)
                                            {
                                                PassFail = fail;
                                                reTest = true;
                                            }
                                            else
                                            {
                                                PassFail = pass;
                                            }
                                        }
                                        else
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        ListViewShow("PORT" + dt.Key, testcaseInTesting.Text, Spec, strLimMin, RS.ToString(), strLimMax, NRTM, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());

                                        dataFreqErr[dt.Key - 1, 1, 0] = PassFail;  // 4 Result
                                        dataFreqErr[dt.Key - 1, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataFreqErr[dt.Key - 1, 1, 2] = dt.Value;     // 1 Reading Value
                                    }
                                    dict.Clear();
                                }
                                #endregion


                                //TODO: when fail =================
                                if (reTest)
                                {
                                    timeout++;
                                    if (timeout == loopTime) //Fail after n times
                                    {
                                        //counting completed task
                                        isTestcasePass = false;
                                        completedTask++;
                                        timeout = 0;
                                        //failedPorts.Add(testingPort);
                                        if (testCase == "Downlink")
                                        {
                                            //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency, PassFail);
                                        }
                                        break;
                                    }
                                }
                                else
                                {
                                    completedTask++;
                                    timeout = 0;
                                    if (testCase == "Downlink" && !testcaseInTesting.Text.Contains("Frequency error")) //TODO: tam thoi ngat phan Frequency error 
                                    {
                                        //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency, PassFail);
                                    }
                                    break;
                                }
                            }
                            //TODO: After a port measurement finish.
                            UpdateProgressBar(completedTask);
                            //add to Result listview
                            string strSpec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                            string strMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                            string strMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                            //if (testcaseInTesting.Text != "RX Power Calibration")
                            //{
                            //    foreach(int testingPort in mtlport)
                            //    {
                            //        ListViewShow("PORT" + testingPort, testcaseInTesting.Text, strSpec, strMin, Result[testingPort], strMax, mode, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                            //    }                
                            //}

                        }

                        //TODO: After Testcase measurement finish, If there are any failed ports, add them to the dictionary
                        if (failedPorts.Count > 0)
                        {
                            failedTestList[testcaseInTesting.Text] = failedPorts;
                            ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Failed);
                        }
                        else
                        {
                            ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Passed);
                        }
                        //refreshing TestcasePending List for 
                        TestcasePending = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Pending);
                        testcaseInTesting = TestcasePending.Count > 0 ? TestcasePending[0] : null;
                    }
                }
            }

            //TODO: After all testcases measurement fisnish, print table fail port lists
            if (failedTestList.Count > 0)
            {
                Log("Failed Measurement List:");
                List<string> headers = new List<string> { "Specification Name", "Failed Port List" };
                log.PrintDictionary(headers, failedTestList);
            }

            clearFlag = true;
        }
        #endregion

        #region  Code cu
        //++++++++++++++++++++++++++++++++++++++++++++++++
        //TODO: Background worker CU ===========
        //for (int i = 0; i < numberMeasClause; i++) //    measClause.GetLength(0)    
        //{
        //    if (measurementBackgroundWorker.CancellationPending) return;
        //    System.Threading.Thread.Sleep(1);
        //    if (measClause[i, 0] != null)
        //    {
        //        #region LoadBlock
        //        //if (measClause[i, 5] == "1")
        //        //{
        //        //    if (loadBlock != "Chapter6_1")
        //        //    {
        //        //        Chapter6_1 chapter6_1Form = new Chapter6_1();
        //        //        chapter6_1Form.ShowDialog();
        //        //        loadBlock = "Chapter6_1";

        //        //    }

        //        //}
        //        //else if (measClause[i, 5] == "2")
        //        //{
        //        //    if (loadBlock != "Chapter6_2")
        //        //    {
        //        //        Chapter6_3 chapter6_2Form = new Chapter6_3();
        //        //        chapter6_2Form.ShowDialog();
        //        //        loadBlock = "Chapter6_2";
        //        //    }
        //        //}
        //        //else if (measClause[i, 5] == "3")
        //        //{
        //        //    if (loadBlock != "Chapter6_3")
        //        //    {
        //        //        Chapter6_3 chapter6_3Form = new Chapter6_3();
        //        //        chapter6_3Form.ShowDialog();
        //        //        loadBlock = "Chapter6_3";
        //        //    }
        //        //}
        //        //else if (measClause[i, 5] == "4")
        //        //{
        //        //    if (loadBlock != "Chapter6_4")
        //        //    {
        //        //        Chapter6_4 chapter6_4Form = new Chapter6_4();
        //        //        chapter6_4Form.ShowDialog();
        //        //        loadBlock = "Chapter6_4";
        //        //    }
        //        //}
        //        //else if (measClause[i, 5] == "5")
        //        //{
        //        //    if (loadBlock != "Chapter6_5")
        //        //    {
        //        //        Chapter6_5 chapter6_5Form = new Chapter6_5();
        //        //        chapter6_5Form.ShowDialog();
        //        //        loadBlock = "Chapter6_5";
        //        //    }
        //        //}
        //        #endregion
        //        if (testingMode == "TX")
        //        {
        //            //set cong xuat dau ra cho thiet bi
        //            BBU.CHG_RRU_POWER(int.Parse(setPower));
        //        }
        //        if (testingMode == "RX")
        //        {
        //            //chuyen RRU ve mode RX_Only
        //            BBU.TDDMODE("RX_ONLY");
        //        }
        //for (int j = 0; j < setPort.Length; j++)
        //{
        //    while (timeout < loopTime)
        //    {
        //        bool reTest = false;// Switch den port Nx
        //        if (setPort[j]) // checking Port J is checked or not , if Port J is checked the return value is true
        //        {
        //            //when switch a new port, check if background worker is canceled.
        //            if (measurementBackgroundWorker.CancellationPending) return;
        //            System.Threading.Thread.Sleep(1);

        //            if (measClause[i, 0] == "+ TX Calibration")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                string gainDefault = measClause[i, 5];
        //                string powerExpected = txtPower.Text;
        //                limitMax = double.Parse(powerExpected) + 0.5;
        //                limitMin = double.Parse(powerExpected) - 0.5;
        //                Measurement.calibTX(VSA, BBU, RRU, "1", (j + 1).ToString(), mode, arrAttDL[j], setFrequency, gainDefault, powerExpected, out Result[0], out Result[1], out Result[2]);
        //                bool isSuccess = double.TryParse(Result[0], out RS);
        //                string gain;
        //                BBU.SHW_TX_GAIN((j + 1).ToString(), out gain);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if ((RS < limitMin || RS > limitMax))
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                    BBU.PREPARE_CALIB_RRU((j + 1).ToString(), gainDefault);
        //                }
        //                BBU.CHG_RRU_POWER(1000);
        //                Log("TRX" + (j + 1) + " Power : vsa reading " + RS + "dBm"); //+ " rru reading " + antPower + "dBm"
        //                dataCalibTx[j, 0] = measClause[i, 2]; // 0 Min limit, 1 Reading Value, 2 Max limit, 3 Testing Time, 4 Result. 
        //                dataCalibTx[j, 1] = result;     // 1 Reading Value
        //                dataCalibTx[j, 2] = measClause[i, 3]; // 2 Max limit
        //                dataCalibTx[j, 3] = DateTime.Now.ToString("hh:mm:ss");
        //                dataCalibTx[j, 4] = PassFail;
        //            }
        //            #region Downlink 
        //            #region Pout
        //            if (measClause[i, 0] == "+ Base station output power")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3].ToString());
        //                limitMin = double.Parse(measClause[i, 2].ToString());

        //                Measurement.Output5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out Result[0]); //arrAttDL duoc update tu ham doc csv
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //                bool isSuccess = double.TryParse(Result[0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS < limitMin || RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataPout[j, 0] = PassFail;  // 4 Result
        //                dataPout[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataPout[j, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion
        //            #region OBW
        //            else if (measClause[i, 0] == "+ Occupied bandwidth")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                limitMin = 0;

        //                Measurement.OBW5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setBandwidth, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //                double tmp;
        //                bool isSuccess = double.TryParse(Result[0], out tmp);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(tmp / 1000000);
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {

        //                        PassFail = pass;

        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataOBW[j, 0] = PassFail;  // 4 Result
        //                dataOBW[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataOBW[j, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion
        //            #region ACLR
        //            else if (measClause[i, 0] == "+ Adjacent channel leakage power TM1.1")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                Measurement.ACLRPower5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out Result[1], out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);

        //                if (double.TryParse(aclrAdjLower, out double tmp))
        //                {
        //                    double.TryParse(aclrAdjLower, out double tmp1);
        //                    double.TryParse(aclrAdjUpper, out double tmp2);
        //                    double.TryParse(aclrAlt1Lower, out double tmp3);
        //                    double.TryParse(aclrAlt1Upper, out double tmp4);
        //                    double tmp5 = Math.Max(tmp1, tmp2);
        //                    double tmp6 = Math.Max(tmp3, tmp4);
        //                    RS = Math.Abs(Math.Max(tmp5, tmp6));
        //                    RS = Math.Round(RS, 1);
        //                    if (RS < limitMin)
        //                    {
        //                        //BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                        PassFail = fail;
        //                        reTest = true;
        //                        if (timeout == loopTime - 1)
        //                        {
        //                            //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataACLR[j, 0, 0] = PassFail;  // 4 Result
        //                dataACLR[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataACLR[j, 0, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Adjacent channel leakage power TM1.2")
        //            {
        //                mode = "NRTM_12";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                Measurement.ACLRPower5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out Result[1], out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
        //                ;
        //                if (double.TryParse(aclrAdjLower, out double tmp))
        //                {
        //                    double.TryParse(aclrAdjLower, out double tmp1);
        //                    double.TryParse(aclrAdjUpper, out double tmp2);
        //                    double.TryParse(aclrAlt1Lower, out double tmp3);
        //                    double.TryParse(aclrAlt1Upper, out double tmp4);
        //                    double tmp5 = Math.Max(tmp1, tmp2);
        //                    double tmp6 = Math.Max(tmp3, tmp4);
        //                    RS = Math.Abs(Math.Max(tmp5, tmp6));
        //                    RS = Math.Round(RS, 1);
        //                    if (RS < limitMin)
        //                    {
        //                        //BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                        PassFail = fail;
        //                        reTest = true;
        //                        if (timeout == loopTime - 1)
        //                        {
        //                            //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataACLR[j, 1, 0] = PassFail;  // 4 Result
        //                dataACLR[j, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataACLR[j, 1, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion
        //            #region OBUE
        //            else if (measClause[i, 0] == "+ Operating band Unwanted emissions TM1.1")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                Measurement.SEM5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out dbRS[1], out dbRS[2], out dbRS[3], out dbRS[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                ////TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1));
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                RS = Math.Round(dbRS[0], 1);
        //                if (RS > limitMax)
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                else
        //                {
        //                    //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                    PassFail = pass;
        //                }
        //                result = RS.ToString();
        //                dataSEM[j, 0, 0] = PassFail;
        //                dataSEM[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataSEM[j, 0, 2] = Math.Round(dbRS[1], 1).ToString();
        //                dataSEM[j, 0, 3] = Math.Round(dbRS[2], 1).ToString();
        //                dataSEM[j, 0, 4] = Math.Round(dbRS[3], 1).ToString();
        //            }
        //            else if (measClause[i, 0] == "+ Operating band Unwanted emissions TM1.2")
        //            {
        //                mode = "NRTM_12";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);

        //                Measurement.SEM5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out dbRS[1], out dbRS[2], out dbRS[3], out dbRS[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                ////TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1));
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                RS = Math.Round(dbRS[0], 1);
        //                if (RS > limitMax)
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                else
        //                {
        //                    //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                    PassFail = pass;
        //                }
        //                result = RS.ToString();
        //                dataSEM[j, 1, 0] = PassFail;
        //                dataSEM[j, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataSEM[j, 1, 2] = dbRS[1].ToString();
        //                dataSEM[j, 1, 3] = dbRS[2].ToString();
        //                dataSEM[j, 1, 4] = dbRS[3].ToString();
        //            }
        //            #endregion
        //            #region EVM
        //            else if (measClause[i, 0] == "+ Modulation quality TM2.0")
        //            {
        //                mode = "NRTM_20";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                Measurement.EVMFreqErr5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out evm, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                dataFreqErr[j, 0, 0] = Result[0];
        //                bool isSuccess = double.TryParse(evm[2], out RS); //evm[3] giá trị EVM 256QAM
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataEVM[j, 0, 0] = PassFail;  // 4 Result
        //                dataEVM[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataEVM[j, 0, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Frequency error TM2.0")
        //            {
        //                mode = "NRTM_20";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(dataFreqErr[j, 0, 0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(Math.Round(RS, 1));
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataFreqErr[j, 0, 0] = PassFail;  // 4 Result
        //                dataFreqErr[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataFreqErr[j, 0, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Modulation quality TM2.0a")
        //            {
        //                mode = "NRTM_20a";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                Measurement.EVMFreqErr5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out evm, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                OFDM[j, 0] = evm[10]; // giá trị OFDM power 2.0a
        //                dataFreqErr[j, 1, 0] = Result[0];
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(evm[3], out RS); //evm[3] giá trị EVM 256QAM
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataEVM[j, 1, 0] = PassFail;  // 4 Result
        //                dataEVM[j, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataEVM[j, 1, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Frequency error TM2.0a")
        //            {
        //                mode = "NRTM_20a";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(dataFreqErr[j, 1, 0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(Math.Round(RS, 1));
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataFreqErr[j, 1, 0] = PassFail;  // 4 Result
        //                dataFreqErr[j, 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataFreqErr[j, 1, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Modulation quality TM3.1")
        //            {
        //                mode = "NRTM_31";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                Measurement.EVMFreqErr5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out evm, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                dataFreqErr[j, 2, 0] = Result[0];
        //                bool isSuccess = double.TryParse(evm[2], out RS); //evm[3] giá trị EVM 64QAM
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataEVM[j, 2, 0] = PassFail;  // 4 Result
        //                dataEVM[j, 2, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataEVM[j, 2, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Frequency error TM3.1")
        //            {
        //                mode = "NRTM_31";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(dataFreqErr[j, 3, 0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(Math.Round(RS, 1));
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataFreqErr[j, 2, 0] = PassFail;  // 4 Result
        //                dataFreqErr[j, 2, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataFreqErr[j, 2, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Modulation quality TM3.1a")
        //            {
        //                mode = "NRTM_31a";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                Measurement.EVMFreqErr5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out evm, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                OFDM[j, 1] = evm[10]; // giá trị OFDM power 2.0a
        //                dataFreqErr[j, 3, 0] = Result[0];
        //                bool isSuccess = double.TryParse(evm[3], out RS); //evm[3] giá trị EVM 256QAM
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataEVM[j, 3, 0] = PassFail;  // 4 Result
        //                dataEVM[j, 3, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataEVM[j, 3, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Frequency error TM3.1a")
        //            {
        //                mode = "NRTM_31a";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(dataFreqErr[j, 3, 0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(Math.Round(RS, 1));
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataFreqErr[j, 3, 0] = PassFail;  // 4 Result
        //                dataFreqErr[j, 3, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataFreqErr[j, 3, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Modulation quality TM3.2")
        //            {
        //                mode = "NRTM_32";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                Measurement.EVMFreqErr5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out evm, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                dataFreqErr[j, 4, 0] = Result[0];
        //                bool isSuccess = double.TryParse(evm[1], out RS); //giá trị EVM 16QAM
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataEVM[j, 4, 0] = PassFail;  // 4 Result
        //                dataEVM[j, 4, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataEVM[j, 4, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Frequency error TM3.2")
        //            {
        //                mode = "NRTM_32";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(dataFreqErr[j, 4, 0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(Math.Round(RS, 1));
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataFreqErr[j, 4, 0] = PassFail;  // 4 Result
        //                dataFreqErr[j, 4, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataFreqErr[j, 4, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Modulation quality TM3.3")
        //            {
        //                mode = "NRTM_33";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                Measurement.EVMFreqErr5G(VSA, BBU, mode, arrAttDL[j], (j + 1), setFrequency, setPower, setBandwidth, out evm, out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                dataFreqErr[j, 5, 0] = Result[0];
        //                bool isSuccess = double.TryParse(evm[0], out RS); //giá trị EVM QPSK
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataEVM[j, 5, 0] = PassFail;  // 4 Result
        //                dataEVM[j, 5, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataEVM[j, 5, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Frequency error TM3.3")
        //            {
        //                mode = "NRTM_33";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                bool isSuccess = double.TryParse(dataFreqErr[j, 5, 0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Abs(Math.Round(RS, 1));
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataFreqErr[j, 5, 0] = PassFail;  // 4 Result
        //                dataFreqErr[j, 5, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataFreqErr[j, 5, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion
        //            #region TDR
        //            else if (measClause[i, 0] == "+ Total power dynamic range")
        //            {
        //                mode = "NRTM_31";
        //                //code hien tai bai do nay chua chay doc lap duoc 
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMin = double.Parse(measClause[i, 2]);
        //                RS = Math.Abs(double.Parse(OFDM[j, 1]) - double.Parse(OFDM[j, 0]));
        //                RS = Math.Round(RS, 1);
        //                if (RS >= limitMin)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = RS.ToString();
        //                dataTotalDR[j, 0] = PassFail;  // 4 Result
        //                dataTotalDR[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataTotalDR[j, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion TDR
        //            #region SPUR
        //            else if (measClause[i, 0] == "+ Transmitter spurious emissions")
        //            {
        //                NRTM = "NRTM_11";
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                lblStatusProgress_TextChanged(measClause[i, 0] + " :Port " + (j + 1));
        //                PassFail = pass;

        //                while (PassFail == pass) // Neu 1 chi tieu FAIL thi thoat vong lap ko do chi tieu khac nua
        //                {
        //                    for (int i1 = 0; i1 < numbersubmeasClause; i1++)
        //                    {
        //                        int count = 0;
        //                        while (count < 3)
        //                        {
        //                            if (measurementBackgroundWorker.CancellationPending) return;
        //                            System.Threading.Thread.Sleep(1);
        //                            if (submeasClause[i1, 0] != null)
        //                            {
        //                                MessageBox.Show("Đổi sang Port" + (j + 1));
        //                                if (measurementBackgroundWorker.CancellationPending) return;
        //                                System.Threading.Thread.Sleep(1);

        //                                if (submeasClause[i1, 0] == "++ General")
        //                                {
        //                                    limitMax = double.Parse(submeasClause[i1, 3]);
        //                                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
        //                                    // Spurious General 
        //                                    Measurement.Spur5G(VSA, BBU, "General", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
        //                                    //==============19.2.2024 save screen shot =====================
        //                                    SaveCSVdata(VSA, "General", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    //TakeScreenshotVSA(VSA, "General", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    BBU.CHG_RRU_POWER(1000);
        //                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                                    for (int i2 = 0; i2 < tseRS.Length; i2++)
        //                                    {
        //                                        if (tseRS[i2] != null)
        //                                        {
        //                                            TSE[0, i2] = tseRS[i2];
        //                                            if (TSE[0, i2] > 0)
        //                                            {
        //                                                reTest = true;
        //                                            }
        //                                            else { reTest = false; }
        //                                        }
        //                                    }
        //                                }
        //                                else if (submeasClause[i1, 0] == "++ Protection")
        //                                {
        //                                    limitMax = double.Parse(submeasClause[i1, 3]);
        //                                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
        //                                    // Spurious Protection
        //                                    Measurement.Spur5G(VSA, BBU, "Protection", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
        //                                    //==============19.2.2024 save screen shot =====================
        //                                    SaveCSVdata(VSA, "Protection", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    //TakeScreenshotVSA(VSA, "Protection", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    BBU.CHG_RRU_POWER(1000);
        //                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                                    for (int i2 = 0; i2 < tseRS.Length; i2++)
        //                                    {
        //                                        if (tseRS[i2] != null)
        //                                        {
        //                                            TSE[1, i2] = tseRS[i2];
        //                                            if (TSE[1, i2] > 0)
        //                                            {
        //                                                reTest = true;
        //                                            }
        //                                            else { reTest = false; }
        //                                        }
        //                                    }
        //                                }
        //                                else if (submeasClause[i1, 0] == "++ Co existence")
        //                                {
        //                                    limitMax = double.Parse(submeasClause[i1, 3]);
        //                                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
        //                                    // Spurious Co-ex BW100K
        //                                    Measurement.Spur5G(VSA, BBU, "Co-existance", NRTM, arrAttDL[j], j + 1, setFrequency, "100kHz", out tseRS);
        //                                    //==============19.2.2024 save screen shot =====================
        //                                    SaveCSVdata(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    //TakeScreenshotVSA(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                                    for (int i2 = 0; i2 < tseRS.Length; i2++)
        //                                    {
        //                                        if (tseRS[i2] != null)
        //                                        {
        //                                            TSE[2, i2] = tseRS[i2];
        //                                            if (TSE[2, i2] > 0)
        //                                            {
        //                                                reTest = true;
        //                                            }
        //                                            else { reTest = false; }
        //                                        }
        //                                    }
        //                                    // Spurious Co-ex BW1M
        //                                    Measurement.Spur5G(VSA, BBU, "Co-existance", NRTM, arrAttDL[j], j + 1, setFrequency, "1MHz", out tseRS);
        //                                    //==============19.2.2024 save screen shot =====================
        //                                    SaveCSVdata(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    //TakeScreenshotVSA(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    BBU.CHG_RRU_POWER(1000);
        //                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                                    for (int i2 = 0; i2 < tseRS.Length; i2++)
        //                                    {
        //                                        if (tseRS[i2] != null)
        //                                        {
        //                                            TSE[3, i2] = tseRS[i2];
        //                                            if (TSE[3, i2] > 0)
        //                                            {
        //                                                reTest = true;
        //                                            }
        //                                            else { reTest = false; }
        //                                        }
        //                                    }
        //                                }
        //                                else if (submeasClause[i1, 0] == "++ Co location")
        //                                {
        //                                    limitMax = double.Parse(submeasClause[i1, 3]);
        //                                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
        //                                    // Spurious Co-lo 
        //                                    Measurement.Spur5G(VSA, BBU, "Co-location", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
        //                                    //==============19.2.2024 save screen shot =====================
        //                                    SaveCSVdata(VSA, "Co-location", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    //TakeScreenshotVSA(VSA, "Co-location", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                                    BBU.CHG_RRU_POWER(1000);
        //                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                                    for (int i2 = 0; i2 < tseRS.Length; i2++)
        //                                    {
        //                                        if (tseRS[i2] != null)
        //                                        {
        //                                            TSE[4, i2] = tseRS[i2];
        //                                            if (TSE[4, i2] > 0)
        //                                            {
        //                                                reTest = true;
        //                                            }
        //                                            else { reTest = false; }
        //                                        }
        //                                    }
        //                                }
        //                                // Neu reTest fail
        //                                if (reTest)
        //                                {
        //                                    count++;
        //                                }
        //                                else
        //                                { count = 3; break; }
        //                            }
        //                        }
        //                        //tseRS[] : Mang ket qua tra ve cua moi lan do. tseRS[i] co the la null
        //                        // TSE[,] : Mang ket qua cua moi lan do 
        //                        // 
        //                        nRS = -100;
        //                        for (int i11 = 0; i11 < tseRS.Length; i11++)
        //                        {
        //                            if ((tseRS[i11] != null))
        //                            {
        //                                for (int i2 = 0; i2 < tseRS.Length; i2++)
        //                                {
        //                                    if ((TSE[i11, i2] != null))
        //                                    {
        //                                        if (TSE[i11, i2] > nRS)
        //                                        {
        //                                            nRS = TSE[i11, i2];
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        } // Tim max tseRS[]
        //                        if (nRS > 0) { PassFail = fail; }
        //                        else { PassFail = pass; }
        //                        result = Math.Round((double)nRS, 1).ToString();
        //                    }
        //                    break; // 
        //                }
        //                dataTXSUPR[j, 0] = PassFail;  // Result
        //                dataTXSUPR[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // Testing Time
        //                Log("Ket qua cuoi cung  " + result);
        //            }
        //            #endregion SPUR
        //            #region ON/OFF Power
        //            else if (measClause[i, 0] == "+ Transmitter OFF Power")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathOnOff, measClause[i, 0], j, mode);

        //                RFSwitchRRUPort("TX", j);
        //                //RRU properties
        //                RRU.Testcase = TestCase.TX;
        //                RRU.CurrentTestingPort = j + 1;
        //                RRU.CurrentTestingSpec = measClause[i, 0].Replace("+ ", "");
        //                RRU.ReadATTfromCSVdir(rruCSVFilePathOnOff, "TX", double.Parse(setFrequency));
        //                MessageBox.Show("Đổi sang port " + (j + 1), "Thông báo");
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                lblStatusProgress_TextChanged(measClause[i, 0] + " :Port " + (j + 1));
        //                Log(measClause[i, 0] + " :Port " + (j + 1));
        //                Log("Test time: " + (timeout + 1));
        //                limitMax = double.Parse(measClause[i, 3]);
        //                Measurement.OnOffPower5G(VSA, BBU, mode, j + 1, arrAttDL[j], setFrequency, out Result[0], out Result[1]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);

        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                Log("On/Off Power port " + (j + 1) + ":" + Result[0] + " dBm/MHz");
        //                Log("On/Off Transient port " + (j + 1) + ":" + Result[1] + " us");
        //                Transient[j, 1] = Result[1];
        //                bool isSuccess = double.TryParse(Result[0], out RS);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                BBU.CHG_RRU_POWER(1000);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataOnOffPower[j, 0] = PassFail;  // 4 Result
        //                dataOnOffPower[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataOnOffPower[j, 2] = result;     // 1 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Transmitter transient period")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathOnOff, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                RS = double.Parse(Transient[j, 1]);
        //                if (RS <= limitMax)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = RS.ToString();
        //                dataTransient[j, 0] = PassFail;  // 4 Result
        //                dataTransient[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataTransient[j, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion
        //            #region TAE
        //            else if (measClause[i, 0] == "+ Time alignment error")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Log("Test time: " + (timeout + 1));
        //                limitMin = double.Parse(measClause[i, 2]);
        //                limitMax = double.Parse(measClause[i, 3]);

        //                string taeref = taetmp;
        //                Measurement.TAEMeasurement(VSA, BBU, mode, (j + 1), arrAttDL[j], setFrequency, ref taeref, ref Result);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                taetmp = taeref;
        //                bool isSuccess = double.TryParse(Result[0], out RS);
        //                if (isSuccess)
        //                {
        //                    RS = Math.Round(RS, 1);
        //                    if (limitMin > RS || RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                }
        //                else
        //                {
        //                    result = error;
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                dataTAE[j, 0] = PassFail;  // 4 Result
        //                dataTAE[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataTAE[j, 2] = result;     // 1 Reading Value
        //            }
        //            #endregion
        //            #region IMD ACLR
        //            else if (measClause[i, 0] == "+ Transmitter intermodulation (ACLR)")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathIMDTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                for (int i1 = 0; i1 < 6; i1++)
        //                {
        //                    if (double.Parse(measClause[i, 6]) < double.Parse(measClause[i, i1 + 7]))
        //                    {
        //                        MessageBox.Show($"Công suất {measClause[i, i1 + 7]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                        BBU.StopProcess();
        //                        clearFlag = true;
        //                        if (measurementBackgroundWorker.IsBusy == true)
        //                        {
        //                            measurementBackgroundWorker.CancelAsync();
        //                        }
        //                        timerProcess.Stop();
        //                        btnConnect_Enable();
        //                        lblStatusProgress_TextChanged("Đã dừng");
        //                    }
        //                }
        //                Measurement.IMDACLR5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setBandwidth, "-25", measClause[i, 7], out RS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -25", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdACLR[0] = RS;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDACLR5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setBandwidth, "25", measClause[i, 12], out RS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +25", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdACLR[1] = RS;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDACLR5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setBandwidth, "-15", measClause[i, 8], out RS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -15", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdACLR[2] = RS;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDACLR5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setBandwidth, "15", measClause[i, 11], out RS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +15", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdACLR[3] = RS;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDACLR5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setBandwidth, "-5", measClause[i, 9], out RS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -5", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdACLR[4] = RS;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDACLR5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setBandwidth, "5", measClause[i, 10], out RS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +5", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdACLR[5] = RS;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                BBU.CHG_RRU_POWER(1000);

        //                RS = 100;
        //                for (int x = 0; x < 6; x++)
        //                {
        //                    if (imdACLR[x] < RS)
        //                    {
        //                        RS = imdACLR[x];
        //                    }
        //                }
        //                Console.WriteLine("Kết quả cuối cùng  " + RS);
        //                if (RS < limitMin)
        //                {
        //                    BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                else
        //                {
        //                    PassFail = pass;
        //                }
        //                result = Math.Round(RS, 1).ToString();
        //            }
        //            #endregion
        //            #region IMD SEM
        //            else if (measClause[i, 0] == "+ Transmitter intermodulation (SEM)")
        //            {
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathIMDTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                for (int i1 = 0; i1 < 6; i1++)
        //                {
        //                    if (double.Parse(measClause[i, 6]) < double.Parse(measClause[i, i1 + 7]))
        //                    {
        //                        MessageBox.Show($"Công suất {measClause[i, i1 + 7]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                        BBU.StopProcess();
        //                        clearFlag = true;
        //                        if (measurementBackgroundWorker.IsBusy == true)
        //                        {
        //                            measurementBackgroundWorker.CancelAsync();
        //                        }
        //                        timerProcess.Stop();
        //                        btnConnect_Enable();
        //                        lblStatusProgress_TextChanged("Đã dừng");
        //                    }
        //                }
        //                Measurement.IMDSEM5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setPower, setBandwidth, "-25", measClause[i, 7], out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -25", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdSEM[0] = double.Parse(Result[0]);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDSEM5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setPower, setBandwidth, "25", measClause[i, 12], out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +25", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdSEM[1] = double.Parse(Result[0]);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDSEM5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setPower, setBandwidth, "-15", measClause[i, 8], out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -15", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdSEM[2] = double.Parse(Result[0]);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDSEM5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setPower, setBandwidth, "15", measClause[i, 11], out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +15", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdSEM[3] = double.Parse(Result[0]);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDSEM5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setPower, setBandwidth, "-5", measClause[i, 9], out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -5", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdSEM[4] = double.Parse(Result[0]);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.IMDSEM5G(VSA, VSG1, BBU, (j + 1), mode, arrAttDL[j], setFrequency, setPower, setBandwidth, "5", measClause[i, 10], out Result[0]);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +5", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                imdSEM[5] = double.Parse(Result[0]);
        //                BBU.CHG_RRU_POWER(1000);
        //                RS = -100;
        //                for (int x = 0; x < 6; x++)
        //                {
        //                    if (imdSEM[x] > RS)
        //                    {
        //                        RS = imdSEM[x];
        //                    }
        //                }
        //                Console.WriteLine("Kết quả cuối cùng  " + RS);

        //                if (RS <= limitMax)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = Math.Round(RS, 1).ToString();
        //            }
        //            #endregion
        //            #region IMD SPUR
        //            else if (measClause[i, 0] == "+ Transmitter intermodulation (SPUR)")
        //            {
        //                mode = "NRTM_11";
        //                //GeneralDataSetupTX(rruCSVFilePathIMDTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                lblStatusProgress_TextChanged(measClause[i, 0] + " :Port " + (j + 1));
        //                MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                for (int i1 = 0; i1 < 6; i1++)
        //                {
        //                    if (double.Parse(measClause[i, 6]) < double.Parse(measClause[i, i1 + 7]))
        //                    {
        //                        MessageBox.Show($"Công suất {measClause[i, i1 + 7]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                        BBU.StopProcess();
        //                        clearFlag = true;
        //                        if (measurementBackgroundWorker.IsBusy == true)
        //                        {
        //                            measurementBackgroundWorker.CancelAsync();
        //                        }
        //                        timerProcess.Stop();
        //                        btnConnect_Enable();
        //                        lblStatusProgress_TextChanged("Đã dừng");
        //                    }
        //                }
        //                Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "-25", measClause[i, 7], out tseRS);
        //                //==============19.2.2024 save screen shot =====================
        //                SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -25", setFrequency);
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -25", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (tseRS[i1] != null)
        //                    {
        //                        TSE[0, i1] = tseRS[i1];
        //                    }
        //                }
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "25", measClause[i, 12], out tseRS);
        //                //==============19.2.2024 save screen shot =====================

        //                SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +25", setFrequency);
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +25", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (tseRS[i1] != null)
        //                    {
        //                        TSE[0, i1] = tseRS[i1];
        //                    }
        //                }
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "-15", measClause[i, 8], out tseRS);
        //                //==============19.2.2024 save screen shot =====================

        //                SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -15", setFrequency);
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -15", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (tseRS[i1] != null)
        //                    {
        //                        TSE[2, i1] = tseRS[i1];
        //                    }
        //                }
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "15", measClause[i, 11], out tseRS);
        //                //==============19.2.2024 save screen shot =====================

        //                SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +15", setFrequency);
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +15", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (tseRS[i1] != null)
        //                    {
        //                        TSE[3, i1] = tseRS[i1];
        //                    }
        //                }
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "-5", measClause[i, 9], out tseRS);
        //                //==============19.2.2024 save screen shot =====================

        //                SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -5", setFrequency);
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -5", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (tseRS[i1] != null)
        //                    {
        //                        TSE[4, i1] = tseRS[i1];
        //                    }
        //                }
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "5", measClause[i, 10], out tseRS);
        //                //==============19.2.2024 save screen shot =====================
        //                SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +5", setFrequency);
        //                //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +5", setFrequency);
        //                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (tseRS[i1] != null)
        //                    {
        //                        TSE[5, i1] = tseRS[i1];
        //                    }
        //                }
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                BBU.CHG_RRU_POWER(1000);
        //                nRS = -100;
        //                for (int x = 0; x < TSE.Length; x++)
        //                {
        //                    for (int y = 0; y < TSE.Length; y++)
        //                    {
        //                        if (TSE[x, y] > RS)
        //                        {
        //                            nRS = TSE[x, y];
        //                        }
        //                    }
        //                }
        //                if (nRS > limitMax)
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                else
        //                {
        //                    PassFail = pass;
        //                }
        //                result = Math.Round((double)nRS, 1).ToString();
        //                Console.WriteLine("Ket qua cuôi cùng  " + result);
        //            }
        //            #endregion
        //            #region Intra ACLR
        //            else if (measClause[i, 0] == "+ Intra-system ACLR")
        //            {
        //                double limitInterfer, limitInterferSetup;
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathIMDTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                BBU.TDDMODE("TX_ONLY");
        //                MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                double.TryParse(measClause[i, 6], out limitInterfer);
        //                double.TryParse(measClause[i, 13], out limitInterferSetup);
        //                if (limitInterfer < limitInterferSetup)
        //                {
        //                    MessageBox.Show($"Công suất {measClause[i, 12]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    BBU.StopProcess();
        //                    clearFlag = true;
        //                    if (measurementBackgroundWorker.IsBusy == true)
        //                    {
        //                        measurementBackgroundWorker.CancelAsync();
        //                    }
        //                    timerProcess.Stop();
        //                    btnConnect_Enable();
        //                    lblStatusProgress_TextChanged("Đã dừng");
        //                }
        //                else
        //                {
        //                    BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                    Measurement.IntraACLR5G(VSA, VSG1, BBU, mode, arrAttDL[j], port, setFrequency, setPower, setBandwidth, measClause[i, 13], ref RS);
        //                    //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                    if (measurementBackgroundWorker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(1);
        //                    BBU.CHG_RRU_POWER(1000);
        //                    RS = Math.Abs(RS);
        //                    if (RS < limitMin)
        //                    {
        //                        BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = Math.Round(RS, 1).ToString();
        //                }

        //            }
        //            #endregion
        //            #region Intra SEM
        //            else if (measClause[i, 0] == "+ Intra-system SEM")
        //            {
        //                double limitInterfer, limitInterferSetup;
        //                mode = "NRTM_11";
        //                GeneralDataSetupTX(rruCSVFilePathIMDTX, measClause[i, 0], j, mode);
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                BBU.TDDMODE("TX_ONLY");
        //                MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 6], out limitInterfer);
        //                double.TryParse(measClause[i, 13], out limitInterferSetup);
        //                if (limitInterfer < limitInterferSetup)
        //                {
        //                    MessageBox.Show($"Công suất {measClause[i, 12]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    BBU.StopProcess();
        //                    clearFlag = true;
        //                    if (measurementBackgroundWorker.IsBusy == true)
        //                    {
        //                        measurementBackgroundWorker.CancelAsync();
        //                    }
        //                    timerProcess.Stop();
        //                    btnConnect_Enable();
        //                    lblStatusProgress_TextChanged("Đã dừng");
        //                }
        //                else
        //                {
        //                    BBU.CHG_RRU_POWER(int.Parse(setPower));
        //                    Measurement.IntraSem(VSA, VSG1, BBU, mode, arrAttDL[j], port, setFrequency, setPower, setBandwidth, measClause[i, 13], out dbRS[0], out dbRS[1], out dbRS[2], ref RS);
        //                    //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
        //                    if (measurementBackgroundWorker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(1);
        //                    BBU.CHG_RRU_POWER(1000);
        //                    RS = Math.Round(dbRS[0], 1);
        //                    if (RS > limitMax)
        //                    {
        //                        PassFail = fail;
        //                        reTest = true;
        //                    }
        //                    else
        //                    {
        //                        PassFail = pass;
        //                    }
        //                    result = RS.ToString();
        //                    dataSEM[j, 0, 0] = PassFail;
        //                    dataSEM[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                    dataSEM[j, 0, 2] = Math.Round(dbRS[1], 1).ToString();
        //                    dataSEM[j, 0, 3] = Math.Round(dbRS[2], 1).ToString();
        //                    dataSEM[j, 0, 4] = Math.Round(dbRS[3], 1).ToString();
        //                }

        //            }
        //            #endregion
        //            #endregion
        //            #region uplink
        //            else if (measClause[i, 0] == "RX Power Calibration" && j == 0)
        //            {
        //                //RRU properties
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);

        //                bool flagrx = true;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                BBU.StopProcess();

        //                Measurement.calibRX(BBU, rfSwitch1, rfSwitch2, rfSwitch3, VSG1, setFrequency, "-30", out RS, out flagrx);
        //                if (flagrx == true)
        //                {
        //                    if (RS <= limitMax)
        //                    {
        //                        result = RS.ToString();
        //                        PassFail = "PASS";
        //                    }
        //                    else
        //                    {
        //                        PassFail = "FAIL";
        //                        result = RS.ToString();
        //                    }
        //                }
        //                else
        //                {
        //                    PassFail = "FAIL";
        //                    result = RS.ToString();
        //                }
        //                dataCalibRx[0] = measClause[i, 2]; // 0 Min limit
        //                dataCalibRx[1] = result;     // 1 Reading Value
        //                dataCalibRx[2] = measClause[i, 3]; // 2 Max limit
        //                dataCalibRx[3] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
        //                dataCalibRx[4] = PassFail;  // 4 Result
        //                ListViewShow("PORT" + (j + 1), measClause[i, 0].Remove(0, 2), measClause[i, 1], measClause[i, 2], result, measClause[i, 3], mode, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
        //            }
        //            /// cac bai do phia uplink chuong 7 3GPP
        //            // TODO: Reference Sensitivity Level
        //            else if (measClause[i, 0] == "+ Reference Sensitivity Level")
        //            {
        //                //RRU properties
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                string Power = measClause[i, 8]; //gia tri cong suat theo 3gpp

        //                //kiem tra neu dang khai bao vsg2 va vsg3 thi tat RF out

        //                if (VSG2 != null)
        //                {
        //                    VSG2.OnOffSignal(0);
        //                }

        //                if (VSG3 != null)
        //                {
        //                    VSG3.OnOffSignal(0);
        //                }

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip
        //                double ReferenceSensitivityThreshold = Measurement.oneVSGMode(VSG1, BBU, "SEN", int.Parse(measClause[i, 6]), measClause[i, 3], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), Power, setFrequency, out tp);
        //                if ((tp >= 95) && (ReferenceSensitivityThreshold <= double.Parse(Power)))
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = ReferenceSensitivityThreshold.ToString();
        //                mode = tp.ToString();
        //                dataPsen[j, 0] = PassFail;  // 0 Result
        //                dataPsen[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataPsen[j, 2] = tp.ToString();     // 3 Reading Value
        //                dataPsen[j, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Dynamic Range")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                string wsPower = measClause[i, 8];

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip
        //                Measurement.oneVSGMode(VSG1, BBU, "DR", int.Parse(measClause[i, 6]), measClause[i, 2], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, out tp);
        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                mode = tp.ToString();
        //                result = measClause[i, 9];
        //                dataDR[j, 0] = PassFail;  // 0 Result
        //                dataDR[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataDR[j, 2] = tp.ToString();     // 3 Reading Value
        //                dataDR[j, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ In-Channel Selectivity (ICS)")
        //            {

        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip


        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];

        //                string wsOffsetL = measClause[i, 14];
        //                string isOffsetH = measClause[i, 11];

        //                string wsOffsetH = measClause[i, 15];
        //                string isOffsetL = measClause[i, 10];

        //                string isFreq = "";

        //                if (VSG2 != null)
        //                {
        //                    //VSG2.SetFrequencyValue = double.Parse(setFrequency) + double.Parse(offset)*1000000;
        //                    //isFreq = VSG2.SetFrequencyValue.ToString();
        //                    VSG2.SetFrequencyValue = double.Parse(setFrequency);
        //                    isFreq = VSG2.SetFrequencyValue.ToString(); //23.2.2024
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", VSG2.SetFrequencyValue);
        //                    Log($"{VSG2.Alias}: Set ATT_Port{j + 1}={VSG2.Attenuators[j]} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
        //                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");
        //                }

        //                //IS at upper edge
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ICS", wsOffsetL, isOffsetH, double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreq, null, null, out tp);
        //                double tmpTP = tp;

        //                //IS at lower edge
        //                LogDashedLine();
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ICS", wsOffsetH, isOffsetL, double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreq, null, null, out tp);
        //                tp = Math.Min(tmpTP, tp);
        //                if (tp >= 95 & tp <= 100)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataICS[j, 0] = PassFail;  // 0 Result
        //                dataICS[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataICS[j, 2] = tp.ToString();     // 3 Reading Value
        //                dataICS[j, 3] = result;     // 4 Reading Value


        //            }
        //            else if (measClause[i, 0] == "+ Adjacent Channel Selectivity (ACS)")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                string offset = measClause[i, 11];  // xem lại giá trị này
        //                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(offset) * 1000000));
        //                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(offset) * 1000000));
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                //upper
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                if (VSG2 != null)
        //                {
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqUpper));
        //                    Log($"{VSG2.Name}: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dBm at f={VSG2.SetFrequencyValue / 1000000}MHz");
        //                    Log($"{VSG2.Name}: CSV Path:{VSG2.AttenuatorCSVpath}");
        //                    VSG2.AssignAlias("Interfering Signal");
        //                }

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip
        //                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ACS", measClause[i, 15], measClause[i, 17], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, null, null, out tp);
        //                dataACS[j, 0, 2] = tp.ToString();

        //                //lower
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                if (VSG2 != null)
        //                {
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqLower));
        //                    Log($"{VSG2.Name}: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dBm at f={VSG2.SetFrequencyValue}");
        //                    Log($"{VSG2.Name}: CSV Path:{VSG2.AttenuatorCSVpath}");
        //                    VSG2.AssignAlias("Interfering Signal");
        //                }

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ACS", measClause[i, 14], measClause[i, 16], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, null, null, out tp);
        //                dataACS[j, 1, 2] = tp.ToString();

        //                if (double.Parse(dataACS[j, 0, 2]) > double.Parse(dataACS[j, 1, 2]))
        //                {
        //                    tp = double.Parse(dataACS[j, 1, 2]);
        //                }
        //                else
        //                {
        //                    tp = double.Parse(dataACS[j, 0, 2]);
        //                }
        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataACS[j, 0, 0] = PassFail;  // 0 Result
        //                dataACS[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataACS[j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataACS[j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Inband Blocking")
        //            {
        //                //code chay voi time sequence 10s
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip
        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                double.TryParse(deltaFoob.Text, out double delta_Foob); //in MHz
        //                delta_Foob *= 1e6; //MHz to Hz
        //                Log($"Delta FOOB: {delta_Foob / 1e6} MHz");
        //                double.TryParse(setFrequency, out double tmpFc);  //in Hz                                  
        //                //define temp variables to store the worst case
        //                double isFrequencyMinL = 0;
        //                double isFrequencyMinR = 0;
        //                double isThroughputMinL = 110;
        //                double isThroughputMinR = 110;
        //                double isFreqSweep = 1e6; //sweep 1MHz

        //                //Left side
        //                //WS set RbOffset = 223 => TBD
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                double isLowerOffset = double.Parse(measClause[i, 10]) * 1e6; //=30MHz = 30*1e6 Hz
        //                double isUpperOffset = double.Parse(measClause[i, 11]) * 1e6;
        //                double isFreqLowerTo = double.Parse(setFrequency) - double.Parse(setBandwidth) * 1e6 / 2 - isLowerOffset;
        //                double isFreqLowerFrom = RRU.ChannelBWBottom - delta_Foob;
        //                double isFreqUpperFrom = double.Parse(setFrequency) + double.Parse(setBandwidth) * 1e6 / 2 + isUpperOffset;
        //                //double isFreqUpperTo = RRU.ChannelBWTop + delta_Foob;

        //                double isFreq = isFreqLowerFrom;
        //                string wsRbOffset = measClause[i, 14];
        //                string isRbOffset = measClause[i, 16];


        //                if (VSG2 != null)
        //                {
        //                    VSG2.AssignAlias("Interfering Signal");
        //                    VSG2.SetFrequencyValue = isFreqLowerFrom;
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", isFreqLowerFrom);
        //                    Log($"{VSG2.Alias}: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dBm at f={VSG2.SetFrequencyValue} Hz");
        //                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

        //                }
        //                Measurement.twoVSGMode(VSG1, VSG2, BBU, "INB2", wsRbOffset, isRbOffset, isFreqSweep, arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreq.ToString(), isFreqLowerTo.ToString(), isFreqUpperFrom.ToString(), out tp);
        //                if (tp >= 95 && tp <= 100)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    //reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataINB[j, 0, 0] = PassFail;  // 0 Result
        //                dataINB[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataINB[j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataINB[j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Narrow band blocking")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip
        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                string isLowerOffset = measClause[i, 10];
        //                string isUpperOffset = measClause[i, 11];

        //                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1e6) / 2 + (double.Parse(isUpperOffset) * 1e6));
        //                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1e6) / 2 - (double.Parse(isLowerOffset) * 1e6));

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);


        //                //19.2.2024 ====== sua lai bai NBB
        //                //IS on right side
        //                string wsRbOffset = measClause[i, 15];
        //                string[] arrRBoffsetR = measClause[i, 17].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                double minTP = 101;

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                if (VSG2 != null)
        //                {

        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqUpper));
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
        //                    Log($"Interfering Signal: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dBm at f={VSG2.SetFrequencyValue}");
        //                    Log($"Interfering Signal: CSV Path:{VSG2.AttenuatorCSVpath}");
        //                    VSG2.AssignAlias("Interfering Signal");
        //                }
        //                if (arrRBoffsetR.Length > 0)
        //                {
        //                    for (int step1NBB = 0; step1NBB < arrRBoffsetR.Length; step1NBB++)
        //                    {
        //                        if (measurementBackgroundWorker.CancellationPending) return;
        //                        LogDashedLine();
        //                        System.Threading.Thread.Sleep(1);
        //                        Log($"Set IS RBOffset at position:{arrRBoffsetR[step1NBB]}");
        //                        Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, arrRBoffsetR[step1NBB], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, null, null, out tp);
        //                        minTP = Math.Min(minTP, tp);
        //                    }
        //                }
        //                else
        //                {
        //                    if (measurementBackgroundWorker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(1);
        //                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, "0", double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, null, null, out tp);
        //                    minTP = Math.Min(minTP, tp);
        //                }

        //                //Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", measClause[i, 6], "0", double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, null, null, out tp);
        //                //dataNBB[j, 0, 2] = tp.ToString();
        //                dataNBB[j, 0, 2] = minTP.ToString();

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                //19.2.2024 ====== sua lai bai NBB
        //                //IS on left side
        //                wsRbOffset = measClause[i, 14];
        //                string[] arrRBoffsetL = measClause[i, 16].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                minTP = 101;
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                if (VSG2 != null)
        //                {
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqLower));
        //                    Log($"Interfering Signal: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dBm at f={VSG2.SetFrequencyValue}");
        //                    Log($"Interfering Signal: CSV Path:{VSG2.AttenuatorCSVpath}");
        //                    VSG2.AssignAlias("Interfering Signal");
        //                }
        //                if (arrRBoffsetL.Length > 0)
        //                {
        //                    for (int step2NBB = 0; step2NBB < arrRBoffsetL.Length; step2NBB++)
        //                    {
        //                        if (measurementBackgroundWorker.CancellationPending) return;
        //                        LogDashedLine();
        //                        System.Threading.Thread.Sleep(1);
        //                        Log($"Set IS RBOffset at position:{arrRBoffsetL[step2NBB]}");
        //                        Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, arrRBoffsetL[step2NBB], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, null, null, out tp);
        //                        minTP = Math.Min(minTP, tp);
        //                    }
        //                }
        //                else
        //                {
        //                    if (measurementBackgroundWorker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(1);
        //                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, "0", double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, null, null, out tp);
        //                    minTP = Math.Min(minTP, tp);
        //                }

        //                //Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", measClause[i, 6], "99", double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, null, null, out tp);
        //                //dataNBB[j, 1, 2] = tp.ToString();
        //                dataNBB[j, 1, 2] = minTP.ToString();

        //                //lay gia tri xau nhat
        //                tp = Math.Min(double.Parse(dataNBB[j, 0, 2]), double.Parse(dataNBB[j, 1, 2]));

        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataNBB[j, 0, 0] = PassFail;  // 0 Result
        //                dataNBB[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataNBB[j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataNBB[j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Out-of-band Blocking")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                testCase = "Uplink";
        //                specName = "OBB";
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip

        //                //code cu
        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                string offset = measClause[i, 11];
        //                string isFreqUpper = Convert.ToString(2690e6 + (double.Parse(offset) * 1e6)); // band n41 2496 – 2690
        //                string isFreqLower = Convert.ToString(2500e6 - (double.Parse(offset) * 1e6));

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                double isFreq = 1e6; //OOBB - CW start at 1MHz
        //                double stepOOBB = 1e6; //step sweep CW signal _1MHz 

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                if (chkOOB3.Checked)
        //                {
        //                    deltaFoobValue = double.Parse(deltaFoob.Text) * 1e6; //in Hz
        //                    Log($"Set CW Signal delta_Foob = {deltaFoobValue / 1e6} MHz");

        //                    double.TryParse(txtOOB1.Text, out double tmpLowerFreq);
        //                    double.TryParse(txtOOB2.Text, out double tmpUpperFreq);
        //                    //string isFreqLowerSpec = Convert.ToString(tmpLowerFreq * 1e6);
        //                    //string isFreqUpperSpec = Convert.ToString(tmpUpperFreq * 1e6);
        //                    //27.3.2024 - sua code
        //                    string isFreqLowerSpec = Convert.ToString(tmpLowerFreq); // MHz
        //                    string isFreqUpperSpec = Convert.ToString(tmpUpperFreq); // MHz
        //                    Log($"Set CW Signal frequency range from {tmpLowerFreq} MHz to {tmpUpperFreq} MHz");

        //                    if (measurementBackgroundWorker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(1);
        //                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "OOB2", measClause[i, 15], measClause[i, 17], stepOOBB, arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreq.ToString(), isFreqLowerSpec, isFreqUpperSpec, out tp);
        //                }
        //                else
        //                {
        //                    Log("ERROR: You don't config these parameters for Out-of-band", LogLevel.WARN);
        //                }

        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    //reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataOBB[j, 0, 0] = PassFail;  // 0 Result
        //                dataOBB[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataOBB[j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataOBB[j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Out-of-band Blocking Co-location")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                testCase = "Uplink";
        //                specName = "OOB1";
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip

        //                //code cu
        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                string offset = measClause[i, 11];
        //                string isFreqUpper = Convert.ToString(2690e6 + (double.Parse(offset) * 1e6)); // band n41 2496 – 2690
        //                string isFreqLower = Convert.ToString(2500e6 - (double.Parse(offset) * 1e6));

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                double isFreq = 1e6; //OOBB - CW start at 1MHz
        //                double stepOOBB = 1e6; //step sweep CW signal _1MHz 
        //                double.TryParse(txtOOB1.Text, out double tmpLowerFreq);
        //                double.TryParse(txtOOB2.Text, out double tmpUpperFreq);
        //                string isFreqLowerSpec = Convert.ToString(tmpLowerFreq * 1e6);
        //                string isFreqUpperSpec = Convert.ToString(tmpUpperFreq * 1e6);

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.twoVSGMode(VSG1, VSG2, BBU, "OOB1", measClause[i, 15], measClause[i, 17], stepOOBB, arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreq.ToString(), isFreqLowerSpec, isFreqUpperSpec, out tp);

        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    //reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataOBB[j, 0, 0] = PassFail;  // 0 Result
        //                dataOBB[j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataOBB[j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataOBB[j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            // TODO: General intermodulation
        //            else if (measClause[i, 0] == "+ General intermodulation")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip

        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                string isLowerOffset = measClause[i, 10];
        //                string isUpperOffset = measClause[i, 11];

        //                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(isLowerOffset) * 1000000));
        //                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(isUpperOffset) * 1000000));
        //                string cwLowerOffset = measClause[i, 12];
        //                string cwUpperOffset = measClause[i, 13];
        //                string cwFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(cwLowerOffset) * 1000000));
        //                string cwFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(cwUpperOffset) * 1000000));

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                if ((VSG2 != null) && (VSG3 != null))
        //                {
        //                    // Get att value for VSG2 or 5G NR Interfering singal at Upper edge
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqUpper));
        //                    Log($"{VSG2.Name}: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dB at {VSG2.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG2.Name}: CSV Path:{VSG2.AttenuatorCSVpath}");

        //                    // Get att value for VSG3 or Cw signal at Upper edge
        //                    VSG3.SetFrequencyValue = double.Parse(cwFreqUpper);
        //                    VSG3.ReadATTfromCSVdir(cwCSVFilePathRX, "CWRX", double.Parse(cwFreqUpper));
        //                    Log($"{VSG3.Name}: Set ATT_Port {j + 1} Level = {VSG3.Attenuators[j]} dB at {VSG3.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG3.Name}: CSV Path: {VSG3.AttenuatorCSVpath}");
        //                }

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "GenIMD", measClause[i, 15], measClause[i, 17], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, cwFreqUpper, out tp);
        //                dataRXIMD[0, j, 0, 2] = tp.ToString();

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                if ((VSG2 != null) && (VSG3 != null))
        //                {
        //                    // Get att value for VSG2 or 5G NR Interfering singal at Lower edge
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqLower));
        //                    Log($"{VSG2.Name}: Set ATT_Port{j + 1} Level= {VSG2.Attenuators[j]} dB at {VSG2.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG2.Name}: CSV Path: {VSG2.AttenuatorCSVpath}");

        //                    // Get att value for VSG3 or Cw signal at Lower edge
        //                    VSG3.SetFrequencyValue = double.Parse(cwFreqLower);
        //                    VSG3.ReadATTfromCSVdir(cwCSVFilePathRX, "CWRX", double.Parse(cwFreqLower));
        //                    Log($"{VSG3.Name}: Set ATT_Port {j + 1} Level = {VSG3.Attenuators[j]} dB at {VSG3.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG3.Name}: CSV Path: {VSG3.AttenuatorCSVpath}");
        //                }

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "GenIMD", measClause[i, 14], measClause[i, 16], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, cwFreqLower, out tp);
        //                dataRXIMD[0, j, 1, 2] = tp.ToString();

        //                if (double.Parse(dataRXIMD[0, j, 0, 2]) > double.Parse(dataRXIMD[0, j, 1, 2]))
        //                {
        //                    tp = double.Parse(dataRXIMD[0, j, 1, 2]);
        //                }
        //                else
        //                {
        //                    tp = double.Parse(dataRXIMD[0, j, 0, 2]);
        //                }
        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataRXIMD[0, j, 0, 0] = PassFail;  // 0 Result
        //                dataRXIMD[0, j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataRXIMD[0, j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataRXIMD[0, j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            // TODO: Narrow-band intermodulation
        //            else if (measClause[i, 0] == "+ Narrow-band intermodulation")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip

        //                //string wsPower = (double.Parse(measClause[i, 8]) + double.Parse(arrAttUL[j])).ToString();
        //                //string isPowerLower = (double.Parse(measClause[i, 9]) + double.Parse(arrAttUL[j])).ToString();
        //                //string isPowerUpper = (double.Parse(measClause[i, 10]) + double.Parse(arrAttUL[j])).ToString();

        //                string wsPower = measClause[i, 8];
        //                string isPower = measClause[i, 9];
        //                //string isPowerUpper = measClause[i, 10];
        //                string isLowerOffset = measClause[i, 10];
        //                string isUpperOffset = measClause[i, 11];

        //                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(isLowerOffset) * 1000000));
        //                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(isUpperOffset) * 1000000));
        //                string cwLowerOffset = measClause[i, 12];
        //                string cwUpperOffset = measClause[i, 13];
        //                string cwFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(cwLowerOffset) * 1000000));
        //                string cwFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(cwUpperOffset) * 1000000));

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                if ((VSG2 != null) && (VSG3 != null))
        //                {
        //                    // Get att value for VSG2 or 5G NR Interfering singal at Upper edge
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqUpper));
        //                    Log($"{VSG2.Name}: Set ATT_Port{j + 1} Level={VSG2.Attenuators[j]} dB at {VSG2.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG2.Name}: CSV Path:{VSG2.AttenuatorCSVpath}");

        //                    // Get att value for VSG3 or Cw signal at Upper edge
        //                    VSG3.SetFrequencyValue = double.Parse(cwFreqUpper);
        //                    VSG3.ReadATTfromCSVdir(cwCSVFilePathRX, "CWRX", double.Parse(cwFreqUpper));
        //                    Log($"{VSG3.Name}: Set ATT_Port {j + 1} Level = {VSG3.Attenuators[j]} dB at {VSG3.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG3.Name}: CSV Path: {VSG3.AttenuatorCSVpath}");
        //                }

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "NBIMD", measClause[i, 15], measClause[i, 17], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, cwFreqUpper, out tp);
        //                dataRXIMD[1, j, 0, 2] = tp.ToString();

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);

        //                if ((VSG2 != null) && (VSG3 != null))
        //                {
        //                    // Get att value for VSG2 or 5G NR Interfering singal at Lower edge
        //                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
        //                    VSG2.ReadATTfromCSVdir(isCSVFilePathRX, "ISRX", double.Parse(isFreqLower));
        //                    Log($"{VSG2.Name}: Set ATT_Port{j + 1} Level= {VSG2.Attenuators[j]} dB at {VSG2.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG2.Name}: CSV Path: {VSG2.AttenuatorCSVpath}");

        //                    // Get att value for VSG3 or Cw signal at Lower edge
        //                    VSG3.SetFrequencyValue = double.Parse(cwFreqLower);
        //                    VSG3.ReadATTfromCSVdir(cwCSVFilePathRX, "CWRX", double.Parse(cwFreqLower));
        //                    Log($"{VSG3.Name}: Set ATT_Port {j + 1} Level = {VSG3.Attenuators[j]} dB at {VSG3.SetFrequencyValue / 1000000} MHz");
        //                    Log($"{VSG3.Name}: CSV Path: {VSG3.AttenuatorCSVpath}");
        //                }

        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "NBIMD", measClause[i, 14], measClause[i, 16], double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, cwFreqLower, out tp);
        //                dataRXIMD[1, j, 1, 2] = tp.ToString();

        //                if (double.Parse(dataRXIMD[1, j, 0, 2]) > double.Parse(dataRXIMD[1, j, 1, 2]))
        //                {
        //                    tp = double.Parse(dataRXIMD[1, j, 1, 2]);
        //                }
        //                else
        //                {
        //                    tp = double.Parse(dataRXIMD[1, j, 0, 2]);
        //                }
        //                if (tp >= 95)
        //                {
        //                    PassFail = pass;
        //                }
        //                else
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                result = measClause[i, 9];
        //                mode = tp.ToString();
        //                dataRXIMD[1, j, 0, 0] = PassFail;  // 0 Result
        //                dataRXIMD[1, j, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataRXIMD[1, j, 0, 2] = tp.ToString();     // 3 Reading Value
        //                dataRXIMD[1, j, 0, 3] = result;     // 4 Reading Value
        //            }
        //            else if (measClause[i, 0] == "+ Receiver spurious emission")
        //            {
        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                NRTM = "NRTM_11";
        //                lblStatusProgress_TextChanged($"{measClause[i, 0]} Port {j + 1}");
        //                //GeneralDataSetupTX(RXspurFilePath, measClause[i, 0], j, mode);
        //                limitMax = double.Parse(measClause[i, 3]);
        //                MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");
        //                BBU.TDDMODE("RX_ONLY");
        //                Measurement.Spur5G(VSA, BBU, "Rx-Spurious", NRTM, arrAttDL[j], j + 1, setFrequency, setBandwidth, out tseRS);
        //                //==============19.2.2024 save screen shot =====================
        //                //TakeScreenshotVSA(VSA, "RX", "Rx-Spurious", $"P{j + 1}", setFrequency);
        //                nRS = -100;
        //                for (int i1 = 0; i1 < tseRS.Length; i1++)
        //                {
        //                    if (nRS < tseRS[i1])
        //                    {
        //                        nRS = tseRS[i1];
        //                    }
        //                }
        //                if (nRS > limitMax)
        //                {
        //                    PassFail = fail;
        //                    reTest = true;
        //                }
        //                else
        //                {
        //                    PassFail = pass;
        //                    result = Math.Round((double)nRS, 1).ToString();
        //                    Console.WriteLine("Ket qua cuoi cung  " + result);
        //                }
        //            }
        //            #endregion
        //            if (measClause[i, 0] != "+ RX Power Calibration")
        //                ListViewShow("PORT" + (j + 1), measClause[i, 0].Remove(0, 2), measClause[i, 1], measClause[i, 2], result, measClause[i, 3], mode, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());

        //            #region Characteristic receiver
        //            // TODO: Characterise sensitivity
        //            if (measClause[i, 0] == "+ Characterise sensitivity")
        //            {
        //                VSG1.SetFrequencyValue = double.Parse(setFrequency);
        //                GeneralDataSetupRX(measClause[i, 0], j);
        //                //BBU.Switch(j); Redhat- tam thoi khong su dung switch chip


        //                double.TryParse(measClause[i, 3], out limitMax);
        //                double.TryParse(measClause[i, 2], out limitMin);
        //                string Power = (double.Parse(measClause[i, 8]) + double.Parse(arrAttUL[j])).ToString();
        //                string modeMeas = "SEN";
        //                double stepdB = double.Parse(measClause[i, 7]);

        //                if (measurementBackgroundWorker.CancellationPending) return;
        //                System.Threading.Thread.Sleep(1);
        //                Measurement.oneVSGModeCharacterise(VSG1, BBU, modeMeas, stepdB, arrAttUL[j], (j + 1), Power, setFrequency, out tp);
        //                result = measClause[i, 8];
        //                mode = tp.ToString();
        //                dataPsen[j, 0] = PassFail;  // 0 Result
        //                dataPsen[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
        //                dataPsen[j, 2] = tp.ToString();     // 3 Reading Value
        //                dataPsen[j, 3] = result;     // 4 Reading Value
        //            }
        //            #endregion Characteristic receiver
        //        }
        //        int maxValue = (numberMeasClause * numberPortChecked);
        //        prgLoad_ValueChange(maxValue, count);
        //        if (reTest)
        //        {
        //            //timeout++; Note 2/9 for testing 1 time
        //            timeout = loopTime;
        //            if (timeout == loopTime)
        //            {
        //                //TODO: add data to chart 21.2.2024
        //                if (!measClause[i, 0].Contains("Blocking"))
        //                {
        //                    if (testCase == "Downlink")
        //                    {
        //                        PopulateMeasurementChart(measClause[i, 0], $"PORT{j + 1}", result, limitMin.ToString("N3"), limitMax.ToString("N3"));
        //                    }
        //                    else if (testCase == "Uplink")
        //                    {
        //                        PopulateMeasurementChart(measClause[i, 0], $"PORT{j + 1}", tp.ToString(), "95", "100");
        //                    }

        //                }
        //                timeout = 0;
        //                break;
        //            }
        //            deleteListView();
        //        }
        //        else
        //        {
        //            //TODO: add data to chart 21.2.2024
        //            if (!measClause[i, 0].Contains("Blocking"))
        //            {
        //                PopulateMeasurementChart(measClause[i, 0], $"PORT{j + 1}", result, limitMin.ToString("N3"), limitMax.ToString("N3"));
        //            }
        //            timeout = 0;
        //            break;
        //        }
        //    }
        //}
        //}
        //}
        //clearFlag = true;
        ////dung phat 
        //VSG1.OnOffSignal(0);
        //VSG2.OnOffSignal(0);
        //VSG3.OnOffSignal(0);
        //BBU.StopProcess();
        //measurementBackgroundWorker.CancelAsync();
        //endingTime = lblRuntime.Text;
        //lblStatusProgress_TextChanged("Đang lưu File kết quả");
        //timerProcess.Stop();
        //ExporttoHtml();
        //ExportListView();
        ////lblStatusProgress_TextChanged("Quá trình đo đã kết thúc");
        //txtSerial_TextChanged("");
        //btnConnect_Enable();
        //}
        #endregion

        #region measurementBackgroundWorker_Completed
        //TODO: Measurement BackgroundWorker_Completed
        public void measurementBackgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Log("Measurement Canceled!", LogLevel.WARN);
            }
            else if (e.Error != null)
            {
                Log($"An Error occured: {e.Error.Message}", LogLevel.ERROR);
            }
            else
            {
                Log("Measurement Completed!", LogLevel.SUCCESS);
            }

            //dung phat 
            VSG1.OnOffSignal(0);
            VSG2.OnOffSignal(0);
            VSG3.OnOffSignal(0);
            //BBU.StopProcess();
            measurementBackgroundWorker.CancelAsync();
            endingTime = lblRuntime.Text;
            lblStatusProgress_TextChanged("Đang lưu File kết quả");
            timerProcess.Stop();
            ExporttoHtml();
            ExportListView();
            ExportLogToTxt();
            txtSerial_TextChanged("");
            btnConnect_Enable();

            lblStatusProgress_TextChanged("Quá trình đo đã kết thúc");
            runningTask.Text = "";
            clearFlag = true;
            enRecordLog = false;
        }
        #endregion
        #region measurementBackgroundWorker_ProgressChanged
        //TODO: Measurement BackgroundWorker_ProgressChanged
        public void measurementBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Check if UserState is an integer (max value)
            if (e.UserState is int max)
            {
                // Ensure max is greater than zero to avoid division by zero
                if (max > 0)
                {
                    lblProgressVal.Text = $"{(e.ProgressPercentage * 100 / max)}%"; // Format to 2 decimal places
                }
            }
            // Check if UserState is a string (status message)
            else if (e.UserState is string state)
            {
                runningTask.Text = state; // Update running task label with status message
            }
            else
            {
                runningTask.Text = "Unknown state"; // Fallback for unexpected types
            }
        }
        #endregion

        #region Process Functions
        public void ProgressLabel(string text)
        {
            if (measurementBackgroundWorker.IsBusy)
            {
                measurementBackgroundWorker.ReportProgress(0, text);
            }
            else
            {
                runningTask.Text = text;
            }

        }

        // Loading Progress Bar
        public void prgLoad_ValueChange(int max, int value)
        {
            ProgressBar status = proLoad;
            status.Invoke(new MethodInvoker(delegate ()
            {
                if (value <= max)
                {
                    proLoad.Maximum = max;
                    proLoad.Minimum = 0;
                    proLoad.Value = value;
                }
                else
                {
                    value = 0;
                };
                lblProgressVal.Text = (max > 0) ? $"{value / max}%" : "0%";

            }));

        }

        private void UpdateProgressBar(int value)
        {
            //calculate Progressbar
            List<TestcaseNode> TestcaseNa = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Na);
            int progressBarMax = testingPortList.Count * (TestcaseNodes.Count - TestcaseNa.Count);
            prgLoad_ValueChange(progressBarMax, value);
            measurementBackgroundWorker.ReportProgress(value, progressBarMax);

        }
        #endregion

        private void PrintMeasurementInfo()
        {

        }
    }
}
