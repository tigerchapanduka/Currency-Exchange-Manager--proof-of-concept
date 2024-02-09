using CEM.Data;
using CEM.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Cache
{
    public interface ICacheRepository
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);

        T GetLatestRates<T>(string @base);
        string GetKeyValue(string @base);
        CEMDbContext dbContext { get; set; }

    }
}
