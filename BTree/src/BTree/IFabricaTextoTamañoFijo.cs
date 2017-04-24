using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstructurasDeDatos
{
    public interface IFixedLengthFactory<T> where T:IFixedLength
    {
        T CreateNull();
        T Create(string s);

    }
}
