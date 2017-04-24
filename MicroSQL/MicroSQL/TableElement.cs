using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstructurasDeDatos;

namespace MicroSQL
{
    public class TableElement : IFixedLength, IAutoFormattable
    {
        public ID id;

        public static string DataSeparator = "/";

        public int[] Enteros = new int[4];
        public string[] VarChars = new string[4];
        public string[] DateTimes = new string[4];

        /// <summary>
        /// Constuctor de TableElement nulo.
        /// </summary>
        public TableElement()
        {
            for (int i = 0; i < Enteros.Length; i++)
            {
                Enteros[i] = Utilities.NullInt;
            }
            for (int i = 0; i < VarChars.Length; i++)
            {
                VarChars[i] = Utilities.NullVarChar;
            }
            for (int i = 0; i < DateTimes.Length; i++)
            {
                DateTimes[i] = Utilities.NullDateTime;
            }
        }

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
            Output.Append(id);
            Output.Append(DataSeparator);
            for (int i = 0; i < 4; i++)
            {
                Output.Append(Utilities.FormatInteger(Enteros[i]));
                Output.Append(DataSeparator);
            }
            for (int i = 0; i < 4; i++)
            {
                Output.Append(Utilities.FormatVarChar(VarChars[i]));
                Output.Append(DataSeparator);
            }
            for (int i = 0; i < 4; i++)
            {
                Output.Append(Utilities.FormatDate(DateTimes[i]));
                Output.Append(DataSeparator);
            }

            return Output.ToString();
        }
    }
}
