namespace PureDelivery.Common.Configuration.Interfaces
{
    public interface IConfiguration<T> where T : class
    {
        /// <summary>
        /// Валидация конфигурации
        /// </summary>
        void Validate();
    }
}