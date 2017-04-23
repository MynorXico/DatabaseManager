using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstructurasDeDatos;

namespace MicroSQL
{
    class TableElementFactory : IFixedLengthFactory<TableElement>
    {
        public TableElement Create(string s)
        {
            string[] Data = s.Split(Utilities.DataSeparator[0]);
            TableElement Output = new TableElement();
            for (int i = 0; i < 4; i++)
            {
                Output.Enteros[i] = int.Parse(Data[i]);
                Output.VarChars[i] = Data[i + 4];
                Output.DateTimes[i] = Data[i + 8];
            }
            return Output;
        }

        public TableElement CreateNull()
        {
            return new TableElement();
        }
    }
}
