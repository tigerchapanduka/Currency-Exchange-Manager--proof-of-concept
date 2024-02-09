using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEM.Model;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using CEM.Data;
namespace CEM.Cache
{
    public class CacheRepository:ICacheRepository
    {
        
        private IDatabase cacheDb;
        private  ConnectionMultiplexer connection;
        private string exchangeRatesURL;

        public CEMDbContext dbContext { get ; set ; }

        public CacheRepository(string config, string exchangeRatesURL)
        {
            connection = ConnectionMultiplexer.Connect(config);
            cacheDb = connection.GetDatabase();
            this.exchangeRatesURL = exchangeRatesURL;
        }
        T ICacheRepository.GetData<T>(string @base)
        {
            var value = cacheDb.StringGet(@base);
            if(!string.IsNullOrEmpty(value))
            {
                return  JsonConvert.DeserializeObject<T>(value);
            
            }

            return default;
        }
        //Get the Dictionary key value pairs(CurrencyCode/Rate) from cache as string
        string ICacheRepository.GetKeyValue(string @base)
        {
            return cacheDb.StringGet(@base);
      
        }
        object ICacheRepository.RemoveData(string key)
        {
            if(cacheDb.KeyExists(key))
                return cacheDb.KeyDelete(key);
            return false;
        }
        //Cache expiry monitor will require more time to complete implementation
        //Once the value has expired we must call either the
        //Latest end point or historical with required date then save those results to database
        //The Redis configuration must be changed to allow for notifications as they are not automatically
        // triggered by default.
        // Apply following command to enable event triggers on cache date config: set notify=keyspace-events x in the Redis console
        bool ICacheRepository.SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

            if (cacheDb.StringSet(key, JsonConvert.SerializeObject(value), expiryTime))
            {
                var subscriber = connection.GetSubscriber();
                subscriber.SubscribeAsync("__keyspace@0__:*", (channel, type) =>
                {
                    var _trigger = GetKey(channel);
                    switch (_trigger)
                    {
                       case "expire":
                            saveExpiringRateToDB(getLatestExchangeRates<SpotRate>(key));
                            break;
                        case "expired":
                            saveExpiringRateToDB(getLatestExchangeRates<SpotRate>(key));
                            break;
                    }
                });
                return true;
            }
            return false;
        }
        static string GetKey(string channel)
        {
            var index = channel.IndexOf(':');

            if (index >= 0 && index < channel.Length - 1)
            {
                return channel[(index + 1)..];
            }

            return channel;
        }
        private void saveExpiringRateToDB(SpotRate currentRate)
        {
            dbContext.Add(currentRate);
            dbContext.SaveChanges();
        }

        //This method is currently  limited to USD base currency as provided
        //By the Free version of exchange rate api
        //The API call will need to be upgraded to dynamically retrieve rates per base currency
        //
        T ICacheRepository.GetLatestRates<T>(string @base)
        {
            return getLatestExchangeRates<T>(@base);

        }

        private T getLatestExchangeRates<T>(string key)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage = httpClient.GetAsync(exchangeRatesURL).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);

            }
            return default;
        }
    }
}
