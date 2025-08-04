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
}
