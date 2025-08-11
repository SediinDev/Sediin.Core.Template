using System.ComponentModel.DataAnnotations;

namespace Sediin.Core.WebUi.Areas.Administrator.Models
{
    public class TestMailSettingConfigModel
    {
        [Required]
        [EmailAddress]
        public string? EmailTo { get; set; }
    }
}
