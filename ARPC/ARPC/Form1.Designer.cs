namespace ARPC
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonGenerare = new System.Windows.Forms.Button();
            this.buttonIncarca = new System.Windows.Forms.Button();
            this.comboBoxGenerare = new System.Windows.Forms.ComboBox();
            this.labelGenerare = new System.Windows.Forms.Label();
            this.labelSemiGrupa = new System.Windows.Forms.Label();
            this.comboBoxSemiGrupa = new System.Windows.Forms.ComboBox();
            this.gridViewPlanificare = new System.Windows.Forms.DataGridView();
            this.labelNumarGenerari = new System.Windows.Forms.Label();
            this.checkBoxGenerareMultipla = new System.Windows.Forms.CheckBox();
            this.textBoxNumarGenerari = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPlanificare)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonGenerare
            // 
            this.buttonGenerare.Location = new System.Drawing.Point(12, 12);
            this.buttonGenerare.Name = "buttonGenerare";
            this.buttonGenerare.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerare.TabIndex = 0;
            this.buttonGenerare.Text = "Generare";
            this.buttonGenerare.UseVisualStyleBackColor = true;
            this.buttonGenerare.Click += new System.EventHandler(this.ButtonGenerare_Click);
            // 
            // buttonIncarca
            // 
            this.buttonIncarca.Location = new System.Drawing.Point(713, 12);
            this.buttonIncarca.Name = "buttonIncarca";
            this.buttonIncarca.Size = new System.Drawing.Size(75, 23);
            this.buttonIncarca.TabIndex = 1;
            this.buttonIncarca.Text = "Incarca";
            this.buttonIncarca.UseVisualStyleBackColor = true;
            this.buttonIncarca.Click += new System.EventHandler(this.ButtonIncarca_Click);
            // 
            // comboBoxGenerare
            // 
            this.comboBoxGenerare.FormattingEnabled = true;
            this.comboBoxGenerare.Location = new System.Drawing.Point(318, 13);
            this.comboBoxGenerare.Name = "comboBoxGenerare";
            this.comboBoxGenerare.Size = new System.Drawing.Size(121, 21);
            this.comboBoxGenerare.TabIndex = 2;
            // 
            // labelGenerare
            // 
            this.labelGenerare.AutoSize = true;
            this.labelGenerare.Location = new System.Drawing.Point(208, 17);
            this.labelGenerare.Name = "labelGenerare";
            this.labelGenerare.Size = new System.Drawing.Size(104, 13);
            this.labelGenerare.TabIndex = 3;
            this.labelGenerare.Text = "Vizualizare Generare";
            // 
            // labelSemiGrupa
            // 
            this.labelSemiGrupa.AutoSize = true;
            this.labelSemiGrupa.Location = new System.Drawing.Point(455, 17);
            this.labelSemiGrupa.Name = "labelSemiGrupa";
            this.labelSemiGrupa.Size = new System.Drawing.Size(59, 13);
            this.labelSemiGrupa.TabIndex = 5;
            this.labelSemiGrupa.Text = "SemiGrupa";
            // 
            // comboBoxSemiGrupa
            // 
            this.comboBoxSemiGrupa.FormattingEnabled = true;
            this.comboBoxSemiGrupa.Location = new System.Drawing.Point(520, 14);
            this.comboBoxSemiGrupa.Name = "comboBoxSemiGrupa";
            this.comboBoxSemiGrupa.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSemiGrupa.TabIndex = 4;
            // 
            // gridViewPlanificare
            // 
            this.gridViewPlanificare.AllowUserToAddRows = false;
            this.gridViewPlanificare.AllowUserToDeleteRows = false;
            this.gridViewPlanificare.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridViewPlanificare.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridViewPlanificare.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridViewPlanificare.Location = new System.Drawing.Point(12, 60);
            this.gridViewPlanificare.Name = "gridViewPlanificare";
            this.gridViewPlanificare.ReadOnly = true;
            this.gridViewPlanificare.Size = new System.Drawing.Size(776, 378);
            this.gridViewPlanificare.TabIndex = 6;
            // 
            // labelNumarGenerari
            // 
            this.labelNumarGenerari.AutoSize = true;
            this.labelNumarGenerari.Location = new System.Drawing.Point(93, 32);
            this.labelNumarGenerari.Name = "labelNumarGenerari";
            this.labelNumarGenerari.Size = new System.Drawing.Size(47, 13);
            this.labelNumarGenerari.TabIndex = 8;
            this.labelNumarGenerari.Text = "Nr. Gen.";
            // 
            // checkBoxGenerareMultipla
            // 
            this.checkBoxGenerareMultipla.AutoSize = true;
            this.checkBoxGenerareMultipla.Location = new System.Drawing.Point(96, 12);
            this.checkBoxGenerareMultipla.Name = "checkBoxGenerareMultipla";
            this.checkBoxGenerareMultipla.Size = new System.Drawing.Size(112, 17);
            this.checkBoxGenerareMultipla.TabIndex = 9;
            this.checkBoxGenerareMultipla.Text = "Generare Multipla ";
            this.checkBoxGenerareMultipla.UseVisualStyleBackColor = true;
            // 
            // textBoxNumarGenerari
            // 
            this.textBoxNumarGenerari.Location = new System.Drawing.Point(146, 29);
            this.textBoxNumarGenerari.Name = "textBoxNumarGenerari";
            this.textBoxNumarGenerari.Size = new System.Drawing.Size(56, 20);
            this.textBoxNumarGenerari.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxNumarGenerari);
            this.Controls.Add(this.checkBoxGenerareMultipla);
            this.Controls.Add(this.labelNumarGenerari);
            this.Controls.Add(this.gridViewPlanificare);
            this.Controls.Add(this.labelSemiGrupa);
            this.Controls.Add(this.comboBoxSemiGrupa);
            this.Controls.Add(this.labelGenerare);
            this.Controls.Add(this.comboBoxGenerare);
            this.Controls.Add(this.buttonIncarca);
            this.Controls.Add(this.buttonGenerare);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPlanificare)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGenerare;
        private System.Windows.Forms.Button buttonIncarca;
        private System.Windows.Forms.ComboBox comboBoxGenerare;
        private System.Windows.Forms.Label labelGenerare;
        private System.Windows.Forms.Label labelSemiGrupa;
        private System.Windows.Forms.ComboBox comboBoxSemiGrupa;
        private System.Windows.Forms.DataGridView gridViewPlanificare;
        private System.Windows.Forms.Label labelNumarGenerari;
        private System.Windows.Forms.CheckBox checkBoxGenerareMultipla;
        private System.Windows.Forms.TextBox textBoxNumarGenerari;
    }
}

