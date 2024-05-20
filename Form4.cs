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
        String nombreUsuario = "Profesional";        

       public Form4()
       {
                InitializeComponent();
       }

        private void button2_Click(object sender, EventArgs e)
        {
            // Ocultar el formulario actual
            this.Hide();

            // Crear una instancia del Form2
            Form3 form3 = new Form3(nombreUsuario);

            // Mostrar el Form2
            form3.Show();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // Ocultar el formulario actual
            this.Hide();

            // Crear una instancia del Form3
            Form3 form3 = new Form3(2, false);

            // Mostrar el Form3
            form3.Show();

            

        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            int ID = int.Parse(textBox5.Text);
            string nombre = textBox1.Text;
            string apellido1 = textBox2.Text;
            string apellido2 = textBox3.Text;
            string dni = textBox4.Text;
            DateTime ingreso = dateTimePicker1.Value;
            DateTime nacimiento = dateTimePicker2.Value;
            string nombreFamiliar = textBox7.Text;
            string telefonoFamiliar = textBox8.Text;


            GuardarDatosEnBaseDeDatos(ID, nombre, apellido1, apellido2, dni, ingreso, nacimiento, nombreFamiliar, telefonoFamiliar);
            // Ocultar el formulario actual
            this.Hide();

            // Crear una instancia del Form3
            Form3 form3 = new Form3(2, false);

            // Mostrar el Form3
            form3.Show();

        }

        private void GuardarDatosEnBaseDeDatos(int ID, string nombre, string apellido1, string apellido2, string dni, DateTime ingreso, DateTime nacimiento, string nombreFamiliar, string telefonoFamiliar)
        {
            Data.Connection con = null; // Definir la variable fuera del bloque try

            try
            {
                // Crear la conexión
                con = new Data.Connection();

                // Abrir la conexión
                con.connOpen();

                // Preparar la consulta SQL de inserción usando parámetros
                string queryInsertarUsuario = "INSERT INTO residentes (ID, nombre, apellido1, apellido2, dni, ingreso, nacimiento, nombrefamiliar, telefonofamiliar) " +
                                              "VALUES (@Id, @nombre, @apellido1, @apellido2, @dni, @ingreso, @nacimiento, @nombrefamiliar, @telefonofamiliar)";

                // Crear un nuevo comando SQL
                MySqlCommand cmdInsertarUsuario = new MySqlCommand(queryInsertarUsuario, Data.Connection.connMaster);

                // Agregar parámetros al comando SQL
                cmdInsertarUsuario.Parameters.AddWithValue("@ID", ID);
                cmdInsertarUsuario.Parameters.AddWithValue("@nombre", nombre);
                cmdInsertarUsuario.Parameters.AddWithValue("@apellido1", apellido1);
                cmdInsertarUsuario.Parameters.AddWithValue("@apellido2", apellido2);
                cmdInsertarUsuario.Parameters.AddWithValue("@dni", dni);
                cmdInsertarUsuario.Parameters.AddWithValue("@ingreso", ingreso.ToString("yyyy-MM-dd HH:mm:ss"));
                cmdInsertarUsuario.Parameters.AddWithValue("@nacimiento", nacimiento.ToString("yyyy-MM-dd HH:mm:ss"));
                cmdInsertarUsuario.Parameters.AddWithValue("@nombrefamiliar", nombreFamiliar);
                cmdInsertarUsuario.Parameters.AddWithValue("@telefonofamiliar", telefonoFamiliar);

                // Ejecutar la consulta
                int rowsAffected = cmdInsertarUsuario.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Los datos se han guardado exitosamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se ha podido insertar el registro en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al intentar guardar los datos en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Cerrar la conexión si se ha abierto previamente
                if (con != null)
                    con.connClose();
            }
        }


    }
    
}
