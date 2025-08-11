using Microsoft.AspNetCore.Identity.UI.Services;
using Sediin.Core.TemplateConfiguration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sediin.Core.WebUi.Controllers
{
    public class EmailSender : IEmailSender
    {
        private readonly ISediinCoreConfiguration _emailSettings;

        public EmailSender(ISediinCoreConfiguration _settings)
        {
            _emailSettings = _settings;
        }

        public async Task SendEmailAsync(string toAddress, string subject, string bodyHtml)
        {
            var _es = _emailSettings.Get().Result.EmailSettings;

            using var smtpClient = new SmtpClient(_es.SmtpServer)
            {
                Port = _es.SmtpPort,
                EnableSsl = _es.SmtpServerUseSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_es.SmtpUsername, _es.SmtpPassword)
            };

            var emailMessage = new MailMessage
            {
                From = new MailAddress(_es.FromEmail, _es.FromName),
                Subject = subject,
                IsBodyHtml = true
            };
            emailMessage.To.Add(toAddress);

            // La tua stringa base64 con prefisso
            string base64WithPrefix = _emailSettings.Get().Result.RagioneSociale.LogoBase64;

            var logoResource = CreateLinkedResourceFromBase64(base64WithPrefix, "logo_cid");

            // Qui costruisci l’HTML con img che fa riferimento al CID
            //string htmlBody = $@"
            //    <html>
            //        <body>
            //            <h1>{subject}</h1>
            //            <p>{bodyHtml}</p>
            //            <img src=""cid:logo_cid"" alt=""Logo"" />
            //        </body>
            //    </html>";

            // Creo AlternateView HTML con risorsa collegata inline
            var htmlView = AlternateView.CreateAlternateViewFromString(bodyHtml, null, MediaTypeNames.Text.Html);
            htmlView.LinkedResources.Add(logoResource);

            emailMessage.AlternateViews.Add(htmlView);

            await smtpClient.SendMailAsync(emailMessage);
        }

        private LinkedResource CreateLinkedResourceFromBase64(string base64WithPrefix, string contentId)
        {
            if (string.IsNullOrWhiteSpace(base64WithPrefix))
                throw new ArgumentException("La stringa base64 non può essere nulla o vuota.");

            var parts = base64WithPrefix.Split(',');

            if (parts.Length != 2)
                throw new ArgumentException("Stringa base64 non valida.");

            string metadata = parts[0];  // es. data:image/png;base64
            string base64Pure = parts[1];

            // Estraggo MIME type
            var mimeType = metadata.Substring("data:".Length, metadata.IndexOf(";") - "data:".Length);

            byte[] imageBytes = Convert.FromBase64String(base64Pure);

            var ms = new MemoryStream(imageBytes);

            var resource = new LinkedResource(ms, mimeType)
            {
                ContentId = contentId,
                TransferEncoding = TransferEncoding.Base64,
                ContentType = { MediaType = mimeType },
                ContentLink = new Uri("cid:" + contentId)
            };

            return resource;
        }
    }
}
