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

namespace MicroSQL
{
    public class Utilities
    {
        // Caracter utilizado para separar datos de los elementos de una tabla
        public static string DataSeparator = "/";


        // Rutas Predeterminadas
        public static string DefaultPath = @"C:/microSQL/";
        public static string DefaultFileName = "microSQL.ini";

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
        public static void FillDictionaries()
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
                Lines.Add("DELETE, DELETE");
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
            DataTypes.Add("VARCHAR", "VARCHAR(");
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
            for (int i = 0; i < ReservedWords.Count; i++)
            {
                if (ReservedWords.ElementAt(i).Value.Equals(sintax[0]))
                {
                    DetectCommand(ReservedWords.ElementAt(i).Key.ToUpper().ToString(), sintax);
                    break;
                }
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

        public static void DetectCommand(string command, string[] sintax)
        {
            if (command.Equals("SELECT"))
            {

            }

            if (command.Equals("DELETE"))
            {

            }

            if (command.Equals("CREATE TABLE"))
            {
                bool tableCreated = false;
                string parameterName;
                string tableName = sintax[1];
                if (sintax[2] != "(")
                {
                    MessageBox.Show("Error en linea 3, falta (");
                }
                else
                {
                    bool primarykey = false;
                    string[] spaces;
                    int count = 3;
                    bool errorSintaxis = true;
                    bool tableEnd = false;

                    while (count < sintax.Length)
                    {
                        spaces = sintax[count].Split(' ');
                        string comando = "";
                        string comandoCompleto = "";
                        if (spaces.Length == 1)
                        {
                            if (spaces[0].Equals(")"))
                            {
                                tableEnd = true;
                            }
                            if (spaces[0].ToUpper().Equals(ReservedWords.ElementAt(8).Key.ToString()))
                            {
                                MessageBox.Show("GO");
                            }
                        }
                        else
                        {
                            comando = spaces[1].Trim(',').ToUpper();
                            comandoCompleto = spaces[1].ToUpper();
                        }
                        if (!tableEnd)
                        {
                            for (int i = 0; i < DataTypes.Count; i++)
                            {
                                if (DataTypes.ElementAt(i).Value.ToString().Equals(comando))
                                {
                                    parameterName = spaces[0];

                                    if (comandoCompleto.Equals("INT,"))
                                    {
                                        errorSintaxis = false;
                                        //CREAR EL NODO DE LA TABLA
                                    }
                                    else if (comandoCompleto.Equals("INT"))
                                    {
                                        if ((spaces[2].ToUpper().Equals("PRIMARY") && spaces[3].ToUpper().Equals("KEY,")) || (spaces[2].ToUpper().Equals("PRIMARY") && spaces[3].ToUpper().Equals("KEY")))
                                        {
                                            if (spaces[3].ToUpper().Equals("KEY,"))
                                            {
                                                primarykey = true;
                                                errorSintaxis = false;
                                            }
                                            else
                                            {
                                                if (sintax[count + 1].Split(' ')[0].Equals(")"))
                                                {
                                                    primarykey = true;
                                                    errorSintaxis = false;
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Error de sintaxis, hace falta una , al final de los argumentos.");
                                                }
                                            }
                                        }
                                    }

                                    if (comandoCompleto.Equals("DATETIME,"))
                                    {
                                        //CREAR EL NODO
                                        errorSintaxis = false;
                                    }
                                    else if (comandoCompleto.Equals("DATETIME"))
                                    {
                                        if (!sintax[count + 1].Split(' ')[0].Equals(")"))
                                        {
                                            MessageBox.Show("Error de sintaxis, hace falta una , al final de los argumentos.");
                                        }
                                        else
                                        {
                                            errorSintaxis = false;
                                        }
                                    }
                                    if (comandoCompleto.Equals("VARCHAR(100),"))
                                    {
                                        //CREAR EL NODO
                                        errorSintaxis = false;
                                    }
                                    else if (comandoCompleto.Equals("VARCHAR(100)"))
                                    {
                                        if (!sintax[count + 1].Split(' ')[0].Equals(")"))
                                        {
                                            MessageBox.Show("Error de sintaxis, hace falta una , al final de los argumentos.");
                                        }
                                        else
                                        {
                                            errorSintaxis = false;
                                        }
                                    }
                                }
                            }
                        }
                        if (!errorSintaxis)
                        {
                            MessageBox.Show("Tabla " + tableName + " creada exitosamente!");
                            tableCreated = true;
                        }
                        count++;
                    }
                }
            }

            if (command.Equals("DROP TABLE"))
            {

            }

            if (command.Equals("INSERT INTO"))
            {

            }

            if (command.Equals("GO"))
            {

            }
        }
       
        public static void UpLoadFile()
        {
            string path = "microsql.ini";
            if (!File.Exists(path))
            {
                FillDictionaries();
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
                for (int i = 0; i < 8; i++)
                {
                    string[] words = lines[i].Split(',');
                    DataTypes.Add(words[0], words[1]);
                }

                for (int i = 8; i < 26; i++)
                {
                    string[] words = lines[i].Split(',');
                    if (i == 25)
                    {
                        Console.WriteLine();
                    }
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
