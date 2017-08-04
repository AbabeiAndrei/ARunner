using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ARunner.Services
{
    public class MailSender : IMailSender
    {
        public string FromName { get; set; }

        public string FromAddress { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public MailSender(IConfigurationRoot configuration)
        {
            var a = configuration["MailServerSettings"];
            FromAddress = configuration["MailServerSettings:FromName"];
            FromAddress = configuration["MailServerSettings:FromAddress"];
            Host = configuration["MailServerSettings:Host"];
            Port = int.Parse(configuration["MailServerSettings:Port"]);
            UseSsl = bool.Parse(configuration["MailServerSettings:UseSsl"]);
            UserName = configuration["MailServerSettings:UserName"];
            Password = configuration["MailServerSettings:Password"];
        }

        public async Task SendMail(MailData mail)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(FromName, FromAddress));
                message.To.Add(new MailboxAddress(mail.ToName, mail.ToAddress));
                message.Subject = mail.Subject;
                message.Body = new TextPart("plain")
                {
                    Text = mail.Body
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(Host, Port, UseSsl);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    // Note: since we don't have an OAuth2 token, disable 	// the XOAUTH2 authentication mechanism.     
                    client.Authenticate(UserName, Password);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception mex)
            {
                throw;
            }

        }
    }
}
