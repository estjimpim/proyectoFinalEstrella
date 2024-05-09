using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;

namespace RegistroUsuarios.Data
{
    class Connection
    {
        public static MySqlConnection connMaster = new MySqlConnection(); //con este objeto se establece la conexion con la BDD, y este objeto
        //devuelve un servidor una bbdd, un usuario y una contraseña,esos datos en este caso son los siguientes:

        public static string server = "127.0.0.1"; //tambien puedo poner localhost, daria igual eso o la IP
        public static string DataBase = "login"; //nombre de la BBDD que he creado en phpmyadmin
        public static string User = "root"; //nombre de usuario con el que nos conectamos
        public static string password = ""; //contraseña con la que accedemos

        public static MySqlConnection DataSource()
        {
            string connectionString = $"server = {server}; database = {DataBase}; user = {User}; password={password};";
            connMaster = new MySqlConnection(connectionString);
            return connMaster;
        }
        public bool isConnected()
        {
            // Verificar si la conexión está abierta
            return (connMaster != null && connMaster.State == ConnectionState.Open);
        }
        public void connOpen()
        {
            try
            {
                DataSource();
                connMaster.Open();
                if (connMaster.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("La conexión está abierta.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No hay conexión.");
            }
        }


        public void connClose()
        {
            DataSource();
            connMaster.Close();
            if (connMaster.State != ConnectionState.Open)
            {
                Console.WriteLine("La conexión está cerrada");
            }
        }
        public bool executeQuery(MySqlCommand cmd)
        {
            try
            {
                MySqlConnection conn = DataSource();
                conn.Open();
                cmd.Connection = conn; // Asignar la conexión al comando
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al ejecutar la consulta SQL: " + ex.Message);
                return false;
            }
            finally
            {
                connMaster.Close(); // Asegurarse de cerrar la conexión en cualquier caso
            }
        }


    }
}
