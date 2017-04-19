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
            int selectStart = rtbUserInput.SelectionStart;
            rtbUserInput.SelectAll();
            rtbUserInput.SelectionColor = Utilities.PlaneTextColor;
            rtbUserInput.Select(selectStart, 0);
           
            foreach(string word in Utilities.DataTypes.Values)
            {
                CheckKeyword(word, Utilities.DataTypesColor, 0);
            }
            foreach(string word in Utilities.ReservedWords.Values)
            {
                CheckKeyword(word, Utilities.ReservedWordsColor, 0);
            }
            Utilities.HighLightQuoted(rtbUserInput, Utilities.QuotedWordsColor);
        }

        private void CheckKeyword(string word, Color color, int startInde)
        {
            if (rtbUserInput.Text.Contains(word))
            {
                int index = -1;
                int selectStart = rtbUserInput.SelectionStart;

                while((index = rtbUserInput.Text.IndexOf(word, (index+1)))!= -1)
                {
                    rtbUserInput.Select((index + startInde), word.Length);
                    rtbUserInput.SelectionColor = color;
                    rtbUserInput.Select(selectStart, 0);
                    rtbUserInput.SelectionColor = Color.White;
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
        }

        private void rtbUserInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((int)e.KeyChar == 39) 
            {
                //rtbUserInput.Text = rtbUserInput.Text + "'";
            }
        }

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            string[] sintax = rtbUserInput.Lines;
            Utilities.reconizeCode(sintax);
        }
    }
}
