namespace EstructurasDeDatos
{
    public interface IFixedLengthFactory<T> where T : IFixedLength
    {
        T CreateNull();
        T Create(string s);

    }
}
