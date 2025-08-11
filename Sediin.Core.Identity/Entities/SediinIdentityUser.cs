using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Sediin.Core.Identity.Entities
{
    /// <summary>
    /// Ogni proprietà aggiunta al modello va sincronizzata anche sul database dell'Identity
    /// AspNetUsers
    /// </summary>
    public class SediinIdentityUser : IdentityUser
    {
        [MaxLength(256)]
        public string Nome { get; set; }

        [MaxLength(256)]
        public string Cognome { get; set; }
    }
}
