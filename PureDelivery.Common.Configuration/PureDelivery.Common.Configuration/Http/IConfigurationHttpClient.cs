using System.Threading;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Http
{
    /// <summary>
    /// Минимальный HTTP клиент для получения конфигурации
    /// </summary>
    public interface IConfigurationHttpClient
    {
        /// <summary>
        /// Получить конфигурацию с удаленного сервера
        /// </summary>
        Task<T?> GetConfigAsync<T>(string baseUrl, string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить доступность сервера конфигурации
        /// </summary>
        Task<bool> IsHealthyAsync(string baseUrl, CancellationToken cancellationToken = default);
    }
}