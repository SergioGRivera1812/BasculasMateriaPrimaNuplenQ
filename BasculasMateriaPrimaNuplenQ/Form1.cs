using MySql.Data.MySqlClient;
using SimpleTCP;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BasculasMateriaPrimaNuplenQ
{
    public partial class Form1 : Form
    {
        Conexion cnn;
        Configuracion c = new Configuracion();
        System.Diagnostics.Process oskProcess = null;


        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        public string Usuario { get; set; }
        public string Rol { get; set; }

        public object SelectedValue { get; }

        public Form1()
        {
            InitializeComponent();
        }
        SimpleTcpClient client;
        private void Form1_Load(object sender, EventArgs e)
        {
            // Ruta del archivo de texto
            string archivoTexto = "C:\\Windows\\keyLog.txt";

            if (File.Exists(archivoTexto))
            {
                // Leer el contenido del archivo
                string contenido = File.ReadAllText(archivoTexto);

                // Verificar si el contenido es válido (por ejemplo, comparándolo con una cadena esperada)
                string contenidoEsperado = "NuplenQuimicos//SergioRivera//";
                if (contenido.Trim().Equals(contenidoEsperado))
                {
                    // El contenido es válido, continuar con la ejecución normal
                }
                else
                {
                    // El contenido es inválido, mostrar un mensaje y cerrar la aplicación
                    MessageBox.Show("El contenido del archivo es inválido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {
                // El archivo no existe, mostrar un mensaje y cerrar la aplicación
                MessageBox.Show("El archivo no fue encontrado. Contacte al desarrollador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }


            c.Configuracion_Load(sender, e);
            string ipServer = c.txtIpMoxa.Text;
            int portServer = Convert.ToInt32(c.txtPortServer.Text);
            /*
            client = new SimpleTcpClient();
            client.DataReceived += Client_DataReceived;
            client.StringEncoder = Encoding.ASCII; // Ajusta el encoding según el protocolo utilizado por el Moxa NPort
            client.Connect(ipServer, portServer); // Ingresa la dirección IP y el puerto del Moxa NPort
           */
            string serverB = c.txtIPServer.Text.ToString();
            string database = c.txtBase.Text.ToString();
            string portB = c.txtPortDB.Text.ToString();
            string user = c.txtUsuario.Text.ToString();
            string password = c.txtPass.Text.ToString();

            cnn = new Conexion(serverB, database, portB, user, password);
            dataGridHistorial.DataSource = Vista();
            lblUsuario.Text =  Usuario;
            lblRol.Text = Rol;

            switch (lblRol.Text)
            {
                case "Admin":
                    MenuConf.Enabled = true;
                    MenuUsua.Enabled = true;
                    MenuHist.Enabled = true;
                    break;

                case "Supervisor":
                    MenuConf.Enabled = true;
                    MenuUsua.Enabled = true;
                    MenuHist.Enabled = true;
                    break;

                case "Operador":
                    MenuConf.Enabled = false;
                    MenuUsua.Enabled = false;
                    MenuHist.Enabled = true;
                    break;
            }



        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            string mensaje = e.MessageString;


            mensaje = mensaje.Replace("(kg)", "");
            mensaje = mensaje.Replace("=", "");
            string cadenaLimpia = mensaje.Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("", "").Replace(" ", "").Replace("\r",string.Empty);
            // Eliminar los ceros no significativos antes del punto decimal
            cadenaLimpia = cadenaLimpia.TrimStart('0');

            if (cadenaLimpia.StartsWith("."))

                // Comprobar si solo hay un dígito después del punto decimal
                cadenaLimpia = "0" + cadenaLimpia;  // Agregar un cero antes del punto decimal si es necesario
            else
                cadenaLimpia = cadenaLimpia.TrimStart('.');  // Eliminar el punto decimal inicial si hay más de un dígito después de él

            // Asegurarse de realizar la operación en el hilo de la interfaz de usuario
            Invoke((MethodInvoker)delegate
            {
                lblIndicador.Text = cadenaLimpia;

            });

        }


        private void button2_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtTara.Text   = "";
            txtBruto.Text = "";
            txtNeto.Text = "";
        }

        

        private BindingSource bsv = new BindingSource();

        private DataTable Vista()
        {
            DataTable vista = new DataTable();
            string vista_gral = "select*from VistaHistorial;";
            using (MySqlCommand cmd = new MySqlCommand(vista_gral, cnn.getConexion()))
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                vista.Load(reader);
            }
            bsv.DataSource = vista;

            return vista;
        }


        private void button1_Click(object sender, EventArgs e)
        {

            string valorBusqueda = txtCodigo.Text;

            try
            {
                using (MySqlConnection conexion = cnn.getConexion())
                {
                    if (conexion.State == ConnectionState.Closed)
                    {
                        conexion.Open();
                    }

                    string consulta = "SELECT Producto FROM Codigos WHERE Codigo = @valorBusqueda;";
                    MySqlCommand comando = new MySqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@valorBusqueda", valorBusqueda);

                    object resultado = comando.ExecuteScalar();

                    if (resultado != null)
                    {
                        txtNombre.Text = resultado.ToString();
                    }
                    else
                    {
                        txtNombre.Text = "No se encontró ningún resultado.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar la búsqueda: " + ex.Message);
            }



        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodigo.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtTara.Text))
            {
                MessageBox.Show("Verifique que todos los campos esten llenos para poder imprimir","AVISO",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                string selectIdQuery = "SELECT ID FROM Usuarios WHERE Usuario = @usuario";
                MySqlCommand idCmd = new MySqlCommand(selectIdQuery, cnn.getConexion());
                idCmd.Parameters.AddWithValue("@usuario", lblUsuario.Text);

                object userIdResult = idCmd.ExecuteScalar();
                if (userIdResult != null && userIdResult != DBNull.Value)
                {
                    int userId = Convert.ToInt32(userIdResult);

                    string GuardarEt = "INSERT INTO Historial (Codigo,Producto,Bruto,Tara,Neto,Fecha,Hora,IdUsuario) values ('" +
                        txtCodigo.Text + "','" + txtNombre.Text + "','" + txtBruto.Text + "','" + txtTara.Text + "','" + txtNeto.Text + "','" + DateTime.Now.ToString("d") + "','" + DateTime.Now.ToString("t") + "','" + userId + "');";

                    MySqlCommand comando = new MySqlCommand(GuardarEt, cnn.getConexion());
                    comando.ExecuteNonQuery();

                    PrinterSettings ps = new PrinterSettings();
                    printDocument1.PrinterSettings = ps;
                    printDocument1.PrintPage += Imprimir;
                    printDocument1.Print();

                    dataGridHistorial.DataSource = Vista();

                    txtTara.Text = string.Empty;
                    txtNeto.Text = string.Empty;
                    txtBruto.Text = string.Empty;
                    txtNombre.Text = string.Empty;
                    txtCodigo.Text = string.Empty;
                }
            }



        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            double bruto = Convert.ToDouble(lblIndicador.Text.ToString());
            double neto;

            if (!string.IsNullOrEmpty(txtTara.Text))
            {
                if (double.Parse(txtTara.Text) >= bruto)
                {
                    MessageBox.Show("Tara Incorrecta");
                   
                }
                else
                {
                    neto = bruto - double.Parse(txtTara.Text);
                    txtNeto.Text = neto.ToString();
                    txtBruto.Text = bruto.ToString();
                }
            }
            else
            {
                MessageBox.Show("Favor de Ingresar Tara");
            }

            
        }

        private void Imprimir(object sender, PrintPageEventArgs e)
        {
            try
            {
                // TITULO
                e.Graphics.DrawString("Almacen de Materia Prima", new Font("arial narrow", 10, FontStyle.Bold), new SolidBrush(Color.Black), 0, 2);
                e.Graphics.DrawString("Báscula de Muelles", new Font("arial narrow", 10, FontStyle.Bold), new SolidBrush(Color.Black), 0, 17);

                //PRODUCTO
                e.Graphics.DrawString("Producto : " + txtNombre.Text, new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 32);

                // BRUTO
                e.Graphics.DrawString("Bruto:", new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 47);
                e.Graphics.DrawString(txtBruto.Text, new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 60, 47);
                e.Graphics.DrawString("Kg", new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 170, 47);
                // TARA
                e.Graphics.DrawString("Tara:", new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 62);
                e.Graphics.DrawString(txtTara.Text, new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 60, 62);
                e.Graphics.DrawString("Kg", new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 170, 62);
                // NETO
                e.Graphics.DrawString("Neto:", new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 77);
                e.Graphics.DrawString(txtNeto.Text, new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 60, 77);
                e.Graphics.DrawString("Kg", new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 170, 77);
                // HORA Y FECHA
                // e.Graphics.DrawString("Fecha:", new Font("arial narrow", 10, FontStyle.Bold), new SolidBrush(Color.Black), 0, 1);
                e.Graphics.DrawString(DateTime.Now.ToString(), new Font("courier new", 9, FontStyle.Bold), new SolidBrush(Color.Black), 0, 92);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Administrador", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void historialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Password p = new Password();
            p.Show();
        }

        private void txtCodigo_Click(object sender, EventArgs e)
        {
            string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";
            string keyboardPath = Path.Combine(progFiles, "TabTip.exe");
            oskProcess = Process.Start(keyboardPath);


        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void configuraciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Password2 p = new Password2();
            p.Show();
        }

        private void txtTara_Click(object sender, EventArgs e)
        {
            string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";
            string keyboardPath = Path.Combine(progFiles, "TabTip.exe");
            oskProcess = Process.Start(keyboardPath);

        }

        private void historialToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Password p = new Password();
            p.Show();
        }

        private void configuracionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Password2 p = new Password2();
            p.Show();
        }

        private void salirToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        
        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string valorBusqueda = txtCodigo.Text;

                try
                {
                    using (MySqlConnection conexion = cnn.getConexion())
                    {
                        if (conexion.State == ConnectionState.Closed)
                        {
                            conexion.Open();
                        }

                        string consulta = "SELECT Producto FROM Codigos WHERE Codigo = @valorBusqueda;";
                        MySqlCommand comando = new MySqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@valorBusqueda", valorBusqueda);

                        object resultado = comando.ExecuteScalar();

                        if (resultado != null)
                        {
                            txtNombre.Text = resultado.ToString();
                            closeOnscreenKeyboard();
                        }
                        else
                        {
                            txtNombre.Text = "No se encontró ningún resultado.";
                            closeOnscreenKeyboard();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al realizar la búsqueda: " + ex.Message);
                }

            }
        }

        private void closeOnscreenKeyboard()
        {

            int iHandle = FindWindow("IPTIP_Main_Window", "");
            if (iHandle > 0)
            {

                SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }

        private void txtTara_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {

                double bruto = Convert.ToDouble(lblIndicador.Text.ToString());
                double neto;

                if (!string.IsNullOrEmpty(txtTara.Text))
                {
                    if (double.Parse(txtTara.Text) >= bruto)
                    {
                        MessageBox.Show("Tara Incorrecta");
                    }
                    else
                    {
                        neto = bruto - double.Parse(txtTara.Text);
                        txtNeto.Text = neto.ToString();
                        txtBruto.Text = bruto.ToString();
                        closeOnscreenKeyboard();
                    }
                }
                else
                {
                    MessageBox.Show("Favor de Ingresar Tara");
                    closeOnscreenKeyboard();
                }
            }
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Usuarios s = new Usuarios();
            s.Show();
        }
    }
}
