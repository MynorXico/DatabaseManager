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
        // Definición de colores
        public static Color BackgroundColorColor = ColorTranslator.FromHtml("#272822");
        public static Color PlaneTextColor = ColorTranslator.FromHtml("#F8F8F2");
        public static Color DataTypesColor = ColorTranslator.FromHtml("#A6E22E");
        public static Color ReservedWordsColor = ColorTranslator.FromHtml("#66D9EF");
        public static Color QuotedWordsColor = ColorTranslator.FromHtml("#F92672");

        public static Dictionary<string, string> DataTypes = new Dictionary<string, string>();

        public static void FillDictionaries()
        {
            DataTypes.Add("INT", "INT");
            DataTypes.Add("VARCHAR", "VARCHAR");
            DataTypes.Add("DATETIME" , "DATETIME");
            DataTypes.Add("PRIMARY KEY", "PRIMARY KEY");
            ReservedWords.Add("SELECT", "SELECT");
            ReservedWords.Add("FROM", "FROM");
            ReservedWords.Add("DELETE", "DELETE");
            ReservedWords.Add("WHERE", "WHERE");
            ReservedWords.Add("CREATE TABLE", "CREATE TABLE");
            ReservedWords.Add("DROP TABLE", "DROP TABLE");
            ReservedWords.Add("INSERT INTO", "INSERT INTO");
            ReservedWords.Add("VALUES", "VALUES");
            ReservedWords.Add("GO", "GO");
            DataTypes.Add("int", "int");
            DataTypes.Add("varchar", "varchar");
            DataTypes.Add("datetime", "datetime");
            DataTypes.Add("primary key", "primary key");
            ReservedWords.Add("select", "select");
            ReservedWords.Add("from", "from");
            ReservedWords.Add("delete", "delete");
            ReservedWords.Add("where", "where");
            ReservedWords.Add("create table", "create table");
            ReservedWords.Add("drop table", "drop table");
            ReservedWords.Add("insert into", "insert into");
            ReservedWords.Add("values", "values");
            ReservedWords.Add("go", "go");
        }

        public static Dictionary<string, string> ReservedWords = new Dictionary<string, string>();

        public static void reconizeCode(string[] sintax)
        {
            for (int i = 0; i < ReservedWords.Count; i++)
            {
                if (ReservedWords.ElementAt(i).Value.Equals(sintax[0]))
                {
                    detectCommand(ReservedWords.ElementAt(i).Key.ToUpper().ToString(), sintax);
                    break;
                }
            }
        }

        public static void detectCommand(string command, string [] sintax)
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

                    while (count<sintax.Length)
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
                    lines[i] = DataTypes.ElementAt(i).Key.ToString()+ ","+DataTypes.ElementAt(i).Value.ToString();
                    count++;
                }

                //Agregar todos los datos del diccionario de palabras reservadas
                for (int i = 0; i < ReservedWords.Count; i++)
                {
                    lines[count + i] = ReservedWords.ElementAt(i).Key.ToString()+ "," + ReservedWords.ElementAt(i).Value.ToString();
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

        public static void HighLightQuoted(RichTextBox rt, Color color)
        {
            Regex RegularExpression = new Regex("'(.*)(.*)'");

            MatchCollection Collection = RegularExpression.Matches(rt.Text);
            int OldSelection = rt.SelectionStart;
            
            foreach(Match m in Collection)
            {
                rt.SelectionStart = m.Index;
                rt.SelectionLength = m.Value.Length;
                rt.SelectionColor = color;
                rt.DeselectAll();
            }
            rt.SelectionStart = OldSelection;
        }
    }
}
