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
    public partial class Password : Form
    {
        public Password()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pass = "admin";

            if (txtPass.Text == pass)
            {
                Historial h = new Historial();
                h.Show();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Contraseña Incorrecta","AVISO",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                this.Dispose();
            }
        }

        private void Password_Load(object sender, EventArgs e)
        {

        }
    }
}
