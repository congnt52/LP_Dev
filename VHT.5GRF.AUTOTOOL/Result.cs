using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    class Result
    {
        public int port { get; set; }
        public string clause { get; set; }
        public double meas { get; set; }
        public DateTime time { get; set; }
        public bool passfail { get; set; }
        public string note { get; set; }
        public Result (Form5GAT mainForm)
        { }
    }
}
