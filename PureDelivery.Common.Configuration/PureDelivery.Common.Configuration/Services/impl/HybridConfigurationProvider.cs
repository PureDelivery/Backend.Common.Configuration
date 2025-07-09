using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using System;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Services.impl
{
    public class HybridConfigurationProvider : ICustomConfigurationProvider
    {
        private readonly ICustomConfigurationProvider _remoteProvider;
        private readonly ICustomConfigurationProvider _localProvider;
        private readonly ILogger<HybridConfigurationProvider> _logger;

        public HybridConfigurationProvider(
            ICustomConfigurationProvider remoteProvider,
            ICustomConfigurationProvider localProvider,
            ILogger<HybridConfigurationProvider> logger)
        {
            _remoteProvider = remoteProvider;
            _localProvider = localProvider;
            _logger = logger;
        }

        public async Task<T> GetConfigurationAsync<T>(string serviceName)
        {
            try
            {
                _logger.LogDebug("Attempting to get configuration for {ServiceName} from remote source", serviceName);
                var config = await _remoteProvider.GetConfigurationAsync<T>(serviceName);
                _logger.LogInformation("Successfully loaded configuration for {ServiceName} from remote source", serviceName);
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get configuration for {ServiceName} from remote source, falling back to local", serviceName);

                try
                {
                    var config = await _localProvider.GetConfigurationAsync<T>(serviceName);
                    _logger.LogInformation("Successfully loaded configuration for {ServiceName} from local fallback", serviceName);
                    return config;
                }
                catch (Exception localEx)
                {
                    _logger.LogError(localEx, "Failed to get configuration for {ServiceName} from both remote and local sources", serviceName);
                    throw new InvalidOperationException($"Failed to get configuration for {serviceName} from both remote and local sources", localEx);
                }
            }
        }
    }
}