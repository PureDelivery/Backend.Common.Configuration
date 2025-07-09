using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PureDelivery.Common.Configuration.Enums;
using PureDelivery.Common.Configuration.Factories;
using PureDelivery.Common.Configuration.Factories.impl;
using PureDelivery.Common.Configuration.Models;
using PureDelivery.Common.Configuration.Services;

namespace PureDelivery.Common.Configuration.Extensions
{
    /// <summary>
    /// Расширения для регистрации провайдеров конфигурации
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавить провайдер конфигурации на основе GeneralConfig
        /// </summary>
        public static IServiceCollection AddConfigurationProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var generalConfig = configuration.GetSection("GeneralConfig").Get<GeneralConfig>();

            if (generalConfig == null)
            {
                throw new InvalidOperationException("GeneralConfig section not found in configuration");
            }

            var factory = new ConfigurationProviderFactory(configuration);

            if (!factory.SupportsSource(generalConfig.ConfigSource))
            {
                var supportedSources = string.Join(", ", Enum.GetValues<ConfigurationSource>());
                throw new NotSupportedException($"Configuration source '{generalConfig.ConfigSource}' is not supported. Supported sources: {supportedSources}");
            }

            factory.RegisterProvider(services, generalConfig.ConfigSource);

            return services;
        }

        /// <summary>
        /// Добавить конкретный провайдер конфигурации
        /// </summary>
        public static IServiceCollection AddConfigurationProvider(
            this IServiceCollection services,
            IConfiguration configuration,
            ConfigurationSource source)
        {
            var factory = new ConfigurationProviderFactory(configuration);
            factory.RegisterProvider(services, source);
            return services;
        }

        /// <summary>
        /// Добавить кастомный провайдер конфигурации
        /// </summary>
        public static IServiceCollection AddCustomConfigurationProvider<T>(
            this IServiceCollection services)
            where T : class, ICustomConfigurationProvider
        {
            services.AddSingleton<ICustomConfigurationProvider, T>();
            return services;
        }
    }
}