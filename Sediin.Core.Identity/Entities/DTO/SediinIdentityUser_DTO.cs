using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.Core.Identity.Entities.DTO
{
    public class SediinIdentityUser_DTO
    {
        public string Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cognome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
