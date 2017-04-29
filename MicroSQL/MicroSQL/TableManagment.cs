using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstructurasDeDatos;
using System.IO;
using System.Windows.Forms;

namespace MicroSQL
{
    public class TableManagment
    {

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void CreateTree(List<string> listaVarchar, List<string> listaDateTime, List<string> listaInt, string primaryKey, string tableName, string TreePath)
        {
            BTree<TableElement, ID> NewTree = new BTree<TableElement, ID>(Utilities.TreesDegree, TreePath, new TableElementFactory(), new IDFactory());


            for (int i = 0; i < listaVarchar.Count; i++)
            {

            }
            for (int i = 0; i < listaDateTime.Count; i++)
            {

            }
            for (int i = 0; i < listaInt.Count; i++)
            {

            }
        }

        public static void CreateTable(List<string> listaVarchar, List<string> listaDateTime, List<string> listaInt, string primaryKey, string tableName)
        {
            string TreePath = Utilities.DefaultPath + Utilities.DefaultTreesFolder + tableName + ".btree";

            if (FileExists(TreePath))
            {
                MessageBox.Show("La tabla "+tableName+" ya existe en el contexto actual.");
            }
            else
            {
                if (!Directory.Exists(Utilities.DefaultPath+Utilities.DefaultTreesFolder))
                {
                    Directory.CreateDirectory(Utilities.DefaultPath + Utilities.DefaultTreesFolder);
                }
                CreateTree(listaVarchar, listaDateTime, listaInt, primaryKey, tableName, TreePath);
                MessageBox.Show("Tabla " + tableName + " creada correctamente!");
            }
        }

        public static void Select(DataGridView dgv,  string[] ColumnsNameSelection, string TableName)
        {
            ResetDataGridView(dgv);
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree, 
                (Utilities.DefaultPath + Utilities.DefaultTreesFolder + TableName + ".arbolb"), 
                new TableElementFactory(), new IDFactory());

            string[] LineasArchivo = FileManagment.OpenFile(Utilities.DefaultPath+
                Utilities.DefaultTablesFolder+".tabla");
            string[] NombresColumna = LineasArchivo[0].Split(',');
            string[] TiposColumna = LineasArchivo[1].Split(',');

            List<object> ColumnsTitle = new List<object>();

            // Genera Nombres de Columnas
            for (int i = 0; i < NombresColumna.Length; i++)
            {

                if (ColumnsNameSelection.Contains(NombresColumna[i])) {
                    ColumnsTitle.Add(NombresColumna[i]);
                }
            }
            // Crea las columnas para Data Grid View
            foreach (string Name in ColumnsTitle)
            {
                dgv.Columns.Add(Name, Name);
            }

            // Crea una fila por cada elemento
            foreach (TableElement Element in BTree.GetElements())
            {
                List<object> Row = new List<object>();
                int Integers = 0;
                int VarChars = 0;
                int DateTimes = 0;
                for(int i = 0; i < TiposColumna.Length; i++)
                {
                    if(TiposColumna[i] == "INT")
                    {
                        Row.Add(Element.Enteros[Integers++]);
                    }else if(TiposColumna[i] == "VARCHAR")
                    {
                        Row.Add(Element.VarChars[VarChars++]);
                    }
                    else if(TiposColumna[i] == "DATETIME")
                    {
                        Row.Add(Element.DateTimes[DateTimes++]);
                    }
                }
                dgv.Rows.Add(Row);
            }
            
        }

        public static void Select(DataGridView dgv, string[] ColumnsNameSelection, string TableName, int Id)
        {
            ResetDataGridView(dgv);
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree,
                (Utilities.DefaultPath + Utilities.DefaultTreesFolder + TableName + ".arbolb"),
                new TableElementFactory(), new IDFactory());

            string[] LineasArchivo = FileManagment.OpenFile(Utilities.DefaultPath +
                Utilities.DefaultTablesFolder + ".tabla");
            string[] NombresColumna = LineasArchivo[0].Split(',');
            string[] TiposColumna = LineasArchivo[1].Split(',');

            ID Query = new ID();
            Query.id = Id;
            TableElement Element = BTree.GetValue(Query);

            List<object> ColumnsTitle = new List<object>();

            // Genera Nombres de Columnas
            for (int i = 0; i < NombresColumna.Length; i++)
            {

                if (ColumnsNameSelection.Contains(NombresColumna[i]))
                {
                    ColumnsTitle.Add(NombresColumna[i]);
                }
            }
            // Crea las columnas para Data Grid View
            foreach (string Name in ColumnsTitle)
            {
                dgv.Columns.Add(Name, Name);
            }

            // Crea una fila por cada elemento
            List<object> Row = new List<object>();
            int Integers = 0;
            int VarChars = 0;
            int DateTimes = 0;
            for (int i = 0; i < TiposColumna.Length; i++)
            {
                if (TiposColumna[i] == "INT")
                {
                    Row.Add(Element.Enteros[Integers++]);
                }
                else if (TiposColumna[i] == "VARCHAR")
                {
                    Row.Add(Element.VarChars[VarChars++]);
                }
                else if (TiposColumna[i] == "DATETIME")
                {
                        Row.Add(Element.DateTimes[DateTimes++]);
                }
            }
            dgv.Rows.Add(Row);
            

        }


        private static void ResetDataGridView(DataGridView rt)
        {
            foreach(DataGridViewColumn column in rt.Columns)
            {
                rt.Columns.Remove(column);
            }
            foreach(DataGridViewRow row in rt.Rows)
            {
                rt.Rows.Remove(row);
            }
        }
        
    }
}
