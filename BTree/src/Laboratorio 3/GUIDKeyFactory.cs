using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstructurasDeDatos;
namespace Laboratorio_3
{
    public class GUIDKeyFactory : IFixedLengthFactory<GUIDKey>
    {
        public GUIDKey Create(string s)
        {
            GUIDKey Key = new GUIDKey();
            Key.Id = s;
            return Key;
        }

        public GUIDKey CreateNull()
        {
            GUIDKey Key = new GUIDKey();
            Key.Id = "000000000000000000000000000000000000";
            return Key;
        }
    }
}
