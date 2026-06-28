using Agilent.SA.Vsa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace _5GAutoTool
{
    [Serializable]
    public class Parameter
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }

        //[XmlIgnore]
        //public Type Unit { get; set; }
        [XmlAttribute("Unit")]
        public String Unit { get; set; }

        [XmlAttribute("Description")]
        public String Description { get; set; }

        public Parameter() { }
    }
}
