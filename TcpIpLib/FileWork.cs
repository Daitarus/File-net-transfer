using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TcpIpLib
{
    public class FileWork
    {

        // создание директории, если её не существует
        public bool CreateDir(string temp)
        {
            string[] subtemps = temp.Split(new char[] {'\\'});
            temp = "";
            try
            {
                foreach(string subtemp in subtemps)
                {
                    if(subtemp!="")
                    {
                        temp += subtemp + '\\';
                        if(!Directory.Exists(temp))
                        {
                            Directory.CreateDirectory(temp);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //создание файла
        public bool CreateFile(string temp, string fileName)
        {
            try
            {
                if (temp[temp.Length - 1] != '\\')
                {
                    temp += '\\';
                }
                File.Create(temp + fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //чтение файла и вывод в byte[]
        public byte[] ReadFile(string fileName)
        {
            byte[] buffer;
            try
            {
                buffer = File.ReadAllBytes(fileName);
                return buffer;
            }
            catch
            {
                return buffer = null;
            }
        }
        //запись в файл и создание его, если его нет
        public bool WriteFile(string temp, string fileName, byte[] text)
        {
            try
            {
                if (temp[temp.Length-1]!='\\')
                {
                    temp += '\\';
                }
                using (var fs = new FileStream(temp + fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(text, 0, text.Length);
                }
                return true;
            }
            catch { return false; }
        }

        //разделение единого текста файла на блоки для пересылки датаграммами
        public byte[][] SplitByte(int size, byte[] text)
        {
            //определяем колличество блоков
            size--;
            byte numBlock = (byte)Math.Ceiling((text.Length + 2) / Convert.ToDouble(size));
            //добавляем в текст служебную информацию: первый байт - колличество блоков, последний - конечный байт (255), который отделит 0 в блоке от
            //исходного текста файла
            text = AddInf(numBlock, (byte)255, text);
            size++; 
            //создаём двумерный массив для записи блоков
            byte[][] block = new byte[numBlock][];
            //цикл заполнения блоков
            for (int i=0;i < block.Length; i++)
            {
                block[i] = new byte[size];
                //0 байт блока - номер блока
                block[i][0] = (byte)i;
                //копируем данные текста в блок
                for(int j = 1; j < size; j++)
                {
                    if (i * (size-1) + j <= text.Length)
                    {
                        block[i][j] = text[i * (size-1) + j - 1];
                    }
                    //если текст кончился, а последний блок не дозаполнен, то цикл завершается и по умолчанию в блоке 0 после конечного байта
                    else
                    {
                        break;
                    }
                }
            }
            return block;
        }
        //добавление служебной информации в текст
        public byte[] AddInf(byte begin, byte end, byte[] text)
        {
            //создаём новый текст с 2 дополнительными байтами
            byte[] newText = new byte[text.Length+2];
            //в 0 байт добавляем общее колличество блоков
            newText[0] = begin;
            //копируем исходный текст в новый с сдвигом
            for (int i=0;i<text.Length;i++)
            {
                newText[i + 1] = text[i];
            }
            //в конец добавляем конечный байт
            newText[text.Length+1] = end;
            return newText;
        }

        //Удаление служебной информации из текста
        public byte[] DeleteInf(byte[] text)
        {
            //создаём счётчик для колличества 0 в конце текста до конечного байта
            int counter = 0;
            for (int i = text.Length - 1; i >= 0; i--) 
            {
                if(text[i] == 0)
                {
                    counter++;
                }
                if(text[i] == 255)
                {
                    counter++;
                    break;
                }
            }
            //создаём нвоый текст
            byte[] newText = new byte[text.Length - counter - 1];
            //циклом заполняем его кроме первого байта (колличество блоков) и последнего (конечный байт)
            for(int i = 1; i <= text.Length - counter - 1 ; i++)
            {
                newText[i-1] = text[i];
            }
            //в итоге получаем исходный текст
            return newText;
        }
        //объединение блоков текста в единый
        public byte[] UnionBlock(byte[][] block)
        {
            //создаём единый текст
            byte[] text = new byte[(block[0].Length - 1) * block.Length];
            //циклом заполняем его всеми блоками, но не передаём первый байт блока (номер/id блока)
            for(int i=0;i<block.Length;i++)
            {
                for (int j = 0; j < block[i].Length-1; j++)
                {
                    text[i * (block[i].Length-1) + j] = block[i][j + 1];
                }
            }        
            //так же удаляем служебную информацию
            return DeleteInf(text);
        }

    }
}
