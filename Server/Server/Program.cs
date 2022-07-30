using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpIpLib;

namespace Server
{
    internal class Program
    {
        static int Main(string[] args)
        {
            args = new string[3];
            args[0] = "127.0.0.1";
            args[1] = "4000";
            args[2] = @"Data\Files\";
            //проверка параметров
            if (args.Length != 3)
            {
                Console.WriteLine("Ошибка: Не верное колличество параметров !!!");
                return -1;
            }
            //преобразование параметров в нужный тип
            string temp;
            TcpIp tcpIp = new TcpIp();
            try
            {
                tcpIp.ip = IPAddress.Parse(args[0]);
                tcpIp.port_tcp = Convert.ToInt32(args[1]);
                temp = args[2];
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            while (true)
            {
                //создание сокета tcp, прослушивание и подключение
                Console.WriteLine("Сервер запущен...");
                if (tcpIp.GetConnection())
                {
                    Console.WriteLine("Новое подключение...");
                }
                else
                {
                    Close("Ошибка подключения !!!", tcpIp);
                    return -1;
                }
                //получение имени файла и номера порт для udp
                string message; string filename = "";
                if (tcpIp.ReadTcp(out message))
                {
                    try
                    {
                        filename = message.Substring(0, message.IndexOf(":"));
                        tcpIp.port_udp = Convert.ToInt32(message.Substring(message.IndexOf(":") + 1, message.Length - message.IndexOf(":") - 1));

                    }
                    catch
                    {
                        Close("Ошибка: Данные о UDP подключении переданны не верно !!!", tcpIp);
                        return -1;
                    }
                }
                else
                {
                    Close("Ошибка: Отсутствует соединение !!!", tcpIp);
                    return -1;
                }
                //создание файла
                FileWork fileWork = new FileWork();
                if (fileWork.CreateDir(temp))
                {
                    Console.WriteLine("Директория создана...");
                }
                else
                {
                    Close("Директория не может быть создана по указанному пути", tcpIp);
                    return -1;
                }
                if (fileWork.CreateFile(temp, filename))
                {
                    Console.WriteLine("Файл создан...");
                }
                else
                {
                    Close("Файл не может быть создан по указанному пути", tcpIp);
                    return -1;
                }
                //получение сообщения по UDP
                if (tcpIp.ReadUdp(out message))
                {
                    Console.WriteLine(message);
                    //отправляем сообщение о получении сообщения
                    if (tcpIp.SendMessageTcp("ok"))
                    {
                        Console.WriteLine("Подтверждение отправленно...");
                    }
                }
                else
                {
                    Close("Ошибка: Отсутствует соединение !!!", tcpIp);
                    return -1;
                }
                //Console.ReadLine();
                Close("Сервер отключён !", tcpIp);
            }
            return 0;
        }      
        
        static void Close(string message, TcpIp tcpIp)
        {
            Console.WriteLine(message);
            tcpIp.Close();
        }
    }
}