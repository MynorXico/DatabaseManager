using System;
using System.Collections.Generic;
using System.IO;

namespace EstructurasDeDatos
{
    public class BTree<T, TKey>
        where T : IFixedLength, IAutoFormattable
        where TKey : IComparable, IFixedLength, IAutoFormattable
    {
        // Raíz del árbol
        int Root;
        // Posición disponible en disco para escritura
        int AvailablePointer;
        // FileStream para leer y/o escribir el archivo
        FileStream FileStream;
        // Ruta del archivo
        string FilePath;
        // Fábrica de contenido
        IFixedLengthFactory<T> ContentFactory;
        // Fábrica de Llaves
        IFixedLengthFactory<TKey> KeyFactory;
        // Orden del árbol
        public int Degree { get; set; }

        /// <summary>
        /// Lista de elementos ordenados de forma ascendente
        /// </summary>
        /// <returns></returns>
        public List<T> GetElements()
        {
            // Se crea el FileStream
            File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            List<T> Elements = new List<T>();
            InOrder(Root, Elements);
            // Se liberan los recursos
            File.Dispose();
            return Elements;
        }
        /// <summary>
        /// Realiza recorrido en orden llenando una lista a su paso
        /// </summary>
        /// <param name="posicionActual"> Posición en la que se encuentra </param>
        /// <param name="Elements"> Lista de elementos que se llena durante 
        /// la revursión </param>
        private void InOrder(int posicionActual, List<T> Elements)
        {
            // Caso base de la inducción
            if (posicionActual == SizesNSpecialCharacters.NullPointer)
            {
                return;
            }
            // Se carga el nodo en el que se encuentra
            BNode<T, TKey> nodoActual = BNode<T, TKey>.DiskRead(File, Degree, posicionActual, ContentFactory, KeyFactory);
            for (int i = 0; i < nodoActual.ChildrenPointers.Count; i++)
            {
                // Para cada hijo, se procede recursivamente
                InOrder(nodoActual.ChildrenPointers[i], Elements);
                if ((i < nodoActual.Keys.Count) && (nodoActual.Keys[i].CompareTo(KeyFactory.CreateNull()) != 0))
                {
                    // Visita nuevo espacio
                    Elements.Add(nodoActual.Values[i]);
                }
            }
        }

        #region Encapsulamiento
        public int RootPointer
        {
            get
            {
                return Root;
            }

            set
            {
                Root = value;
            }
        }
        public int FirstAvailablePointer
        {
            get
            {
                return AvailablePointer;
            }

            set
            {
                AvailablePointer = value;
            }
        }
        public FileStream File
        {
            get
            {
                return FileStream;
            }

            set
            {
                FileStream = value;
            }
        }
        public string FileName
        {
            get
            {
                return FilePath;
            }

            set
            {
                FilePath = value;
            }
        }
        public IFixedLengthFactory<T> TFactory
        {
            get
            {
                return ContentFactory;
            }

            set
            {
                ContentFactory = value;
            }
        }
        public IFixedLengthFactory<TKey> TKeyFactory
        {
            get
            {
                return KeyFactory;
            }

            set
            {
                KeyFactory = value;
            }
        }
        #endregion

        /// <summary>
        ///  Actualización del encabezado
        /// </summary>
        private void UpdateHeader()
        {
            // Se escriben las tres líneas que contiene el encabezado
            FileManagment.WriteLine(File, 0, RootPointer);
            FileManagment.WriteLine(File, 1, FirstAvailablePointer);
            FileManagment.WriteLine(File, 2, Degree);
            // Se liberan los recursos
            File.Flush();
        }
        /// <summary>
        /// Insersión recursiva d un elemento en base a su llave
        /// </summary>
        /// <param name="CrawlerPointer"> Posición actual en la recursión</param>
        /// <param name="Key"> Llave</param>
        /// <param name="Value">Valor</param>
        private void Insert(int CrawlerPointer, TKey Key, T Value)
        {
            // Nodo actual
            BNode<T, TKey> Crawler = BNode<T, TKey>.DiskRead(File, Degree, CrawlerPointer, TFactory, TKeyFactory);
            // En caso de que se encuentre en hoja
            if (Crawler.Leaf)
            {
                // Se sube y actualiza el encabezado
                MoveUp(Crawler, Key, Value, int.MinValue);
                UpdateHeader();
            }
            else
            {
                // Se inserta recursivamente en el nodo hijo que debería contener la clave
                Insert(Crawler.ChildrenPointers[Crawler.FirstPointerOfNodeContaining(Key)], Key, Value);
            }
        }
        private void MoveUp(BNode<T, TKey> Crawler, TKey Key, T Value, int RightSibling)
        {
            // Si aún hay espacio, se añade
            if (!Crawler.Full)
            {
                Crawler.Insert(Key, Value, RightSibling);
                Crawler.DiskWrite(File);
                // se interrumple el flujo
                return;
            }
            // Se crea un nuevo hermano y se prepara para subir los datos al padre
            BNode<T, TKey> Sibling = new BNode<T, TKey>(Degree, FirstAvailablePointer++, Crawler.Parent, TFactory, TKeyFactory);
            TKey UpwardMovingKey = TKeyFactory.CreateNull();
            T UpwardMovingValue = TFactory.CreateNull();

            // Se separa el nodo en dos
            Crawler.Split(Key, Value, RightSibling, Sibling, ref UpwardMovingKey, ref UpwardMovingValue);
            BNode<T, TKey> Child = null;
            // Se cambia el apuntador en  todos los hijos
            for (int i = 0; i < Sibling.ChildrenPointers.Count; i++)
            {
                if (Sibling.ChildrenPointers[i] != int.MinValue)
                {
                    Child = BNode<T, TKey>.DiskRead(File, Degree, Sibling.ChildrenPointers[i], TFactory, TKeyFactory);
                    Child.Parent = Sibling.Pointer;
                    Child.DiskWrite(File);
                }
                else
                {
                    break;
                }
            }
            // Se evalúa el caso del caso Padre
            if (Crawler.Parent == int.MinValue)
            {
                // En caso de que la raíz del padre sea nula,
                // Se debe asignar una nueva raíz.
                BNode<T, TKey> NewRoot = new BNode<T, TKey>(Degree, FirstAvailablePointer, int.MinValue, TFactory, TKeyFactory);
                FirstAvailablePointer++;

                // Se actualiza la información de la nueva raíz
                NewRoot.ChildrenPointers[0] = Crawler.Pointer;
                NewRoot.Insert(UpwardMovingKey, UpwardMovingValue, Sibling.Pointer);

                // Se actualizan los apuntadores del padre
                Crawler.Parent = NewRoot.Pointer;
                Sibling.Parent = NewRoot.Pointer;

                RootPointer = NewRoot.Pointer;
                // Se guardan los cambios
                NewRoot.DiskWrite(File);
                Crawler.DiskWrite(File);
                Sibling.DiskWrite(File);
            }
            else
            {
                // No se modificó la raíz
                Crawler.DiskWrite(File);
                Sibling.DiskWrite(File);

                // Se carga el nuevo nodo padre
                BNode<T, TKey> Parent = BNode<T, TKey>.DiskRead(File, Degree, Crawler.Parent, TFactory, TKeyFactory);
                MoveUp(Parent, UpwardMovingKey, UpwardMovingValue, Sibling.Pointer);
            }
        }

        /// <summary>
        /// Búsqueda recursiva en subárboles
        /// </summary>
        /// <param name="CrawlerPointer"> P</param>
        /// <param name="Key"></param>
        /// <param name="Pointer"></param>
        /// <returns></returns>
        private BNode<T, TKey> BTreeSearch(int CrawlerPointer, TKey Key, out int Pointer)
        {
            BNode<T, TKey> Crawler = BNode<T, TKey>.DiskRead(File, Degree, CrawlerPointer, TFactory, TKeyFactory);
            Pointer = Crawler.PrecisePointer(Key);
            if (Pointer != -1)
                return Crawler;
            else
                if (Crawler.Leaf)
                    return null;
                else
                {
                    int FirstPointerOfNodeContaining = Crawler.FirstPointerOfNodeContaining(Key);
                    return BTreeSearch(Crawler.ChildrenPointers[FirstPointerOfNodeContaining], Key, out Pointer);
                }
        }
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="Degree"></param>
        /// <param name="FilePath"></param>
        /// <param name="ContentFactory"></param>
        /// <param name="KeyFactory"></param>
        public BTree(int Degree, string FilePath, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            // Se crea el nuevo árbol con los parámetros recibidos
            FileName = FilePath;
            TFactory = ContentFactory;
            TKeyFactory = KeyFactory;

            // Se abre el FileStream para poder modificar el archivo
            File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.Read);

            // Se realiza la lectura del archivo para obtener los datos del encabezado
            RootPointer = FileManagment.ReadLine(File, 0);
            FirstAvailablePointer = FileManagment.ReadLine(File, 1);
            this.Degree = FileManagment.ReadLine(File, 2);

            // En caso de que no haya encontrado ningún dato
            // Se inicializan los datos por que no existían antes
            if (FirstAvailablePointer == int.MinValue)
            {
                FirstAvailablePointer = 0;
            }

            if (this.Degree == int.MinValue)
            {
                this.Degree = Degree;
            }
            if (RootPointer == int.MinValue)
            {
                // Se crea la raíz
                BNode<T, TKey> Root = new BNode<T, TKey>(this.Degree, FirstAvailablePointer, int.MinValue,
               TFactory, TKeyFactory);
                FirstAvailablePointer++;
                RootPointer = Root.Pointer;
                Root.DiskWrite(File);
            }
            // Se actualiza el encabezado
            UpdateHeader();
            // Se Liberan los recursos
            File.Dispose();
        }
        /// <summary>
        /// Inserta un elemento con llave Key
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="dato"></param>
        public void Insert(TKey Key, T dato)
        {
            File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                       FileShare.Read);
            // Inserta el elemento recursivamente
            Insert(RootPointer, Key, dato);
            File.Dispose();
        }
        /// <summary>
        /// Devuelve el valor con llave Key en caso de que exista
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public T GetValue(TKey Key)
        {
            File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            int Pointer = -1;
            BNode<T, TKey> ReachedNode = BTreeSearch(RootPointer, Key, out Pointer);
            File.Dispose();
            return ReachedNode.Values[Pointer];
        }
        /// <summary>
        /// Devuelve verdadero en caso de encontrar la llave, en caso contrario, falso.
        /// </summary>
        /// <param name="Key">Llave</param>
        /// <returns></returns>
        public bool BTreeSearch(TKey Key)
        {
            // FileStream para poder leer y escribir el archivo
            File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            int posicion = -1;
            // Se intenta buscar el nodo
            BNode<T, TKey> nodoObtenido = BTreeSearch(RootPointer, Key, out posicion);
            File.Dispose();
            // Si no se encontró nodo, no existe el nodo
            if (nodoObtenido == null)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }
        /// <summary>
        /// Método de eliminar
        /// </summary>
        /// <param name="Key"> Llave por eliminar </param>
        public void Delete(TKey Key)
        {
            // FileStream para poder leer y escribir
            FileStream File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            // Se obtiene la raíz
            BNode<T, TKey> Root = BNode<T, TKey>.DiskRead(File, Degree, RootPointer, TFactory, TKeyFactory);
            // Comienza el proceso de eliminación recursiva
            Root.Delete(File, Key);

            if (!Root.Leaf && Root.AmountOfData == 0)
            {
                Root = BNode<T, TKey>.DiskRead(File, Degree, Root.ChildrenPointers[0], TFactory, TKeyFactory);
            }
            // Se escribe el archivo
            Root.DiskWrite(File);
            // Se liberan los recursos
            File.Dispose();
        }
    }
}