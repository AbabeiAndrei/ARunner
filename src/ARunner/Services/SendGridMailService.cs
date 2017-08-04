using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ARunner.Services
{
    public class SendGridMailService : IMailSender
    {
        private readonly IConfigurationRoot _configuration;

        public SendGridMailService(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMail(MailData mail)
        {
            var apiKey = _configuration["SendGridKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["MailServerSettings:FromAddress"], _configuration["MailServerSettings:FromName"]);
            var subject = mail.Subject;
            var to = new EmailAddress(mail.ToAddress, mail.ToName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, mail.Body, string.Empty);
            var response = await client.SendEmailAsync(msg);


            if(response.StatusCode != HttpStatusCode.Accepted)
                throw new Exception($"Mail not send, error {(int)response.StatusCode}");
        }
    }
}
