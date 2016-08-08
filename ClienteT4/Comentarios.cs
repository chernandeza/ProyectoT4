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
/*Crear eventos en bitacora de Windows*/
/*eventcreate /ID 1 /L APPLICATION /T INFORMATION  /SO MYEVENTSOURCE /D "My first log"*/

namespace ClienteT4
{
    public partial class Comentarios : Form
    {
        private RedCliente conexionRed;

        public Comentarios()
        {
            InitializeComponent();
            conexionRed = new RedCliente();
            conexionRed.Connected += ConexionRed_Connected;
            conexionRed.Disconnected += ConexionRed_Disconnected;
            conexionRed.ComentariosRecibidos += ConexionRed_ComentariosRecibidos;
            conexionRed.ListaCedulas += ConexionRed_ListaCedulas;
            conexionRed.ServerError += ConexionRed_ServerError;
            conexionRed.SinComentarios += ConexionRed_SinComentarios;
        }

        private void ConexionRed_SinComentarios(object sender, EventArgs e)
        {
            dgvComentarios.DataSource = null;
        }

        private void ConexionRed_ServerError(object sender, EventArgs e)
        {
            MessageBox.Show("Error en el servidor", "Cliente T4", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ConexionRed_ListaCedulas(object sender, CedulaEventos e)
        {
            cmbCedulas.DataSource = null;
            cmbCedulas.DataSource = e.Cedulas;
            cmbCedulasComent.DataSource = null;
            cmbCedulasComent.DataSource = e.Cedulas;
        }

        private void ConexionRed_ComentariosRecibidos(object sender, ComentarioEventos e)
        {
            dgvComentarios.DataSource = null;
            dgvComentarios.DataSource = e.Comentarios;
            dgvComentarios.Columns["Texto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvComentarios.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvComentarios.Columns.Remove("Cedula");
        }

        private void ConexionRed_Disconnected(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ConexionRed_Connected(object sender, EventArgs e)
        {
            MessageBox.Show("Cliente Conectado", "Cliente T4", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Comentarios_Load(object sender, EventArgs e)
        {
            //conexionRed.ObtenerCedulas();
        }

        private void cmbCedulas_SelectedIndexChanged(object sender, EventArgs e)
        {
            conexionRed.ObtenerComentariosPersona(cmbCedulas.Text);
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            conexionRed.Conectar();
            conexionRed.ObtenerCedulas();
        }

        private void btnGuardarCom_Click(object sender, EventArgs e)
        {
            Comentario nCom = new Comentario();
            nCom.Cedula = cmbCedulasComent.Text;
            nCom.Texto = txtTexto.Text;
            conexionRed.EnviarComentarioAServidor(nCom);
            conexionRed.ObtenerComentariosPersona(cmbCedulas.Text);
        }

        private void cmbCedulasComent_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTexto.Text = "";
        }
    }
}
