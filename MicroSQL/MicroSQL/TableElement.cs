using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstructurasDeDatos;

namespace MicroSQL
{
    class TableElement : IFixedLength, IAutoFormattable
    {
        public int[] Enteros = new int[4];
        public string[] VarChars = new string[4];
        public string[] DateTimes = new string[4];

        // IFixedLength
        public int Length
        {
            get
            {
                int Sum = 0;
                Sum += 4 * Utilities.IntegerSize;
                Sum += 4 * Utilities.VarCharSize;
                Sum += 4 * Utilities.DateTimeSize;

                return Sum;
            }
        }
        public string FixedSize()
        {
            return this.Format();
        }

        // IAutoFormattable
        public string Format()
        {
            StringBuilder Output = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                Output.Append(Utilities.FormatInteger(Enteros[i]));
                Output.Append(Utilities.DataSeparator);
            }
            for (int i = 0; i < 4; i++)
            {
                Output.Append(Utilities.FormatVarChar(VarChars[i]));
                Output.Append(Utilities.DataSeparator);
            }
            for (int i = 0; i < 4; i++)
            {
                Output.Append(Utilities.FormatDate(DateTimes[i]));
                Output.Append(Utilities.DataSeparator);
            }

            return Output.ToString();
        }
    }
}
