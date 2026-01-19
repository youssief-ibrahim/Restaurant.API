using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Application.Interfaces;

namespace Restaurant.Infrastructure.Services.Email
{
    public class EmailSender : IEmailService
    {
        public async Task SendEmailAsync(string Email, string subject, string body)
        {
            // Your Gmail
            var fromMail = "youssiefibrahim3@gmail.com";

            // Your App Password (NOT Gmail password)
            var fromPassword = "eyhk ucfi zuvl eqgn";
            var message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.To.Add(new MailAddress(Email));
            message.Subject = subject;
            message.Body = $"<html><body>{body}</body></html>";
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new System.Net.NetworkCredential(fromMail, fromPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }

        }
    }

}
