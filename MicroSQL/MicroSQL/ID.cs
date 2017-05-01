using System;
using EstructurasDeDatos;

namespace MicroSQL
{
    public class ID : IFixedLength, IComparable, IAutoFormattable
    {
        public int id;

        public ID()
        {
            id = int.MinValue;
        }

        public int Length
        {
            get
            {
                return Utilities.IntegerSize;
            }
        }

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
        public int CompareTo(int i)
        {
            if (id < i)
                return -1;
            else if (id > i)
                return 1;
            else
                return 0;
        }

        public string FixedSize()
        {
            return Format();
        }

        public string Format()
        {
            return id.ToString().PadLeft(Length, '0');
        }
    }
}
