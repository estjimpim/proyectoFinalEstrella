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
        private DataGridView dataGridViewForm3;

        public Form4(DataGridView dataGridViewForm3)
        {
            InitializeComponent();
            this.dataGridViewForm3 = dataGridViewForm3;
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

        private void button1_Click(object sender, EventArgs e)
        {
            string nombre = textBox1.Text;
            string apellido = textBox2.Text;

            // Agregar una nueva fila al DataGridView en el TabPage del Form3
            DataGridViewRow fila = new DataGridViewRow();
            fila.CreateCells(dataGridViewForm3);
            fila.SetValues(nombre, apellido);
            dataGridViewForm3.Rows.Add(fila);
        }
    }
    
}
