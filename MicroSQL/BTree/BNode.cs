using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EstructurasDeDatos
{
    /// <summary>
    /// Estructura que contiene información sobre un nodo de un BTree
    /// </summary>
    /// <typeparam name="T"> Tipo de dato que se almacenará. </typeparam>
    /// <typeparam name="TKey"> Tipo de dato de la llave a almacenar</typeparam>
    public class BNode<T, TKey>
        where T : IFixedLength, IAutoFormattable
        where TKey : IComparable, IFixedLength, IAutoFormattable
    {
        //Fábricas necesarias para realizar la creación de nuevos nodos en base
        //a cadenas de texto o en su defecto, nulos.
        private IFixedLengthFactory<TKey> KeyFactory;
        private IFixedLengthFactory<T> ContentFactory;
        // Grado del Nodo
        private int Degree;
        // Linea que corresponde al nodo
        public int Pointer;
        // Linea del nodo del cual es hijo el nodo
        public int Parent;
        // Mitad del nodo
        private int Middle;
        // Número mínimo de datos permitidos en un nodo
        public int t;
        // Posiciones de los hijos del nodo
        public List<int> ChildrenPointers;
        // Llaves que almacena el nodo
        public List<TKey> Keys;
        // Valores que almacena el nodo
        public List<T> Values;
        // Calcula la cantidad de datos en un nodo
        public int AmountOfData
        {
            get
            {
                int i = 0;
                while (i < Keys.Count && Keys[i].CompareTo(KeyFactory.CreateNull()) != 0)
                {
                    i++;
                }
                return i;
            }
        }
        // Verifica si el nodo está o no lleno
        public bool Full
        {
            get
            {
                return (AmountOfData >= Degree - 1);
            }
        }
        // Verifica si el nodo es un hoja ono
        public bool Leaf
        {
            get
            {
                for (int i = 0; i < ChildrenPointers.Count; i++)
                {
                    if (ChildrenPointers[i] != SizesNSpecialCharacters.NullPointer)
                        // En caso de encontrar un apuntador que no sea vacío, 
                        // éste no es un nodo hoja
                        return false;
                }
                return true;
            }
        }
        // Tamaño que ocupa el nodo en Bytes
        private int SizeInBytes
        {
            get
            {
                int Counter = 0;
                Counter += (SizesNSpecialCharacters.ReadablePointerSize + 1)+2;
                Counter += ((SizesNSpecialCharacters.ReadablePointerSize + 1) * Degree) + 2;
                Counter += (KeyFactory.CreateNull().Format().Length + 1) * (Degree - 1);
                Counter += (Values[0].Format().Length + 1) * (Degree - 1); 
                Counter += Environment.NewLine.Length;
                return Counter * Encoding.UTF8.GetByteCount("1");
            }
        }
        // Posición exacta para acceso aleatorio
        private int DiskPosition()
        {
            return SizesNSpecialCharacters.HeaderSizeInBytes + (Pointer * SizeInBytes);
        }
        /// <summary>
        /// Devuelve el nodo con un formato adecuado para su posterior escritura a disco
        /// </summary>
        /// <returns></returns>
        private string FixedSize()
        {
            StringBuilder output = new StringBuilder();
            output.Append(Parent.ToString().PadLeft(11, '0'));
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            for (int i = 0; i < ChildrenPointers.Count; i++)
            {
                output.Append(ChildrenPointers[i].ToString().PadLeft(11, '0'));
                output.Append(SizesNSpecialCharacters.GroupSeparator);
            }
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            for (int i = 0; i < Keys.Count; i++)
            {
                output.Append(Keys[i].Format());
                output.Append(SizesNSpecialCharacters.GroupSeparator);
            }
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            output.Append(SizesNSpecialCharacters.GroupSeparator);
            for (int i = 0; i < Values.Count; i++)
            {
                output.Append(Values[i].FixedSize().Replace(SizesNSpecialCharacters.GroupSeparator, "#"));
                output.Append(SizesNSpecialCharacters.GroupSeparator);
            }
            output.Append(Environment.NewLine);
            return output.ToString();
        }
        /// <summary>
        /// Limpia el nodo llenándolo de llaves y contenido nulo
        /// </summary>
        /// <param name="ContentFactory"> Fábrica de contenido </param>
        /// <param name="KeyFactory">Fábrica de llaves </param>
        private void Clear(IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            // Listas con nuevas llaves. valores y apuntadores a hijos
            Keys = new List<TKey>();
            Values = new List<T>();
            ChildrenPointers = new List<int>();
            // Llena las llaves y valores
            for (int i = 0; i < Degree - 1; i++)
            {
                Keys.Add(KeyFactory.CreateNull());
                Values.Add(ContentFactory.CreateNull());
            }
            // Llena la lista de apuntadores a hijos            
            for (int i = 0; i < Degree; i++)
            {
                ChildrenPointers.Add(SizesNSpecialCharacters.NullPointer);
            }
        }
        /// <summary>
        /// Constructor del nodo
        /// </summary>
        /// <param name="Degree"> Grado del Nodo </param>
        /// <param name="Pointer"> Número de linea en archivo </param>
        /// <param name="Parent"> Número de línea del padres </param>
        /// <param name="ContentFactory"> Fábrica de contenido</param>
        /// <param name="KeyFactory"> Fábrica de Llaves</param>
        internal BNode(int Degree, int Pointer, int Parent, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            this.Degree = Degree;
            this.Pointer = Pointer;
            this.Parent = Parent;
            this.KeyFactory = KeyFactory;
            this.ContentFactory = ContentFactory;
            this.Middle = Degree / 2;
            this.t = (Degree / 2) - 1;
            // Se limpia el nodo
            Clear(ContentFactory, KeyFactory);
        }
        /// <summary>
        /// Lectura de archivo desde disco
        /// </summary>
        /// <param name="File"> FileStream para escritua</param>
        /// <param name="Degreee"> Grado del nodo </param>
        /// <param name="Pointer">´Número de línea en el archivo </param>
        /// <param name="ContentFactory"> Fábrica de contenido</param>
        /// <param name="KeyFactory"> Fábrica de Llaves</param>
        /// <returns></returns>
        public static BNode<T, TKey> DiskRead(FileStream File, int Degreee, int Pointer, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            // Se crea un nuevo nodo
            BNode<T, TKey> NewNode = new BNode<T, TKey>(Degreee, Pointer, 0, ContentFactory, KeyFactory);
            // Arreglo en el que se almacena la información codificada 
            byte[] BytesArray = new byte[NewNode.SizeInBytes];
            int FieldsIndex = 0;
            // Se ubica el seek del FileStream en la posición en la que se encuentra
            File.Seek(NewNode.DiskPosition(), SeekOrigin.Begin);
            // Se obtiene la información del archivo
            File.Read(BytesArray, 0, NewNode.SizeInBytes);
            // Se separa la información según el separador escogido
            string[] Fields = Encoding.UTF8.GetString(BytesArray).Replace(Environment.NewLine, "").Replace("".PadRight(3, SizesNSpecialCharacters.GroupSeparator[0]), SizesNSpecialCharacters.GroupSeparator.ToString()).Split(SizesNSpecialCharacters.GroupSeparator[0]);
            // Se obtiene el padres
            NewNode.Parent = int.Parse(Fields[FieldsIndex]);
            FieldsIndex++;
            // Se obtienen las líneas de los nodos hijos
            for (int i = 0; i < NewNode.ChildrenPointers.Count; i++)
            {
                NewNode.ChildrenPointers[i] = int.Parse(Fields[FieldsIndex]);
                FieldsIndex++;
            }
            // Se obtienen las llaves que contiene el nodo
            for (int i = 0; i < NewNode.Keys.Count; i++)
            {
                NewNode.Keys[i] = KeyFactory.Create(Fields[FieldsIndex]);
                FieldsIndex++;
            }
            // Se obtiene y fabrica el contenido que tiene el nodo
            for (int i = 0; i < NewNode.Values.Count; i++)
            {
                NewNode.Values[i] = ContentFactory.Create(Fields[FieldsIndex]);
                FieldsIndex++;
            }
            // Nodo con información cargada
            return NewNode;
        }
        /// <summary>
        /// Escritura de Nodo a Disco
        /// </summary>
        /// <param name="File"> FileStream para realizar la escritura </param>
        internal void DiskWrite(FileStream File)
        {
            // Arreglo con bytes a guardar en el archivo
            byte[] datosBinarios = Encoding.UTF8.GetBytes(FixedSize());
            // Se ubica la posición en la que se debe escribir
            File.Seek(DiskPosition(), SeekOrigin.Begin);
            // Se escribe el arreglo codificado
            File.Write(datosBinarios, 0, SizeInBytes);
            File.Flush();
        }
        /// <summary>
        /// Posición aproximada del nodo que contien la llave
        /// </summary>
        /// <param name="Key"> Llave de búsqueda </param>
        /// <returns></returns>
        public int FirstPointerOfNodeContaining(TKey Key)
        {
            int Position = Keys.Count;
            for (int i = 0; i < Keys.Count; i++)
            {
                if ((Keys[i].CompareTo(Key) > 0) || (Keys[i].CompareTo(KeyFactory.CreateNull()) == 0))
                {
                    Position = i;
                    break;
                }
            }
            return Position;
        }
        /// <summary>
        /// Elimina un eleneto con determinada clave de un subárbol con raíz en este nodo
        /// </summary>
        /// <param name="Stream">FileStream</param>
        /// <param name="Key"> Llave de dato que se insertará </param>
        public void Delete(FileStream Stream, TKey Key)
        {
            if (Leaf)
            {
                DeleteFromLeaf(Stream, Key);
            }
            else
            {
                // Se determina si la llave se encuentra en el nodo
                int i = 0;
                while (i < AmountOfData && Keys[i].CompareTo(Key) < 0)
                {
                    i++;
                }
                if (i < AmountOfData && Keys[i].CompareTo(Key) == 0)
                {
                    // La llave se encuentra dentro del nodo, así que se borra
                    DeleteFromInternalNode(Stream, i);
                }
                else
                {
                    // Si se encuentra en un subárbol con raíz en este nodo
                    // entonces se encuentra en el hijo i
                    BNode<T, TKey> Child = DiskRead(Stream, Degree, ChildrenPointers[i], ContentFactory, KeyFactory);
                    // Se asegura que el nodo no quede en underflow
                    EnsureFullEnough(Stream, i);
                    // Se procede con la recursión
                    Child.Delete(Stream, Key);
                }
            }
        }
        /// <summary>
        /// Asegura que un hijo de este hijo tiene al menos (n-1)/2 llaves
        /// </summary>
        /// <param name="File">FileSteam para lectura</param>
        /// <param name="i"> El i-ésimo hijo de este nodo es el que tiene al menos
        /// (n-1)/2 llaves /param>
        private void EnsureFullEnough(FileStream File, int i)
        {
            BNode<T, TKey> Child = DiskRead(File, Degree, ChildrenPointers[i], ContentFactory, KeyFactory);
            if (Child.AmountOfData < t)
            {
                BNode<T, TKey> LeftSibling; // Hermano izquierdo del hijo
                int LeftSiblingN;           // Cantidad de datos en el hhermano izquierdo

                if (i > 0)
                {
                    LeftSibling = DiskRead(File, Degree, ChildrenPointers[i - 1], ContentFactory, KeyFactory);
                    LeftSiblingN = LeftSibling.AmountOfData;
                }
                else
                {
                    LeftSibling = null;
                    LeftSiblingN = 0;
                }
                // Verifica si el hermano izquierdo tiene al menos (n-1)/2 llaves
                if (LeftSiblingN >= t)
                {
                    // Mueve todas las llaves del hijo y sus posiciones una posición
                    // a la derecha
                    for (int j = Child.AmountOfData - 1; j >= 0; j--)
                    {
                        Child.Values[j + 1] = Child.Values[j];
                        Child.Keys[j + 1] = Child.Keys[j];
                    }
                    if (!Child.Leaf)
                    {
                        for (int j = Child.AmountOfData; j >= 0; j--)
                        {
                            Child.ChildrenPointers[j + 1] = Child.ChildrenPointers[j];
                        }
                    }
                    // Baja una llave de este nodo a uno de los hijos
                    // y de la izquierda en el nodo (Rotación)
                    Child.Values[0] = Child.Values[i - 1];
                    Child.Keys[0] = Keys[i - 1];
                    Values[i - 1] = LeftSibling.Values[LeftSiblingN - 1];
                    Keys[i - 1] = LeftSibling.Keys[LeftSiblingN - 1];
                    LeftSibling.Values[LeftSiblingN - 1] = ContentFactory.CreateNull();
                    LeftSibling.Keys[LeftSiblingN - 1] = KeyFactory.CreateNull();
                    // Si nueve un puntero de la izquierda al hijo (Rotación)
                    if (!Child.Leaf)
                    {
                        Child.ChildrenPointers[0] = LeftSibling.ChildrenPointers[LeftSiblingN];
                        LeftSibling.ChildrenPointers[LeftSiblingN] = SizesNSpecialCharacters.NullPointer;
                    }
                    DiskWrite(File);
                    Child.DiskWrite(File);
                    LeftSibling.DiskWrite(File);
                }
                else
                {
                    // Se realiza lo mismo pero con el hermano derecho
                    // El proceso es simétrico
                    BNode<T, TKey> RightSibling; // Hermano derecho del hijo
                    int RightSiblingN;           // Cardinalidad del hermano derecho

                    if (i < AmountOfData)
                    {
                        RightSibling = DiskRead(File, Degree, ChildrenPointers[i + 1], ContentFactory, KeyFactory);
                        RightSiblingN = RightSibling.AmountOfData;
                    }
                    else
                    {
                        RightSibling = null;
                        RightSiblingN = 0;
                    }

                    if (RightSiblingN >= t)
                    {
                        // Se baja una llave del nodo al hijo y del hermano derecho
                        // a se mueve una llave a este nodo
                        Child.Values[Child.AmountOfData] = Values[i];
                        Child.Keys[Child.AmountOfData] = Keys[i];
                        Values[i] = RightSibling.Values[0];
                        Keys[i] = RightSibling.Keys[0];
                        // Si no es hoja, se mueve un puntero dl hermano drecho al hijo (Rotación)
                        if (!Child.Leaf)
                        {
                            Child.ChildrenPointers[Child.AmountOfData] = RightSibling.ChildrenPointers[0];
                        }
                        // Se mueven las llaves del hermano derecho ( y sus punteros) una posición a la izquierda
                        for (int j = 1; j < RightSiblingN; j++)
                        {
                            RightSibling.Values[j - 1] = RightSibling.Values[j];
                            RightSibling.Keys[j - 1] = RightSibling.Keys[j];
                        }
                        RightSibling.Values[RightSiblingN - 1] = ContentFactory.CreateNull();
                        RightSibling.Keys[RightSiblingN - 1] = KeyFactory.CreateNull();
                        if (!RightSibling.Leaf)
                        {
                            for (int j = 1; j <= RightSiblingN; j++)
                            {
                                RightSibling.ChildrenPointers[j - 1] = RightSibling.ChildrenPointers[j];
                            }
                            RightSibling.ChildrenPointers[RightSiblingN] = SizesNSpecialCharacters.NullPointer;
                        }
                        DiskWrite(File);
                        Child.DiskWrite(File);
                        RightSibling.DiskWrite(File);
                    }
                    else
                    {
                        // El hijo y los dos hermanos tienen (n-1)/2 llaves.
                        // Se mezcla uno de los hermanos dentro del hijo, bajando una llave de este nodo
                        // al hijo.
                        if (LeftSiblingN > 0)
                        {
                            // Se mezcla el hermano izquierdo con el hijo.
                            // Se comienza moviendo todo lo de la derecha del hijo (n-1)/2 posiciones
                            for (int j = Child.AmountOfData - 1; j >= 0; j--)
                            {
                                Child.Values[j + t] = Child.Values[j];
                                Child.Keys[j + t] = Child.Keys[j];
                            }
                            if (!Child.Leaf)
                            {
                                for (int j = Child.AmountOfData; j >= 0; j--)
                                {
                                    Child.ChildrenPointers[j + t] = Child.ChildrenPointers[j];
                                }
                            }
                            // Se toma todo el hermano izquierdo
                            for (int j = 0; j < LeftSiblingN; j++)
                            {
                                Child.Values[j] = LeftSibling.Values[j];
                                Child.Keys[j] = LeftSibling.Keys[j];
                                LeftSibling.Values[j] = ContentFactory.CreateNull();
                                LeftSibling.Keys[j] = KeyFactory.CreateNull();
                            }
                            if (!Child.Leaf)
                            {
                                for (int j = 0; j <= LeftSiblingN; j++)
                                {
                                    Child.ChildrenPointers[j] = LeftSibling.ChildrenPointers[j];
                                    LeftSibling.ChildrenPointers[j] = SizesNSpecialCharacters.NullPointer;
                                }
                            }

                            // Se baja una de las llaves al hijo
                            Child.Values[t - 1] = Values[i - 1];
                            Child.Keys[t - 1] = Keys[i - 1];

                            // Debido a que el nodo está perdiendo la llave de la posición i-1 
                            // y el puntero i-1, se mueve las llaves desde i hasta n-1 e hijos
                            // desde i hasta n hacia la izquierda una posición en este nodo.
                            for (int j = i; j < AmountOfData; j++)
                            {
                                Values[j - 1] = Values[j];
                                Keys[j - 1] = Keys[j];
                                ChildrenPointers[j - 1] = ChildrenPointers[j];
                            }
                            ChildrenPointers[AmountOfData - 1] = ChildrenPointers[AmountOfData];

                            Values[AmountOfData - 1] = ContentFactory.CreateNull();
                            ChildrenPointers[AmountOfData] = SizesNSpecialCharacters.NullPointer;
                            Keys[AmountOfData - 1] = KeyFactory.CreateNull();

                            LeftSibling.DiskWrite(File);
                            DiskWrite(File);
                            Child.DiskWrite(File);
                        }
                        else
                        {
                            // Mezcla el hermano derecho en el hijo.
                            // Comienza por tomar todo del hijo derecho
                            for (int j = 0; j < RightSiblingN; j++)
                            {
                                Child.Values[j + Child.AmountOfData + 1] = RightSibling.Values[j];
                                Child.Keys[j + Child.AmountOfData + 1] = RightSibling.Keys[j];
                                RightSibling.Values[j] = ContentFactory.CreateNull();
                                RightSibling.Keys[j] = KeyFactory.CreateNull();
                            }
                            if (!Child.Leaf)
                            {
                                for (int j = 0; j <= RightSiblingN; j++)
                                {
                                    Child.ChildrenPointers[j + Child.AmountOfData + 1] = RightSibling.ChildrenPointers[j];
                                    RightSibling.ChildrenPointers[j] = SizesNSpecialCharacters.NullPointer;
                                }
                            }
                            // Se baja una llave de este nodo a un hijo (Rotación)                            
                            Child.Values[t - 1] = Values[i];
                            Child.Keys[t - 1] = Keys[i];
                            // Debido a que el nodo pierde la llave de la posición i y el puntero
                            // hijo de la posición i, se mueven las llaves [(i+1),(n-1) y los hijos (i+1),n 
                            // hacia la izquierda una posición en este nodo
                            for (int j = i + 1; j < AmountOfData; j++)
                            {
                                Values[j - 1] = Values[j];
                                Keys[j - 1] = Keys[j];
                                ChildrenPointers[j] = ChildrenPointers[j + 1];

                            }
                            Values[AmountOfData - 1] = ContentFactory.CreateNull();
                            ChildrenPointers[AmountOfData] = SizesNSpecialCharacters.NullPointer;
                            Keys[AmountOfData - 1] = KeyFactory.CreateNull();

                            RightSibling.DiskWrite(File);
                            DiskWrite(File);
                            Child.DiskWrite(File);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Bora una determinada llave del nodo
        /// </summary>
        /// <param name="File">FileStream de lectura y escritura </param>
        /// <param name="i">La llave en la posición i se borra de este nodo</param>
        private void DeleteFromInternalNode(FileStream File, int i)
        {
            // Se carga la clave del i-ésimo hijo que precede a la llave
            TKey Key = Keys[i];
            T Value = Values[i];
            BNode<T, TKey> y = DiskRead(File, Degree, ChildrenPointers[i], ContentFactory, KeyFactory);
            // Se verifica que el nodo tenga por lo menos (n/2)-1 llaves
            if (y.AmountOfData >= t)
            {
                // Se encuentra el predecesor de la llave con subárbol en la raíz y.
                T PrimeValue;
                TKey KPrime = y.LeftGreatest(File, out PrimeValue);
                // Se elimina recursivamente la llave del predecesor
                y.Delete(File, KPrime);
                // Se realiza el intercambio
                Values[i] = PrimeValue;
                Keys[i] = KPrime;
            }
            else
            {
                // Se realiza lo mismo con el siguiente hijo
                // Ahora KPrime es el sucesor de la llave
                // El procedimiento es simétrico al anterior.
                BNode<T, TKey> z = DiskRead(File, Degree, ChildrenPointers[i + 1], ContentFactory, KeyFactory);

                if (z.AmountOfData >= (t))
                {
                    T PrimeValue;
                    TKey KPrime = z.RightLowest(File, out PrimeValue);
                    z.Delete(File, KPrime);
                    Values[i] = PrimeValue;
                    Keys[i] = KPrime;
                }
                else
                {
                    // El predecesor y el sucesor tienen (n/2)-1 llaves.
                    // Se debe mezclar el nodo con z dentro de y. Este nodo pierde
                    // los punteros a la llave y el puntero al sucesor y. 
                    // Se elimina recursivamente la llave del predecesor
                    y.Values[y.AmountOfData] = Value;
                    y.Keys[y.AmountOfData] = Key;
                    for (int j = 0; j < z.AmountOfData; j++)
                    {
                        y.Values[y.AmountOfData + j + 1] = z.Values[j];
                        y.Keys[y.AmountOfData + j + 1] = z.Keys[j];
                    }
                    // Si el predecesor y el sucesor no son hojas, se copian
                    // los punteros
                    if (!y.Leaf)
                    {
                        for (int j = 0; j <= z.AmountOfData; j++)
                        {
                            y.Values[y.AmountOfData + j + 1] = z.Values[j];
                            y.Keys[y.AmountOfData + j + 1] = z.Keys[j];
                        }
                    }
                    // Se borra la llave y el sucesor de este nodo
                    for (int j = i + 1; j < AmountOfData; j++)
                    {
                        Values[j - 1] = Values[j];
                        Keys[j - 1] = Keys[j];
                        ChildrenPointers[j] = ChildrenPointers[j + 1];
                    }
                    ChildrenPointers[AmountOfData] = SizesNSpecialCharacters.NullPointer;
                    Values[AmountOfData - 1] = ContentFactory.CreateNull();
                    Keys[AmountOfData - 1] = KeyFactory.CreateNull();

                    DiskWrite(File);
                    y.DiskWrite(File);
                    z.DiskWrite(File);
                    // Se elimina recursivamente del predecesor
                    y.Delete(File, Key);
                }
            }
        }
        /// <summary>
        /// Encuentra el elemento con la llave más a la izquierda
        /// del subárbol derecho. Toma siempre el hijo más izquierdo
        /// hasta que se topa con una hoja.
        /// </summary>
        /// <param name="File"> FileStream para lectura </param>
        /// <param name="Value"> Valor del elemento que será encontrado </param>
        /// <returns> Elemento con la llave más a la izquierda del subárbol
        /// derecho </returns>
        private TKey RightLowest(FileStream File, out T Value)
        {
            if (Leaf)
            {
                Value = Values[0];
                TKey output = Keys[0];
                return output;
            }
            else
            {
                return DiskRead(File, Degree, ChildrenPointers[0], ContentFactory, KeyFactory).RightLowest(File, out Value);
            }
        }
        /// <summary>
        /// Encuentra el elemento con la llave más a la derecha
        /// del subárbol izquierdo. Toma siempre el hijo más derecho
        /// hasta que se topa con una hoja.
        /// </summary>
        /// <param name="File"> FileStream para lectura </param>
        /// <param name="Value"> Valor del elemento que será encontrado </param>
        /// <returns> Elemento con la llave más a la derecha del subárbol
        /// izquierda </returns>
        private TKey LeftGreatest(FileStream File, out T Value)
        {
            TKey Output;
            if (Leaf)
            {
                Value = Values[AmountOfData - 1];
                Output = Keys[AmountOfData - 1];
                return Output;
            }
            else
            {

                return DiskRead(File, Degree, ChildrenPointers[AmountOfData], ContentFactory, KeyFactory).LeftGreatest(File, out Value);

            }
        }
        /// <summary>
        /// Elimina un elemento con determinada llave del nodo.
        /// No hace nada si no contiene la llave
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Key"></param>
        private void DeleteFromLeaf(FileStream File, TKey Key)
        {
            // Se determina si la llave se encuentra en este nodo
            int i = 0;
            while (i < AmountOfData && Keys[i].CompareTo(Key) < 0)
            {
                i++;
            }
            if (i < AmountOfData && Keys[i].CompareTo(Key) == 0)
            {
                // La llave se encuentra en este nodo y está en la posición
                // i. Así que se mueven todas llaves que se encuentran a la derecha
                // una posición hacia la izquierda
                for (int j = i + 1; j < AmountOfData; j++)
                {
                    Values[j - 1] = Values[j];
                    Keys[j - 1] = Keys[j];
                }
                // El valor y llave de la última posición se elimina
                Values[AmountOfData - 1] = ContentFactory.CreateNull();
                Keys[AmountOfData - 1] = KeyFactory.CreateNull();
            }
            DiskWrite(File);
        }
        /// <summary>
        /// Posición exacta de una llave dentro de un nodo
        /// </summary>
        /// <param name="Key"> Llave que se busca</param>
        /// <returns></returns>
        internal int PrecisePointer(TKey Key)
        {            
            int posicion = -1;
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Key.CompareTo(Keys[i]) == 0)
                {
                    posicion = i;
                    break;
                }
            }
            return posicion;
        }
        /// <summary>
        /// Se inserta un nodo cuando este no está vacío
        /// </summary>
        /// <param name="Key"> Llave</param>
        /// <param name="Value"> Valor</param>
        /// <param name="RightSiblingPointer"> Linea del hermano del nodo</param>
        internal void Insert(TKey Key, T Value, int RightSiblingPointer)
        {
            // Se inserta un nodo cuando el mismo no se encuentra aún vacío
            Insert(Key, Value, RightSiblingPointer, true);
        }
        /// <summary>
        /// Insersión de elemento
        /// </summary>
        /// <param name="Key"> Llave a agregar </param>
        /// <param name="Value"> Valor a agregar </param>
        /// <param name="RightSiblingPointer"> Linea del hermano derecho </param>
        /// <param name="EnsureFullness"> </param>
        internal void Insert(TKey Key, T Value, int RightSiblingPointer, bool EnsureFullness)
        {
            // Posición de insersión aproximada
            int InsertionPosition = FirstPointerOfNodeContaining(Key);
            // Se realiza corrimiento de punteros a hijos
            for (int i = ChildrenPointers.Count - 1; i > InsertionPosition + 1; i--)
            {
                ChildrenPointers[i] = ChildrenPointers[i - 1];
            }
            // El hermano derecho pasa a la ser hijo en la poición de insersión
            ChildrenPointers[InsertionPosition + 1] = RightSiblingPointer;
            // Se realiza un corrimiento de llaves y valores
            for (int i = Keys.Count - 1; i > InsertionPosition; i--)
            {
                Keys[i] = Keys[i - 1];
                Values[i] = Values[i - 1];
            }
            // Se inserta la llave en la posición indicada
            Keys[InsertionPosition] = Key;
            // Se inserta el valor en la posición indicada
            Values[InsertionPosition] = Value;
        }
        /// <summary>
        /// Se realiza la separación del nodo en dos
        /// </summary>
        /// <param name="Key"> Llave a insertar</param>
        /// <param name="Value"> Valor a insertar</param>
        /// <param name="RightSibling"> Linea del hermano derecho</param>
        /// <param name="NewNode"> Nuevo nodo </param>
        /// <param name="UpwardMovingKey"> Llave que se subirá </param>
        /// <param name="UpwardMovingValue"> Valor que se subirá</param>
        public void Split(TKey Key, T Value, int RightSibling, BNode<T, TKey> NewNode, ref TKey UpwardMovingKey, ref T UpwardMovingValue)
        {
            // Se realiza la separación del nodo en dos
            Keys.Add(KeyFactory.CreateNull());
            Values.Add(Value);
            ChildrenPointers.Add(SizesNSpecialCharacters.NullPointer);
            // Se inserta la llave y posición en el hermano derecho
            Insert(Key, Value, RightSibling, false);
            // Se obtiene la llave que subirá
            UpwardMovingKey = Keys[Middle];
            // Se obtiene el valor que subirá
            UpwardMovingValue = Values[Middle];
            // E arreglo con la posición a la mitad es el que separa los dos nodos.
            Keys[Middle] = KeyFactory.CreateNull();
            int j = 0;
            // Se llena el nuevo nodo con las llaves y valores que corresponden
            for (int i = Middle + 1; i < Keys.Count; i++)
            {
                NewNode.Keys[j] = Keys[i];
                NewNode.Values[j] = Values[i];
                Keys[i] = KeyFactory.CreateNull();
                j++;
            }
            j = 0;
            // Se llenan los apuntadores a los hijos del nuevo nodo.
            for (int i = Middle + 1; i < ChildrenPointers.Count; i++)
            {
                NewNode.ChildrenPointers[j] = ChildrenPointers[i];
                ChildrenPointers[i] = SizesNSpecialCharacters.NullPointer;
                j++;
            }
            // Se ellimina la llave del arreglo de llaves
            Keys.RemoveAt(Keys.Count - 1);
            // Se elimina el valor del arreglo de llaves
            Values.RemoveAt(Values.Count - 1);
            // Se elimina el apuntador a la últmima posición de ese arreglo
            ChildrenPointers.RemoveAt(ChildrenPointers.Count - 1);
        }
        
    }
}
