using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace RegistroUsuarios
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string nombreUsuario = textBox1.Text;
            string contraseña = textBox2.Text;

            if (nombreUsuario == "admin" && contraseña == "admin")
            {
                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();
            }
            else
            {
                Data.Connection con = new Data.Connection();
                con.connOpen();

                string queryBuscarUsuario = $"SELECT rol FROM usuarios WHERE nombre = '{nombreUsuario}' AND contra = '{contraseña}'";
                MySqlCommand cmdBuscarUsuario = new MySqlCommand(queryBuscarUsuario, Data.Connection.connMaster);
                MySqlDataReader reader = cmdBuscarUsuario.ExecuteReader();

                bool usuarioValido = false;
                string rol = "";

                if (reader.Read())
                {
                    usuarioValido = true;
                    rol = reader.GetString(0);
                }

                reader.Close();
                con.connClose();

                if (usuarioValido)
                {
                    Form3 form3 = new Form3(nombreUsuario);
                    form3.Show();         
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
