using System.ComponentModel.DataAnnotations;

namespace Sediin.Core.Identity.Entities.DTO
{
    public class SediinIdentityUser_DTO
    {
        public string Id { get; set; }

        [Required]
        public string Nome { get; set; }
       
        public string Username { get; set; }

        [Required]
        public string Cognome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Compare("Email")]
        public string ConfirmEmail { get; set; }

    }
}
