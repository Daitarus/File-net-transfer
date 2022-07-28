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
            string[] args1 = new string[5];
            args1[0] = "127.0.0.1";
            args1[1] = "4000";
            args1[2] = "5000";
            args1[3] = "fgf";
            args1[4] = "500";
            //проверка параметров
            if (args1.Length != 5)
            {
                Console.WriteLine("Ошибка: Не верное колличество параметров !!!");
                return -1;
            }
            //преобразование параметров в нужный тип
            int time=0; string filename;
            try
            {
                TcpIp.ip = IPAddress.Parse(args1[0]);
                TcpIp.port_tcp = Convert.ToInt32(args1[1]);
                TcpIp.port_udp = Convert.ToInt32(args1[2]);
                filename = args1[3];
                time = Convert.ToInt32(args1[4]);
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            //подключение TCP
            if(TcpIp.Connection())
            {
                Console.WriteLine("Подключено...");
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                return -1;
            }
            //отправка имени файла и порта для udp
            if(TcpIp.SendMessageTcp($"{filename}:{TcpIp.port_udp}"))
            {
                Console.WriteLine("Имя файла и номер порта для UDP отправленны...");
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                return -1;
            }

            Thread.Sleep(5000);
            //отправка сообщения udp
            if(TcpIp.SendMessageUdp("Новое сообщение!!!"))
            {
                Console.WriteLine("Сообщение отправленно...");
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                return -1;
            }

            TcpIp.Close(false);
            TcpIp.Close(true);
            return 0;
        }
    }
}