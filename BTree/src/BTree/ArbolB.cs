using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace EstructurasDeDatos
{
    public class BTree<T, TKey>
        where T : IFixedLength
        where TKey : IComparable, IFixedLength, IAutoFormattable
    {
        int Root;
        int AvailablePointer;
 
        FileStream FileStream;
        string FilePath;
        IFixedLengthFactory<T> ContentFactory;
        IFixedLengthFactory<TKey> KeyFactory;
        public int Degree { get; set; }

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
        private void UpdateHeader()
        {
            FileManagment.WriteLine(File, 0, RootPointer);
            FileManagment.WriteLine(File, 1, FirstAvailablePointer);
            FileManagment.WriteLine(File, 2, Degree);
            File.Flush(); // Actualizar achivo
        }
        private void Insert(int CrawlerPointer, TKey Key, T Value)
        {
            BNode<T, TKey> Crawler = BNode<T, TKey>.DiskRead(File, Degree, CrawlerPointer, TFactory, TKeyFactory);
            if (Crawler.Leaf)
            {
                MoveUp(Crawler, Key, Value, int.MinValue);
                UpdateHeader();
            }
            else
            {
                Insert(Crawler.ChildrenPointers[Crawler.FirstPointerOfNodeContaining(Key)], Key, Value);
            }
        }
        private void MoveUp(BNode<T, TKey> Crawler, TKey Key, T Value, int RightSibling)
        {
            if (!Crawler.Full)
            {
                Crawler.Insert(Key, Value, RightSibling);
                Crawler.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
                return;
            }
            BNode<T, TKey> Sibling = new BNode<T, TKey>(Degree, FirstAvailablePointer, Crawler.Parent, TFactory, TKeyFactory);
            FirstAvailablePointer++;
            TKey UpwardMovingKey = TKeyFactory.CreateNull();
            T UpwardMovingValue = TFactory.CreateNull();
            Crawler.Split(Key, Value, RightSibling, Sibling, ref UpwardMovingKey, UpwardMovingValue);
            BNode<T, TKey> Child = null;
            for (int i = 0; i < Sibling.ChildrenPointers.Count; i++)
            {
                if (Sibling.ChildrenPointers[i] != int.MinValue)
                {
                    Child = BNode<T, TKey>.DiskRead(File, Degree, Sibling.ChildrenPointers[i], TFactory, TKeyFactory);
                    Child.Parent = Sibling.Pointer;
                    Child.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
                }
                else
                {
                    break;
                }
            }
            if (Crawler.Parent == int.MinValue)
            {
                BNode<T, TKey> NewRoot = new BNode<T, TKey>(Degree, FirstAvailablePointer, int.MinValue, TFactory, TKeyFactory);
                FirstAvailablePointer++;
                NewRoot.ChildrenPointers[0] = Crawler.Pointer;
                NewRoot.Insert(UpwardMovingKey, UpwardMovingValue, Sibling.Pointer);
                Crawler.Parent = NewRoot.Pointer;
                Sibling.Parent = NewRoot.Pointer;
                RootPointer = NewRoot.Pointer;
                NewRoot.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
                Crawler.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
                Sibling.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
            }
            else
            {

                Crawler.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
                Sibling.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);

                BNode<T, TKey> Parent = BNode<T, TKey>.DiskRead(File, Degree,  Crawler.Parent, TFactory, TKeyFactory);
                MoveUp(Parent, UpwardMovingKey, UpwardMovingValue, Sibling.Pointer);
            }
        }
        private BNode<T, TKey> BTreeSearch(int CrawlerPointer, TKey Key, out int Pointer)
        {
            BNode<T, TKey> Crawler = BNode<T, TKey>.DiskRead(File, Degree, CrawlerPointer, TFactory, TKeyFactory);
            Pointer = Crawler.PrecisePointer(Key);
            if (Pointer != -1)
            {
                return Crawler;
            }
            else
            {
                if (Crawler.Leaf)
                {
                    return null;
                }
                else
                {
                    int FirstPointerOfNodeContaining = Crawler.FirstPointerOfNodeContaining(Key);
                    return BTreeSearch(Crawler.ChildrenPointers[FirstPointerOfNodeContaining], Key, out Pointer);
                }
            }
        }
        public BTree(int Degree, string FilePath, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            FileName = FilePath;
            TFactory = ContentFactory;
            TKeyFactory = KeyFactory;
            File = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
           FileShare.Read);
            RootPointer = FileManagment.ReadLine(File, 0);
            FirstAvailablePointer = FileManagment.ReadLine(File, 1);
            this.Degree = FileManagment.ReadLine(File, 2);
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
                BNode<T, TKey> Root = new BNode<T, TKey>(this.Degree, FirstAvailablePointer, int.MinValue,
               TFactory, TKeyFactory);
                FirstAvailablePointer++;
                RootPointer = Root.Pointer;
                Root.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
            }
            UpdateHeader();
        }
        public void Insert(TKey Key, T dato)
        {
            Insert(RootPointer, Key, dato);
        }
        public T GetValue(TKey Key)
        {
            int Pointer = -1;
            BNode<T, TKey> ReachedNode = BTreeSearch(RootPointer, Key, out Pointer);
            return ReachedNode.Values[Pointer];
        }
        public bool BTreeSearch(TKey Key)
        {
            int posicion = -1;
            BNode<T, TKey> nodoObtenido = BTreeSearch(RootPointer, Key, out posicion);
            if (nodoObtenido == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }      
        public void Delete(TKey Key)
        {
            BNode<T, TKey> Root = BNode<T, TKey>.DiskRead(File, Degree, RootPointer, TFactory, TKeyFactory);
            Root.Delete(File, Key);

            if(!Root.Leaf && Root.N == 0)
            {
                Root = BNode<T,TKey>.DiskRead(File, Degree, Root.ChildrenPointers[0], TFactory, TKeyFactory);
            }
            Root.DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
        }
    }
}