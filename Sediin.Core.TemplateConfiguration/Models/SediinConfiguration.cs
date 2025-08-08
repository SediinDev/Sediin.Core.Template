namespace Sediin.Core.TemplateConfiguration.Models
{
    public class SediinConfiguration
    {
        public string? UploadFolder { get; set; }
        public string? UriPortale { get; set; }
        public string? LogoBase64 { get; set; }
        public RagioneSociale? RagioneSociale { get; set; }
        public EmailSettings? EmailSettings { get; set; }
    }

    public class RagioneSociale
    {
        public string? Nome { get; set; }
        public string? NomeBreve { get; set; }
        public string? Regione { get; set; }
        public string? Provincia { get; set; }
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? Cap { get; set; }
        public string? Telefono { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? Pec { get; set; }
        public string? CodiceFiscale { get; set; }
        public string? PartitaIva { get; set; }
        public string? SitoWeb { get; set; }
    }

    public class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public bool SmtpServerAutentication { get; set; }
        public bool SmtpServerUseSSL { get; set; }
        public string? FromName { get; set; }
        public string? FromEmail { get; set; }
    }

}
