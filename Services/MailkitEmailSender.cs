using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class MailkitEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailKitOptions _mailKitOptions;
        public MailkitEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return sendMessage(email, subject, htmlMessage);

        }

        public Task sendMessage(string email, string subject, string htmlMessage)
        {
            _mailKitOptions = _configuration.GetSection("MailKit").Get<MailKitOptions>();

            MimeMessage message = new MimeMessage();
            //Sender's Email
            message.From.Add(new MailboxAddress(_mailKitOptions.name, _mailKitOptions.email));

            //Receiver's Email
            message.To.Add(MailboxAddress.Parse(email));

            //Message Subject
            message.Subject = subject;

            //Message Body
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };


            SmtpClient client = new SmtpClient();

            try
            {
                client.Connect(_mailKitOptions.smtpserver, _mailKitOptions.portnumber, _mailKitOptions.trustedconnection);
                client.Authenticate(_mailKitOptions.username, _mailKitOptions.password);
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

            return Task.FromResult(true);
        }
    }
}
