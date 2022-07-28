using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    internal class Program
    {
        static int Main(string[] args)
        {
            //проверка параметров
            if(args.Length != 3)
            {
                Console.WriteLine("Ошибка: Не верное колличество параметров !!!");
                return -1;
            }
            //преобразование параметров в нужный тип
            string temp; IPAddress ip = IPAddress.Parse("127.0.0.1"); int port = 0;
            try
            {
                ip = IPAddress.Parse(args[0]);
                port = Convert.ToInt32(args[1]);
                temp = args[2];
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            //создание сокета и прослушивание на подключение
            Socket socket_Tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipPoint = new IPEndPoint(ip, port);
            try
            {
                socket_Tcp.Bind(ipPoint);
                socket_Tcp.Listen(1);
                Console.WriteLine("Ожидание поключения ...");
                socket_Tcp.Accept();
            }
            catch
            {
                Console.WriteLine("Ошибка соединения !!!");
                return -1;
            }
            Console.WriteLine("Новое подключение !!!");
            
            Console.ReadLine();
            socket_Tcp.Shutdown(SocketShutdown.Both);
            socket_Tcp.Close();
            return 0;
        }
    }
}