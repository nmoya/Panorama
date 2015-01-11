namespace Paranoya
{
    partial class parametros
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.limiarCantos = new System.Windows.Forms.TextBox();
            this.maxCantos = new System.Windows.Forms.TextBox();
            this.janelaCor = new System.Windows.Forms.TextBox();
            this.toleranciaCor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nroMaioresCantos = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Salvar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Limiar Cantos";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Maximo Cantos [100,500]";
            // 
            // limiarCantos
            // 
            this.limiarCantos.Location = new System.Drawing.Point(149, 24);
            this.limiarCantos.Name = "limiarCantos";
            this.limiarCantos.Size = new System.Drawing.Size(123, 20);
            this.limiarCantos.TabIndex = 3;
            // 
            // maxCantos
            // 
            this.maxCantos.Location = new System.Drawing.Point(149, 50);
            this.maxCantos.Name = "maxCantos";
            this.maxCantos.Size = new System.Drawing.Size(123, 20);
            this.maxCantos.TabIndex = 4;
            // 
            // janelaCor
            // 
            this.janelaCor.Location = new System.Drawing.Point(149, 115);
            this.janelaCor.Name = "janelaCor";
            this.janelaCor.Size = new System.Drawing.Size(123, 20);
            this.janelaCor.TabIndex = 8;
            // 
            // toleranciaCor
            // 
            this.toleranciaCor.Location = new System.Drawing.Point(149, 89);
            this.toleranciaCor.Name = "toleranciaCor";
            this.toleranciaCor.Size = new System.Drawing.Size(123, 20);
            this.toleranciaCor.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Janela Cores Vizinhas";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Tolerância Cor [0,255]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Nro de maiores cantos:";
            // 
            // nroMaioresCantos
            // 
            this.nroMaioresCantos.Location = new System.Drawing.Point(149, 151);
            this.nroMaioresCantos.Name = "nroMaioresCantos";
            this.nroMaioresCantos.Size = new System.Drawing.Size(123, 20);
            this.nroMaioresCantos.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(146, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Default 1. (Maior canto)";
            // 
            // parametros
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nroMaioresCantos);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.janelaCor);
            this.Controls.Add(this.toleranciaCor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxCantos);
            this.Controls.Add(this.limiarCantos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "parametros";
            this.Text = "parametros";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox limiarCantos;
        public System.Windows.Forms.TextBox maxCantos;
        public System.Windows.Forms.TextBox janelaCor;
        public System.Windows.Forms.TextBox toleranciaCor;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox nroMaioresCantos;
        private System.Windows.Forms.Label label6;
    }
}