using System.ComponentModel.DataAnnotations;

namespace Sediin.Core.TemplateConfiguration.Models
{
    public class SediinConfiguration
    {
        public string? UploadFolder { get; set; }
        public RagioneSociale? RagioneSociale { get; set; }
        public EmailSettings? EmailSettings { get; set; }
    }

    public class RagioneSociale
    {
        [Required] 
        public string? Nome { get; set; }
        [Required]
        public string? NomeBreve { get; set; }
        [Required]
        public string? Regione { get; set; }
        [Required]
        public string? Provincia { get; set; }
        [Required]
        public string? Indirizzo { get; set; }
        [Required]
        public string? Citta { get; set; }
        [Required]
        public string? Cap { get; set; }
        [Required]
        public string? Telefono { get; set; }

        public string? Fax { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Pec { get; set; }
        [Required]
        public string? CodiceFiscale { get; set; }
        [Required]
        public string? PartitaIva { get; set; }
        [Required]
        public string? SitoWeb { get; set; }
        [Required]
        public string? UriPortale { get; set; }
        [Required]
        public string? LogoBase64 { get; set; }
    }

    public class EmailSettings
    {
        [Required]
        public string? SmtpServer { get; set; }
        [Required] 
        public int SmtpPort { get; set; }
        [Required]
        public string? SmtpUsername { get; set; }
        [Required] 
        public string? SmtpPassword { get; set; }
        [Required] 
        public bool SmtpServerAutentication { get; set; }
        [Required]
        public bool SmtpServerUseSSL { get; set; }
        [Required]
        public string? FromName { get; set; }
        [Required]
        public string? FromEmail { get; set; }
    }

}
