using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WatchSite
{
    class Email
    {
        
        public static void SendMail(string login, string pass, string to, string body)
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
                smtp.Credentials = new NetworkCredential(login, pass);
                smtp.EnableSsl = true;
                MailMessage Message = new MailMessage();
                Message.From = new MailAddress(login);  
                Message.To.Add(new MailAddress(to));
                Message.Subject = "Отслеживание магазина";
                Message.Body = body;

            
                smtp.Send(Message);
            }
            catch (SmtpException)
            {
                MessageBox.Show("Ошибка отправки почты!");
            }

        }


        //public static void SendMail(string smtpServer, string from, string password,
        //string mailto, string caption, string message, string attachFile = null)
        //{
        //    try
        //    {
        //        MailMessage mail = new MailMessage();
        //        mail.From = new MailAddress(from);
        //        mail.To.Add(new MailAddress(mailto));
        //        mail.Subject = caption;
        //        mail.Body = message;
        //        if (!string.IsNullOrEmpty(attachFile))
        //            mail.Attachments.Add(new Attachment(attachFile));
        //        SmtpClient client = new SmtpClient();
        //        client.Host = smtpServer;
        //        client.Port = 587;
        //        client.EnableSsl = true;
        //        client.Credentials = new NetworkCredential(from.Split('@')[0], password);
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.Send(mail);
        //        mail.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Mail.Send: " + e.Message);
        //    }
        //}


    }
}
