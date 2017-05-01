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
        public ID id = new ID();

        public static string DataSeparator = "/";

        public int[] Enteros = new int[4];
        public string[] VarChars = new string[4];
        public string[] DateTimes = new string[4];

        public string[] EnterosColumnName;
        public string[] VarCharsColumnName;
        public string[] DateTimesColumnName;

        /// <summary>
        /// Constuctor de TableElement nulo.
        /// </summary>
        public TableElement()
        {
            id.id = Utilities.NullInt;
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
                Sum += 1 * Utilities.IntegerSize + 1;   // ID (+1 separador)
                Sum += 4 * Utilities.IntegerSize + 1;       // 4 Enteros
                Sum += 4 * Utilities.VarCharSize + 1;       // 4 VarChars
                Sum += 4 * Utilities.DateTimeSize + 2;      // 4 DateTimes

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
            Output.Append(Utilities.FormatInteger(id.id));
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
