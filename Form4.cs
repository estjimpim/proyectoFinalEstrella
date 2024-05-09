using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegistroUsuarios
{
    public partial class Form4 : Form
    {
        private string nombreUsuario;
        private Data.Connection con;

        public Form4(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            con = new Data.Connection();

            // Verificar si es la primera vez que el usuario accede
            if (esPrimeraVezAbrirFormulario())
            {
                // Mostrar un cuadro de diálogo para cambiar la contraseña
                DialogResult result = MessageBox.Show("Es la primera vez que accede. ¿Desea cambiar su contraseña?", "Cambio de contraseña", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CambiarContraseña();
                }
                else
                {
                    MessageBox.Show("Debe cambiar su contraseña para continuar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }

            // Cargar los datos del usuario al cargar el formulario
            Form4_Load(this, EventArgs.Empty);
        }

        private bool esPrimeraVezAbrirFormulario()
        {
            con.connOpen();

            try
            {
                string query = $"SELECT primera_vez FROM usuarios WHERE nombre = '{nombreUsuario}'";
                MySqlCommand cmd = new MySqlCommand(query, Data.Connection.connMaster);
                object resultado = cmd.ExecuteScalar();

                if (resultado != null && resultado != DBNull.Value)
                {
                    bool primeraVez = Convert.ToBoolean(resultado);
                    return !primeraVez;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar si es la primera vez que abre el formulario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                con.connClose();
            }
        }

        private void CambiarContraseña()
        {
            string nuevaContraseña = Prompt.ShowDialog("Ingrese su nueva contraseña:", "Cambio de contraseña");

            if (!string.IsNullOrEmpty(nuevaContraseña))
            {
                try
                {
                    con.connOpen();
                    string query = $"UPDATE usuarios SET contra = '{nuevaContraseña}', primera_vez = true WHERE nombre = '{nombreUsuario}'";
                    MySqlCommand cmd = new MySqlCommand(query, Data.Connection.connMaster);
                    int filasActualizadas = cmd.ExecuteNonQuery();

                    if (filasActualizadas > 0)
                    {
                        MessageBox.Show("La contraseña se ha actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar la contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar la contraseña en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.connClose();
                }
            }
            else
            {
                MessageBox.Show("La contraseña no puede estar vacía.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        // Método para cargar los datos del usuario al cargar el formulario
        // Método para cargar los datos del usuario al cargar el formulario
        private void Form4_Load(object sender, EventArgs e)
        {
            try
            {
                con.connOpen();
                string query = $"SELECT u.nombre, u.correo, u.contra, u.telefono, u.curso, t.nombre AS tutor " +
                               $"FROM usuarios u " +
                               $"INNER JOIN usuarios t ON u.curso = t.curso " +
                               $"WHERE u.nombre = '{nombreUsuario}'";
                MySqlCommand cmd = new MySqlCommand(query, Data.Connection.connMaster);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string nombre = reader.GetString("nombre");
                    string correo = reader.GetString("correo");
                    string contra = reader.GetString("contra");
                    string telefono = reader.GetString("telefono");
                    string curso = reader.GetString("curso");
                    string tutor = reader.GetString("tutor");

                    textBox1.Text = nombre;
                    textBox2.Text = correo;
                    textBox3.Text = contra;
                    textBox4.Text = telefono;
                    textBox5.Text = curso;
                    textBox6.Text = tutor;
                }
                else
                {
                    MessageBox.Show("No se encontraron datos para el usuario actual.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener datos del usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.connClose();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Ocultar el formulario actual
            this.Hide();

            // Crear una instancia del Form2
            Form2 form2 = new Form2();

            // Mostrar el Form2
            form2.Show();
        }
    }

    // Clase auxiliar para mostrar un cuadro de diálogo de entrada de texto
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmation = new Button() { Text = "Aceptar", Left = 250, Width = 100, Top = 100, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

}
