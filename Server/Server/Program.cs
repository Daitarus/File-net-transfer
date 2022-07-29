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
            args = new string[3];
            args[0] = "127.0.0.1";
            args[1] = "4000";
            args[2] = "sdf";
            //проверка параметров
            if(args.Length != 3)
            {
                Console.WriteLine("Ошибка: Не верное колличество параметров !!!");
                return -1;
            }
            //преобразование параметров в нужный тип
            string temp;
            try
            {
                TcpIp.ip = IPAddress.Parse(args[0]);
                TcpIp.port_tcp = Convert.ToInt32(args[1]);
                temp = args[2];
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            //создание сокета tcp, прослушивание и подключение
            Console.WriteLine("Сервер запущен...");
            if(TcpIp.GetConnection())
            {
                Console.WriteLine("Новое подключение...");
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                return -1;
            }
            //получение имени файла и номера порт для udp
            string message;
            if (TcpIp.ReadTcp(out message))
            {
                string filename;
                try
                {
                    filename = message.Substring(0, message.IndexOf(":"));
                    TcpIp.port_udp = Convert.ToInt32(message.Substring(message.IndexOf(":") + 1, message.Length - message.IndexOf(":") - 1));

                }
                catch
                {
                    Console.WriteLine("Ошибка: Данные о UDP подключении переданны не верно !!!");
                    return -1;
                }
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                return -1;
            }
            //получение сообщения по UDP
            if (TcpIp.ReadUdp(out message))
            {
                Console.WriteLine(message);
                //отправляем сообщение о получении сообщения
                TcpIp.SendMessageTcp("ok");
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                return -1;
            }
            Console.ReadLine();
            TcpIp.Close(false);
            TcpIp.Close(true); ;
            return 0;
        }       
    }
}