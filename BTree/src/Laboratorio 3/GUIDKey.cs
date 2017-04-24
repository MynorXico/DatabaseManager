using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstructurasDeDatos;
namespace Laboratorio_3
{
    public class GUIDKey : IFixedLength, IComparable, IAutoFormattable
    {
        public string Id;
        public GUIDKey()
        {
            Id = Guid.NewGuid().ToString();
        }
        public int Length
        {
            get
            {
                return 36;
            }
        }

        public int CompareTo(object obj)
        {
            if (string.Compare(Id, ((GUIDKey)(obj)).Id) > 0)
            {
                return 1;
            }
            else if (string.Compare(Id, ((GUIDKey)(obj)).Id) < 0)
            {
                return -1;
            }
            else {
                return 0;
            };
        }

        public string FixedSize()
        {
            return Id;
        }

        public string Format()
        {
            return Id;
        }

       
    }
}
