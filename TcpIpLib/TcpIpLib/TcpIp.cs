using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TcpIpLib
{
    public class TcpIp
    {
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port_tcp;
        public int port_udp;

        private int mtu = 256;

        private Socket socket_Tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Socket socket_Udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        public bool GetConnection()
        {
            IPEndPoint ipPoint = new IPEndPoint(ip, port_tcp);
            try
            {
                socket_Tcp.Bind(ipPoint);
                socket_Tcp.Listen(1);

                socket_Tcp = socket_Tcp.Accept();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Connection()
        {
            IPEndPoint ipPoint = new IPEndPoint(ip, port_tcp);
            try
            {
                socket_Tcp.Connect(ipPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadTcp(out string str)
        {
            StringBuilder message = new StringBuilder();
            int bytes = 0;
            byte[] buffer = new byte[mtu];
            try
            {
                do
                {
                    bytes = socket_Tcp.Receive(buffer);
                    message.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                }
                while (socket_Tcp.Available > 0);
                str = message.ToString();
                return true;
            }
            catch
            {
                str = "";
                return false;
            }
        }
        public bool SendMessageTcp(string str)
        {
            byte[] buffer = new byte[mtu];
            try
            {
                buffer = Encoding.Unicode.GetBytes(str);
                socket_Tcp.Send(buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadUdp(out string str)
        {
            StringBuilder message = new StringBuilder();
            int bytes = 0;
            byte[] buffer = new byte[mtu];
            IPEndPoint ipPoint = new IPEndPoint(ip, port_udp);
            EndPoint getIpPoint = new IPEndPoint(IPAddress.Any, 0);
            socket_Udp.Bind(ipPoint);
            try
            {
                do
                {
                    bytes = socket_Udp.ReceiveFrom(buffer, ref getIpPoint);
                    message.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                }
                while (socket_Udp.Available > 0);
                str = message.ToString();
                return true;
            }
            catch
            {
                str = "";
                return false;
            }
        }
        public bool SendMessageUdp(string str)
        {
            byte[] buffer = new byte[mtu];
            try
            {
                buffer = Encoding.Unicode.GetBytes(str);
                IPEndPoint ipPoint = new IPEndPoint(ip, port_udp);
                socket_Udp.SendTo(buffer, ipPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Close(bool key)
        {
            try
            {
                if (key)
                {
                    socket_Tcp.Shutdown(SocketShutdown.Both);
                    socket_Tcp.Close();
                }
                else
                {
                    socket_Udp.Shutdown(SocketShutdown.Both);
                    socket_Udp.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}