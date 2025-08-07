using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Sediin.Core.WebUi.Models
{
    public class Authentication
    {
        [ValidateNever]
        public string ReturnUrl { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [MaxLength(75)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [MaxLength(35)]
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [MaxLength(25)]
        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, ErrorMessage = "La {0} deve contenere almeno {2} caratteri.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$",
        ErrorMessage = "La password deve contenere almeno una lettera maiuscola, una minuscola, un numero e un carattere speciale.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [MaxLength(25)]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class RegisterViewModel
    {
        [MaxLength(256)]
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [MaxLength(256)]
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [MaxLength(256)]
        [Required]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        [Required]
        [Display(Name = "Indirizzo E-Mail")]
        public string Email { get; set; }

        [EmailAddress]
        [Required]
        [Compare(otherProperty:"Email")]
        [Display(Name = "Conferma Indirizzo E-Mail")]
        public string ConfirmEmail { get; set; }

    }

}
