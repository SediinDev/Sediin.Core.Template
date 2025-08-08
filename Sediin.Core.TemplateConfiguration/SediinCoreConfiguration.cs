using Sediin.Core.TemplateConfiguration.Models;
using System.Text.Json;

namespace Sediin.Core.TemplateConfiguration
{
    public interface ISediinCoreConfiguration
    {
        /// <summary>
        /// Il percorso dei json sono impostati in program.cs "App_Data"
        /// Il file json usato per la configurazione si chiama 
        /// "config.json"
        /// </summary>
        /// <returns></returns>
        SediinConfiguration Get();
    }

    public class SediinCoreConfiguration : ISediinCoreConfiguration
    {
        private readonly string _configFilePath;

        public SediinCoreConfiguration(string jsonFilePath)
        {
            _configFilePath = jsonFilePath;
        }

        public SediinConfiguration Get()
        {
            if (!File.Exists(_configFilePath))
                throw new FileNotFoundException("File configurazione \"config.json\" non trovato, percorso: " + _configFilePath);

            string json = File.ReadAllText(_configFilePath);

            return JsonSerializer.Deserialize<SediinConfiguration>(json);
        }
    }
}
