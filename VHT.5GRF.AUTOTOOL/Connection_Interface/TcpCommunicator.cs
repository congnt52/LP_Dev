
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace _5GAutoTool
{
    public interface ICommunicator : IDisposable
    {
        void Connect();
        void Send(byte[] data);
        byte[] Receive();
    }
    public class TcpCommunicator : ICommunicator
    {
        private IPAddress IpAddress { get; }
        private IPEndPoint RemoteEndpoint { get; }
        private Socket RemoteSocket { get; }

        public bool IsDisposed { get; private set; }
        public bool IsConnected { get; set; }

        protected TcpCommunicator(string host, int port)
            : this(IPAddress.Parse(host), port) { }

        protected TcpCommunicator(IPAddress host, int port)
        {
            IsDisposed = false;
            IpAddress = host;
            RemoteEndpoint = new IPEndPoint(IpAddress, port);
            RemoteSocket = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

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
        //==============
        public virtual void Connect()
        {
            try
            {
                RemoteSocket.Connect(RemoteEndpoint);
                //Console.WriteLine($@"Socket connected to {RemoteEndpoint}");
                IsConnected = true;
            }
            catch (Exception e)
            {
                IsConnected = false;
                throw new Exception($@"Cannot connect to {RemoteEndpoint}: {e.Message}");
            }
        }

        public virtual void Send(byte[] data)
        {
            try
            {
                if (!IsConnected) throw new Exception();
                RemoteSocket.Send(data);
                // time out after 2 senconds
                RemoteSocket.ReceiveTimeout = 5000; //change to 5s
                //Console.WriteLine($@"Sent {data.Length} bytes to remote socket.");
            }
            catch
            {
                throw new Exception(@"Cannot send data to remote socket!");
            }
        }

        public virtual byte[] Receive()
        {
            try
            {
                if (!IsConnected) throw new Exception();
                var response = new byte[4096];
                // time out after 60 seconds
                RemoteSocket.ReceiveTimeout = 1200000; //change to 2mins
                //// Begin receiving the data from the remote device.  
                var length = RemoteSocket.Receive(response);
                //Console.WriteLine($@"Received {length} bytes from remote socket.");

                return new ArraySegment<byte>(response, 0, length - 1).ToArray();
            }
            catch(TimeoutException e)
            {
                Console.WriteLine(@"Cannot receive respond from remote socket " + e);
                throw new Exception(@"Cannot receive respond from remote socket " + e);

            }
        }

        public void Dispose()
        {
            try
            {
                IsDisposed = true;
                IsConnected = false;
                RemoteSocket.Shutdown(SocketShutdown.Both);
                RemoteSocket.Close();
                RemoteSocket.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}
