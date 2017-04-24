using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstructurasDeDatos;
namespace Laboratorio_3
{
    public class TreeElementFactory:IFixedLengthFactory<TreeElement>
    {
        public TreeElement Create(string FixedLength)
        {
            TreeElement nl = new TreeElement();
            nl.Number = Convert.ToInt32(FixedLength.Substring(0, FixedLength.Length-1));
            return nl;
        }
        public TreeElement CreateNull()
        {
            return new TreeElement();
        }
    }
}
