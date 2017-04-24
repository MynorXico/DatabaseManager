using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MicroSQL
{
    static class FileManagment
    {
        public static string[] OpenFile(string FilePath)
        {
            return File.ReadAllLines(FilePath);
        }
    }
}

