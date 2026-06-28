using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    #region OAM Communicator
    public class OamCommunicator : TcpCommunicator
    {
        private OamCommunicator(string host, int port = 1969) : base(host, port) { }

        public byte[] Send(string command, bool showCmd = true)
        {
            if(showCmd) Log(command);
            var oamHeader = new OamHeader { MessageLength = (ushort)command.Length }; 
            var data = oamHeader.GetHeader();
            Encoding.UTF8.GetBytes(command).CopyTo(data, 16); // convert command to byte and coppy to data
            base.Send(data);
            return data;
        }

        public string Receive(OamHeader header)
        {
            var received = base.Receive();
            if (received == null)
            {
                return null;
            }
            header.SetHeader(received.Take(16).ToArray());
            return Encoding.UTF8.GetString(received.Skip(16).ToArray());
        }

        public static OamCommunicator GetInstance(string host, int port)
        {
            return new OamCommunicator(host, port);
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel);
        }
    }
    #endregion
}
