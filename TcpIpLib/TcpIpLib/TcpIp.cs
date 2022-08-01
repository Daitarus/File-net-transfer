using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

namespace TcpIpLib
{
    public class TcpIp
    {
        public IPAddress ip;
        public int port_tcp;
        public int port_udp;

        private int mtu = 256;

        private TcpClient tcpClient;
        private NetworkStream tcpStream;

        public bool GetConnection()
        {
            TcpListener tcpServer = new TcpListener(ip, port_tcp);
            try
            {
                tcpServer.Start();
                tcpClient = tcpServer.AcceptTcpClient();
                tcpStream = tcpClient.GetStream();
                if (tcpServer != null)
                {
                    tcpServer.Stop();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Connection()
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(ip, port_tcp);
                tcpStream = tcpClient.GetStream();
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
            int bytes;
            byte[] buffer = new byte[mtu];
            try
            {
                do
                {
                    bytes = tcpStream.Read(buffer, 0, buffer.Length);
                    message.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                }
                while (tcpStream.DataAvailable);
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
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                tcpStream.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadUdp(out string str)
        {
            UdpClient udpClient = new UdpClient(port_udp);
            IPEndPoint ipPoint = null;
            try
            {
                byte[] buffer = udpClient.Receive(ref ipPoint);
                str = Encoding.UTF8.GetString(buffer);
                udpClient.Close();
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
            try
            {
                UdpClient udpClient = new UdpClient();
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                udpClient.Send(buffer, buffer.Length, ip.ToString(), port_udp);
                udpClient.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Close()
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
                if (tcpStream != null)
                {
                    tcpStream.Close();
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