using System;
using System.Text;
namespace EstructurasDeDatos
{
    public class SizesNSpecialCharacters
    {
        public const int ReadablePointerSize = 11;
        public const string GroupSeparator = "|";
        public const int NullPointer = int.MinValue;
        public static int HeaderSizeInBytes = 3 * (ReadablePointerSize * Encoding.UTF8.GetByteCount("1") + Encoding.UTF8.GetByteCount(Environment.NewLine));
    }
}
