using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcl_pm64;

namespace _5GAutoTool
{
    public class PowerSensor
    {
        mcl_pm64.usb_pm pm1;
        private Form5GAT mainForm;
        public string Name = "PS";
        public PowerSensor(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        short Status = 0;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "PowerSensor");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Connect()
        {
            string pm_SN = "";
            pm1 = new mcl_pm64.usb_pm();
            //if more then 1 sensor connected to the computer than the Serial Number of the sensor should provide
            Status  = pm1.Open_Sensor(ref (pm_SN));
            Console.WriteLine(Status);
            if (Status == 1)
            {
                pm1.Format_mW = false;
                pm1.AVG = 1;        // Set the Average = 100
                pm1.AvgCount = 100;
            }
        }
        public void ReadPower(float Freq, out float ReadResult)
        {
            ReadResult = 0;
            if(Status == 1)
            {
                pm1.Freq = 2550;      // Set the Frequency cal factor in MHz
                ReadResult = pm1.ReadPower(); // read the power in dbm
                Console.WriteLine(ReadResult);
            }
        }
        public void Close()
        {
            pm1.Close_Sensor();
        }

    }
}
