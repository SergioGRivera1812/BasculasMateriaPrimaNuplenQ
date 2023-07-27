using MySql.Data.MySqlClient;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasculasMateriaPrimaNuplenQ
{
    public partial class Historial : Form
    {
        Conexion cnn;
        Configuracion c = new Configuracion();
        public Historial()
        {
            InitializeComponent();
        }

        private void Historial_Load(object sender, EventArgs e)
        {
            c.Configuracion_Load(sender, e);
            string serverB = c.txtIPServer.Text.ToString();
            string database = c.txtBase.Text.ToString();
            string portB = c.txtPortDB.Text.ToString();
            string user = c.txtUsuario.Text.ToString();
            string password = c.txtPass.Text.ToString();

            cnn = new Conexion(serverB, database, portB, user, password);
            dataGridHistorial.DataSource = load();
        }

        private BindingSource bs = new BindingSource();

        private DataTable load()
        {
            DataTable vista = new DataTable();
            string vista_gral = "select*from Historial;";
            using (MySqlCommand cmd = new MySqlCommand(vista_gral, cnn.getConexion()))
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                vista.Load(reader);
            }
            bs.DataSource = vista;

            return vista;
        }

        private void txtFiltrar_KeyUp(object sender, KeyEventArgs e)
        {
            FilterData();
        }

        void FilterData()
        {
            try
            {
                if (bs.DataSource != null)
                {
                    bs.Filter = $"Codigo LIKE '%{txtFiltrar.Text}%' OR Fecha LIKE '%{txtFiltrar.Text}%'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Filtrando datos...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridHistorial_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridHistorial.Columns[e.ColumnIndex].Name == "Imprimir")
            {
                PrinterSettings ps = new PrinterSettings();
                printDocument1.PrinterSettings = ps;
                printDocument1.PrintPage += Reimpresion;
                printDocument1.Print();
            }
        }

        private void Reimpresion(object sender, PrintPageEventArgs e)
        {
            string Fec = this.dataGridHistorial.SelectedRows[0].Cells[7].Value.ToString();
            string hora = this.dataGridHistorial.SelectedRows[0].Cells[8].Value.ToString();
            string act = this.dataGridHistorial.SelectedRows[0].Cells[4].Value.ToString();
            string tara = this.dataGridHistorial.SelectedRows[0].Cells[5].Value.ToString();
            string neto = this.dataGridHistorial.SelectedRows[0].Cells[6].Value.ToString();
            string nomb = this.dataGridHistorial.SelectedRows[0].Cells[3].Value.ToString();



            // TITULO
            e.Graphics.DrawString("Almacen de Materia Prima", new Font("arial narrow", 10, FontStyle.Bold), new SolidBrush(Color.Black), 0, 2);
            e.Graphics.DrawString("Báscula de Muelles", new Font("arial narrow", 10, FontStyle.Bold), new SolidBrush(Color.Black), 0, 17);

            //PRODUCTO
            e.Graphics.DrawString("Producto : " + nomb, new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 32);

            // BRUTO
            e.Graphics.DrawString("Bruto:", new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 47);
            e.Graphics.DrawString(act, new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 60, 47);
            e.Graphics.DrawString("Kg", new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 170, 47);
            // TARA
            e.Graphics.DrawString("Tara:", new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 62);
            e.Graphics.DrawString(tara, new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 60, 62);
            e.Graphics.DrawString("Kg", new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 170, 62);
            // NETO
            e.Graphics.DrawString("Neto:", new Font("arial narrow", 11, FontStyle.Bold), new SolidBrush(Color.Black), 0, 77);
            e.Graphics.DrawString(neto, new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 60, 77);
            e.Graphics.DrawString("Kg", new Font("courier new", 11, FontStyle.Bold), new SolidBrush(Color.Black), 170, 77);
            // HORA Y FECHA
            // e.Graphics.DrawString("Fecha:", new Font("arial narrow", 10, FontStyle.Bold), new SolidBrush(Color.Black), 0, 1);
            e.Graphics.DrawString(DateTime.Now.ToString(), new Font("courier new", 9, FontStyle.Bold), new SolidBrush(Color.Black), 0, 92);
        }

        private void atrasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SLDocument s1 = new SLDocument();
            int celdaCabecera = 2;

            s1.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Reporte Bascula de Muelles");
            s1.SetCellValue("B" + celdaCabecera, "ID");
            s1.SetCellValue("C" + celdaCabecera, "Codigo");
            s1.SetCellValue("D" + celdaCabecera, "Producto");
            s1.SetCellValue("E" + celdaCabecera, "Bruto");
            s1.SetCellValue("F" + celdaCabecera, "Tara");
            s1.SetCellValue("G" + celdaCabecera, "Neto");
            s1.SetCellValue("H" + celdaCabecera, "Hora");
            s1.SetCellValue("I" + celdaCabecera, "Fecha");

            // Obtener los datos del DataGridView
            DataGridView dgvDatos = dataGridHistorial; // Reemplaza "tuDataGridView" por el nombre de tu DataGridView
            int rowCount = dgvDatos.Rows.Count;
            int colCount = dgvDatos.Columns.Count;

            for (int i = 0; i < rowCount; i++)
            {
                celdaCabecera++;

                for (int j = 1; j < colCount; j++) // Iniciar desde j=1 para omitir la primera columna
                {
                    string valorCelda = dgvDatos.Rows[i].Cells[j].Value.ToString();
                    s1.SetCellValue(celdaCabecera, j + 1, valorCelda); // +1 para compensar la columna de encabezado agregada en Excel
                }
            }

            // Crear el diálogo de guardar archivo
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos de Excel|*.xlsx";
            saveFileDialog.FileName = "Reporte Bascula de Muelles.xlsx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Mostrar el diálogo de guardar archivo
            DialogResult result = saveFileDialog.ShowDialog();

            // Verificar si se ha seleccionado un archivo y se ha presionado "Guardar"
            if (result == DialogResult.OK)
            {
                // Guardar el archivo en la ubicación seleccionada
                s1.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Exportación generada", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Error en la exportación", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
    }
}