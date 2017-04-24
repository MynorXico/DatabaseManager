using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstructurasDeDatos;

namespace MicroSQL
{
    public class TableElementFactory : IFixedLengthFactory<TableElement>
    {
        public TableElement Create(string s)
        {
            string[] Data = s.Split(TableElement.DataSeparator[0]);
            TableElement Output = new TableElement();

            ID id = new ID();
            id.id = int.Parse(Data[0]);
            Output.id = id;

            for (int i = 0; i < 4; i++)
            {
                Output.Enteros[i+1] = int.Parse(Data[i+1]);
                Output.VarChars[i+1] = Data[i +1 + 4];
                Output.DateTimes[i+1] = Data[i + 1 + 8];
            }
            return Output;
        }

        public TableElement CreateNull()
        {
            return new TableElement();
        }
    }
}
