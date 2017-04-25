﻿using System;
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
                }
                if (sintax.Length>2)
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
                                    MessageBox.Show("Se eliminara el dato en la tabla: "+tableName+" con ID: "+ID);
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
                        //ELIMINAR TODO
                        MessageBox.Show("Toda la tabla "+tableName+" fue eliminada!");
                        if (sintax[2].Equals(ReservedWords.ElementAt(8).Value.ToString()))
                        {
                            if (sintax.Length>3)
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
                    MessageBox.Show("Toda la tabla " + tableName + " fue eliminada!");
                }
            }

            if (command.Equals("CREATE TABLE"))
            {
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
                    while (count < sintax.Length && !tableEnd)
                    {
                        spaces = sintax[count].Split(' ');
                        string comando = "";
                        bool commandExist = false;
                        string comandoCompleto = "";
                        if (spaces.Length == 1)
                        {
                            if (spaces[0].Equals(")") && !errorSintaxis)
                            {
                                tableEnd = true;
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
                                if (comando.Equals(""))
                                {
                                    commandExist = false;
                                }
                                else
                                {
                                    if (DataTypes.ElementAt(i).Value.ToString().Equals(comando))
                                    {
                                        commandExist = true;
                                    }
                                }
                            }

                            if (commandExist)
                            {
                                parameterName = spaces[0];

                                if (comandoCompleto.Equals("INT,"))
                                {
                                    if (sintax[count + 1].Split(' ')[0].Equals(")"))
                                    {
                                        MessageBox.Show("No puede ir una coma en la ultima linea, eliminela e intente de nuevo!");
                                        break;
                                    }
                                    else
                                    {
                                        errorSintaxis = false;
                                    }
                                    //CREAR EL NODO DE LA TABLA
                                }
                                else if (comandoCompleto.Equals("INT"))
                                {
                                    if ((spaces[2].ToUpper().Equals("PRIMARY") && spaces[3].ToUpper().Equals("KEY,")) || (spaces[2].ToUpper().Equals("PRIMARY") && spaces[3].ToUpper().Equals("KEY")))
                                    {
                                        if (spaces[3].ToUpper().Equals("KEY,"))
                                        {
                                            if (sintax[count + 1].Split(' ')[0].Equals(")"))
                                            {
                                                MessageBox.Show("No puede ir una coma en la ultima linea, eliminela e intente de nuevo!");
                                                break;
                                            }
                                            else
                                            {
                                                primarykey = true;
                                                errorSintaxis = false;
                                            }
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
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (comandoCompleto.Equals("DATETIME,"))
                                {
                                    //CREAR EL NODO
                                    if (sintax[count + 1].Split(' ')[0].Equals(")"))
                                    {
                                        MessageBox.Show("No puede ir una coma en la ultima linea, eliminela e intente de nuevo!");
                                        break;
                                    }
                                    else
                                    {
                                        errorSintaxis = false;
                                    }
                                }
                                else if (comandoCompleto.Equals("DATETIME"))
                                {
                                    if (!sintax[count + 1].Split(' ')[0].Equals(")"))
                                    {
                                        MessageBox.Show("Error de sintaxis, hace falta una , al final de los argumentos.");
                                        break;
                                    }
                                    else
                                    {
                                        errorSintaxis = false;
                                    }
                                }
                                if (comandoCompleto.Equals("VARCHAR(100),"))
                                {
                                    //CREAR EL NODO
                                    if (sintax[count + 1].Split(' ')[0].Equals(")"))
                                    {
                                        MessageBox.Show("No puede ir una coma en la ultima linea, eliminela e intente de nuevo!");
                                        break;
                                    }
                                    else
                                    {
                                        errorSintaxis = false;
                                    }
                                }
                                else if (comandoCompleto.Equals("VARCHAR(100)"))
                                {
                                    if (!sintax[count + 1].Split(' ')[0].Equals(")"))
                                    {
                                        MessageBox.Show("Error de sintaxis, hace falta una , al final de los argumentos.");
                                        break;
                                    }
                                    else
                                    {
                                        errorSintaxis = false;
                                    }
                                }
                            }else{
                                MessageBox.Show("Hace falta tipo de dato, especifiquelo y vuelva a intentar!");
                                break;
                            }
                        }
                        if (!primarykey && tableEnd)
                        {
                            MessageBox.Show("Hace falta la llave primaria, agreguela y vuelva a ejecutar!");
                            break; 
                        }
                        else
                        {
                            if (!errorSintaxis && tableEnd)
                            {
                                MessageBox.Show("Tabla " + tableName + " creada exitosamente!");
                                
                                if ((count+1) < sintax.Length)
                                {
                                    if (sintax[count + 1].ToUpper().Equals(ReservedWords.ElementAt(8).Value))
                                    {
                                        string[] newSintax = new string[sintax.Length - (count + 2)];
                                        for (int i = 0; i < sintax.Length - (count + 2); i++)
                                        {
                                            newSintax[i] = sintax[(count + 2) + i];
                                        }
                                        DetectCommand(newSintax[0], newSintax);
                                    }
                                }
                            }
                        }
                        count++;
                    }
                }
            }

            if (command.Equals("DROP TABLE"))
            {
                if (sintax.Length>1)
                {
                    string tableName = sintax[1];
                    if (/*VALIDACION SI EXISTE O NO LA TABLA*/true)
                    {
                        //ELIMINAR LA TABLA
                    }
                    else
                    {
                        MessageBox.Show("Nombre de la tabla no existe");
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
                    if (true)//VALIDAR SI EXISTE LA TABLA
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
                                                if (sintax.Length>(parameters.Count+newValues.Count+6))
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
                        //MOSTRAR MENSAJE QUE LA TABLA NO EXISTE
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
