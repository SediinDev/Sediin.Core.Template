using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.Core.DataAccess.Entities
{
    [Table("MenuRuoli")]
    public class MenuRuoli
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Codmenu { get; set; }

        public string Ruolo { get; set; }

        [ForeignKey("Codmenu")]
        public virtual Menu Menu { get; set; }
    }
}
