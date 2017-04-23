using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace EstructurasDeDatos
{
    public static class FileManagment
    {
        public static void WriteLine(FileStream File, int LineNumber, int Content)
        {
            File.Seek(LineNumber*(11 * Encoding.UTF8.GetByteCount("1") + Encoding.UTF8.GetByteCount(Environment.NewLine)), SeekOrigin.Begin);
            byte[] array = Encoding.UTF8.GetBytes(Content.ToString().PadLeft(11, '0') + Environment.NewLine);
            int offset = 0;
            int count = array.Length;
            File.Write(array, offset, count);
        }
        public static int ReadLine(FileStream File, int LineNumber)
        {
            try
            {
                File.Seek(LineNumber * (11 * Encoding.UTF8.GetByteCount("1") + Encoding.UTF8.GetByteCount(Environment.NewLine)), SeekOrigin.Begin);
                byte[] array = new byte[(11 * Encoding.UTF8.GetByteCount("1") + Encoding.UTF8.GetByteCount(Environment.NewLine))];
                int offset = 0;
                int count = array.Length;
                File.Read(array, offset, count);
                string Content = Encoding.UTF8.GetString(array);
                return int.Parse(Content);
            }catch(Exception e)
            {
                return int.MinValue;
            }           
        }
    }
}
