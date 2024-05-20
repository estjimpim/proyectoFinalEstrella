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
        private int tabIndex;

        public Form3(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            con = new Data.Connection();
            this.Load += Form3_Load;
            ActualizarListaDocumentos();
            listBox2.DoubleClick += new EventHandler(listBox2_DoubleClick);

            if (esPrimeraVezAbrirFormulario())
            {
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

        public Form3(int tabIndex = 0, bool cargarDatos = true)
        {
            InitializeComponent();
            
            this.tabIndex = tabIndex;
            con = new Data.Connection();

            this.Load += Form3_Load;

            if (cargarDatos)
            {
                CargarDatosUsuarios();
                CargarDatosResidentes();
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            CargarDatosUsuarios();
            CargarDatosResidentes();
            CargarNotas();
            if (tabIndex >= 0 && tabIndex < tabControl1.TabCount)
            {
                tabControl1.SelectedIndex = tabIndex;
            }
        }

        private void CargarDatosUsuarios()
        {
            dataGridView2.Rows.Clear();

            try
            {
                con.connOpen();

                string queryDatosUsuario = $"SELECT * FROM usuarios";
                MySqlCommand cmdDatosUsuario = new MySqlCommand(queryDatosUsuario, Data.Connection.connMaster);
                MySqlDataReader reader = cmdDatosUsuario.ExecuteReader();

                while (reader.Read())
                {
                    string nombre = reader.GetString(0);
                    string correo = reader.GetString(2);
                    string telefono = reader.GetString(3);
                    string rol = reader.GetString(4);

                    dataGridView2.Rows.Add(nombre, correo, telefono, rol);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos de usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.connClose();
            }
        }

        private void CargarDatosResidentes()
        {
            dataGridView1.Rows.Clear();

            try
            {
                con.connOpen();

                string queryDatosUsuario = $"SELECT * FROM residentes";
                MySqlCommand cmdDatosUsuario = new MySqlCommand(queryDatosUsuario, Data.Connection.connMaster);
                MySqlDataReader reader = cmdDatosUsuario.ExecuteReader();

                while (reader.Read())
                {
                    int ID = reader.GetInt32(0);
                    string nombre = reader.GetString(1);
                    string apellido1 = reader.GetString(2);
                    string apellido2 = reader.GetString(3);
                    string dni = reader.GetString(4);
                    DateTime ingreso = reader.GetDateTime(5);
                    DateTime nacimiento = reader.GetDateTime(6);
                    string nombrefamiliar = reader.GetString(7);
                    int telefonofamiliar = reader.GetInt32(8);


                    dataGridView1.Rows.Add(ID, nombre, apellido1, apellido2, dni, ingreso, nacimiento, nombrefamiliar, telefonofamiliar);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos de usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.connClose();
            }
        }

        private void CargarItemsDesdeArchivo()
        {
            listBox1.Items.Clear(); 

            string filePath = "Fechas validez documentos.txt";

            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        listBox1.Items.Add(linea); 
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
                Label textLabel = new Label()
                {
                    Left = 50,
                    Top = 20,
                    Text = text,
                    Font = new Font("Century Gothic", 10)
                };


                TextBox textBox = new TextBox()
                {
                    Left = 50,
                    Top = 50,
                    Width = 300,
                    Font = new Font("Century Gothic", 10)
                };

                Button confirmation = new Button()
                {
                    Text = "Aceptar",
                    Left = 250,
                    Width = 100,
                    Top = 100,
                    DialogResult = DialogResult.OK,
                    Font = new Font("Century Gothic", 10)
                };
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
            this.Hide();

            Form2 form2 = new Form2();

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

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void tabPage6_Load(object sender, EventArgs e)
        {
            var fechaActual = DateTime.Now;
            dateTimePicker1.MinDate = fechaActual;
            monthCalendar1.MinDate = fechaActual;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = dateTimePicker1.Value; // primero obtengo la fecha seleccionada del DateTimePicker

            string fechaFormateada = fechaSeleccionada.ToString("dd/MM/yyyy");
            string itemListBox = string.Format("Identificador: {0}\tProfesional: {1}\tDocumento que caduca: {2}\tFecha: {3}",
                                                id.Text, profesional.SelectedItem, documento.SelectedItem, fechaFormateada);

            // Agrego la cadena al ListBox
            listBox1.Items.Add(itemListBox);

            using (StreamWriter sw = new StreamWriter("Fechas validez documentos.txt", true))
            {
                sw.WriteLine(itemListBox);
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.SelectAll(); // Borra el contenido del TextBox cuando se hace clic en él
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            string criterioBusqueda = textBox2.Text.Trim(); // obtengo el criterio de búsqueda desde el TextBox

            BuscarDocumentos(criterioBusqueda);
        }

        private List<string> documentosOriginales = new List<string>(); // mantengo una copia de los documentos originales

        private void BuscarDocumentos(string criterio)
        {
            // creo una nueva lista para almacenar los documentos que coinciden con el criterio de búsqueda
            List<string> documentosCoincidentes = new List<string>();

            // recorro todos los documentos originales y agrego aquellos que coincidan con el criterio de búsqueda a la nueva lista
            foreach (string documento in documentosOriginales)
            {
                if (documento.Contains(criterio))
                {
                    documentosCoincidentes.Add(documento);
                }
            }
            // asigno la nueva lista como origen de datos del ListBox
            listBox1.DataSource = documentosCoincidentes;
        }
        private void CargarDocumentosOriginales()
        {
            documentosOriginales.Clear(); // Limpiar la lista antes de cargar los documentos

            // Cargar todos los documentos desde el archivo "Fechas validez documentos.txt"
            using (StreamReader sr = new StreamReader("Fechas validez documentos.txt"))
            {
                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    documentosOriginales.Add(linea); // Agregar cada línea a la lista de documentos originales
                }
            }
        }

    private void vencimientos_Click(object sender, EventArgs e)
    {
        // primero limpio los elementos anteriores
        if (listBox1.DataSource == null)
        {
            listBox1.Items.Clear();
            CargarDocumentosOriginales();

            DateTime fechaActual = DateTime.Today;

            // Paso a recorrer los documentos originales y agregar solo aquellos cuya fecha sea posterior a la fecha actual
            foreach (string documento in documentosOriginales)
            {
                DateTime fechaDocumento = ObtenerFechaVencimiento(documento);

                if (fechaDocumento >= fechaActual)
                {
                    listBox1.Items.Add(documento); 
                }
            }
        }
    }
        private DateTime ObtenerFechaVencimiento(string documento)
        {
            int indiceSeparador = documento.LastIndexOf(':');

            if (indiceSeparador != -1 && indiceSeparador < documento.Length - 1)
            {
                string fechaString = documento.Substring(indiceSeparador + 1).Trim();

                string[] partesFecha = fechaString.Split('/');

                if (partesFecha.Length == 3)
                {
                    int dia, mes, anio;
                    if (int.TryParse(partesFecha[0], out dia) && int.TryParse(partesFecha[1], out mes) && int.TryParse(partesFecha[2], out anio))
                    {
                        if (dia >= 1 && dia <= 31 && mes >= 1 && mes <= 12 && anio >= 1900 && anio <= 9999)
                        {
                            return new DateTime(anio, mes, dia);
                        }
                    }
                }

                throw new FormatException("Formato de fecha no válido en el documento: " + documento);
            }
            else
            {
                throw new FormatException("Separador de fecha no encontrado en el documento: " + documento);
            }
        }

        public void SeleccionarTabPage(string nombreTabPage)
        {
            TabPage tabPage = tabControl1.TabPages[nombreTabPage];

            if (tabPage != null)
            {
                tabControl1.SelectedTab = tabPage;
            }
            else
            {
                MessageBox.Show("No se encontró el TabPage especificado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
         
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string tabla = "residentes";

                string nombreResidente = selectedRow.Cells[0].Value.ToString();

                DialogResult result = MessageBox.Show("¿Estás seguro que deseas borrar esta fila?", "Confirmar Borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dataGridView1.Rows.Remove(selectedRow);

                    EliminarFilaDeBaseDeDatos(tabla, nombreResidente);
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
                Data.Connection con = new Data.Connection();
                con.connOpen();

                if (con.isConnected())
                {
                    string queryEliminarFila = $"DELETE FROM {tabla} WHERE nombre = @nombre";
                    MySqlCommand cmdEliminarFila = new MySqlCommand(queryEliminarFila, Data.Connection.connMaster);
                    cmdEliminarFila.Parameters.AddWithValue("@nombre", nombreUsuario);

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

        private void button11_Click(object sender, EventArgs e)
        {            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Seleccionar documento";
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string nombreArchivo = Path.GetFileName(openFileDialog.FileName);
                string destino = Path.Combine(Application.StartupPath, "Documentos", nombreArchivo);

                // copio el archivo seleccionado a la carpeta de Documentos de la aplicación
                File.Copy(openFileDialog.FileName, destino);

                MessageBox.Show("Documento subido con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // llamo al método 
                ActualizarListaDocumentos();
            }
        }

        private void ActualizarListaDocumentos()
        {
            listBox2.Items.Clear();

            // obtengo la lista de archivos en la carpeta de Documentos
            string[] archivos = Directory.GetFiles(Path.Combine(Application.StartupPath, "Documentos"));

            foreach (string archivo in archivos)
            {
            listBox2.Items.Add(Path.GetFileName(archivo));
            }   
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string nombreArchivo = listBox2.SelectedItem.ToString();
                string rutaArchivo = Path.Combine(Application.StartupPath, "Documentos", nombreArchivo);

                if (File.Exists(rutaArchivo))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(rutaArchivo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo abrir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("El archivo no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarNotas()
        {
            listBox3.Items.Clear();

            try
            {
                con.connOpen();

                string query = "SELECT nota FROM notasdb";
                MySqlCommand cmd = new MySqlCommand(query, Data.Connection.connMaster);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string nota = reader.GetString(0);
                    listBox3.Items.Add(nota);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las notas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.connClose();
            }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            string nuevaNota = Prompt.ShowDialog("Nota:", "Agregar Nota");

            if (!string.IsNullOrEmpty(nuevaNota))
            {
                try
                {
                    con.connOpen();
                    string query = "INSERT INTO notasdb (nota) VALUES (@nota)";
                    MySqlCommand cmd = new MySqlCommand(query, Data.Connection.connMaster);
                    cmd.Parameters.AddWithValue("@nota", nuevaNota);
                    int filasInsertadas = cmd.ExecuteNonQuery();

                    if (filasInsertadas > 0)
                    {
                        MessageBox.Show("La nota se ha agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        listBox3.Items.Add(nuevaNota);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo agregar la nota.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar la nota: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.connClose();
                }
            }
            else
            {
                MessageBox.Show("La nota no puede estar vacía.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex != -1)
            {
                int indiceSeleccionado = listBox3.SelectedIndex;
                string notaSeleccionada = listBox3.SelectedItem.ToString();

                try
                {
                    con.connOpen();

                    string query = "DELETE FROM notasdb WHERE Nota = @nota";
                    MySqlCommand cmd = new MySqlCommand(query, Data.Connection.connMaster);
                    cmd.Parameters.AddWithValue("@nota", notaSeleccionada);
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        listBox3.Items.RemoveAt(indiceSeleccionado);

                        MessageBox.Show("La nota seleccionada se ha eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar la nota seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar eliminar la nota: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.connClose();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una nota para borrar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

