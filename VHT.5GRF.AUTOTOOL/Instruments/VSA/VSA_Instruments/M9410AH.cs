using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    public class M9410A
    {
        public Form5GAT mainForm;
        public M9410A(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 m9410a = new Ivi.Visa.Interop.FormattedIO488();
        //Form5GAT MainForm = new Form5GAT();
        /*...........................................Connection...................................................................................*/
        string rruinfo = "";
        public void RRUinfo(string rruSerial)
        {
            rruinfo = rruSerial;
        }
        string port = "";
        public void Port(string Port)
        {
            port = Port;
        }
        public void Connection(string IP, string Cmd, string Instrument, out string manufacturer, out string model, out string serial)
        {
            manufacturer = "ERROR";
            model = "ERROR";
            serial = "ERROR";
            try
            {
                //    Disconnect();
                m9410a.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                m9410a.IO.Timeout = 2000;
                m9410a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])m9410a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                manufacturer = tmp[0].ToString();
                model = tmp[1].ToString();
                serial = tmp[2].ToString();
                if (tmp[1].ToString() == Instrument)
                {
                    mainForm.LogText("M9410A: connected to " + IP);
                }
            }
            catch (Exception e)
            {
                mainForm.LogText("M9410A connect: fail");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (m9410a != null)
                {
                    m9410a.IO.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {

                m9410a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])m9410a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception)
            {
                mainForm.LogText("M9410A connect: fail");
            }
        }
    }
}
