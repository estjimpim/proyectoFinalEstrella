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
using System.IO;
using System.Diagnostics;


namespace RegistroUsuarios
{
    public partial class Form3 : Form
    {
        private string nombreUsuario;
        private Data.Connection con;

        public Form3(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            con = new Data.Connection();
            this.Load += Form3_Load;

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
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            CargarItemsDesdeArchivo();
        }

        private void CargarItemsDesdeArchivo()
        {
            listBox1.Items.Clear(); // Limpiamos los elementos anteriores

            string filePath = "Fechas validez documentos.txt";

            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        listBox1.Items.Add(linea); // Agregar cada línea como un elemento separado
                    }
                }
            }
            else
            {
                MessageBox.Show("El archivo no existe.");
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Ocultar el formulario actual
            this.Hide();

            // Crear una instancia del Form2
            Form2 form2 = new Form2();

            // Mostrar el Form2
            form2.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage6;
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage7;
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

        //En este caso, he metido fechas de forma directa, no cogiéndolas de una BBDD:
     

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4(dataGridView1);
            form4.Show();
        }

        private void profesional_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void tabPage6_Load(object sender, EventArgs e)
        {
            var fechaActual = DateTime.Now;
            dateTimePicker1.MinDate = fechaActual;
            monthCalendar1.MinDate = fechaActual;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("Identificador: "+ id.Text + "\tNombre: " + nombreU.Text + "\tApellidos: " + apellidosU.Text + "\tProfesional: " + profesional.SelectedItem + "\tDocumento que caduca: " + documento.SelectedItem + "\tFecha: "+ dateTimePicker1.Text);
            StreamWriter sw = new StreamWriter("Fechas validez documentos.txt", true);
            sw.WriteLine("Identificador: "+ id.Text + "\tNombre: " + nombreU.Text + "\tApellidos: " + apellidosU.Text + "\tProfesional: " + profesional.SelectedItem + "\tDocumento que caduca: " + documento.SelectedItem + "\tFecha: "+ dateTimePicker1.Text);
            sw.Close();
        }

        private void vencimientos_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear(); // Limpiamos los elementos anteriores
            string linea;
            StreamReader sr = new StreamReader("Fechas validez documentos.txt");
            while ((linea = sr.ReadLine()) != null){
                    listBox1.Items.Add(linea); // Agregar cada línea como un elemento separado
            }
        }
    }
}
