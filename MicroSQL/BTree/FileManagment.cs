using System;
using System.Text;
using System.IO;

namespace EstructurasDeDatos
{
    public static class FileManagment
    {
        /// <summary>
        /// Escritura de una línea Content en el número LineNumber
        /// </summary>
        /// <param name="File"></param>
        /// <param name="LineNumber"></param>
        /// <param name="Content"></param>
        public static void WriteLine(FileStream File, int LineNumber, int Content)
        {
            // Se obtiene el arreglo codificado
            byte[] array = Encoding.UTF8.GetBytes(Content.ToString().PadLeft(11, '0') + Environment.NewLine);
            int offset = 0;
            int count = array.Length;
            // Se busca la posición de insersión
            File.Seek(LineNumber * Encoding.UTF8.GetByteCount(SizesNSpecialCharacters.NullPointer + Environment.NewLine), SeekOrigin.Begin);
            File.Write(array, offset, count);
        }
        /// <summary>
        /// Lectura de una línea con número LineNumber
        /// </summary>
        /// <param name="File"></param>
        /// <param name="LineNumber"></param>
        /// <returns></returns>
        public static int ReadLine(FileStream File, int LineNumber)
        {
            try
            {
                // En caso de no poder hacerlo, asigna un apuntaor vacío
                File.Seek(LineNumber * (11 * Encoding.UTF8.GetByteCount("1") + Encoding.UTF8.GetByteCount(Environment.NewLine)), SeekOrigin.Begin);
                byte[] array = new byte[(11 * Encoding.UTF8.GetByteCount("1") + Encoding.UTF8.GetByteCount(Environment.NewLine))];
                int offset = 0;
                int count = array.Length;
                File.Read(array, offset, count);
                string Content = Encoding.UTF8.GetString(array);
                return int.Parse(Content);
            }
            catch (Exception e)
            {
                return int.MinValue;
            }
        }
    }
}
