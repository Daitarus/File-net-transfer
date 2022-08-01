using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpIpLib;

namespace Client
{
    public class Program
    {
        static bool flagConUp = true;
        static bool flag = false;
        static string[] strCon = new string[4];

        static int Main(string[] args)
        {
            args = new string[5];
            args[0] = "127.0.0.1";
            args[1] = "4000";
            args[2] = "5000";
            args[3] = "test.txt";
            args[4] = "1000";
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
            //проверка и считывание файла
            FileWork fileWork = new FileWork();
            byte[] FileText = fileWork.ReadFile(filename);
            //старт работы клиента
            Task consoleUpdate = ConsoleUpdate();
            //подключение TCP
            if (tcpIp.Connection())
            {
                strCon[0] = "Подключено...";
            }
            else
            {
                strCon[0] = "Ошибка подключения !!!";
                tcpIp.Close();
                return -1;
            }
            //отправка имени файла и порта для udp
            if (tcpIp.SendMessageTcp($"{filename}:{tcpIp.port_udp}"))
            {
                strCon[1] = "Имя файла и номер порта для UDP отправленны...";
            }
            else
            {
                strCon[1] = "Ошибка: Имя файла и номер порта для UDP не отправленны, возможно отсутствует соединение !!!";
                tcpIp.Close();
                return -1;
            }

            //udp
            string answer; string message;
            Task sendMess;
            do
            {
                strCon[3] = "Введите сообщение: ";
                message = Console.ReadLine();
                flag = true;
                answer = "";
                sendMess = SendMess(message, time, tcpIp);
                if (tcpIp.ReadTcp(out answer))
                {
                    if (answer == "ok")
                    {
                        flag = false;
                    }
                }
                Task.WaitAll(sendMess);
            } while (message != "exit");

            //конец работы клиента
            tcpIp.Close();
            strCon[0] = "Отключено...";
            strCon[1] = "Передача завершена...";
            strCon[2] = "";
            strCon[3] = "Конец работы программы!";
            flagConUp = false;
            Task.WhenAll(consoleUpdate);
            return 0;
        }

        static async Task SendMess(string message, int time, TcpIp tcpIp)
        {
            await Task.Run(async () =>
            {
                while (flag)
                {
                    //отправка сообщения udp
                    if (tcpIp.SendMessageUdp(message))
                    {
                        strCon[2] = $"Сообщение {message} отправленно...";
                    }
                    else
                    {
                        strCon[2] = "Ошибка: Отсутствует соединение !!!";
                        flag = false;
                    }
                    Thread.Sleep(time);
                }
            });
        }

        static async Task ConsoleUpdate()
        {
            bool flagCon = true;
            string[] strConBuf = new string[strCon.Length];
            await Task.Run(async () =>
            {
                while (flagConUp)
                {
                    for (int i = 0; i < strCon.Length; i++)
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
                        for (int i = 0; i < strCon.Length; i++)
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