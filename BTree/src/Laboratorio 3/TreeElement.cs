using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstructurasDeDatos;
namespace Laboratorio_3
{
    public class TreeElement : IFixedLength, IComparable
    {
        // Solo es ejemplo de como hacer los objetos para utilizarlos con este árbolB
        private const string Format = "000000000000";
        public int Number { get; set; }

        public TreeElement()
        {
            Number = 0;
        }
        public int Length
        {
            get
            {
                return 12;
            }
        }

        public string FixedSize()
        {
            return Number.ToString(Format);
        }

        public int CompareTo(object obj)
        {
            return string.Compare(FixedSize(), ((TreeElement)(obj)).FixedSize());
        }
    }
}
