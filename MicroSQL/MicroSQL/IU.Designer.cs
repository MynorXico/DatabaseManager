namespace MicroSQL
{
    partial class IU
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IU));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tvTables = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.rtbUserInput = new System.Windows.Forms.RichTextBox();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExport = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pbNumbering = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNumbering)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(499, 453);
            this.dataGridView1.TabIndex = 2;
            // 
            // tvTables
            // 
            this.tvTables.ImageIndex = 0;
            this.tvTables.ImageList = this.imageList1;
            this.tvTables.Location = new System.Drawing.Point(21, 46);
            this.tvTables.Name = "tvTables";
            this.tvTables.SelectedImageIndex = 0;
            this.tvTables.Size = new System.Drawing.Size(239, 486);
            this.tvTables.TabIndex = 3;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Select Table-96.png");
            this.imageList1.Images.SetKeyName(1, "Variable-96[1].png");
            this.imageList1.Images.SetKeyName(2, "Type-96[1].png");
            // 
            // rtbUserInput
            // 
            this.rtbUserInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rtbUserInput.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbUserInput.Location = new System.Drawing.Point(307, 47);
            this.rtbUserInput.Name = "rtbUserInput";
            this.rtbUserInput.Size = new System.Drawing.Size(302, 485);
            this.rtbUserInput.TabIndex = 4;
            this.rtbUserInput.Text = "";
            this.rtbUserInput.TextChanged += new System.EventHandler(this.rtbUserInput_TextChanged);
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.Location = new System.Drawing.Point(374, 554);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(75, 23);
            this.btnEjecutar.TabIndex = 5;
            this.btnEjecutar.Text = "Ejecutar";
            this.btnEjecutar.UseVisualStyleBackColor = true;
            this.btnEjecutar.Click += new System.EventHandler(this.btnEjecutar_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(615, 47);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(513, 485);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(505, 459);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Results";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1140, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.loadToolStripMenuItem.Text = "Load dictionary";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(642, 538);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(86, 23);
            this.btnExport.TabIndex = 8;
            this.btnExport.Text = "Exportar *.csv";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(734, 538);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Exportar *.xls";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pbNumbering
            // 
            this.pbNumbering.Location = new System.Drawing.Point(272, 47);
            this.pbNumbering.Name = "pbNumbering";
            this.pbNumbering.Size = new System.Drawing.Size(27, 485);
            this.pbNumbering.TabIndex = 10;
            this.pbNumbering.TabStop = false;
            this.pbNumbering.Paint += new System.Windows.Forms.PaintEventHandler(this.pbNumbering_Paint);
            // 
            // IU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 589);
            this.Controls.Add(this.pbNumbering);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnEjecutar);
            this.Controls.Add(this.rtbUserInput);
            this.Controls.Add(this.tvTables);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IU";
            this.Text = "Data Base Manager";
            this.Load += new System.EventHandler(this.IU_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNumbering)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TreeView tvTables;
        private System.Windows.Forms.RichTextBox rtbUserInput;
        private System.Windows.Forms.Button btnEjecutar;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pbNumbering;
    }
}

