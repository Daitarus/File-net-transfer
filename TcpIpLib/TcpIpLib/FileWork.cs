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
        private string fileName;
        private string temp;

        public FileWork() { }
        public FileWork(string _temp, string _fileName)
        {
            this.temp = _temp;
            this.fileName = _fileName;
        }

        public bool CreateDir(string _temp)
        {
            string[] subtemps = _temp.Split(new char[] {'\\'});
            _temp = "";
            try
            {
                foreach(string subtemp in subtemps)
                {
                    if(subtemp!="")
                    {
                        _temp += subtemp + '\\';
                        if(!Directory.Exists(_temp))
                        {
                            Directory.CreateDirectory(_temp);
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
        public bool CreateFile()
        {
            try
            {
                File.Create(temp + fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CreateFile(string _temp, string _fileName)
        {
            try
            {
                File.Create(_temp + _fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
