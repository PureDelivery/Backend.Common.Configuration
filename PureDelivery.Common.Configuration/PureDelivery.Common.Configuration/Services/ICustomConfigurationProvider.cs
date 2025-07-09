using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Services
{
    public interface ICustomConfigurationProvider
    {
        Task<T> GetConfigurationAsync<T>(string serviceName);
    }
}
