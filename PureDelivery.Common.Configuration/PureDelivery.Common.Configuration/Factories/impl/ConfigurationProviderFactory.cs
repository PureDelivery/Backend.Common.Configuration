using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Enums;
using PureDelivery.Common.Configuration.Factories;
using PureDelivery.Common.Configuration.Http;
using PureDelivery.Common.Configuration.Http.impl;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.Common.Configuration.Services.impl;

namespace PureDelivery.Common.Configuration.Factories.impl
{
    /// <summary>
    /// Фабрика провайдеров конфигурации
    /// </summary>
    public class ConfigurationProviderFactory : IConfigurationProviderFactory
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<ConfigurationSource, Action<IServiceCollection>> _providers;

        public ConfigurationProviderFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _providers = new Dictionary<ConfigurationSource, Action<IServiceCollection>>
            {
                { ConfigurationSource.Local, RegisterLocalProvider },
                { ConfigurationSource.Remote, RegisterRemoteProvider },
                { ConfigurationSource.Hybrid, RegisterHybridProvider },
                { ConfigurationSource.Environment, RegisterEnvironmentProvider }
            };
        }

        public void RegisterProvider(IServiceCollection services, ConfigurationSource source)
        {
            if (!_providers.TryGetValue(source, out var registrationAction))
            {
                var supportedSources = string.Join(", ", _providers.Keys);
                throw new NotSupportedException($"Configuration source '{source}' is not supported. Supported sources: {supportedSources}");
            }

            registrationAction(services);
        }

        public bool SupportsSource(ConfigurationSource source)
        {
            return _providers.ContainsKey(source);
        }

        private void RegisterLocalProvider(IServiceCollection services)
        {
            services.AddSingleton<ICustomConfigurationProvider, LocalJsonConfigurationProvider>();
        }

        private void RegisterRemoteProvider(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationHttpClient, ConfigurationHttpClient>();
            services.AddSingleton<ICustomConfigurationProvider, RemoteConfigurationProvider>();
        }

        private void RegisterHybridProvider(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationHttpClient, ConfigurationHttpClient>();

            services.AddSingleton<ICustomConfigurationProvider>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<HybridConfigurationProvider>>();
                var httpClient = sp.GetRequiredService<IConfigurationHttpClient>();
                var localProvider = new LocalJsonConfigurationProvider(_configuration,
                    sp.GetRequiredService<ILogger<LocalJsonConfigurationProvider>>());
                var remoteProvider = new RemoteConfigurationProvider(httpClient,
                    sp.GetRequiredService<ILogger<RemoteConfigurationProvider>>());

                return new HybridConfigurationProvider(remoteProvider, localProvider, logger);
            });
        }

        private void RegisterEnvironmentProvider(IServiceCollection services)
        {
            services.AddSingleton<ICustomConfigurationProvider, EnvironmentConfigurationProvider>();
        }
    }
}