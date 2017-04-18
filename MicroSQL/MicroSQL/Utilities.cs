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
            DataTypes.Add("INT", "INT ");
            DataTypes.Add("VARCHAR", "VARCHAR ");
            DataTypes.Add("DATETIME" , "DATETIME ");
            ReservedWords.Add("SELECT", "SELECT ");
            ReservedWords.Add("FROM", "FROM ");
            ReservedWords.Add("DELETE", "DELETE ");
            ReservedWords.Add("WHERE", "WHERE");
            ReservedWords.Add("CREATE TABLE", "CREATE TABLE");
            ReservedWords.Add("DROP TABLE", "DROP TABLE");
            ReservedWords.Add("INSERT INTO", "INSERT INTO");
            ReservedWords.Add("VALUES", "VALUES");
            ReservedWords.Add("GO", "GO");
        }

        public static Dictionary<string, string> ReservedWords = new Dictionary<string, string>();


     
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
