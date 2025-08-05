using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.Core.Identity.Entities
{
    public class SediinIdentityUser : IdentityUser
    {
        [MaxLength(256)]
        public string Nome { get; set; }

        [MaxLength(256)]
        public string Cognome { get; set; }
    }
}
