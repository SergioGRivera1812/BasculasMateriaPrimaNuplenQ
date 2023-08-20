using MySql.Data.MySqlClient;
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
    public partial class Usuarios : Form
    {
        Conexion cnn;
        Configuracion c = new Configuracion();

        public Usuarios()
        {
            InitializeComponent();
        }

        private void Usuarios_Load(object sender, EventArgs e)
        {
            c.Configuracion_Load(sender, e);

            string serverB = c.txtIPServer.Text.ToString();
            string database = c.txtBase.Text.ToString();
            string portB = c.txtPortDB.Text.ToString();
            string user = c.txtUsuario.Text.ToString();
            string password = c.txtPass.Text.ToString();

            cnn = new Conexion(serverB, database, portB, user, password);
            dataGridUsuarios.DataSource = load();


            dataGridUsuarios.Columns[2].HeaderText = "Apellido M.";
            dataGridUsuarios.Columns[3].HeaderText = "Apellido P.";


        }

        private BindingSource bs = new BindingSource();

        private DataTable load()
        {
            DataTable vista = new DataTable();
            string vista_gral = "select*from Usuarios;";
            using (MySqlCommand cmd = new MySqlCommand(vista_gral, cnn.getConexion()))
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                vista.Load(reader);
            }
            bs.DataSource = vista;

            return vista;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string GuardarEt = "INSERT INTO Usuarios (Nombre,ApellidoP,ApellidoM,Usuario,Pass,Rol) values ('" +
                            txtNombre.Text + "','" + txtApP.Text + "','" + txtApM.Text + "','" + txtUsuario.Text + "','" + txtPass.Text + "','" + comboRol.Text + "');";

                MySqlCommand comando = new MySqlCommand(GuardarEt, cnn.getConexion());
                comando.ExecuteNonQuery();

                MessageBox.Show("Usuario creado exitosamente", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
