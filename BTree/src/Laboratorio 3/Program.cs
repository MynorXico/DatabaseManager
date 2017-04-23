using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstructurasDeDatos;
using System.Diagnostics;
using System.IO;
namespace Laboratorio_3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch RunningTime = new Stopwatch();
            RunningTime.Start();
            // Cronómetro que llevará registro de las operaciones realizadas
            Stopwatch sw = new Stopwatch();
            string FilePath = "";
            List<string> OutputFileContent = new List<string>();

            Console.WriteLine("Ingrese el nombre del archivo en el cual desea revisar el resultado final de las operaciones.");
            FilePath = Console.ReadLine() + ".csv";
            OutputFileContent.Add("Grado de Arbol, Tiempo Promedio de Busqueda (ms), Tiempo Promedio de Eliminacion (ms)");

            TreeElementFactory ElementFactory = new TreeElementFactory();
            GUIDKeyFactory KeyFactory = new GUIDKeyFactory();
            List<BTree<TreeElement, GUIDKey>> BTreeList = new List<BTree<TreeElement, GUIDKey>>();
            for (int i = 3; i < 21; i++)
            {
                BTreeList.Add(new BTree<TreeElement, GUIDKey>(i, ($"ArbolB-{i}.btree"), ElementFactory, KeyFactory));
            }
            int cantidadErrores = 0;

            foreach (BTree<TreeElement, GUIDKey> BTree in BTreeList)
            {
                Stopwatch TiempoArbol = new Stopwatch();
                TiempoArbol.Restart();
                sw.Start();
                List<GUIDKey> KeysList = new List<GUIDKey>();
                LinkedList<GUIDKey> QueriesList = new LinkedList<GUIDKey>();

                List<TimeSpan> SearchTimes = new List<TimeSpan>();
                double SearchAverageTime = -1;

                List<TimeSpan> DeletionTimes = new List<TimeSpan>();
                double DeletionAverageTime = -1;

                sw.Reset();

                for (int i = 0; i < 1000000; i++)
                {
                    KeysList.Add(new GUIDKey());
                    BTree.Insert(KeysList[i], new TreeElement());
                    if (i % 10000 == 0)
                        QueriesList.AddLast(KeysList[i]);
                }

                foreach (GUIDKey Query in QueriesList)
                {
                    sw.Restart();
                    BTree.BTreeSearch(Query);
                    SearchTimes.Add(sw.Elapsed);
                }
                sw.Stop();
                SearchAverageTime = SearchTimes.Average(x => x.TotalMilliseconds);

                foreach (GUIDKey Query in QueriesList)
                {
                    sw.Restart();
                    try
                    {
                        BTree.Delete(Query);
                    }
                    catch
                    {
                        cantidadErrores++;
                    }
                    DeletionTimes.Add(sw.Elapsed);
                }
                sw.Stop();
                DeletionAverageTime = DeletionTimes.Average(x => x.TotalMilliseconds);
                OutputFileContent.Add($"{BTree.Degree},{SearchAverageTime},{DeletionAverageTime}");
                OutputFileContent.Add($"Finaliza Arbol de Grado {BTree.Degree}. Tiempo: {TiempoArbol.Elapsed.Hours} h, {TiempoArbol.Elapsed.Minutes} min, {TiempoArbol.Elapsed.Seconds} s, {TiempoArbol.Elapsed.Seconds} ms");
                Console.WriteLine($"Finaliza Arbol de Grado {BTree.Degree}. Tiempo: {TiempoArbol.Elapsed.Hours} h, {TiempoArbol.Elapsed.Minutes} min, {TiempoArbol.Elapsed.Seconds} s, {TiempoArbol.Elapsed.Seconds} ms");

            }
            OutputFileContent.Add($"Tiempo Total de Ejecución: {RunningTime.Elapsed.Hours} h, {RunningTime.Elapsed.Minutes} min, {RunningTime.Elapsed.Seconds} s, {RunningTime.Elapsed.Milliseconds} ms");
            OutputFileContent.Add($"Cantidad de Errores: {cantidadErrores}");
            File.WriteAllLines(FilePath, OutputFileContent);
            
            Console.ReadLine();
        }
    }
}
