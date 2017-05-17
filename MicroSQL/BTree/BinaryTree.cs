using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTree
{
    public class ArbolBinario<T> : IEnumerable<T> where T : Comparable
    {
        public delegate bool cmp(T t1, T t2);
        public int tamaño = 0;
        NodoArbol<T> raiz;

        public ArbolBinario()
        {
            raiz = null;
        }

        public NodoArbol<T> getFirst()
        {
            if (raiz != null)
            {
                return raiz;
            }
            return raiz;
        }

        public void insertar(T dato)
        {
            NodoArbol<T> nuevoNodo = new NodoArbol<T>(dato);
            insertarNodo(nuevoNodo);
        }


        public void insertarNodo(NodoArbol<T> nodo)
        {
            tamaño++;
            NodoArbol<T> nuevo;
            nuevo = new NodoArbol<T>();
            nuevo = nodo;
            nuevo.izquierda = null;
            nuevo.derecha = null;
            if (raiz == null)
            {
                raiz = nuevo;
            }
            else
            {
                NodoArbol<T> anterior = null, recorrido;
                recorrido = raiz;
                while (recorrido != null)
                {
                    anterior = recorrido;
                    if (nodo < recorrido)
                    {
                        recorrido = recorrido.izquierda;
                    }
                    else
                    {
                        recorrido = recorrido.derecha;
                    }
                }
                if (nodo < anterior)
                {
                    anterior.izquierda = nuevo;
                }
                else
                {
                    anterior.derecha = nuevo;
                }
            }
        }

        public T buscar(T elemento, cmp comparador)
        {
            T elementoBusqueda = null;
            buscarRec(elemento, raiz, comparador, ref elementoBusqueda);
            return elementoBusqueda;
        }

        public bool cmpId(T t1, T t2)
        {
            return t1.ID == t2.ID;
        }

        public void buscarRec(T elemento, NodoArbol<T> inicio, cmp comparador, ref T elementoBusqueda)
        {
            if (comparador(elemento, inicio.dato))
            {
                elementoBusqueda = inicio.dato;
                return;
            }
            if (inicio.izquierda != null)
            {
                buscarRec(elemento, inicio.izquierda, comparador, ref elementoBusqueda);
            }
            if (inicio.derecha != null)
            {
                buscarRec(elemento, inicio.derecha, comparador, ref elementoBusqueda);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return raiz.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return raiz.GetEnumerator();
        }
    }
}