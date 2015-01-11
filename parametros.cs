using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Paranoya
{
    public partial class parametros : Form
    {
        public parametros()
        {
            InitializeComponent(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.limiarCanto = Math.Abs(Double.Parse(this.limiarCantos.Text)) * -1;
            Form1.nroMaxCantos = Int32.Parse(this.maxCantos.Text);
            Form1.toleranciaCor = Double.Parse(this.toleranciaCor.Text);
            Form1.tamJanelaCor = Int32.Parse(this.janelaCor.Text);
            Form1.nroMaioresCantos = Int32.Parse(this.nroMaioresCantos.Text);

            this.Dispose();
        }
    }
}
