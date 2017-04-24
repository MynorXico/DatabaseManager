using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
namespace EstructurasDeDatos
{
    public interface IFixedLength
    {
        string FixedSize();
        int Length
        {
            get;
        }
    }
}
