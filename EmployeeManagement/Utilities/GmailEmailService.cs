using EmployeeManagement.Models;
using Org.BouncyCastle.Crypto.Macs;
using System;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace EmployeeManagement.Utilities
{
    public class GmailEmailService
    {
        public GmailEmailService(string userEmail, string Subject, string Body, string InformMailSubject = null, string InformMailBody = null)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(userEmail.Trim());
            mail.From = new MailAddress("eslamghazi20001@gmail.com", "Employee Management");
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("eslamghazi20001@gmail.com", "piakcneisfjjtrvw");
            smtp.Send(mail);

            InformEmail(userEmail, InformMailSubject, InformMailBody);
        }

        public string InformEmail(string userEmail, string InformMailSubject, string InformMailBody = null)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("eslamghazi20002@gmail.com".Trim());
            mail.From = new MailAddress("eslamghazi20001@gmail.com", "Employee Management");
            mail.Subject = InformMailSubject;
            mail.Body = InformMailBody;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("eslamghazi20001@gmail.com", "piakcneisfjjtrvw");
            smtp.Send(mail);

            return "ok";
        }
    }
}
