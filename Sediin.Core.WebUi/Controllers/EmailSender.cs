using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Sediin.Core.WebUi.Controllers
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool SmtpServerAutentication { get; set; }
        public bool SmtpServerUseSSL { get; set; }
        public string FromName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.SmtpPort,
                EnableSsl = _emailSettings.SmtpServerUseSSL,
                UseDefaultCredentials = false,               
            };
            smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

            var emailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                //Sender = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            };

            emailMessage.To.Add(toAddress);
            
            try
            {
                await smtpClient.SendMailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log error)
                //Debug.WriteLine($"Error sending email: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
