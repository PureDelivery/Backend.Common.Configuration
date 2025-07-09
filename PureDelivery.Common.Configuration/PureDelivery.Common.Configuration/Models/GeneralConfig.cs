using PureDelivery.Common.Configuration.Enums;
using PureDelivery.Common.Configuration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Models
{
    public class GeneralConfig : IConfiguration
    {
        public ConfigurationSource ConfigSource { get; set; } = ConfigurationSource.Local;
        public Dictionary<string, string> ConfigUrls { get; set; } = new();
        public Dictionary<string, string> ConfigPaths { get; set; } = new();
        public bool EnableCaching { get; set; } = true;
        public int CacheExpirationMinutes { get; set; } = 15;

        public void Validate()
        {
            if (ConfigSource == ConfigurationSource.Local && !ConfigPaths.Any())
                throw new InvalidOperationException("Local configuration source requires at least one path to be specified.");
            if (ConfigSource == ConfigurationSource.Remote && !ConfigUrls.Any())
                throw new InvalidOperationException("Remote configuration source requires at least one URL to be specified.");
            if (CacheExpirationMinutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(CacheExpirationMinutes), "Cache expiration must be greater than zero.");
        }
    }
}
