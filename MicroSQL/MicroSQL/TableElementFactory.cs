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
        /// <summary>
        /// Crea un TableElementFactory en base a una cadena con formato predeterminado
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public TableElement Create(string s)
        {
            string[] Data = s.Split(TableElement.DataSeparator[0]);
            TableElement Output = new TableElement();

            ID id = new ID();
            id.id = int.Parse(Data[0]);
            Output.id = id;

            for (int i = 0; i < 3; i++)
            {
                Output.Enteros[i] = int.Parse(Data[i+1]);
                Output.VarChars[i] = Data[i + 4];
                Output.DateTimes[i] = Data[i + 7];
            }
            return Output;
        }
        /// <summary>
        /// Crea un TableElement con propieades vacías
        /// </summary>
        /// <returns></returns>
        public TableElement CreateNull()
        {
            return new TableElement();
        }
    }
}
