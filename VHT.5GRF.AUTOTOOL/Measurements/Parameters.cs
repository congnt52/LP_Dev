using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    public class Parameters
    {
        public Form5GAT MainForm;
        public Parameters(Form5GAT mainForm)
        {
            MainForm = mainForm;
            VSA = MainForm.VSA;
            VSG1 = MainForm.VSG1;
            VSG2 = MainForm.VSG2;
            VSG3 = MainForm.VSG3;
            RRU = MainForm.RRU;
            BBU = MainForm.BBU;
            RFS1 = MainForm.rfSwitch1;
            RFS2 = MainForm.rfSwitch2;
            RFS3 = MainForm.rfSwitch3;

        }

        //define Devices
        public VSA VSA;
        public VSG VSG1;
        public VSG VSG2;
        public VSG VSG3;
        public RRU RRU;
        public BBU BBU;
        public RFSwitch RFS1;
        public RFSwitch RFS2;
        public RFSwitch RFS3;

        //Device's properties



    }
}
