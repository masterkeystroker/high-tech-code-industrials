using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace libUsuarios
{
    public partial class frmLogin : Form
    {

        public DialogResult Resp;
        public string usuario;
        public string pass;

        public frmLogin()
        {
            InitializeComponent();
            Resp = System.Windows.Forms.DialogResult.Cancel;
            usuario = "";
            pass = "";
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            usuario = txtNombre.Text;
            pass = txtPass.Text;
            Resp = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Resp = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
