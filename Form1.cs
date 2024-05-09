﻿using MySql.Data.MySqlClient;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Data.Connection con = new Data.Connection();

            con.connOpen();

            string queryDatosUsuario = $"SELECT * FROM usuarios";

            MySqlCommand cmdDatosUsuario = new MySqlCommand(queryDatosUsuario, Data.Connection.connMaster);
            MySqlDataReader reader = cmdDatosUsuario.ExecuteReader();

            while (reader.Read())
            {
                string nombre = reader.GetString(0);
                string contra = reader.GetString(1);
                string correo = reader.GetString(2);
                string telefono = reader.GetString(3);
                string rol = reader.GetString(4);
                string curso = reader.GetString(5);

                if (rol == "Profesor")
                {
                    dataGridView1.Rows.Add(correo, contra, nombre, telefono, rol, curso);
                }
                else if (rol == "Alumno")
                {
                    dataGridView2.Rows.Add(correo, contra, nombre, telefono, rol, curso);
                }
            }

            reader.Close();
            con.connClose();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Profesor")
            {
                //Enviar correo
                Data.Correo newCorreo = new Data.Correo();
                newCorreo.enviarCorreo(textBox3.Text, "Alta " + textBox1.Text, textBox1.Text, textBox2.Text);

                //Añadir los datos a la tabla dataGridView1
                dataGridView1.Rows.Add();
                int n = dataGridView1.Rows.Count;

                dataGridView1.Rows[n - 1].Cells[0].Value = textBox1.Text;
                dataGridView1.Rows[n - 1].Cells[1].Value = textBox2.Text;
                dataGridView1.Rows[n - 1].Cells[2].Value = textBox3.Text;
                dataGridView1.Rows[n - 1].Cells[3].Value = textBox5.Text;
                dataGridView1.Rows[n - 1].Cells[4].Value = comboBox1.SelectedItem.ToString();
                dataGridView1.Rows[n - 1].Cells[5].Value = comboBox2.SelectedItem.ToString();

                // Guardar los datos en la base de datos
                GuardarDatosEnBaseDeDatos(textBox1.Text, textBox2.Text, textBox3.Text, textBox5.Text, comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
            }
            else if (comboBox1.SelectedItem.ToString() == "Alumno")
            {
               
                //Enviar correo
                Data.Correo newCorreo = new Data.Correo();
                newCorreo.enviarCorreo(textBox3.Text, "Alta " + textBox1.Text, textBox1.Text, textBox2.Text);

                //Añadir los datos a la tabla dataGridView2
                dataGridView2.Rows.Add();
                int n = dataGridView2.Rows.Count;

                dataGridView2.Rows[n - 1].Cells[0].Value = textBox1.Text;
                dataGridView2.Rows[n - 1].Cells[1].Value = textBox2.Text;
                dataGridView2.Rows[n - 1].Cells[2].Value = textBox3.Text;
                dataGridView2.Rows[n - 1].Cells[3].Value = textBox5.Text;
                dataGridView2.Rows[n - 1].Cells[4].Value = comboBox1.SelectedItem.ToString();
                dataGridView2.Rows[n - 1].Cells[5].Value = comboBox2.SelectedItem.ToString();

                // Guardar los datos en la base de datos
                GuardarDatosEnBaseDeDatos(textBox1.Text, textBox2.Text, textBox3.Text, textBox5.Text, comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
            }

            // Limpiar los cuadros de texto después de agregar los datos
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataGridView dataGridView = null;
            string tabla = "";

            // Determinar el DataGridView activo y la tabla correspondiente en la base de datos
            if (tabControl1.SelectedTab == tabPage1)
            {
                dataGridView = dataGridView1;
                tabla = "usuarios";
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                dataGridView = dataGridView2;
                tabla = "usuarios";
            }

            if (dataGridView != null && dataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

                // Obtener el nombre del usuario de la fila seleccionada
                string nombreUsuario = selectedRow.Cells[0].Value.ToString();

                DialogResult result = MessageBox.Show("¿Estás seguro que deseas borrar esta fila?", "Confirmar Borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Eliminar la fila del DataGridView
                    dataGridView.Rows.Remove(selectedRow);

                    // Eliminar la fila correspondiente de la base de datos
                    EliminarFilaDeBaseDeDatos(tabla, nombreUsuario);
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado una fila para borrar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EliminarFilaDeBaseDeDatos(string tabla, string nombreUsuario)
        {
            try
            {
                // Abrir la conexión
                Data.Connection con = new Data.Connection();
                con.connOpen();

                if (con.isConnected())
                {
                    // Consulta SQL para eliminar la fila
                    string queryEliminarFila = $"DELETE FROM {tabla} WHERE nombre = @nombre";
                    MySqlCommand cmdEliminarFila = new MySqlCommand(queryEliminarFila, Data.Connection.connMaster);
                    cmdEliminarFila.Parameters.AddWithValue("@nombre", nombreUsuario);

                    // Ejecutar la consulta
                    bool exito = con.executeQuery(cmdEliminarFila);

                    if (exito)
                    {
                        MessageBox.Show("La fila se ha eliminado correctamente de la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al intentar eliminar la fila de la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo conectar a la base de datos.", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al intentar eliminar la fila de la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        int currentSelected;
        private void button5_Click(object sender, EventArgs e)
        {
            DataGridView dataGridView = null;
            int currentRowIndex = -1;

            // Determinar el DataGridView activo
            if (tabControl1.SelectedTab == tabPage1)
            {
                dataGridView = dataGridView1;
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                dataGridView = dataGridView2;
            }

            if (dataGridView != null)
            {
                // Obtener la fila seleccionada
                DataGridViewRow selectedRow = dataGridView.CurrentRow;
                if (selectedRow != null)
                {
                    // Mostrar los valores de la fila seleccionada en los cuadros de texto
                    textBox1.Text = selectedRow.Cells[0].Value.ToString();
                    textBox2.Text = selectedRow.Cells[1].Value.ToString();
                    textBox3.Text = selectedRow.Cells[2].Value.ToString();
                    textBox5.Text = selectedRow.Cells[3].Value.ToString();

                    // Obtener los valores de rol y curso de la fila seleccionada
                    string rol = selectedRow.Cells[4].Value.ToString();
                    string curso = selectedRow.Cells[5].Value.ToString();

                    // Establecer los valores de rol y curso en los combobox correspondientes
                    comboBox1.SelectedItem = rol;
                    comboBox2.SelectedItem = curso;

                    // Guardar el índice de la fila seleccionada para futuras referencias
                    currentRowIndex = selectedRow.Index;
                }
            }

            // Guardar el índice de la fila seleccionada para futuras referencias
            currentSelected = currentRowIndex;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataGridView dataGridView = null;
            string tabla = "";

            // Determinar el DataGridView activo y la tabla correspondiente en la base de datos
            if (tabControl1.SelectedTab == tabPage1)
            {
                dataGridView = dataGridView1;
                tabla = "usuarios";
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                dataGridView = dataGridView2;
                tabla = "usuarios";
            }

            if (dataGridView != null && dataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

                // Obtener los valores de la fila seleccionada
                string nombre = selectedRow.Cells[0].Value.ToString();
                string contra = selectedRow.Cells[1].Value.ToString();
                string correo = selectedRow.Cells[2].Value.ToString();
                string telefono = selectedRow.Cells[3].Value.ToString();
                string rol = selectedRow.Cells[4].Value.ToString();
                string curso = selectedRow.Cells[5].Value.ToString();

                // Actualizar los valores con los datos de los cuadros de texto
                selectedRow.Cells[0].Value = textBox1.Text;
                selectedRow.Cells[1].Value = textBox2.Text;
                selectedRow.Cells[2].Value = textBox3.Text;
                selectedRow.Cells[3].Value = textBox5.Text;
                selectedRow.Cells[4].Value = comboBox1.SelectedItem.ToString();
                selectedRow.Cells[5].Value = comboBox2.SelectedItem.ToString();

                // Actualizar los datos en la base de datos
                ActualizarDatosEnBaseDeDatos(tabla, nombre, textBox1.Text, textBox2.Text, textBox3.Text, comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("No se ha seleccionado una fila para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ActualizarDatosEnBaseDeDatos(string tabla, string nombreAntiguo, string nombreNuevo, string contra, string correo, string rol, string curso)
        {
            try
            {
                // Abrir la conexión
                Data.Connection con = new Data.Connection();
                con.connOpen();

                if (con.isConnected())
                {
                    // Consulta SQL para actualizar los datos en la base de datos
                    string queryActualizarFila = $"UPDATE {tabla} SET nombre = @nombreNuevo, contra = @contra, correo = @correo, telefono = @telefono, rol = @rol, curso = @curso WHERE nombre = @nombreAntiguo";
                    MySqlCommand cmdActualizarFila = new MySqlCommand(queryActualizarFila, Data.Connection.connMaster);
                    cmdActualizarFila.Parameters.AddWithValue("@nombreNuevo", nombreNuevo);
                    cmdActualizarFila.Parameters.AddWithValue("@nombreAntiguo", nombreAntiguo);
                    cmdActualizarFila.Parameters.AddWithValue("@contra", contra);
                    cmdActualizarFila.Parameters.AddWithValue("@correo", correo);
                    cmdActualizarFila.Parameters.AddWithValue("@telefono", textBox5.Text); // Suponiendo que textBox5 contiene el teléfono
                    cmdActualizarFila.Parameters.AddWithValue("@rol", rol);
                    cmdActualizarFila.Parameters.AddWithValue("@curso", curso);

                    // Ejecutar la consulta
                    bool exito = con.executeQuery(cmdActualizarFila);

                    if (exito)
                    {
                        MessageBox.Show("Los cambios se han guardado exitosamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ha ocurrido un error al intentar guardar los cambios en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo conectar a la base de datos.", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al intentar guardar los cambios en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string nombreBuscado = textBox4.Text.ToLower(); // Convertir a minúsculas para hacer la búsqueda sin distinción entre mayúsculas y minúsculas

            DataGridView dataGridViewToSearch = null;

            // Determinar qué DataGridView está activo en la pestaña actual del TabControl
            if (tabControl1.SelectedTab == tabPage1)
            {
                dataGridViewToSearch = dataGridView1;
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                dataGridViewToSearch = dataGridView2;
            }

            if (dataGridViewToSearch != null)
            {
                foreach (DataGridViewRow row in dataGridViewToSearch.Rows)
                {
                    bool matchFound = false;

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(nombreBuscado))
                        {
                            matchFound = true;
                            break;
                        }
                    }

                    // Mostrar u ocultar la fila según si se encuentra la coincidencia
                    row.Visible = matchFound;
                }
            }
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        private void GuardarDatosEnBaseDeDatos(string nombre, string contra, string correo, string telefono, string rol, string curso)
        {
            Data.Connection con = null; // Definir la variable fuera del bloque try

            try
            {
                // Crear la conexión
                con = new Data.Connection();

                // Abrir la conexión
                con.connOpen();

                // Preparar la consulta SQL de inserción
                string queryInsertarUsuario = $"INSERT INTO usuarios (nombre, contra, correo, telefono, rol, curso) VALUES ('{nombre}', '{contra}', '{correo}','{telefono}', '{rol}', '{curso}')";

                // Crear un nuevo comando SQL
                MySqlCommand cmdInsertarUsuario = new MySqlCommand(queryInsertarUsuario, Data.Connection.connMaster);

                // Ejecutar la consulta
                bool exito = con.executeQuery(cmdInsertarUsuario);

                if (exito)
                {
                    MessageBox.Show("Los datos se han guardado exitosamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ha ocurrido un error al intentar guardar los datos en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}