using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;

namespace BrainHope.Services.Services
{
    //public class EmailService : IEmailService
    //{
    //    private readonly EmailConfiguration _emailConfiguration;

    //    public EmailService(EmailConfiguration emailConfiguration)
    //    {
    //        this._emailConfiguration = emailConfiguration;
    //    }
    //    public void SendEmail(Message message)
    //    {
    //        var emailmessage = CreateEmailMessage(message);
    //        Send(emailmessage);
    //    }
    //    private MimeMessage CreateEmailMessage(Message message)
    //    {
    //        var emailmessage = new MimeMessage();
    //        emailmessage.From.Add(new MailboxAddress("Brain Hope", _emailConfiguration.From));
    //        emailmessage.To.AddRange(message.To);
    //        emailmessage.Subject = message.Subject;
    //        emailmessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
    //        return emailmessage;
    //    }
    //    private void Send(MimeMessage mailMessage)
    //    {
    //        using var client = new SmtpClient();
    //        try
    //        {
    //            client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
    //            client.AuthenticationMechanisms.Remove("XOAUTH2");
    //            client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);
    //            client.Send(mailMessage);
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            client.Disconnect(true);
    //            client.Dispose();
    //        }
    //    }

    //}
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var email = new MimeMessage
            {
                Subject = message.Subject,
                Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content }
            };
            email.From.Add(new MailboxAddress("Brain Hope", _emailConfig.From));
            email.To.AddRange(message.To);

            using var client = new SmtpClient();

            try
            {
                //client.Connect(_emailConfig.SmtpServer, 465, SecureSocketOptions.SslOnConnect);
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);

                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(email);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message);
            }
        }

    }
}
