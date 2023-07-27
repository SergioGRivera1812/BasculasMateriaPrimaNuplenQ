using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasculasMateriaPrimaNuplenQ
{
    public partial class Password2 : Form
    {
        public Password2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pass = "admin";

            if (txtPass.Text == pass)
            {
                Configuracion c = new Configuracion();
                c.Show();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Contraseña Incorrecta", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Dispose();
            }
        }
    }
}
