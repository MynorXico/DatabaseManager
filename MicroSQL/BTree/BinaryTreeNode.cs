using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTree
{
    public class NodoArbol<T> where T : Comparable
    {
        public T dato { get; set; }
        public NodoArbol<T> derecha, izquierda;
        public NodoArbol() { }
        public NodoArbol(T dato)
        {
            this.dato = dato;
        }
        public static bool operator <(NodoArbol<T> nodo1, NodoArbol<T> nodo2)
        {
            return nodo1.dato < nodo2.dato;
        }
        public static bool operator >(NodoArbol<T> nodo1, NodoArbol<T> nodo2)
        {
            return nodo1.dato > nodo2.dato;
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (this.izquierda != null)
            {
                foreach (var v in izquierda)
                {
                    yield return v;
                }
            }
            yield return dato;
            if (this.derecha != null)
            {
                foreach (var v in derecha)
                {
                    yield return v;
                }
            }
        }
    }
}