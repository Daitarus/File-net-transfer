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
            //для дебага
            //args = new string[3];
            //args[0] = "127.0.0.1";
            //args[1] = "4000";
            //args[2] = @"Data\Files\";

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
            }
            catch
            {
                Console.WriteLine("Ошибка: Параметры введены неверно !!!");
                return -1;
            }
            //старт работы сервера
            ConsoleUpdate();
            strCon[0] = "Сервер запущен...";
            NetWork(args[2]);
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
       
        static async void NetWork(string temp)
        {
            string message, filename; 
            byte[] textMess; 
            int num = 0; int counter = 0; 
            byte[][] textBlock=new byte[1][];
            FileWork fileWork = new FileWork();
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

                    //получение сообщения по UDP
                    textMess = null; num = 1; counter = 0;
                    while (counter < num)
                    {
                        tcpIp.ReadUdp(out textMess);
                        if (textMess != null)
                        {
                            if (num == 1)
                            {
                                //получение колличества датаграмм из первой
                                num = (int)textMess[1];
                                textBlock = new byte[num][];
                            }
                            textBlock[counter] = textMess;
                            //отправляем сообщение о получении сообщения
                            tcpIp.SendMessageTcp("ok");
                            counter++;
                            strCon[3] = $"Полученно данных {counter}/{num} ...";
                        }
                    }
                    //формирование единого текста и удаление служебной информации
                    textMess = fileWork.UnionBlock(textBlock);
                    //создание и запись файла
                    //создание директории
                    if (fileWork.CreateDir(temp))
                    {
                        strCon[2] = "Директория создана...";
                    }
                    else
                    {
                        strCon[2] = "Ошибка: Директория не может быть создана по указанному пути !!!";
                        break;
                    }
                    //создание файла
                    if (fileWork.WriteFile(temp, filename, textMess))
                    {
                        strCon[2] = "Файл создан...";
                    }
                    else
                    {
                        strCon[2] = "Ошибка: Файл не может быть создан по указанному пути !!!";
                        break;
                    }
                    strCon[1] = "Клиент отключился...";
                    tcpIp.Close();
                }
            });
        }

        //обновление текста консоли
        static async void ConsoleUpdate()
        {
            bool flagCon = true;
            string[] strConBuf = new string[strCon.Length];
            await Task.Run(async () =>
            {
                while (flag)
                {
                    //проверка на отличие от текущего
                    for(int i = 0; i < strCon.Length; i++)
                    {
                        if (strConBuf[i] != strCon[i])
                        {
                            flagCon = true;
                            break;
                        }
                    }
                    //обновление текста
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