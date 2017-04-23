using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EstructurasDeDatos
{
    public class BNode<T, TKey> 
        where T : IFixedLength
        where TKey: IComparable, IFixedLength, IAutoFormattable
    {
        private IFixedLengthFactory<TKey> KeyFactory;
        private IFixedLengthFactory<T> ContentFactory;
        private int Degree { get; set; }
        public int Pointer { get; set; }
        public int Parent { get; set; }
        private int Middle
        {
            get
            {
                return Degree / 2;
            }
        }
        public List<int> ChildrenPointers { get; set; }
        private List<TKey> Keys { get; set; }
        public List<T> Values { get; set; }
        public int N
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
        internal bool Full
        {
            get
            {
                return (N >= Degree - 1);
            }
        }
        internal bool Leaf
        {
            get {
                for (int i = 0; i < ChildrenPointers.Count; i++)
                {
                    if (ChildrenPointers[i] != SizesNSpecialCharacters.NullPointer)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        internal int SizeInBytes
        {
            get
            {
                int Counter = 0;
                Counter += SizesNSpecialCharacters.ReadablePointerSize + 1; // Tamaño apuntador al Padre
                Counter += 2; // Separadores adicionales
                Counter += (SizesNSpecialCharacters.ReadablePointerSize + 1) * Degree; // Tamaño Hijos
                Counter += 2; // Separadores adicionales
                Counter += (KeyFactory.CreateNull().Format().Length + 1) * (Degree - 1); // Tamaño Llaves
                Counter += 2; // Separadores adicionales
                Counter += (Values[0].Length + 1) * (Degree - 1); // Tamaño Datos
                Counter += Environment.NewLine.Length; // Tamaño del Enter
                return Counter * Encoding.UTF8.GetByteCount("1");
            }
        }
        private int DiskPosition()
        {
            return SizesNSpecialCharacters.HeaderSizeInBytes + (Pointer * SizeInBytes);
        }
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
      
        private void Clear(IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {            
            Keys = new List<TKey>();
            Values = new List<T>();
            for (int i = 0; i < Degree - 1; i++)
            {
                Keys.Add(KeyFactory.CreateNull());
                Values.Add(ContentFactory.CreateNull());
            }
            ChildrenPointers = new List<int>();
            for (int i = 0; i < Degree; i++)
            {
                ChildrenPointers.Add(SizesNSpecialCharacters.NullPointer);
            }
        }
        internal BNode(int Degree, int Pointer, int Parent, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            this.Degree = Degree;
            this.Pointer = Pointer;
            this.Parent = Parent;
            this.KeyFactory = KeyFactory;
            this.ContentFactory = ContentFactory;
            Clear(ContentFactory, KeyFactory);
        }


        public static BNode<T, TKey> DiskRead(FileStream File, int Degreee, int Pointer, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            BNode<T, TKey> NewNode = new BNode<T, TKey>(Degreee, Pointer, 0, ContentFactory, KeyFactory);
            byte[] BytesArray = new byte[NewNode.SizeInBytes];
            int FieldsIndex = 0;
            File.Seek(NewNode.DiskPosition(), SeekOrigin.Begin);
            File.Read(BytesArray, 0, NewNode.SizeInBytes);
            string[] Fields = Encoding.UTF8.GetString(BytesArray).Replace(Environment.NewLine, "").Replace("".PadRight(3, SizesNSpecialCharacters.GroupSeparator[0]), SizesNSpecialCharacters.GroupSeparator.ToString()).Split(SizesNSpecialCharacters.GroupSeparator[0]);
            NewNode.Parent = int.Parse(Fields[FieldsIndex]);
            FieldsIndex++;
            for (int i = 0; i < NewNode.ChildrenPointers.Count; i++)
            {
                NewNode.ChildrenPointers[i] = int.Parse(Fields[FieldsIndex]);
                FieldsIndex++;
            }
            for (int i = 0; i < NewNode.Keys.Count; i++)
            {
                NewNode.Keys[i] = KeyFactory.Create(Fields[FieldsIndex]);
                FieldsIndex++;
            }
            for (int i = 0; i < NewNode.Values.Count; i++)
            {
                NewNode.Values[i] = ContentFactory.Create(Fields[FieldsIndex]);
                FieldsIndex++;
            }
            return NewNode;
        }


        internal void DiskWrite(FileStream File, int tamañoEncabezado)
        {
            byte[] datosBinarios = Encoding.UTF8.GetBytes(FixedSize());
            File.Seek(DiskPosition(), SeekOrigin.Begin);
            File.Write(datosBinarios, 0, SizeInBytes);
            File.Flush();
        }
        public void Clear(FileStream File, IFixedLengthFactory<T> ContentFactory, IFixedLengthFactory<TKey> KeyFactory)
        {
            Clear(ContentFactory, KeyFactory);
            DiskWrite(File, SizesNSpecialCharacters.HeaderSizeInBytes);
        }
        public int FirstPointerOfNodeContaining(TKey Key)
        {
            int Position = Keys.Count;
            for (int i = 0; i < Keys.Count; i++)
            {
                if ((Keys[i].CompareTo(Key)> 0) || (Keys[i].CompareTo(KeyFactory.CreateNull()) == 0))
                {
                    Position = i;
                    break;
                }
            }
            return Position;
        }
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
        internal void Insert(TKey Key, T Value, int RightSiblingPointer)
        {
            Insert(Key, Value, RightSiblingPointer, true);
        }
        internal void Insert(TKey Key, T Value, int RightSiblingPointer, bool ValidarLleno)
        {
            int InsertionPosition = FirstPointerOfNodeContaining(Key);
            for (int i = ChildrenPointers.Count - 1; i > InsertionPosition + 1; i--)
            {
                ChildrenPointers[i] = ChildrenPointers[i - 1];
            }
            ChildrenPointers[InsertionPosition + 1] = RightSiblingPointer;
            for (int i = Keys.Count - 1; i > InsertionPosition; i--)
            {
                Keys[i] = Keys[i - 1];
                Values[i] = Values[i - 1];
            }
            Keys[InsertionPosition] = Key;
            Values[InsertionPosition] = Value;
        }

        
        public void Split(TKey Key, T Value, int RightSibling, BNode<T, TKey> NewNode, ref TKey UpwardMovingKey, T UpwardMovingValue)
        {
            Keys.Add(KeyFactory.CreateNull());
            Values.Add(Value);
            ChildrenPointers.Add(SizesNSpecialCharacters.NullPointer);
            Insert(Key, Value, RightSibling, false);
            UpwardMovingKey = Keys[Middle];
            UpwardMovingValue = Values[Middle];
            Keys[Middle] = KeyFactory.CreateNull();
            int j = 0;
            for (int i = Middle + 1; i < Keys.Count; i++)
            {
                NewNode.Keys[j] = Keys[i];
                NewNode.Values[j] = Values[i];
                Keys[i] = KeyFactory.CreateNull();
                j++;
            }
            j = 0;
            for (int i = Middle + 1; i < ChildrenPointers.Count; i++)
            {
                NewNode.ChildrenPointers[j] = ChildrenPointers[i];
                ChildrenPointers[i] = SizesNSpecialCharacters.NullPointer;
                j++;
            }
            Keys.RemoveAt(Keys.Count - 1);
            Values.RemoveAt(Values.Count - 1);
            ChildrenPointers.RemoveAt(ChildrenPointers.Count - 1);
        }
        internal void Delete(FileStream Stream, TKey Key)
        {
            if (Leaf)
            {
                DeleteFromLeaf(Stream, Key);
            }else
            {
                int i = 0;

                while(i < N && Keys[i].CompareTo(Key) < 0)
                {
                    i++;
                }
                if(i < N && Keys[i].CompareTo(Key) == 0)
                {
                    DeleteFromInternalNode(Stream, i);
                }else
                {
                    BNode<T, TKey> Child = DiskRead(Stream, Degree, ChildrenPointers[i], ContentFactory, KeyFactory);
                    EnsureFullEnough(Stream, i);
                    Child.Delete(Stream, Key);
                }
            }
        }


        private void EnsureFullEnough(FileStream File, int i)
        {
            BNode<T, TKey> Children =  DiskRead(File, Degree, ChildrenPointers[0], ContentFactory, KeyFactory);
            if(Children.N < (Degree/2 - 1))
            {
                BNode<T, TKey> LeftSibling = DiskRead(File, Degree, ChildrenPointers[i - 1], ContentFactory, KeyFactory);
                int LeftSiblingN = LeftSibling.N;

                if(i > 0)
                {
                    LeftSibling = DiskRead(File, Degree, ChildrenPointers[i - 1], ContentFactory,KeyFactory);
                }
                else
                {
                    LeftSibling = null;
                    LeftSiblingN = 0;
                }
                if(N >= (Degree/2 - 1))
                {
                    for(int j = Children.N-1; j >= 0; j--)
                    {
                        Children.Keys[j + 1] = Children.Keys[j];
                    }
                    if (!Children.Leaf)
                    {
                        for(int j = Children.N; j >= 0; j--)
                        {
                            Children.ChildrenPointers[j + 1] = Children.ChildrenPointers[j];
                        }
                    }
                    Children.Keys[0] = Keys[i - 1];
                    Keys[i - 1] = LeftSibling.Keys[LeftSiblingN - 1];
                    LeftSibling.Keys[LeftSiblingN - 1] = KeyFactory.CreateNull();

                    if (!Children.Leaf)
                    {
                        Children.ChildrenPointers[0] = LeftSibling.ChildrenPointers[LeftSiblingN];
                        LeftSibling.ChildrenPointers[LeftSiblingN] = SizesNSpecialCharacters.NullPointer;
                    }
                    DiskWrite(File, 65);
                    Children.DiskWrite(File, 65);
                    LeftSibling.DiskWrite(File, 65);
                }else
                {
                    BNode<T, TKey> RightSibling;
                    int RightSiblingN;

                    if(i < N)
                    {
                        RightSibling = DiskRead(File, Degree, ChildrenPointers[i + 1], ContentFactory, KeyFactory);
                        RightSiblingN = RightSibling.N;
                    }else
                    {
                        RightSibling = null;
                        RightSiblingN = 0;
                    }

                    if(RightSiblingN >= (Degree / 2 - 1))
                    {
                        Children.Keys[Children.N] = Keys[i];
                        Keys[i] = RightSibling.Keys[0];

                        if (!Children.Leaf)
                        {
                            Children.ChildrenPointers[Children.N] = RightSibling.ChildrenPointers[0];
                        }

                        for(int j=1; j < RightSiblingN; j++)
                        {
                            RightSibling.Keys[j - 1] = RightSibling.Keys[j];
                        }
                        RightSibling.Keys[RightSiblingN - 1] = KeyFactory.CreateNull();
                        if (!RightSibling.Leaf)
                        {
                            for(int j=1; j <= RightSiblingN; j++)
                            {
                                RightSibling.ChildrenPointers[j - 1] = RightSibling.ChildrenPointers[j];
                            }
                            RightSibling.ChildrenPointers[RightSiblingN] = SizesNSpecialCharacters.NullPointer;
                        }
                        DiskWrite(File, 65);
                        Children.DiskWrite(File, 65);
                        RightSibling.DiskWrite(File, 65);
                    }
                    else
                    {
                        if (LeftSiblingN > 0)
                        {
                            for (int j = Children.N - 1; j >= 0; j--)
                            {
                                Children.Keys[j + (Degree / 2 - 1)] = Children.Keys[j];
                            }
                            if (!Children.Leaf)
                            {
                                for (int j = Children.N; j >= 0; j--)
                                {
                                    Children.ChildrenPointers[j + (Degree / 2 - 1)] = Children.ChildrenPointers[j];
                                }
                            }
                            for (int j = 0; j < LeftSiblingN; j++)
                            {
                                Children.Keys[j] = LeftSibling.Keys[j];
                                LeftSibling.Keys[j] = KeyFactory.CreateNull();
                            }
                            if (!Children.Leaf)
                            {
                                for (int j = 0; j < LeftSiblingN; j++)
                                {
                                    Children.ChildrenPointers[j] = LeftSibling.ChildrenPointers[j];
                                    LeftSibling.ChildrenPointers[j] = SizesNSpecialCharacters.NullPointer;
                                }
                            }

                            Children.Keys[(Degree / 2 - 1)] = Keys[i - 1];

                            for (int j = i; j < N; j++)
                            {
                                Keys[j - 1] = Keys[j];
                                ChildrenPointers[j - 1] = ChildrenPointers[j];
                            }
                            ChildrenPointers[N - 1] = ChildrenPointers[N];
                            Keys[N - 1] = KeyFactory.CreateNull();
                            ChildrenPointers[N] = SizesNSpecialCharacters.NullPointer;

                            DiskWrite(File, 65);
                            Children.DiskWrite(File, 65);
                        }
                        else
                        {
                            for(int j = 0; j < RightSiblingN; j++)
                            {
                                Children.Keys[j + Children.N + 1] = RightSibling.Keys[j];
                                RightSibling.Keys[j] = KeyFactory.CreateNull();
                            }
                            if (!Children.Leaf)
                            {
                                for(int j = 0; j <= RightSiblingN; j++)
                                {
                                    Children.ChildrenPointers[j + Children.N + 1] = RightSibling.ChildrenPointers[j];
                                    RightSibling.ChildrenPointers[j] = SizesNSpecialCharacters.NullPointer;
                                }
                            }

                            Children.Keys[(Degree / 2 - 1) - 1] = Keys[i];
                           
                            for(int j = i+1; j < N; j++)
                            {
                                Keys[j - 1] = Keys[j];
                                ChildrenPointers[j] = ChildrenPointers[j + 1];

                            }
                            Keys[N - 1] = KeyFactory.CreateNull();
                            ChildrenPointers[N] = SizesNSpecialCharacters.NullPointer;

                            RightSibling.DiskWrite(File, 65);
                            DiskWrite(File, 65);
                            Children.DiskWrite(File, 65);
                        }
                    }
                }
            }
        }

        private void DeleteFromInternalNode(FileStream File, int i)
        {
            TKey Key = Keys[i];
            BNode<T, TKey> y = DiskRead(File, Degree, ChildrenPointers[i], ContentFactory, KeyFactory);

            if (y.N >= (Degree / 2 - 1))
            {
                TKey KPrime = y.LeftGreatest(File);
                y.Delete(File, KPrime);
                Keys[i] = KPrime;

            }else
            {
                BNode<T, TKey> z = DiskRead(File,  Degree, ChildrenPointers[i + 1], ContentFactory, KeyFactory);

                if(z.N >= ((Degree/2) - 1)){
                    TKey KPrime = z.RightLowest(File);
                    z.Delete(File, KPrime);
                    Keys[i] = KPrime;
                }else
                {
                    y.Keys[y.N] = Key;
                    for(int j = 0; j < z.N; j++)
                    {
                        y.Keys[y.N + j + 1] = z.Keys[j];
                    }
                    if (!y.Leaf)
                    {
                        for(int j = 0; j <= z.N; j++)
                        {
                            y.Keys[y.N + j + 1] = z.Keys[j];
                        }
                    }
                    
                    for(int j = i+1; j < N; j++)
                    {
                        Keys[j - 1] = Keys[j];
                        ChildrenPointers[j] = ChildrenPointers[j + 1];
                    }
                    Keys[N - 1] = KeyFactory.CreateNull();
                    ChildrenPointers[N] = SizesNSpecialCharacters.NullPointer;

                    DiskWrite(File, 65);
                    y.DiskWrite(File, 65);
                    z.DiskWrite(File, 65);
                    y.Delete(File, Key);
                }
            }

            DiskWrite(File, 65);

        }

        private TKey RightLowest(FileStream File)
        {
            if (Leaf)
            {
                return Keys[0];
            }else
            {
                return DiskRead(File, Degree, ChildrenPointers[0], ContentFactory, KeyFactory).RightLowest(File);
            }
        }

        private TKey LeftGreatest(FileStream File)
        {
            if (Leaf)
            {
                return Keys[N - 1];
            }else
            {
                return DiskRead(File, Degree, ChildrenPointers[N], ContentFactory, KeyFactory).LeftGreatest(File);

            }
        }

        private void DeleteFromLeaf(FileStream File, TKey Key)
        {
            int i = 0;
            while (i < N && Keys[i].CompareTo(Key) < 0)
            {
                i++;
            }
            if (i < N && Keys[i].CompareTo(Key) == 0)
            {
                for (int j = i + 1; j < N; j++)
                {
                    Keys[j - 1] = Keys[j];
                }
                Keys[N - 1] = KeyFactory.CreateNull();
            }
            DiskWrite(File, 65);
        }
    }
}
