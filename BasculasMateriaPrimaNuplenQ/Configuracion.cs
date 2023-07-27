using BasculasMateriaPrimaNuplenQ.Properties;
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
    public partial class Configuracion : Form
    {
        public Configuracion()
        {
            InitializeComponent();
        }

        private void GuardarCOM()
        {
            Settings.Default.IpServer = txtIPServer.Text;
            Settings.Default.Port = txtPortServer.Text;

            Settings.Default.DataBase = txtBase.Text;
            Settings.Default.Usuario = txtUsuario.Text;
            Settings.Default.Password = txtPass.Text;
            Settings.Default.DataPort = txtPortDB.Text;
            Settings.Default.IPMoxa = txtIpMoxa.Text;

            Settings.Default.Save();
        }

        private void CargarConfiguracionCOM()
        {
            txtIPServer.Text = Settings.Default.IpServer;
            txtPortServer.Text = Settings.Default.Port;

            txtBase.Text = Settings.Default.DataBase;
            txtUsuario.Text = Settings.Default.Usuario;
            txtPass.Text = Settings.Default.Password;
            txtPortDB.Text = Settings.Default.DataPort;
            txtIpMoxa.Text = Settings.Default.IPMoxa;


        }

        public void Configuracion_Load(object sender, EventArgs e)
        {
            CargarConfiguracionCOM();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GuardarCOM();
                MessageBox.Show("Configuración Guardada", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }catch(Exception ex)
            {
                MessageBox.Show("Error al guardar "+ex, "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Configuracion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.IpServer = txtIPServer.Text;
            Settings.Default.Port = txtPortServer.Text;
            Settings.Default.DataBase = txtBase.Text;
            Settings.Default.Usuario = txtUsuario.Text;
            Settings.Default.Password = txtPass.Text;
            Settings.Default.DataPort = txtPortDB.Text;
            Settings.Default.IPMoxa = txtIpMoxa.Text;
        }
    }
}
