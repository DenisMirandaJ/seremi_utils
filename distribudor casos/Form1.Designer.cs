
namespace run_python_script_argv
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openCorteButton = new System.Windows.Forms.Button();
            this.OpenTrazadoresButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.corteTextBox = new System.Windows.Forms.TextBox();
            this.trazadoresTextBox = new System.Windows.Forms.TextBox();
            this.corteFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.trazadoresFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.submitButton = new System.Windows.Forms.Button();
            this.casosPorTrazadorNumericSelect = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.casosPorTrazadorNumericSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // openCorteButton
            // 
            this.openCorteButton.Location = new System.Drawing.Point(494, 46);
            this.openCorteButton.Name = "openCorteButton";
            this.openCorteButton.Size = new System.Drawing.Size(75, 23);
            this.openCorteButton.TabIndex = 0;
            this.openCorteButton.Text = "Seleccionar";
            this.openCorteButton.UseVisualStyleBackColor = true;
            this.openCorteButton.Click += new System.EventHandler(this.openCorteButton_Click);
            // 
            // OpenTrazadoresButton
            // 
            this.OpenTrazadoresButton.Location = new System.Drawing.Point(494, 96);
            this.OpenTrazadoresButton.Name = "OpenTrazadoresButton";
            this.OpenTrazadoresButton.Size = new System.Drawing.Size(75, 23);
            this.OpenTrazadoresButton.TabIndex = 1;
            this.OpenTrazadoresButton.Text = "Seleccionar";
            this.OpenTrazadoresButton.UseVisualStyleBackColor = true;
            this.OpenTrazadoresButton.Click += new System.EventHandler(this.OpenTrazadoresButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Corte TTA";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Archivo trazadores";
            // 
            // corteTextBox
            // 
            this.corteTextBox.Location = new System.Drawing.Point(39, 46);
            this.corteTextBox.Name = "corteTextBox";
            this.corteTextBox.Size = new System.Drawing.Size(449, 23);
            this.corteTextBox.TabIndex = 4;
            // 
            // trazadoresTextBox
            // 
            this.trazadoresTextBox.Location = new System.Drawing.Point(39, 96);
            this.trazadoresTextBox.Name = "trazadoresTextBox";
            this.trazadoresTextBox.Size = new System.Drawing.Size(449, 23);
            this.trazadoresTextBox.TabIndex = 5;
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(471, 195);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(98, 37);
            this.submitButton.TabIndex = 6;
            this.submitButton.Text = "Empezar";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // casosPorTrazadorNumericSelect
            // 
            this.casosPorTrazadorNumericSelect.Location = new System.Drawing.Point(39, 150);
            this.casosPorTrazadorNumericSelect.Name = "casosPorTrazadorNumericSelect";
            this.casosPorTrazadorNumericSelect.Size = new System.Drawing.Size(448, 23);
            this.casosPorTrazadorNumericSelect.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(287, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Casos a asignar por trazador (0 para asignarlos todos)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 244);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.casosPorTrazadorNumericSelect);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.trazadoresTextBox);
            this.Controls.Add(this.corteTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenTrazadoresButton);
            this.Controls.Add(this.openCorteButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.casosPorTrazadorNumericSelect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openCorteButton;
        private System.Windows.Forms.Button OpenTrazadoresButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox corteTextBox;
        private System.Windows.Forms.TextBox trazadoresTextBox;
        private System.Windows.Forms.OpenFileDialog corteFileDialog;
        private System.Windows.Forms.OpenFileDialog trazadoresFileDialog;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.NumericUpDown casosPorTrazadorNumericSelect;
        private System.Windows.Forms.Label label3;
    }
}

