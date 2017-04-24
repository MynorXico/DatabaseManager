using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstructurasDeDatos;

namespace MicroSQL
{
    public class IDFactory : IFixedLengthFactory<ID>
    {
        public ID Create(string s)
        {
            ID id = new ID();
            id.id = int.Parse(s);
            return id;
        }

        public ID CreateNull()
        {
            return new MicroSQL.ID();
        }
    }
}
