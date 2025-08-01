namespace Sediin.Core.WebUi.Models
{
    public class MenuModel
    {
        public int? IdMenuPadre { get; set; }
        public int IdMenu { get; set; }
        public bool? HaSubmenu { get; set; }
        public string? TipoMenu { get; set; }
        public string? Descrizione { get; set; }
        public string? DescrizioneSmall { get; set; }
        public string? IconFa { get; set; }
        public string? Area { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public int? Ordine { get; set; }
        public bool Visible { get; set; }
        public string[]? Ruoli { get; set; }
    }
}
