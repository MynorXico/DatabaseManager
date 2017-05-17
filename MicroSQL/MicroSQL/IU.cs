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
using System.IO;
using Microsoft.Office.Interop.Excel;
namespace MicroSQL
{
    public partial class IU : Form
    {
        int caracter; // Número de caracteres del RichTextBox
        public IU()
        {
            InitializeComponent();
            Utilities.Initialize();
            TableManagment.FillTreeeView(tvTables, Utilities.CompleteDefaultTreesFolder, Utilities.CompleteDefaultTablesFolder);
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
            Utilities.RecognizeCode(SIntax, dataGridView1);
            TableManagment.FillTreeeView(tvTables, Utilities.CompleteDefaultTreesFolder, Utilities.CompleteDefaultTablesFolder);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            List<string> Data = new List<string>();

            StringBuilder LineaActual = new StringBuilder();
            // Se añade encabezados
            foreach(DataGridViewColumn Column in dataGridView1.Columns)
            {
                LineaActual.Append(Column.HeaderText);
                LineaActual.Append(",");
            }
            Data.Add(LineaActual.ToString());

            // Se añade elementos
            for(int i = 0; i < dataGridView1.RowCount; i++)
            {
                LineaActual = new StringBuilder();
                for(int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    LineaActual.Append(dataGridView1[j, i].Value);
                    LineaActual.Append(",");
                }
                Data.Add(LineaActual.ToString());                
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.CSV| Comma Separated Values";
            sfd.ShowDialog();

            try
            {
                File.WriteAllLines(sfd.FileName + ".csv", Data.ToArray());
            }
            catch
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*xls| Excel";
            sfd.ShowDialog();

            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misvalue = System.Reflection.Missing.Value;
            try
            {
                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                for(int i = 1; i <= dataGridView1.Columns.Count; i++)
                {
                    oSheet.Cells[1, i] = dataGridView1.Columns[i-1].HeaderText;
                    for (int j = 1; j <= dataGridView1.Rows.Count; j++)
                    {
                        oSheet.Cells[j+1, i] = dataGridView1[i-1, j-1].Value;
                    }
                }

                //Format A1:D1 as bold, vertical alignment = center.
                int totalWidth = dataGridView1.Columns.Count;
                int totalHeight = dataGridView1.Rows.Count;

                string FinalColumn = ((char)(((int)'A') + totalWidth - 1)).ToString();
                string FinallCell = ((char)((int)'A' + totalWidth-1)).ToString()+int.Parse((totalHeight+1).ToString());

                string to = $"A{dataGridView1.Columns.Count}";
                oSheet.get_Range("A1", FinalColumn + "1").Style = "Accent2"; 
                oSheet.get_Range("A1", FinalColumn+"1").Font.Bold = true;

                oSheet.get_Range("A1", FinallCell).EntireColumn.AutoFit();
                oSheet.get_Range("A2", FinallCell).Style = "40% - Accent2";


                oXL.Visible = false;
                oXL.UserControl = false;
                


                
                oWB.SaveAs(sfd.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                                        false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                
                oWB.Close();
            }
            catch
            {

            }
        }

        private void IU_Load(object sender, EventArgs e)
        {
            timer1.Interval = 10;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pbNumbering.Refresh();
        }

        private void pbNumbering_Paint(object sender, PaintEventArgs e)
        {
            caracter = 0;
            int altura = rtbUserInput.GetPositionFromCharIndex(0).Y;
            if (rtbUserInput.Lines.Length > 0)
                for (int i = 0; i < rtbUserInput.Lines.Length; i++)
                {
                    e.Graphics.DrawString((i + 1).ToString(), rtbUserInput.Font, Brushes.Blue, pbNumbering.Width - (e.Graphics.MeasureString("1", RichTextBox.DefaultFont).Width + 10), altura);
                    caracter += rtbUserInput.Lines[i].Length + 1;
                    altura = rtbUserInput.GetPositionFromCharIndex(caracter).Y;
                }
            else
                e.Graphics.DrawString(1.ToString(), rtbUserInput.Font, Brushes.Blue, pbNumbering.Width - (e.Graphics.MeasureString(1.ToString(), rtbUserInput.Font).Width + 10), altura);


        }
    }
}
