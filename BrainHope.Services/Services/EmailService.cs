using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            this._emailConfiguration = emailConfiguration;
        }
        public void SendEmail(Message message)
        {
            var emailmessage = CreateEmailMessage(message);
            Send(emailmessage);
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailmessage = new MimeMessage();
            emailmessage.From.Add(new MailboxAddress("Brain Hope", _emailConfiguration.From));
            emailmessage.To.AddRange(message.To);
            emailmessage.Subject = message.Subject;
            emailmessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailmessage;
        }
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);
                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
          
    }
}
