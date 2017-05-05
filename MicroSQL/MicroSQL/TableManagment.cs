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

        public static void CreateTable(List<string> Tipos, List<string> Parametros, string primaryID, string tableName)
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

                ColumnNames.Append("ID,");
                ColumnTypes.Append("ID,");

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
                (Utilities.DefaultPath + Utilities.DefaultTreesFolder + TableName + ".btree"),
                new TableElementFactory(), new IDFactory());

            string[] LineasArchivo = FileManagment.OpenFile(Utilities.DefaultPath +
                Utilities.DefaultTablesFolder + TableName + ".table");
            string[] TiposColumna = LineasArchivo[0].Split(',');
            string[] NombresColumna = LineasArchivo[1].Split(',');

            List<string> ColumnsTitle = new List<string>();
            List<string> DataTypeSelection = new List<string>();

            bool continuar = true;
            List<string> ListaErrores = new List<string>();
            for (int i = 0; i < ColumnsNameSelection.Length; i++)
            {
                if (!NombresColumna.Contains(ColumnsNameSelection[i]))
                {
                    ListaErrores.Add(ColumnsNameSelection[i]);
                    continuar = false;
                }
            }
            if (!continuar)
            {
                StringBuilder s = new StringBuilder();
                s.Append("Los siguientes campos no existen en la tabla:\n");
                foreach (String error in ListaErrores)
                {
                    s.Append("\t"+error);
                    s.Append("\n");
                }
                MessageBox.Show(s.ToString());
                return;
            }


            // Genera Nombres de Columnas
            for (int i = 0; i < NombresColumna.Length-1; i++)
            {
                if (ColumnsNameSelection.Contains(NombresColumna[i]))
                {
                    ColumnsTitle.Add(NombresColumna[i]);
                    DataTypeSelection.Add(TiposColumna[i]);
                }
            }
            // Crea las columnas para Data Grid View
            foreach (string Name in ColumnsTitle)
            {
                dgv.Columns.Add(Name, Name);
            }

            // Crea una fila por cada elemento
            List<TableElement> Elements = BTree.GetElements();
            foreach (TableElement Element in Elements)
            {
                List<object> Row = new List<object>();
                int Integers = 0;
                int VarChars = 0;
                int DateTimes = 0;
                for (int i = 0; i < DataTypeSelection.Count; i++)
                {
                    if(DataTypeSelection[i] == "ID")
                    {
                        Row.Add(Element.id.id.ToString());
                    }
                    if (DataTypeSelection[i] == "INT")
                    {
                        Row.Add(Element.Enteros[Integers++]);
                    }
                    else if (DataTypeSelection[i] == "VARCHAR")
                    {
                        Row.Add(Element.VarChars[VarChars++].Trim('\'').Trim('-'));
                    }
                    else if (DataTypeSelection[i] == "DATETIME")
                    {
                        Row.Add(Element.DateTimes[DateTimes++]);
                    }
                }
                dgv.Rows.Add(Row.ToArray());
            }

        }

        public static void SelectAllFields(DataGridView dgv, string TableName)
        {
            string[] LineasArchivo = FileManagment.OpenFile(Utilities.DefaultPath +
                Utilities.DefaultTablesFolder + TableName + ".table");
            string[] TiposColumna = LineasArchivo[0].Split(',');
            string[] NombresColumna = LineasArchivo[1].Split(',');
            Select(dgv, NombresColumna, TableName);
        }

        public static void Select(DataGridView dgv, string[] ColumnsNameSelection, string TableName, int Id)
        {
            ResetDataGridView(dgv);
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree,
                (Utilities.DefaultPath + Utilities.DefaultTreesFolder + TableName + ".btree"),
                new TableElementFactory(), new IDFactory());

            string[] LineasArchivo = FileManagment.OpenFile(Utilities.DefaultPath +
                Utilities.DefaultTablesFolder + TableName + ".table");
            string[] TiposColumna = LineasArchivo[0].Split(',');
            string[] NombresColumna = LineasArchivo[1].Split(',');

            List<string> ColumnsTitle = new List<string>();
            List<string> DataTypeSelection = new List<string>();

            bool continuar = true;
            List<string> ListaErrores = new List<string>();
            for(int i = 0; i < ColumnsNameSelection.Length; i++)
            {
                if (!NombresColumna.Contains(ColumnsNameSelection[i]))
                {
                    ListaErrores.Add(ColumnsNameSelection[i]);
                    continuar = false;
                }
            }
            if (!continuar)
            {
                StringBuilder s = new StringBuilder();
                s.Append("Los siguientes elementos no existen:\n");
                MessageBox.Show("Los siguientes elementos no existen: ");
                foreach(String error in ListaErrores)
                {
                    s.Append(error);
                    s.Append("\n");
                }
                MessageBox.Show(s.ToString());
                return;
            }


            // Genera Nombres de Columnas
            for (int i = 0; i < NombresColumna.Length; i++)
            {
                if (ColumnsNameSelection.Contains(NombresColumna[i]))
                {
                    ColumnsTitle.Add(NombresColumna[i]);
                    DataTypeSelection.Add(TiposColumna[i]);
                }
            }
            // Crea las columnas para Data Grid View
            foreach (string Name in ColumnsTitle)
            {
                dgv.Columns.Add(Name, Name);
            }

            ID Query = new ID();
            Query.id = Id;
            // Crea una fila por cada elemento
            TableElement Element = BTree.GetValue(Query);
            List<object> Row = new List<object>();
            int Integers = 0;
            int VarChars = 0;
            int DateTimes = 0;
            for (int i = 0; i < DataTypeSelection.Count; i++)
            {
                if (DataTypeSelection[i] == "ID")
                {
                    Row.Add(Element.id.id.ToString());
                }
                if (DataTypeSelection[i] == "INT")
                {
                    Row.Add(Element.Enteros[Integers++]);
                }
                else if (DataTypeSelection[i] == "VARCHAR")
                {
                    Row.Add(Element.VarChars[VarChars++].Trim('\'').Trim('-'));
                }
                else if (DataTypeSelection[i] == "DATETIME")
                {
                    Row.Add(Element.DateTimes[DateTimes++]);
                }
            }
                dgv.Rows.Add(Row.ToArray());

        }

        private static void ResetDataGridView(DataGridView rt)
        {
            rt.Columns.Clear();
        }

        public static bool Insert(List<string> Insersion, string TreeName, List<string> Parameters)
        {
            string TreePath = Utilities.DefaultPath + Utilities.DefaultTreesFolder + TreeName + ".btree";

            if (!File.Exists(TreePath))
            {
                MessageBox.Show("No existe la tabla a la cual se desea insertar");
                return false;
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
                    int Test;
                    if (int.TryParse(Insersion[i], out Test))
                    {
                        NewElement.Enteros[IntegerCounter++] = int.Parse(Insersion[i]);
                    }
                    else
                    {
                        MessageBox.Show($"Se esperaba un entero y se recibió {Insersion[i]}");
                        return false;
                    }
                }
                else if (DataTypes[i] == "VARCHAR")
                {
                    if (Insersion[i].StartsWith("'") && Insersion[i].EndsWith(""))
                        NewElement.VarChars[VarCharCounter++] = Insersion[i];
                    else
                    {
                        MessageBox.Show("Las variables VARCHAR deben estar encerradas con comillas simples");
                        return false;
                    }
                }
                else if (DataTypes[i] == "DATETIME")
                {
                    if (Insersion[i].StartsWith("'") && Insersion[i].EndsWith(""))
                        NewElement.DateTimes[VarCharCounter++] = Insersion[i];
                    else
                    {
                        MessageBox.Show("Las variables DATETIME deben estar encerradas con comillas simples");
                        return false;
                    }

                    DateTime Date;
                    if(DateTime.TryParse(Insersion[i].Trim('\''), out Date))
                    {
                        NewElement.DateTimes[DateTimeCounter++] = Insersion[i];
                    }else
                    {
                        MessageBox.Show("Por favor proporcionar la fecha en un formato válido.");
                        return false;
                    }
                }
                else if (DataTypes[i] == "ID")
                {
                    int Test;
                    if (int.TryParse(Insersion[i], out Test))
                    {
                        NewElement.id.id = int.Parse(Insersion[i]);
                        id.id = int.Parse(Insersion[i]);
                    }
                    else
                    {
                        MessageBox.Show($"Se esperaba un entero y se recibió {Insersion[i]}");
                        return false;
                    }
                }
            }
            BTree<TableElement, ID> BTree = new BTree<TableElement, ID>(Utilities.TreesDegree, TreePath, new TableElementFactory(), new IDFactory());
            if (BTree.BTreeSearch(id))
            {
                MessageBox.Show("Ya existe un elemento con ese ID en la tabla.");
                return false;
            }
            BTree.Insert(id, NewElement);
            return true;
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
