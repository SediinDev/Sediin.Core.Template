using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;
using Sediin.Core.TemplateConfiguration;
using Sediin.Core.TemplateConfiguration.Models;
using System.Net;
using System.Net.Mail;

namespace Sediin.Core.WebUi.Controllers
{
    //public class EmailSettings
    //{
    //    public string SmtpServer { get; set; } = string.Empty;
    //    public int SmtpPort { get; set; }
    //    public string SmtpUsername { get; set; } = string.Empty;
    //    public string SmtpPassword { get; set; } = string.Empty;
    //    public bool SmtpServerAutentication { get; set; }
    //    public bool SmtpServerUseSSL { get; set; }
    //    public string FromName { get; set; } = string.Empty;
    //    public string FromEmail { get; set; } = string.Empty;
    //}

    public class EmailSender : IEmailSender
    {
        private readonly ISediinCoreConfiguration _emailSettings;

        public EmailSender(ISediinCoreConfiguration _settings)
        {
            _emailSettings = _settings;
        }

        public async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            try
            {
                var _es= _emailSettings.Get().Result.EmailSettings;

                var smtpClient = new SmtpClient(_es.SmtpServer)
                {
                    Port = _es.SmtpPort,
                    EnableSsl = _es.SmtpServerUseSSL,
                    UseDefaultCredentials = false,
                };
                smtpClient.Credentials = new NetworkCredential(_es.SmtpUsername, _es.SmtpPassword);

                var emailMessage = new MailMessage
                {
                    From = new MailAddress(_es.FromEmail, _es.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    //Sender = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                };

                emailMessage.To.Add(toAddress);

                await smtpClient.SendMailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
