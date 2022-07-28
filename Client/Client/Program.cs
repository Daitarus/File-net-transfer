using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    internal class Program
    {
        static int Main(string[] args)
        {
            //проверка параметров
            if (args.Length != 5)
            {
                Console.WriteLine("Ошибка: Не верное колличество параметров !!!");
                return -1;
            }
            //преобразование параметров в нужный тип
            IPAddress ip = IPAddress.Parse("127.0.0.1"); int port_Tcp = 0, port_Udp=0, time=0; string file_temp;
            try
            {
                ip = IPAddress.Parse(args[0]);
                port_Tcp = Convert.ToInt32(args[1]);
                port_Udp = Convert.ToInt32(args[2]);
                file_temp = args[3];
                time = Convert.ToInt32(args[4]);
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            Console.WriteLine($"{ip} {port_Tcp} {port_Udp} {file_temp} {time}");
            //подключение TCP
            IPEndPoint ipPoint = new IPEndPoint(ip, port_Tcp);
            Socket socket_Tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket_Tcp.Connect(ipPoint);
                Console.WriteLine("Подключено !!!");
            }
            catch
            {
                Console.WriteLine("Ошибка подключения !!!");
            }
            return 0;
        }
    }
}