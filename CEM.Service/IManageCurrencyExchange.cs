using CEM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Service
{
    public interface IManageCurrencyExchange
    {
        void SaveDailyHistory();
        T getLatestExchangeRates<T>();

        string ExchangeRatesHistory { get; set; }

        CEMDbContext CEMDbContext { get; set; }
    }
}
