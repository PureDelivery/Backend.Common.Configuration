using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Http;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Http.impl
{
    /// <summary>
    /// Минимальная реализация HTTP клиента для получения конфигурации
    /// </summary>
    public class ConfigurationHttpClient : IConfigurationHttpClient, IDisposable
    {
        private readonly ILogger<ConfigurationHttpClient> _logger;
        private const int DefaultTimeoutSeconds = 30;
        private const int DefaultRetryCount = 3;
        private const int DefaultRetryDelayMs = 1000;

        public ConfigurationHttpClient(ILogger<ConfigurationHttpClient> logger)
        {
            _logger = logger;
        }

        public async Task<T?> GetConfigAsync<T>(string baseUrl, string endpoint, CancellationToken cancellationToken = default)
        {
            var client = CreateRestClient(baseUrl);
            var request = new RestRequest(endpoint, Method.Get);

            Exception? lastException = null;

            for (int attempt = 0; attempt <= DefaultRetryCount; attempt++)
            {
                try
                {
                    _logger.LogDebug("Getting configuration from {BaseUrl}{Endpoint} (attempt {Attempt}/{Total})",
                        baseUrl, endpoint, attempt + 1, DefaultRetryCount + 1);

                    var response = await client.ExecuteAsync<T>(request, cancellationToken);

                    if (response.IsSuccessful && response.Data != null)
                    {
                        _logger.LogInformation("Successfully retrieved configuration from {BaseUrl}{Endpoint}", baseUrl, endpoint);
                        return response.Data;
                    }
                    else
                    {
                        var errorMsg = $"HTTP {(int)response.StatusCode} {response.StatusCode}: {response.ErrorMessage}";
                        _logger.LogWarning("Failed to get configuration: {Error} (attempt {Attempt}/{Total})",
                            errorMsg, attempt + 1, DefaultRetryCount + 1);

                        if (attempt == DefaultRetryCount)
                        {
                            throw new InvalidOperationException($"Failed to get configuration after {DefaultRetryCount + 1} attempts: {errorMsg}");
                        }
                    }
                }
                catch (Exception ex) when (attempt < DefaultRetryCount)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, "Configuration request failed, retrying in {Delay}ms (attempt {Attempt}/{Total})",
                        DefaultRetryDelayMs * (attempt + 1), attempt + 1, DefaultRetryCount + 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Configuration request failed after {Attempts} attempts", DefaultRetryCount + 1);
                    throw;
                }

                // Задержка перед повторной попыткой
                if (attempt < DefaultRetryCount)
                {
                    await Task.Delay(DefaultRetryDelayMs * (attempt + 1), cancellationToken);
                }
            }

            throw lastException ?? new InvalidOperationException($"Failed to get configuration after {DefaultRetryCount + 1} attempts");
        }

        public async Task<bool> IsHealthyAsync(string baseUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateRestClient(baseUrl);
                var request = new RestRequest("/health", Method.Get);

                var response = await client.ExecuteAsync(request, cancellationToken);
                var isHealthy = response.IsSuccessful;

                _logger.LogDebug("Health check for {BaseUrl}: {IsHealthy}", baseUrl, isHealthy);
                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check failed for {BaseUrl}", baseUrl);
                return false;
            }
        }

        private static RestClient CreateRestClient(string baseUrl)
        {
            var options = new RestClientOptions(baseUrl)
            {
                Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds),
                UserAgent = "PureDelivery-ConfigClient/1.0"
            };

            return new RestClient(options);
        }

        public void Dispose()
        {
            // RestClient будет создаваться для каждого запроса, поэтому здесь ничего не делаем
        }
    }
}