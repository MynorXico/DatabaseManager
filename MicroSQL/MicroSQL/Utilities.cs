using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using EstructurasDeDatos;

namespace MicroSQL
{
    public class Utilities
    {
        // Grado de los árboles a utilizar
        public static int TreesDegree = 8;

        // Datos utilizados como nulos
        public static int NullInt = int.MinValue;
        public static string NullDateTime = "00/00/00/";
        public static string NullVarChar = "";

        // Rutas Predeterminadas
        public static string DefaultPath = @"C:/microSQL/";
        public static string DefaultTablesFolder = @"tablas/";
        public static string DefaultTreesFolder = @"arbolesb/";
        public static string DefaultFileName = "microSQL.ini";
        public static string CompleteDefaultTablesFolder = DefaultPath + DefaultTablesFolder;
        public static string CompleteDefaultTreesFolder = DefaultPath + DefaultTreesFolder;

        // Formato de Variables de Diferentes Tipos de Datos
        public static int IntegerSize = 11;
        public static int VarCharSize = 100;
        public static int DateTimeSize = 10;

        // Definición de colores
        public static Color BackgroundColorColor = ColorTranslator.FromHtml("#272822");
        public static Color PlainTextColor = ColorTranslator.FromHtml("#F8F8F2");
        public static Color DataTypesColor = ColorTranslator.FromHtml("#A6E22E");
        public static Color ReservedWordsColor = ColorTranslator.FromHtml("#66D9EF");
        public static Color QuotedWordsColor = ColorTranslator.FromHtml("#F92672");

        // Diccionarios con palabras reservadas
        public static Dictionary<string, string> DataTypes = new Dictionary<string, string>();
        public static Dictionary<string, string> ReservedWords = new Dictionary<string, string>();

        public static bool DictionaryIsLoaded = false;

        /// <summary>
        /// Llenar diccionarios con los datos existentes en el archivo o en caso de no
        /// existir, crea uno nuevo.
        /// </summary>
        public static void Initialize()
        {
            if (File.Exists(DefaultPath + DefaultFileName))
            {
                FillDictionaries(DefaultPath + DefaultFileName);
            }
            else
            {
                List<string> Lines = new List<string>();
                Lines.Add("SELECT, SELECT");
                Lines.Add("FROM, FROM");
                Lines.Add("DELETE FROM, DELETE FROM");
                Lines.Add("WHERE, WHERE");
                Lines.Add("CREATE TABLE, CREATE TABLE");
                Lines.Add("DROP TABLE, DROP TABLE");
                Lines.Add("INSERT INTO, INSERT INTO");
                Lines.Add("VALUES, VALUES");
                Lines.Add("GO, GO");

                if (!Directory.Exists(DefaultPath))
                {
                    Directory.CreateDirectory(DefaultPath);
                }
                File.WriteAllLines(DefaultPath + DefaultFileName, Lines.ToArray());
                FillDictionaries(DefaultPath + DefaultFileName);
            }
            DataTypes.Add("INT", "INT");
            DataTypes.Add("VARCHAR(100)", "VARCHAR(100)");
            DataTypes.Add("DATETIME", "DATETIME");
            DataTypes.Add("PRIMARY KEY", "PRIMARY KEY");
        }

        /// <summary>
        /// Llenar diccionario tomando los datos de un archivo .ini
        /// </summary>
        /// <param name="FilePath"></param>
        public static void FillDictionaries(string FilePath)
        {
            ReservedWords = new Dictionary<string, string>();
            string[] Lines = FileManagment.OpenFile(FilePath);
            foreach (string Line in Lines)
            {
                string[] SeparatedValues = Line.Split(',');
                string Key = SeparatedValues[0].Trim(' ');
                string Value = SeparatedValues[1].Trim(' ');
                ReservedWords.Add(Key, Value);
            }
            if (!Directory.Exists(DefaultPath))
            {
                Directory.CreateDirectory(DefaultPath);
            }
            File.WriteAllLines(DefaultPath + DefaultFileName, Lines);

            DictionaryIsLoaded = true;
        }

        public static void RecognizeCode(string[] sintax)
        {
            if (sintax.Length>1)
            {
                for (int i = 0; i < ReservedWords.Count; i++)
                {
                    if (ReservedWords.ElementAt(i).Value.Equals(sintax[0]))
                    {
                        DetectCommand(ReservedWords.ElementAt(i).Key.ToUpper().ToString(), sintax);
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("No hay nada para ejecutar!");
            }
        }

        /// <summary>
        /// Da color a la una palabra ingresada.
        /// </summary>
        /// <param name="word"> La palabra que será buscada y será formateada.</param>
        /// <param name="color"> Color con que la palabra será formateada.</param>
        /// <param name="startIndex"> Índice de la primera posición de búsqueda</param>
        /// <param name="rt"> RichTextBox que contiene la palabra ingresada.</param>
        public static void HighLightKeyWord(string word, Color color, int startIndex, RichTextBox rt)
        {
            string rtText = rt.Text.ToUpper();
            if (rtText.Contains(word))
            {
                int index = -1;
                int selectStart = rt.SelectionStart;

                while ((index = rtText.IndexOf(word, (index + 1))) != -1)
                {
                    rt.Select((index + startIndex), word.Length);
                    rt.SelectionColor = color;
                    rt.Select(selectStart, 0);
                    rt.SelectionColor = Color.White;
                }
            }
        }

        public static void SendToDetectCommand(string [] sintax, int count){
            string[] newSintax = new string[sintax.Length - (count + 2)];
            for (int i = 0; i < sintax.Length - (count + 2); i++)
            {
                newSintax[i] = sintax[(count + 2) + i];
            }
            DetectCommand(newSintax[0], newSintax);
        }

        public static void DetectCommand(string command, string[] sintax)
        {
            if (command.Equals("SELECT"))
            {
                List<string> parameters = new List<string>();
                string tableName;
                string parameter;
                for (int i = 1; i < sintax.Length; i++)
                {
                    if (sintax[i].Contains(','))
                    {
                        if ((i) < sintax.Length)
                        {
                            if (!sintax[i + 1].ToUpper().Equals(ReservedWords.ElementAt(1).Value.ToString()))
                            {
                                parameters.Add(sintax[i].Trim(','));
                                if ((i+1)<sintax.Length)
                                {
                                    if (!sintax[i + 1].ToUpper().Equals(ReservedWords.ElementAt(1).Value.ToString()))
                                    {
                                        MessageBox.Show("Falta pa palabra reservada FROM.");
                                        break;
                                    }
                                    else
                                    {
                                        if ((i+2)<sintax.Length)
                                        {
                                            if (true) //tableName exists
                                            {
                                                if (true)
                                                {

                                                }
                                            }
                                            //AGREGAR VALIDACION SI LA TABLA EXISTE
                                            /*
                                            if(arbol.Buscar(sintax[i+2])){
                                                TABLA ENCONTRADA
                                                if ((i+3)<sintax.Length)
                                                {
                                                    if(sintax[i+3].ToUpper().Equals(ReservedWords.ElementAt(3).Value.ToString())){
                                                        if(i+4 < sintax.Length){
                                                            //VALIDAR QUE NO SEA UNA PALABRA RESERVADA
                                                            parameter = sintax[i+4];
                                                        }else{
                                                            AGREGAR ERROR
                                                        }
                                                    }
                                                }
                                            //AGREGAR VALIDACION SI LA TABLA EXISTE
                                            /*
                                        }
                                       */
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("No puede haber una coma antes de la palabra reservada FROM.");
                                break;
                            }
                        }
                    }
                    else
                    {
                        if ((i)<sintax.Length)
                        {
                            if (sintax[i+1].ToUpper().Equals(ReservedWords.ElementAt(1).Value.ToString()))
                            {
                                parameters.Add(sintax[i]);
                            }
                            else
                            {
                                MessageBox.Show("Hace falta una coma dentro de los parametros, agreguela e intente de nuevo.");
                                break;
                            }
                        }
                    }
                }
            }

            if (command.Equals("DELETE FROM"))
            {
                string tableName = "";
                string ID = "";
                if (sintax.Length>1)
                {
                    tableName = sintax[1];
                    if (File.Exists(Utilities.DefaultPath+Utilities.DefaultTreesFolder+tableName+".btree"))
                    {
                        if (sintax.Length > 2)
                        {
                            if (sintax[2].ToUpper().Equals(ReservedWords.ElementAt(3).Value.ToString()))
                            {
                                if (sintax.Length>3)
                                {
                                    string[] elements = sintax[3].Split(' ');
                                    if (elements.Length == 3)
                                    {
                                        if (elements[1] == "=")
                                        {
                                            ID = sintax[3];
                                            //PROCEDER A LA ELIMINACION
                                            #region Se elimina de árbol
                                            ID id = new MicroSQL.ID();
                                            id.id = int.Parse(Regex.Replace(ID, @"[^\d]", ""));
                                            TableManagment.Delete(id, tableName);
                                            #endregion
                                            MessageBox.Show("Se eliminara el dato en la tabla: " + tableName + " con ID: " + ID);
                                            if (sintax.Length>4)
                                            {
                                                if (sintax[4].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                {
                                                    if (sintax.Length > 4)
                                                    {
                                                        string[] newSintax = new string[sintax.Length - 5];
                                                        for (int i = 0; i < sintax.Length - (5); i++)
                                                        {
                                                            newSintax[i] = sintax[5 + i];
                                                        }
                                                        DetectCommand(newSintax[0], newSintax);
                                                    }
                                                    else
                                                    {
                                                        string [] newSintax2 = { "" };
                                                        DetectCommand("", newSintax2);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Necesita haber un = en la condicion de eliminacion!");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("La condicion de eliminacion no cuenta con el formato adecuado!");
                                    }
                                }
                                
                            }
                            else
                            {
                                TableManagment.Delete(tableName);

                                //ELIMINAR TODO
                                MessageBox.Show("Toda la tabla " + tableName + " fue eliminada!");
                                if (sintax[2].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                {
                                    if (sintax.Length > 3)
                                    {
                                        string[] newSintax = new string[sintax.Length - 3];
                                        for (int i = 0; i < sintax.Length - (3); i++)
                                        {
                                            newSintax[i] = sintax[3 + i];
                                        }
                                        DetectCommand(newSintax[0], newSintax);
                                    }
                                }
                            }
                        }
                        else
                        {
                            TableManagment.Delete(tableName);
                            MessageBox.Show("Toda la tabla " + tableName + " fue eliminada!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error, la tabla "+tableName+" no existe, sentencia: "+command);
                    }
                }
            }

            if (command.Equals("CREATE TABLE"))
            {
                string tableName = "";
                string primaryKey = "";
                List<string> listaInt = new List<string>();
                List<string> listaVarchar = new List<string>();
                List<string> listaDateTime = new List<string>();
                List<string> Parametros = new List<string>();
                List<string> Tipos = new List<string>();
                if (sintax.Length>1)
                {
                    tableName = sintax[1];
                    if (sintax.Length>2)
                    {
                        if (sintax[2] != "(")
                        {
                            MessageBox.Show("Sintaxis incorrecta, agregue ( en la sentencia de "+command); 
                        }
                        else
                        {
                            if (sintax.Length > 3)
                            {
                                for (int i = 3; i < sintax.Length; i++)
                                {
                                    string[] spaces = sintax[i].Split(' ');
                                    if (spaces.Length == 2)
                                    {
                                        if (sintax[i].EndsWith(","))
                                        {
                                            if (DatatypeExists(spaces[1].Trim(',')))
                                            {
                                                if (spaces[1].Trim(',').ToUpper().Equals("INT"))
                                                {
                                                    listaInt.Add(spaces[0]);
                                                    Tipos.Add("INT");
                                                    Parametros.Add(spaces[0]);
                                                }
                                                if (spaces[1].Trim(',').ToUpper().Equals("VARCHAR(100)"))
                                                {
                                                    listaVarchar.Add(spaces[0]);
                                                    Tipos.Add("VARCHAR");
                                                    Parametros.Add(spaces[0]);
                                                }
                                                if (spaces[1].Trim(',').ToUpper().Equals("DATETIME"))
                                                {
                                                    listaDateTime.Add(spaces[0]);
                                                    Tipos.Add("DATETIME");
                                                    Parametros.Add(spaces[0]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (i+1<sintax.Length)
                                            {
                                                if (sintax[i + 1] == ")")
                                                {
                                                    if (spaces[1].ToUpper().Equals("INT"))
                                                    {
                                                        listaInt.Add(spaces[0]);
                                                        Tipos.Add("INT");
                                                        Parametros.Add(spaces[0]);
                                                    }
                                                    if (spaces[1].ToUpper().Equals("VARCHAR(100)"))
                                                    {
                                                        listaVarchar.Add(spaces[0]);
                                                        Tipos.Add("VARCHAR");
                                                        Parametros.Add(spaces[0]);
                                                    }
                                                    if (spaces[1].ToUpper().Equals("DATETIME"))
                                                    {
                                                        listaDateTime.Add(spaces[0]);
                                                        Tipos.Add("DATETIME");
                                                        Parametros.Add(spaces[0]);
                                                    }
                                                    if (primaryKey == "")
                                                    {
                                                        MessageBox.Show("Debe existir una llave primaria dentro de una sentencia "+command);
                                                    }
                                                    else
                                                    {
                                                        // SE CREA LA TABLA
                                                        TableManagment.CreateTable(Tipos, Parametros, primaryKey, tableName);
                                                        if (sintax.Length > listaDateTime.Count+listaInt.Count+listaVarchar.Count+1+4)
                                                        {
                                                            int count = listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4;
                                                            if (sintax[count].ToUpper().Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                            {
                                                                if (sintax.Length > listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4 + 1)
                                                                {
                                                                    string[] newSintax = new string[sintax.Length - (listaDateTime.Count + listaInt.Count + listaVarchar.Count + 1 + 4 + 1)];
                                                                    if (newSintax.Length>=2)
                                                                    {
                                                                        for (int j = 0; j < sintax.Length - (count+1); j++)
                                                                        {
                                                                            newSintax[j] = sintax[count+ 1 + j];
                                                                        }
                                                                        i = sintax.Length;
                                                                        DetectCommand(newSintax[0], newSintax);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Error de sintaxis, debe haber una , al final de los tipos de dato!");
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (spaces.Length == 4)
                                    {
                                        string newCommand = spaces[2] + " " + spaces[3].Trim(',');
                                        if (DatatypeExists(newCommand))
                                        {
                                            if (i + 1 < sintax.Length)
                                            {
                                                if (sintax[i].EndsWith(","))
                                                {
                                                    if (primaryKey =="")
                                                    {
                                                        primaryKey = spaces[0];
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("ERROR: solo puede haber una llave primaria!");
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (sintax[i + 1] == ")")
                                                    {
                                                        if (primaryKey == "")
                                                        {
                                                            primaryKey = spaces[0];
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("ERROR: solo puede haber una llave primaria!");
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Error de sintaxis, debe haber una , al final de los tipos de dato!");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (command.Equals("DROP TABLE"))
            {
                if (sintax.Length > 1)
                {
                    string tableName = sintax[1];
                    if (File.Exists(Utilities.DefaultPath + Utilities.DefaultTreesFolder + tableName + ".btree"))
                    {
                        File.Delete(Utilities.DefaultPath + Utilities.DefaultTreesFolder + tableName + ".btree");
                        File.Delete(Utilities.DefaultPath + Utilities.DefaultTablesFolder + tableName + ".table");
                    }
                    else
                    {
                        MessageBox.Show("La tabla " + tableName + " no existe, no puede ser eliminada!");
                    }
                }
            }

            if (command.Equals("INSERT INTO"))
            {
                string tableName = "";
                List<string> parameters = new List<string>();
                List<string> newValues = new List<string>();
                if (sintax.Length>1)
                {
                    tableName = sintax[1];
                    if (File.Exists(Utilities.DefaultPath + Utilities.DefaultTreesFolder + tableName + ".btree"))
                    {
                        if (sintax.Length>2)
                        {
                            if (sintax[2] != "(")
                            {
                                MessageBox.Show("Hace falta un '(' en la sintaxis de: "+command);
                            }
                            else
                            {
                                for (int i = 3; i < sintax.Length; i++)
                                {
                                    if (sintax[i].Contains(','))
                                    {
                                        if (sintax[i].Equals(")"))
                                        {
                                            MessageBox.Show("No se permiten las comas al final de la instruccion.");
                                            break;
                                        }else{
                                            parameters.Add(sintax[i].Trim(','));
                                        }
                                    }
                                    else
                                    {
                                        if ((i+1)<sintax.Length)
                                        {
                                            if (sintax[i + 1].Equals(")"))
                                            {
                                                parameters.Add(sintax[i].Trim(','));
                                                i = sintax.Length;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Deben haber comas entre los parametros!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (parameters.Count > 0)
                        {
                            if (sintax.Length>(parameters.Count+3))
                            {
                                if (sintax[parameters.Count + 4].ToUpper().Equals(ReservedWords.ElementAt(7).Value.ToString()))
                                {
                                    if (sintax.Length >parameters.Count+4)
                                    {
                                        if (!sintax[parameters.Count+5].Equals("("))
                                        {
                                            MessageBox.Show("Hace falta un ( en la sintaxis de "+command);
                                        }
                                        else
                                        {
                                            if (sintax.Length > parameters.Count+5)
                                            {
                                                for (int i = parameters.Count + 6; i < sintax.Length; i++)
                                                {
                                                    if (sintax[i].Contains(','))
                                                    {
                                                        if (sintax[i].Equals(")"))
                                                        {
                                                            MessageBox.Show("No se permiten las comas al final de la instruccion.");
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            newValues.Add(sintax[i].Trim(','));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((i + 1) < sintax.Length)
                                                        {
                                                            if (sintax[i + 1].Equals(")"))
                                                            {
                                                                newValues.Add(sintax[i].Trim(','));
                                                                i = sintax.Length;
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Deben haber comas entre los parametros!");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (parameters.Count==newValues.Count)
                                            {
                                                MessageBox.Show("Datos correctamente agregados a la tabla: "+tableName);
                                                //Agregar valores al arbol
                                                TableManagment.Insert(newValues, tableName, parameters);
                                                if (sintax.Length>(parameters.Count+newValues.Count+7))
                                                {
                                                    if (sintax[parameters.Count + newValues.Count + 7].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                                                    {
                                                        if (sintax.Length > (parameters.Count + newValues.Count + 7))
                                                        {

                                                        }
                                                        string[] newSintax = new string[sintax.Length - (parameters.Count + newValues.Count + 8)];
                                                        for (int i = 0; i < sintax.Length - (parameters.Count + newValues.Count + 8); i++)
                                                        {
                                                            newSintax[i] = sintax[(parameters.Count + newValues.Count + 8) + i];
                                                        }
                                                        DetectCommand(newSintax[0], newSintax);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Los parametros y los nuevos valores no coinciden, reviselos y vuelva a intentar!");
                                            }
                                        }
                                    }
                                }
                            } 
                        }
                    }
                    else
                    {
                        MessageBox.Show("La tabla "+tableName+" no existe, sentencia: "+command);
                    }
                }
                else
                {

                }
            }

            if (command.Equals("GO"))
            {

            }
        }
       
        public static bool DatatypeExists(string command)
        {
            foreach (var item in DataTypes.Values)
            {
                if (command.ToUpper().Equals(item.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpLoadFile()
        {
            string path = "microsql.ini";
            if (!File.Exists(path))
            {
                Initialize();
                string[] lines = new string[DataTypes.Count + ReservedWords.Count];
                int count = 0;
                //Agregar todos los datos del diccionario de Tipos de Dato 
                for (int i = 0; i < DataTypes.Count; i++)
                {
                    lines[i] = DataTypes.ElementAt(i).Key.ToString() + "," + DataTypes.ElementAt(i).Value.ToString();
                    count++;
                }

                //Agregar todos los datos del diccionario de palabras reservadas
                for (int i = 0; i < ReservedWords.Count; i++)
                {
                    lines[count + i] = ReservedWords.ElementAt(i).Key.ToString() + "," + ReservedWords.ElementAt(i).Value.ToString();
                }
                File.WriteAllLines(path, lines);
            }
            else
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < 4; i++)
                {
                    string[] words = lines[i].Split(',');
                    DataTypes.Add(words[0], words[1]);
                }

                for (int i = 4; i < 13; i++)
                {
                    string[] words = lines[i].Split(',');
                    ReservedWords.Add(words[0], words[1]);
                }
            }
        }

        /// <summary>
        /// Da color a las palabras entre comillas simples.
        /// </summary>
        /// <param name="rt">RichTextBox que contiene las palabras entre comillas.</param>
        /// <param name="color">Color con el que se formateará las palabras entre comillas.</param>
        public static void HighLightQuotedWords(RichTextBox rt, Color color)
        {
            Regex RegularExpression = new Regex("'(.*)(.*)'");

            MatchCollection Collection = RegularExpression.Matches(rt.Text.ToUpper());
            int OldSelection = rt.SelectionStart;

            foreach (Match m in Collection)
            {
                rt.SelectionStart = m.Index;
                rt.SelectionLength = m.Value.Length;
                rt.SelectionColor = color;
                rt.DeselectAll();
            }
            rt.SelectionStart = OldSelection;
        }

        /// <summary>
        /// Convierte un INT a string con el formato indicado.
        /// </summary>
        /// <param name="i"> INT que será formateado.</param>
        /// <returns> Cadena con el INT formateado.</returns>
        public static string FormatInteger(int i)
        {
            return i.ToString().PadLeft(IntegerSize, '0');
        }

        /// <summary>
        /// Convierte un VARCHAR a string con el formato indicado.
        /// </summary>
        /// <param name="s"> VARCHAR que será formateado.</param>
        /// <returns> Cadena con el VARCHAR formateado.</returns>
        public static string FormatVarChar(string s)
        {
            if (s.Length > 100)
            {
                throw new FormatException("El tamaño de la variable de tipo de dato VARCHAR no puede ser suprerior a 100.");
            }
            return s.PadLeft(IntegerSize, '-');
        }

        /// <summary>
        /// Convierte un DATETIME a string con el formato indicado
        /// </summary>
        /// <param name="s"> DATETIME que será formateado.</param>
        /// <returns> Cadena con el VARCHAR formateado.</returns>
        public static string FormatDate(string s)
        {
            StringBuilder Output = new StringBuilder();
            DateTime Input;
            if (DateTime.TryParse(s, out Input))
            {
                Output.Append(Input.Day);
                Output.Append("/");
                Output.Append(Input.Month);
                Output.Append("/");
                Output.Append(Input.Year);
            }
            return Output.ToString();
        }

    }
}
