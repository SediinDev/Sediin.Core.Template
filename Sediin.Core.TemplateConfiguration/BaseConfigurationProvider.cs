using Sediin.Core.TemplateConfiguration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sediin.Core.TemplateConfiguration
{
    public interface IBaseConfiguration
    {

        BaseConfigurationModel Get();
    }
    public class BaseConfiguration : IBaseConfiguration
    {
        private readonly string _jsonFilePath;

        public BaseConfiguration(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
        }

        public BaseConfigurationModel Get()
        {
            if (!File.Exists(_jsonFilePath))
                throw new FileNotFoundException("Config file not found", _jsonFilePath);

            string json = File.ReadAllText(_jsonFilePath);

            return JsonSerializer.Deserialize<BaseConfigurationModel>(json);
        }
    }
}
