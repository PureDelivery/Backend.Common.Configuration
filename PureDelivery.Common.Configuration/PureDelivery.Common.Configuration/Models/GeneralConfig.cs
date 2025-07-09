using PureDelivery.Common.Configuration.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Models
{
    public class GeneralConfig
    {
        public ConfigurationSource ConfigSource { get; set; } = ConfigurationSource.Local;
        public Dictionary<string, string> ConfigUrls { get; set; } = new();
        public Dictionary<string, string> ConfigPaths { get; set; } = new();
        public bool EnableCaching { get; set; } = true;
        public int CacheExpirationMinutes { get; set; } = 15;
    }
}
