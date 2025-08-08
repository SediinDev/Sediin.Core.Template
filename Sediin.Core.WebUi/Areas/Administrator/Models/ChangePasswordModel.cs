using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sediin.Core.WebUi.Areas.Administrator.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Vecchia password")]
        public string? OldPassword { get; set; }

        [MaxLength(25)]
        [DisplayName("Nuova password")]
        [Required]
        [StringLength(100, ErrorMessage = "La {0} deve contenere almeno {2} caratteri.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$",
        ErrorMessage = "La password deve contenere almeno una lettera maiuscola, una minuscola, un numero e un carattere speciale.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "La conferma password non corrisponde.")]
        [DisplayName("Conferma nuova password")]
        public string? ConfirmNewPassword { get; set; }
    }
}
