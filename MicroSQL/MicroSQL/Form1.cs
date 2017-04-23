using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EstructurasDeDatos;
using System.Collections;

namespace MicroSQL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Utilities.FillDictionaries();
        }

        private void rtbUserInput_TextChanged(object sender, EventArgs e)
        {
            int SelectStart = rtbUserInput.SelectionStart;
            rtbUserInput.SelectAll();
            rtbUserInput.SelectionColor = Utilities.PlainTextColor;
            rtbUserInput.Select(SelectStart, 0);
           
            foreach(string word in Utilities.DataTypes.Values)
            {
                Utilities.HighLightKeyWord(word, Utilities.DataTypesColor, 0, rtbUserInput);
            }
            foreach(string word in Utilities.ReservedWords.Values)
            {
                Utilities.HighLightKeyWord(word, Utilities.ReservedWordsColor, 0, rtbUserInput);
            }
            Utilities.HighLightQuotedWords(rtbUserInput, Utilities.QuotedWordsColor);
        }

        

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            Utilities.FillDictionaries(ofd.FileName);
        }


        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            string[] SIntax = rtbUserInput.Lines;
            Utilities.RecognizeCode(SIntax);
        }
    }
}
