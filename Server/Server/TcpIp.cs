using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    public class TcpIp
    {
        public static IPAddress ip = IPAddress.Parse("127.0.0.1");
        public static int port_tcp;
        public static int port_udp;

        private static StringBuilder message = new StringBuilder();
        private static int bytes = 0;
        private static byte[] buffer = new byte[256];

        private static Socket socket_Tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket socket_Udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 0);


        public static bool GetConnection()
        {
            ipPoint = new IPEndPoint(ip, port_tcp);
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
        public static bool Connection()
        {
            ipPoint = new IPEndPoint(ip, port_tcp);
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
        public static bool ReadTcp(out string str)
        {
            message = new StringBuilder();
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
        public static bool SendMessageTcp(string str)
        {
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
        public static bool ReadUdp(out string str)
        {
            message = new StringBuilder();
            ipPoint = new IPEndPoint(ip, port_udp);
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
        public static bool SendMessageUdp(string str)
        {
            try
            {
                buffer = Encoding.Unicode.GetBytes(str);
                ipPoint = new IPEndPoint(ip, port_udp);
                socket_Udp.SendTo(buffer, ipPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Close(bool key)
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
