using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Http;
using PureDelivery.Common.Configuration.Models;
using PureDelivery.Common.Configuration.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Services.impl
{
    public class RemoteConfigurationProvider : ICustomConfigurationProvider
    {
        private readonly IConfigurationHttpClient _httpClient;
        private readonly ILogger<RemoteConfigurationProvider> _logger;

        public RemoteConfigurationProvider(IConfigurationHttpClient httpClient, ILogger<RemoteConfigurationProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            var generalConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("app-config.json", optional: true)
                .Build()
                .GetSection("GeneralConfig")
                .Get<GeneralConfig>();

            if (generalConfig == null || !generalConfig.ConfigUrls.TryGetValue(serviceName, out var configUrl))
            {
                _logger.LogError("Configuration URL for {ServiceName} not found in GeneralConfig", serviceName);
                throw new InvalidOperationException($"Configuration URL for {serviceName} not found");
            }

            _logger.LogInformation("Loading configuration for {ServiceName} from {ConfigUrl}", serviceName, configUrl);

            var uri = new Uri(configUrl);
            var baseUrl = $"{uri.Scheme}://{uri.Authority}";
            var endpoint = uri.PathAndQuery;

            var response = await _httpClient.GetConfigAsync<T>(baseUrl, endpoint);

            if (response == null)
            {
                _logger.LogError("Failed to load configuration for {ServiceName} from {ConfigUrl}", serviceName, configUrl);
                throw new InvalidOperationException($"Failed to load configuration for {serviceName}");
            }

            _logger.LogInformation("Successfully loaded configuration for {ServiceName} from {ConfigUrl}", serviceName, configUrl);
            return response;
        }
    }
}