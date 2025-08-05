using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.Core.DataAccess.Entities
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Codmenu { get; set; }

        public int? CodmenuPadre { get; set; }

        public string Descrizione { get; set; }

        public string IconFa { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public int? Ordine { get; set; }

        public bool? Visible { get; set; }

        public string Area { get; set; }

        [ForeignKey("Codmenu")]
        public virtual ICollection<MenuRuoli> Ruoli { get; set; }
    }

}
