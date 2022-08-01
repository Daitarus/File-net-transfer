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
        public byte[] ReadFile(string _fileName)
        {
            byte[] buffer;
            try
            {
                using (FileStream fstream = File.OpenRead(_fileName))
                {
                    buffer = new byte[fstream.Length];
                    fstream.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            catch
            {
                return buffer = new byte[0];
            }
        }
    }
}
