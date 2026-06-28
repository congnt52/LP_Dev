using Keysight.SignalStudio.N7631;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool.Measurements
{
    public class PortMapping
    {
        public int Port { get; set; }
        public int XRanIndex { get; set; }
        public int StreamIndex { get; set; }
        public  string Result { get; set; }
        public bool IsPassed { get; set; }


        public PortMapping(int port, int xRanIndex, int streamIndex)
        {
            Port=port;
            XRanIndex=xRanIndex;
            StreamIndex=streamIndex;
            Result = "NA";
            IsPassed = false;
        }  
        public PortMapping()
        {
            Port=0;
            XRanIndex=0;
            StreamIndex=0;
            Result = "NA";
            IsPassed = false;
        }

        public void Clear()
        {
            Result = "NA";
            IsPassed = false;
        }
    }
}
