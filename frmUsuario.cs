using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using libUsuarios;

namespace Usuarios
{
    public partial class frmUsuario : Form
    {
        private cUsuarios U;
        private int idUsuario;

        public frmUsuario()
        {
            InitializeComponent();
            U = new cUsuarios();
        }
        private void frmUsuario_Load(object sender, EventArgs e)
        {
            idUsuario = U.ShowLogin(this);
            if (idUsuario >= 1)
            {
                lblId.Text = idUsuario.ToString();
                txtNombre.Text = U.GetNombre(idUsuario);
                txtUsuario.Text = U.GetUsuario(idUsuario);
                string[] Permisos = U.GetDepartamentos(idUsuario);
                foreach (string S in Permisos) listBox1.Items.Add(S);
                btnAceptar.Enabled = false;
            }
            else
            {
                MessageBox.Show("Error");
                this.Close();
            }
        }
        private void frmUsuario_FormClosed(object sender, FormClosedEventArgs e)
        {
            U.Dispose();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                if (U.SetNombre(idUsuario, txtNombre.Text.Trim()) && U.SetUsuario(idUsuario, txtUsuario.Text.Trim()))
                {
                    btnAceptar.Enabled = false;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error");
                }
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (frmSetPass F = new frmSetPass())
            {
                F.ShowDialog(this);
                if (F.Respuesta == System.Windows.Forms.DialogResult.OK)
                {
                    if (U.SetPass(U.GetUsuario(idUsuario), F.PassViejo, F.PassNuevo))
                    {
                        MessageBox.Show("OK");
                    }
                    else
                    {
                        MessageBox.Show("La contraseña anterior no es válida");
                    }
                }
            }
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            btnAceptar.Enabled = true;
        }
        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            btnAceptar.Enabled = true;
        }
        
    }
}
