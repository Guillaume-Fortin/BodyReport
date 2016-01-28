using BodyReport.Framework;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BodyReport.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                // client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
               // client.Authenticate("username", "password");

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("BodyReport", WebAppConfiguration.SmtpEmail));
                mimeMessage.To.Add(new MailboxAddress(email, email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html") { Text = message };
                
                client.Connect(WebAppConfiguration.SmtpServer, WebAppConfiguration.SmtpPort, SecureSocketOptions.None);
                client.Authenticate(WebAppConfiguration.SmtpUserName, WebAppConfiguration.SmtpPassword);

                client.Send(mimeMessage);
                client.Disconnect(true);
            }
            
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
