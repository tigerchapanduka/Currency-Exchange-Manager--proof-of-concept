using CEM.Data;
using CEM.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Service
{
    public class CacheHistoryService(CEMDbContext sp, IManageCurrencyExchange manageCurrencyExchangeHistory, int delay, string exchangeRatesHistory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(delay, stoppingToken);
                manageCurrencyExchangeHistory.ExchangeRatesHistory = exchangeRatesHistory;
                manageCurrencyExchangeHistory.CEMDbContext = sp;
                manageCurrencyExchangeHistory.SaveDailyHistory();
            }
        }
    }
}
