using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Services.impl
{
    /// <summary>
    /// Провайдер конфигурации из переменных окружения
    /// </summary>
    public class EnvironmentConfigurationProvider : ICustomConfigurationProvider
    {
        private readonly ILogger<EnvironmentConfigurationProvider> _logger;

        public EnvironmentConfigurationProvider(ILogger<EnvironmentConfigurationProvider> logger)
        {
            _logger = logger;
        }

        public async Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            var envVarName = $"PUREDELIVERY_{serviceName.ToUpper()}_CONFIG";
            var configJson = Environment.GetEnvironmentVariable(envVarName);

            if (string.IsNullOrEmpty(configJson))
            {
                _logger.LogError("Environment variable {EnvVarName} not found or empty", envVarName);
                throw new InvalidOperationException($"Environment variable {envVarName} not found");
            }

            try
            {
                var config = JsonSerializer.Deserialize<T>(configJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (config == null)
                {
                    throw new InvalidOperationException($"Failed to deserialize configuration from {envVarName}");
                }

                _logger.LogInformation("Successfully loaded configuration for {ServiceName} from environment variable {EnvVarName}",
                    serviceName, envVarName);

                return await Task.FromResult(config);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize configuration from environment variable {EnvVarName}", envVarName);
                throw new InvalidOperationException($"Invalid JSON in environment variable {envVarName}", ex);
            }
        }
    }
}