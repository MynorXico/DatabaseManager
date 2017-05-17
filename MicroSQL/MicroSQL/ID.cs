using System;
using EstructurasDeDatos;

namespace MicroSQL
{
    /// <summary>
    /// Identificador del TableElement
    /// </summary>
    public class ID : IFixedLength, IComparable, IAutoFormattable
    {
        // identificador de la clase
        public int id;
        /// <summary>
        /// Constructor de un ID nulo
        /// </summary>
        public ID()
        {
            id = int.MinValue;
        }
        /// <summary>
        /// Tamaño en caracteres del objeto ID
        /// </summary>
        public int Length
        {
            get
            {
                return Utilities.IntegerSize;
            }
        }
        /// <summary>
        /// Comparación con otros objetos
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if ((id) < ((ID)(obj)).id)
            {
                return -1;
            }
            else if ((id) > ((ID)(obj)).id)
            {
                return 1;
            }else
            {
                return 0;
            }
        }
        /// <summary>
        /// Comparación de ID's con enteros
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int CompareTo(int i)
        {
            if (id < i)
                return -1;
            else if (id > i)
                return 1;
            else
                return 0;
        }
        /// <summary>
        /// Texto formateado para escribir en disco
        /// </summary>
        /// <returns></returns>
        public string FixedSize()
        {
            return Format();
        }
        /// <summary>
        /// Texto formateado listo para ser escrito en el archivo
        /// </summary>
        /// <returns></returns>
        public string Format()
        {
            return id.ToString().PadLeft(Length, '0');
        }
    }
}
