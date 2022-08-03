using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpIpLib;

namespace Client
{
    public class Program
    {
        static bool flag = false;

        static int Main(string[] args)
        {
            //для дебага
            //args = new string[5];
            //args[0] = "127.0.0.1";
            //args[1] = "4000";
            //args[2] = "5000";
            //args[3] = "Client1.exe";
            //args[4] = "1000";

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
            byte[] fileText = fileWork.ReadFile(filename);
            if(fileText == null)
            {
                Console.WriteLine("Ошибка: Файл не был прочитан !!!");
                return -1;
            }
            byte[][] fileBlock = fileWork.SplitByte(tcpIp.mtu, fileText);

            //старт работы клиента
            //подключение TCP
            if (tcpIp.Connection())
            {
                Console.WriteLine("Подключено...");
            }
            else
            {
                Console.WriteLine("Ошибка подключения !!!");
                tcpIp.Close();
                return -1;
            }
            //отправка имени файла и порта для udp
            if (tcpIp.SendMessageTcp($"{filename}:{tcpIp.port_udp}"))
            {
                Console.WriteLine("Имя файла и номер порта для UDP отправленны...");
            }
            else
            {
                Console.WriteLine("Ошибка: Имя файла и номер порта для UDP не отправленны, возможно отсутствует соединение !!!");
                tcpIp.Close();
                return -1;
            }

            //udp
            string answer;
            Task sendMess;
            Console.WriteLine("Передача файла...");
            for (int i=0;i<fileBlock.Length;i++)
            {
                flag = true;
                answer = "";
                sendMess = SendMess(fileBlock[i], time, tcpIp);
                if (tcpIp.ReadTcp(out answer))
                {
                    if (answer == "ok")
                    {
                        flag = false;
                    }
                }
                Task.WaitAll(sendMess);
            }
            //завершение передачи
            Console.WriteLine("Передача завершена...");
            Console.WriteLine("Работа завершена !");
            //конец работы клиента
            tcpIp.Close();
            return 0;
        }

        static async Task SendMess(byte[] message, int time, TcpIp tcpIp)
        {
            await Task.Run(async () =>
            {
                while (flag)
                {
                    //отправка сообщения udp
                    if (!tcpIp.SendMessageUdp(message))
                    {
                        Console.WriteLine("Ошибка: Отсутствует соединение !!!");
                        flag = false;
                    }
                    Thread.Sleep(time);
                }
            });
        }
    }
}