using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpIpLib;

namespace Client
{
    public class Program
    {
        static bool flag = true;

        static int Main(string[] args)
        {
            args = new string[5];
            args[0] = "127.0.0.1";
            args[1] = "4000";
            args[2] = "5000";
            args[3] = "test.txt";
            args[4] = "500";
            //проверка параметров
            if (args.Length != 5)
            {
                Console.WriteLine("Ошибка: Не верное колличество параметров !!!");
                return -1;
            }
            //преобразование параметров в нужный тип
            int time=0; string filename; TcpIp tcpIp = new TcpIp();
            try
            {
                tcpIp.ip = IPAddress.Parse(args[0]);
                tcpIp.port_tcp = Convert.ToInt32(args[1]);
                tcpIp.port_udp = Convert.ToInt32(args[2]);
                filename = args[3];
                time = Convert.ToInt32(args[4]);
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            //подключение TCP
            if(tcpIp.Connection())
            {
                Console.WriteLine("Подключено...");
            }
            else
            {
                Close("Ошибка подключения !!!", tcpIp);
                return -1;
            }
            //отправка имени файла и порта для udp
            if(tcpIp.SendMessageTcp($"{filename}:{tcpIp.port_udp}"))
            {
                Console.WriteLine("Имя файла и номер порта для UDP отправленны...");
            }
            else
            {
                Close("Ошибка: Отсутствует соединение !!!", tcpIp);
                return -1;
            }

            //udp
            string mess;
            SendMess(time, tcpIp);
            //проверка на присутствие tcp соединения
            //.......................................
            //
            if(tcpIp.ReadTcp(out mess))
            {
                if(mess == "ok")
                {
                    flag = false;
                    Console.WriteLine("Передача завершена...");
                }
            }
            Console.ReadLine();
            Close("Передача завершена !", tcpIp);
            return 0;
        }

        static async void SendMess(int time, TcpIp tcpIp)
        {
            await Task.Run(async () =>
            {
                while (flag)
                {
                    //отправка сообщения udp
                    if (tcpIp.SendMessageUdp("Новое сообщение!!!"))
                    {
                        Console.WriteLine("Сообщение отправленно...");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: Отсутствует соединение !!!");
                        flag = false;
                    }
                    Thread.Sleep(time);
                }
            });
        }

        static void Close(string message, TcpIp tcpIp)
        {
            Console.WriteLine(message);
            tcpIp.Close();
        }

    }
}