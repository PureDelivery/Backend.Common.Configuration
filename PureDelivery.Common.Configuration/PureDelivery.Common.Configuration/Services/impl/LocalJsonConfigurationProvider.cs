using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Models;
using PureDelivery.Common.Configuration.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Services.impl
{
    public class LocalJsonConfigurationProvider : ICustomConfigurationProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalJsonConfigurationProvider> _logger;

        public LocalJsonConfigurationProvider(IConfiguration configuration, ILogger<LocalJsonConfigurationProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            var generalConfig = _configuration.GetSection("GeneralConfig").Get<GeneralConfig>();
            if (generalConfig == null || !generalConfig.ConfigPaths.TryGetValue(serviceName, out var configPath))
            {
                _logger.LogError("Configuration path for {ServiceName} not found in GeneralConfig", serviceName);
                throw new InvalidOperationException($"Configuration path for {serviceName} not found");
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), configPath);

            if (!File.Exists(fullPath))
            {
                _logger.LogError("Configuration file not found at {FilePath}", fullPath);
                throw new FileNotFoundException($"Configuration file not found: {fullPath}");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build()
                .Get<T>();

            if (config == null)
            {
                _logger.LogError("Failed to load configuration for {ServiceName} from {ConfigPath}", serviceName, configPath);
                throw new InvalidOperationException($"Failed to load configuration for {serviceName}");
            }

            _logger.LogInformation("Loaded configuration for {ServiceName} from {ConfigPath}", serviceName, configPath);
            return await Task.FromResult(config);
        }
    }
}