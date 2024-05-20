using System;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;

namespace RegistroUsuarios.Data
{
    class Correo
    {

        public void enviarCorreo(string _to, string _subject, string _nombre, string _contraseña)
        {
            //Creamos variables para el correo desde el que enviamos el correo, la contraseña creada
            //en google y el asunto
            string from = "estjimpim@gmail.com";
            string password = "nrts skyz ilrl nneq";
            string alias = "Alta usuario ResiCare";

            //Cuerpo del correo electrónico
            string body;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\HTMLPage1.html");

            using (StreamReader reader = new StreamReader(filePath))
            {
                body = reader.ReadToEnd();
            }
            //Reemplazar el texto
            body = body.Replace("_usuario", _nombre).Replace("_contra", _contraseña);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from, alias);
            mail.To.Add(_to);
            mail.Subject = _subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            //Configurar el cliente SMTP
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential(from, password);
            smtpClient.EnableSsl = true; //Establecer EnableSsl en true

            try
            {
                smtpClient.Send(mail);
                MessageBox.Show("Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message);
            }

        }

    }
}
