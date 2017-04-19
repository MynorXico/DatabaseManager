using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
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

        public static void reconizeCode(string [] sintax)
        {
            foreach (var command in ReservedWords.Keys)
            {
                if (command == sintax[0])
                {
                    detectCommand(command, sintax);
                    break;
                }
            }
        }

        public static void detectCommand(string command, string [] sintax)
        {
            if (command.ToUpper().Equals("SELECT"))
            {
               
            }

            if (command.ToUpper().Equals("DELETE"))
            {

            }

            if (command.ToUpper().Equals("CREATE TABLE"))
            {
                string tableName = sintax[1];
                if (sintax[2] != "(")
                {
                    MessageBox.Show("Error en linea 3, falta (");
                }
                else
                {
                    string[] spaces = sintax[3].Split(' ');
                    
                    
                }
            }

            if (command.ToUpper().Equals("DROP TABLE"))
            {

            }

            if (command.ToUpper().Equals("INSERT INTO"))
            {

            }

            if (command.ToUpper().Equals("GO"))
            {

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
