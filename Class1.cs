using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using MySql.Conexion;

namespace libUsuarios
{


    public class cUsuarios : IDisposable 
    {

        private cMySql DB;
        private int _idUsuario;
        private string _Usuario;

        public cUsuarios()
        {
            //Conecta
            _idUsuario = -1;
            _Usuario = "";
            DB = new cMySql("cretaofitec", "usuarios", "root", "12550", false, false);
        }
        public void Dispose()
        {
            DB.Dispose();
        }
        ~cUsuarios()
        {
            this.Dispose();
        }

        /// <summary>
        /// Hace un login en el sistema
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="pass"></param>
        /// <returns>-1 en caso de que el usuario o pass no sean correctos, o el Id de usuario</returns>
        public int Login(string usuario, string pass)
        {
            try
            {
                int i = Convert.ToInt32(DB.ExecuteScalar("SELECT id FROM usuarios WHERE usuario='" + usuario + "' AND pass=AES_ENCRYPT('" + pass + "',UNHEX('E1229A5B57FDD5D94C1B330D81A300A3'));"));
                if (i > 0)
                {
                    _idUsuario = i;
                    _Usuario = usuario;
                    return i;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="usuario">nombre de usuario</param>
        /// <param name="oldPass">pass viejo</param>
        /// <param name="newPass">pass nuevo</param>
        /// <returns></returns>
        public bool SetPass(string usuario, string oldPass, string newPass)
        {
            try
            {
                int i = Convert.ToInt32(DB.ExecuteScalar("SELECT id FROM usuarios WHERE usuario='" + usuario + "' AND pass=AES_ENCRYPT('" + oldPass + "',UNHEX('E1229A5B57FDD5D94C1B330D81A300A3'));"));
                if (i > 0)
                {
                    int j = Convert.ToInt32(DB.ExecuteNonQuery("UPDATE usuarios SET pass=AES_ENCRYPT('" + newPass + "',UNHEX('E1229A5B57FDD5D94C1B330D81A300A3')) WHERE id='" + i.ToString() + "';"));
                    if (j == 1)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario por Id. No se deberia de usar de normal por seguridad
        /// </summary>
        /// <param name="usuario">nombre de usuario</param>
        /// <param name="oldPass">pass viejo</param>
        /// <param name="newPass">pass nuevo</param>
        /// <returns></returns>
        public bool SetPass(int idUsuario, string newPass)
        {
            try
            {
                int j = Convert.ToInt32(DB.ExecuteNonQuery("UPDATE usuarios SET pass=AES_ENCRYPT('" + newPass + "',UNHEX('E1229A5B57FDD5D94C1B330D81A300A3')) WHERE id='" + idUsuario.ToString() + "';"));
                if (j == 1)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve la lista de departamentos por id de usuario
        /// </summary>
        /// <param name="IdUsuario">Id de usuario</param>
        /// <returns>Lista de departamentos</returns>
        public string[] GetDepartamentos(int IdUsuario)
        {
            using (DataTable t = DB.GetTabla("SELECT usuario_dep.idDepartamento AS idDep, (SELECT nombre FROM departamentos WHERE departamentos.id=usuario_dep.idDepartamento) AS dep FROM usuario_dep WHERE usuario_dep.idUsuario=" + IdUsuario.ToString() + ";"))
            {
                if (t == null)
                    return new string[0];
                else
                {
                    List<string> X = new List<string>();
                    foreach (DataRow Fila in t.Rows)
                        X.Add(Convert.ToString(Fila["dep"]));
                    return X.ToArray();
                }
            }
        }

        /// <summary>
        /// Devuelve la lista de departamentos de un usuario por su nombre
        /// </summary>
        /// <param name="Usuario">Nombre de usuario</param>
        /// <returns>Lista de departamentos</returns>
        public string[] GetDepartamentos(string Usuario)
        {
            int i = Convert.ToInt32(DB.ExecuteScalar("SELECT id FROM usuarios WHERE usuario='" + Usuario + "';"));
            if (i >= 1)
                return GetDepartamentos(i);
            else
                return new string[0];
        }

        /// <summary>
        /// Muestra el formulario para hacer login
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="pass"></param>
        /// <returns>0=Cancelar, -1=Error pass, >0=Usuario ok</returns>
        public int ShowLogin(System.Windows.Forms.Form Paret)
        {
            using (frmLogin F = new frmLogin())
            {
                F.ShowDialog(Paret);
                if (F.Resp == System.Windows.Forms.DialogResult.OK)
                    return Login(F.usuario, F.pass);
                else
                    return 0;
            }
        }


        public string GetNombre(int IdUsuario)
        {
            try
            {
                return Convert.ToString(DB.ExecuteScalar("SELECT nombre FROM usuarios WHERE id=" + IdUsuario.ToString() + ";"));
            }
            catch
            {
                return "";
            }
        }
        public bool SetNombre(int IdUsuario, string Nombre)
        {
            try
            {
                int i = DB.ExecuteNonQuery("UPDATE usuarios SET nombre='" + Nombre + "' WHERE id=" + IdUsuario.ToString() + ";");
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetUsuario(int IdUsuario)
        {
            try
            {
                return Convert.ToString(DB.ExecuteScalar("SELECT usuario FROM usuarios WHERE id=" + IdUsuario.ToString() + ";"));
            }
            catch
            {
                return "";
            }
        }
        public bool SetUsuario(int IdUsuario, string Usuario)
        {
            try
            {
                int i = DB.ExecuteNonQuery("UPDATE usuarios SET usuario='" + Usuario + "' WHERE id=" + IdUsuario.ToString() + ";");
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

    }


}
