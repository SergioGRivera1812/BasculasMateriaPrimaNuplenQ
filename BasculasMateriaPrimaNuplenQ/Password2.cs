using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasculasMateriaPrimaNuplenQ
{
    public partial class Password2 : Form
    {
        System.Diagnostics.Process oskProcess = null;


        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public object SelectedValue { get; }
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

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                closeOnscreenKeyboard();
            }
        }

        private void Password2_Load(object sender, EventArgs e)
        {
            
        }

        private void closeOnscreenKeyboard()
        {
            int iHandle = FindWindow("IPTIP_Main_Window", "");
            if (iHandle > 0)
            {

                SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }

        private void txtPass_Click(object sender, EventArgs e)
        {
            string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";
            string keyboardPath = Path.Combine(progFiles, "TabTip.exe");
            oskProcess = Process.Start(keyboardPath);

        }
    }
}
