using CEM.Data;
using CEM.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEM.Service
{
    public class ManageCurrencyExchange(ILogger<ManageCurrencyExchange> log) : IManageCurrencyExchange
    {
        public string ExchangeRatesHistory { get; set; }
        public CEMDbContext CEMDbContext { get; set; }
        public void SaveDailyHistory()
        {
            try
            {
                CEMDbContext.Add(getLatestExchangeRates<SpotRate>());
                CEMDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
            }
           //Get History from DB
        }

        public T getLatestExchangeRates<T>()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage = httpClient.GetAsync(ExchangeRatesHistory.Replace("date",DateTime.Now.ToString("yyyy-MM-dd"))).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
            }
            return default;
        }
    }
}
