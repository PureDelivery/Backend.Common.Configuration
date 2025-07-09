using Microsoft.Extensions.DependencyInjection;
using PureDelivery.Common.Configuration.Enums;

namespace PureDelivery.Common.Configuration.Factories
{
    /// <summary>
    /// Фабрика для создания провайдеров конфигурации
    /// </summary>
    public interface IConfigurationProviderFactory
    {
        /// <summary>
        /// Зарегистрировать провайдер конфигурации
        /// </summary>
        void RegisterProvider(IServiceCollection services, ConfigurationSource source);

        /// <summary>
        /// Проверить поддержку источника конфигурации
        /// </summary>
        bool SupportsSource(ConfigurationSource source);
    }
}