using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpIpLib;

namespace Server
{
    internal class Program
    {
        static bool flag = true;
        static string[] strCon = new string[5];
        static TcpIp tcpIp;

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
            tcpIp = new TcpIp();
            //преобразование параметров в нужный тип
            try
            {
                tcpIp.ip = IPAddress.Parse(args[0]);
                tcpIp.port_tcp = Convert.ToInt32(args[1]);
                //temp = args[2];
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            //старт работы сервера
            ConsoleUpdate();
            strCon[0] = "Сервер запущен...";
            NetWork();
            //команда на завершение работы
            string com = "";
            while (com!="exit")
            {
                strCon[4] = "Если вы хотите завершить работу сервера - введите exit: ";
                com = Console.ReadLine();
                if (com == "exit")
                {
                    strCon[4] = "Вы уверены, что хотите завершить работу сервера? (yes/no): ";
                    com = Console.ReadLine();
                    if((string.Equals(com, "yes", StringComparison.OrdinalIgnoreCase))||(string.Equals(com, "y", StringComparison.OrdinalIgnoreCase)))
                    {
                        com = "exit";
                    }
                }
            }
            flag = false;
            return 0;
        }      
       
        static async void NetWork()
        {
            string message, filename;
            bool flagConnection = false;
            await Task.Run(async () =>
            {
                while(flag)
                {
                    //tcp прослушивание
                    if (tcpIp.GetConnection())
                    {
                        strCon[1] = "Новое подключение...";
                    }
                    else
                    {
                        strCon[2] = "Ошибка подключения !!!";
                        tcpIp.Close();
                        break;
                    }
                    //получение имени файла и номера порта для udp
                    if (tcpIp.ReadTcp(out message))
                    {
                        try
                        {
                            filename = message.Substring(0, message.IndexOf(":"));
                            tcpIp.port_udp = Convert.ToInt32(message.Substring(message.IndexOf(":") + 1, message.Length - message.IndexOf(":") - 1));
                            flagConnection = true;
                            strCon[2] = "Имя файла и данные о UDP подключении получены...";

                        }
                        catch
                        {
                            strCon[2] = "Ошибка: Данные о UDP подключении переданны не верно !!!";
                            tcpIp.Close();
                            break;
                        }
                    }
                    else
                    {
                        strCon[2] = "Ошибка: Отсутствует соединение !!!";
                        tcpIp.Close();
                        break;
                    }
                    ////создание директории
                    //FileWork fileWork = new FileWork();
                    //if (fileWork.CreateDir(temp))
                    //{
                    //    Console.WriteLine("Директория создана...");
                    //}
                    //else
                    //{
                    //    Close("Директория не может быть создана по указанному пути", tcpIp);
                    //    break;
                    //}
                    ////создание файла
                    //if (fileWork.CreateFile(temp, filename))
                    //{
                    //    Console.WriteLine("Файл создан...");
                    //}
                    //else
                    //{
                    //    Close("Файл не может быть создан по указанному пути", tcpIp);
                    //    break;
                    //}

                    //получение сообщения по UDP
                    message = "";
                    while(flagConnection)
                    {
                        tcpIp.ReadUdp(out message);
                        if (message != "")
                        {
                            if (message == "exit")
                            {
                                flagConnection = false;

                            }
                            else
                            {
                                strCon[3] = message;                              
                            }
                            message = "";
                            //отправляем сообщение о получении сообщения
                            if (tcpIp.SendMessageTcp("ok"))
                            {
                                strCon[2] = "Подтверждение отправленно...";
                            }
                        }
                    }
                    strCon[1] = "Клиент отключился...";
                    strCon[2] = "";
                    tcpIp.Close();
                }
            });
        }

        static async void ConsoleUpdate()
        {
            bool flagCon = true;
            string[] strConBuf = new string[strCon.Length];
            await Task.Run(async () =>
            {
                while (flag)
                {
                    for(int i = 0; i < strCon.Length; i++)
                    {
                        if (strConBuf[i] != strCon[i])
                        {
                            flagCon = true;
                            break;
                        }
                    }
                    if (flagCon)
                    {
                        flagCon = false;
                        Console.Clear();
                        for(int i = 0; i < strCon.Length; i++)
                        {
                            strConBuf[i] = strCon[i];
                            Console.WriteLine(strCon[i]);
                        }
                    }
                }
            });
        }
    }
}