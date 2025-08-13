namespace Sediin.Core.WebUi.Areas.TemplateEmail.Models
{
    public class ConfirmEmailModel
    {
        public string Url { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Url { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
