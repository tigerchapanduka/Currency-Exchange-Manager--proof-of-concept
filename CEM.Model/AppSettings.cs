using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Model
{
    public class AppSettings
    {
        public int HistoricalRefreshTime { get; set; }
        public string RedisURL { get; set; }
        public string? ExchangeRatesURL { get; set; }
        public string? ExchangeRatesHistoricalURL { get; set; }
        public int? CacheExpiry { get; set; }
    }
}
