using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibreriaT4;

namespace ServidorT4
{
    public partial class FormServidor : Form
    {
        private RedServidor conexionServer;
        Task taskAsync;
        private int ContadorClientes;
        private List<Comentario> lCom;

        public FormServidor()
        {
            InitializeComponent();
            conexionServer = new RedServidor();
            conexionServer.ClientConnected += ConexionServer_ClientConnected;
            conexionServer.ClientDisconnected += ConexionServer_ClientDisconnected;
            conexionServer.ComentarioRecibido += ConexionServer_ComentarioRecibido;
            ContadorClientes = 0;
            lCom = new List<Comentario>();
        }

        private void ConexionServer_ComentarioRecibido(object sender, ComentarioGUI e)
        {
            lCom.Add(e.Coment);
            this.BeginInvoke(new MethodInvoker(delegate
            {
                dgvComRec.DataSource = null;
                dgvComRec.DataSource = lCom;
                dgvComRec.Columns["Texto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComRec.Columns["Cedula"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvComRec.Columns.Remove("ID");
            }));
        }

        private void ConexionServer_ClientDisconnected(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                ContadorClientes -= 1;
                lblClientesCon.Text = ContadorClientes.ToString();
            }));
        }

        private void ConexionServer_ClientConnected(object sender, EventArgs e)
        {
            //MessageBox.Show("Nuevo Cliente Conectado");
            this.BeginInvoke(new MethodInvoker(delegate
            {
                ContadorClientes += 1;
                lblClientesCon.Text = ContadorClientes.ToString();
            }));
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (conexionServer.running)
            {
                DialogResult result = MessageBox.Show("Seguro que desea apagar el servidor?", "Servidor T4", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    taskAsync.Dispose();
                    conexionServer.End();
                }
            }
            Application.Exit();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (!conexionServer.running)
            {
                taskAsync = new Task(conexionServer.Initialize, TaskCreationOptions.LongRunning);
                taskAsync.Start();
                MessageBox.Show("Servidor Iniciado...", "Servidor T4", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnCerrar.Enabled = true;
            }
        }

        private void FormServidor_Load(object sender, EventArgs e)
        {
            btnCerrar.Enabled = false;
        }
    }
}
