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
        public static void FillTreeeView(TreeView tv, string TreesFolder, string TablesFolder)
        {
            tv.Nodes.Clear();
            if (!Directory.Exists(TreesFolder) || !Directory.Exists(TablesFolder))
                return; 

            List<string> TreesName = Directory.EnumerateFiles(TreesFolder).ToArray().ToList();
            List<string> TablesName = Directory.EnumerateFiles(TablesFolder).ToArray().ToList();

           foreach(string TreeName in TreesName)
            {
                string tag = TreeName.Split('/')[3].Split('.')[0];
                TreeNode TmpNode = new TreeNode(tag, 0, 0);
                string[] TableInfo = File.ReadAllLines(TablesFolder + tag + ".table");
                string[] DataTypes = TableInfo[0].Split(',');
                string[] VariableValues = TableInfo[1].Split(',');
                for(int i = 0; i < DataTypes.Length-1; i++)
                {
                    TreeNode TypeNode = new TreeNode(DataTypes[i], 2, 2);
                    TreeNode ValueNode = new TreeNode(VariableValues[i], 1, 1);


                    ValueNode.Nodes.Add(TypeNode);
                    TmpNode.Nodes.Add(ValueNode);
                }

                tv.Nodes.Add(TmpNode);

            }

        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void CreateTree(string tableName, string TreePath)
        {
            BTree<TableElement, ID> NewTree = new BTree<TableElement, ID>(Utilities.TreesDegree, TreePath, new TableElementFactory(), new IDFactory());

        }

        public static void CreateTable(List<string> Tipos, List<string> Parametros, string primaryKey, string tableName)
        {
            string TreePath = Utilities.DefaultPath + Utilities.DefaultTreesFolder + tableName + ".btree";
            string TablePath = Utilities.DefaultPath + Utilities.DefaultTablesFolder + tableName + ".table";

            if (FileExists(TablePath))
            {
                MessageBox.Show("La tabla " + tableName + " ya existe en el contexto actual.");
            }
            else
            {
                if (!Directory.Exists(Utilities.DefaultPath + Utilities.DefaultTablesFolder))
                {
                    Directory.CreateDirectory(Utilities.DefaultPath + Utilities.DefaultTablesFolder);
                }
                List<string> content = new List<string>();
                StringBuilder ColumnNames = new StringBuilder();
                StringBuilder ColumnTypes = new StringBuilder();

                ColumnNames.Append("KEY,");
                ColumnTypes.Append("KEY,");

                foreach (string Parametro in Parametros)
                {
                    ColumnNames.Append(Parametro);
                    ColumnNames.Append(",");
                }
                foreach (string Type in Tipos)
                {
                    ColumnTypes.Append(Type);
                    ColumnTypes.Append(",");
                }
                content.Add(ColumnTypes.ToString());
                content.Add(ColumnNames.ToString());
                File.WriteAllLines(TablePath, content.ToArray());



            }

            if (FileExists(TreePath))
            {
                MessageBox.Show("La tabla " + tableName + " ya existe en el contexto actual.");
            }
            else
            {
                if (!Directory.Exists(Utilities.DefaultPath + Utilities.DefaultTreesFolder))
                {
                    Directory.CreateDirectory(Utilities.DefaultPath + Utilities.DefaultTreesFolder);
                }
                CreateTree(tableName, TreePath);
                MessageBox.Show("Tabla " + tableName + " creada correctamente!");
            }

        }

        public static void Select(DataGridView dgv, string[] ColumnsNameSelection, string TableName)
        {
            ResetDataGridView(dgv);
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree,
                (Utilities.DefaultPath + Utilities.DefaultTreesFolder + TableName + ".arbolb"),
                new TableElementFactory(), new IDFactory());

            string[] LineasArchivo = FileManagment.OpenFile(Utilities.DefaultPath +
                Utilities.DefaultTablesFolder + ".tabla");
            string[] NombresColumna = LineasArchivo[0].Split(',');
            string[] TiposColumna = LineasArchivo[1].Split(',');

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
            foreach (TableElement Element in BTree.GetElements())
            {
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
            foreach (DataGridViewColumn column in rt.Columns)
            {
                rt.Columns.Remove(column);
            }
            foreach (DataGridViewRow row in rt.Rows)
            {
                rt.Rows.Remove(row);
            }
        }

        public static void Insert(List<string> Insersion, string TreeName, List<string> Parameters)
        {
            string TreePath = Utilities.DefaultPath + Utilities.DefaultTreesFolder + TreeName + ".btree";

            if (!File.Exists(TreePath))
            {
                MessageBox.Show("No existe la tabla a la cual se desea insertar");
                return;
            }

            TableElement NewElement = new TableElement();

            string[] InformacionTablArchivos = File.ReadAllLines(Utilities.DefaultPath + Utilities.DefaultTablesFolder + TreeName + ".table");
            string[] DataTypes = InformacionTablArchivos[0].Split(',');

            int IntegerCounter = 0;
            int VarCharCounter = 0;
            int DateTimeCounter = 0;
            ID id = new ID();


            for (int i = 0; i < DataTypes.Length; i++)
            {
                if (DataTypes[i] == "INT")
                {
                    NewElement.Enteros[IntegerCounter++] = int.Parse(Insersion[i]);
                }
                else if (DataTypes[i] == "VARCHAR")
                {
                    NewElement.VarChars[VarCharCounter++] = Insersion[i];
                }
                else if (DataTypes[i] == "DATETIME")
                {
                    NewElement.DateTimes[DateTimeCounter++] = Insersion[i];
                }
                else if (DataTypes[i] == "KEY")
                {
                    id.id = int.Parse(Insersion[i]);
                    NewElement.id = id;
                }
            }
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree, TreePath, new TableElementFactory(), new IDFactory());
            BTree.Insert(id, NewElement);
        }

        public static void Delete(ID id, string TreeName)
        {
            string TreePath = Utilities.DefaultPath + Utilities.DefaultTreesFolder + TreeName + ".btree";

            if (!File.Exists(TreePath))
            {
                MessageBox.Show("No existe la tabla de la cual se desea eliminar.");
                return;
            }
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree, TreePath, new TableElementFactory(), new IDFactory());
            BTree.Delete(id);
        }

        public static void Delete(string TreeName)
        {
            string TreePath = Utilities.DefaultPath + Utilities.DefaultTreesFolder + TreeName + ".btree";
            if (!File.Exists(TreePath))
            {
                MessageBox.Show("La tabla a eliminar no existe");
                return;
            }
            File.Delete(TreePath);
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree, TreePath, new TableElementFactory(), new IDFactory());
        }
    }
}
