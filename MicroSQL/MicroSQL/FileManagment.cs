using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MicroSQL
{
    /// <summary>
    /// Clase encargada del manejo de archivos
    /// </summary>
    static class FileManagment
    {
        /// <summary>
        /// Devuelve un arreglo con las lineas del archivo con ruta FilePath
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string[] OpenFile(string FilePath)
        {
            return File.ReadAllLines(FilePath);
        }
    }
}

