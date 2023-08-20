using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace BasculasMateriaPrimaNuplenQ
{
    public partial class Login : Form
    {
        Conexion cnn;
        Configuracion c = new Configuracion();
        public Login()
        {
            InitializeComponent();
        }


        private void Login_Load(object sender, EventArgs e)
        {
            c.Configuracion_Load(sender, e);
            string serverB = c.txtIPServer.Text.ToString();
            string database = c.txtBase.Text.ToString();
            string portB = c.txtPortDB.Text.ToString();
            string user = c.txtUsuario.Text.ToString();
            string password = c.txtPass.Text.ToString();

            cnn = new Conexion(serverB, database, portB, user, password);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
                // Intento de inicio de sesión
                string loginUsername = txtUsuario.Text;
                string loginPassword = txtPass.Text;

                string selectQuery = "SELECT * FROM Usuarios WHERE usuario = @usuario";
                MySqlCommand cmd = new MySqlCommand(selectQuery, cnn.getConexion());
                
                    cmd.Parameters.AddWithValue("@usuario", loginUsername);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {


                            string storedPassword = reader.GetString("Pass");
                            if (storedPassword == loginPassword)
                            {
                                MessageBox.Show("Inicio de sesión exitoso");

                                // Obtener el rol desde la columna "Rol" en el resultado del lector
                                string userRole = reader.GetString("Rol");

                                // Crear y mostrar la ventana principal
                                Form1 form1 = new Form1();
                                form1.Usuario = loginUsername; // Asignar el nombre de usuario
                                form1.Rol = userRole;          // Asignar el rol
                                form1.FormClosed += (s, args) => this.Close(); // Cerrar la aplicación cuando se cierre la ventana principal
                                form1.Show();

                                this.Hide();
                            }

                            else
                            {
                                    MessageBox.Show("Credenciales incorrectas");
                            }
                        
                        }

                        else
                        {
                            MessageBox.Show("Usuario no encontrado");
                        }
                    }
                
            
        }

    }
}
