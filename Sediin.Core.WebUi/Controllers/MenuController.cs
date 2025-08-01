using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sediin.Core.WebUi.Models;
using System.Data;

namespace Sediin.Core.WebUi.Controllers
{
    public class MenuController : Controller
    {
        private string MenuFilename { get; set; } = string.Empty;

        public MenuController()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json")
                             .Build();
            MenuFilename = config.GetSection("ApplicationSettings")["MenuFilename"]?.ToString();
        }
        
        public IActionResult Index()
        {
            List<MenuModel> model = GetMenu("Administrator");
            return PartialView("Menu", model);
        }

        public List<MenuModel> GetMenu(string Role)
        {
            if (string.IsNullOrWhiteSpace(MenuFilename) == false)
            {
                try
                {
                    using StreamReader reader = new StreamReader(MenuFilename);
                    string json = reader.ReadToEnd();
                    var resultJson = JsonConvert.DeserializeObject<List<MenuModel>>(json);
                    var model = resultJson.Where(x => x.Ruoli.Contains(Role, StringComparer.OrdinalIgnoreCase)).ToList<MenuModel>();

                    return model;
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }

            return new List<MenuModel>();
        }
    }
}
