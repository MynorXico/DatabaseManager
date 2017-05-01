using EstructurasDeDatos;

namespace MicroSQL
{
    public class IDFactory : IFixedLengthFactory<ID>
    {
        public ID Create(string s)
        {
            ID id = new ID();
            id.id = int.Parse(s.Trim('\''));
            return id;
        }

        public ID CreateNull()
        {
            return new MicroSQL.ID();
        }
    }
}
