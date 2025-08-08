using Microsoft.Extensions.Caching.Memory;
using Sediin.Core.TemplateConfiguration.Models;
using System.Text.Json;

namespace Sediin.Core.TemplateConfiguration
{
    public interface ISediinCoreConfiguration
    {
        /// <summary>
        /// Prende da MemoryCache
        /// </summary>
        Task<SediinConfiguration> Get();
        Task Save(SediinConfiguration config);
        Task SaveRagioneSociale(RagioneSociale ragioneSociale);
        Task SaveEmailSettings(EmailSettings emailSettings);
    }

    public class SediinCoreConfiguration : ISediinCoreConfiguration
    {
        private readonly string _configFilePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;
        private readonly string CacheKey = "SediinConfiguration";
        private readonly TimeSpan _cacheDuration;

        public SediinCoreConfiguration(string jsonFilePath, IMemoryCache memoryCache, TimeSpan cacheDuration)
        {
            _configFilePath = jsonFilePath;
            _cache = memoryCache;
            _cacheDuration = cacheDuration;

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<SediinConfiguration> Get()
        {
            if (_cache.TryGetValue(CacheKey, out SediinConfiguration? config) && config != null)
                return config;

            config = await GetFromFile();
            _cache.Set(CacheKey, config, _cacheDuration);
            return config;
        }

        private async Task<SediinConfiguration> GetFromFile()
        {
            if (!File.Exists(_configFilePath))
                throw new FileNotFoundException($"File configurazione \"config.json\" non trovato. Percorso: {_configFilePath}");

            using var stream = File.OpenRead(_configFilePath);
            var config = await JsonSerializer.DeserializeAsync<SediinConfiguration>(stream, _jsonOptions);
            return config ?? new SediinConfiguration();
        }

        public async Task Save(SediinConfiguration config)
        {
            using var stream = File.Create(_configFilePath);
            await JsonSerializer.SerializeAsync(stream, config, _jsonOptions);

            _cache.Set(CacheKey, config, _cacheDuration);
        }

        public async Task SaveRagioneSociale(RagioneSociale ragioneSociale)
        {
            var config = await GetFromFile();
            config.RagioneSociale = ragioneSociale;
            await Save(config);
        }

        public async Task SaveEmailSettings(EmailSettings emailSettings)
        {
            var config = await GetFromFile();
            config.EmailSettings = emailSettings;
            await Save(config);
        }
    }
}
