using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Usuarios
{
    public partial class frmSetPass : Form
    {

        public DialogResult Respuesta;
        public string PassViejo;
        public string PassNuevo;

        public frmSetPass()
        {
            InitializeComponent();
            Respuesta = System.Windows.Forms.DialogResult.Cancel;
            PassViejo = "";
            PassNuevo = "";
        }
        private void frmSetPass_Load(object sender, EventArgs e)
        {

        }

        private void txtNueva1_TextChanged(object sender, EventArgs e)
        {
            txtNueva2_TextChanged(sender, e);
        }
        private void txtNueva2_TextChanged(object sender, EventArgs e)
        {
            if (txtNueva1.Text == "" || txtNueva2.Text == "" || txtNueva1.Text != txtNueva2.Text)
            {
                btnAceptar.Enabled = false;
            }
            else
            {
                btnAceptar.Enabled = true;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Respuesta = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Respuesta = System.Windows.Forms.DialogResult.OK;
            PassViejo = txtAntes.Text;
            PassNuevo = txtNueva1.Text;
            this.Close();
        }

    }
}
