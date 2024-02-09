using CEM.Cache;
using CEM.Data;
using CEM.Model;
using CEM.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Newtonsoft.Json;
using System.Text.Json;

namespace CEMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrrencyConversionController : ControllerBase
    {
        protected readonly CEMDbContext dbContext;
        ICacheRepository _cacheRepository;
        private ILogger<CurrrencyConversionController> _Logger;
        private IConfiguration appSettings;
        public CurrrencyConversionController( ILogger<CurrrencyConversionController> logger,ICacheRepository cacheRepository, CEMDbContext databaseContext, IConfiguration config)
        {
            _cacheRepository = cacheRepository;
            _cacheRepository.dbContext = databaseContext;
            _Logger = logger;
            appSettings = config;
            
            this.dbContext = databaseContext;

        }

        /// <summary>
        /// 
        /// Receives three parameters 
        /// 1 Base currency such as USD, The current Implementation is USD by Default but this can change
        /// 2 TargetGet currency such as EUR
        /// 3 Amount in Base currency
        /// Calculates the equivalent of base amount in target currency
        /// using the daily conversion rate
        /// </summary>
        /// <param name="base">base</param>
        /// <param name="target">target</param>
        /// <param name="amount">amount</param>
        /// <returns>amount</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("convert/{base}/{target}/{amount}")]
        public IActionResult Convert(string @base, string target, double amount)
        {
            
            try
            {
                double total = 0;
                //Get Cached values for base currency code
                string cachedRates = _cacheRepository.GetKeyValue(@base);
                if (cachedRates != null)
                {
                    //Get the exchange rate for target currency from the Json object
                    //JsonHelper implemements functionality for manipulating and dynamically querying the Json object for required values
                    double targetRate = JsonHelper.getTargetCurrencyRate(cachedRates, target.ToUpper());
                    total = targetRate * amount;
                }
                else
                {
                    //Retrieve latest values from third party provider if none is in cache
                    SpotRate latestRate = _cacheRepository.GetLatestRates<SpotRate>(@base);
                    if (latestRate != null)
                    {
                        //Read expiry time from 
                        int expTime = appSettings.GetValue<int>("CacheExpiry");
                        var expiryTime = DateTimeOffset.Now.AddMinutes((double)expTime);
                        if (!_cacheRepository.SetData<SpotRate>(@base, latestRate, expiryTime))
                        {
                            return BadRequest("There was an expected error. Please contect admin if issue persists");
                        }
                        double rate = JsonHelper.getTargetCurrencyRate(JsonConvert.SerializeObject(latestRate), target.ToUpper());
                        total += rate * amount;
                    }

                }
                if (total > 0)
                {
                    return Ok(total);
                }

                return BadRequest("There was an expected error. Please contect admin if issue persists");

            }catch (Exception ex)
            {
                _Logger.LogInformation("Log to console");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Receives three parameters 
        /// 1 Base currency such as USD
        /// 2 Start TimeStamp
        /// 3 End TimeStamp
        /// Retrieves the conversion history for 
        /// period within start and end time stamp
        /// </summary>
        /// <param name="base">base</param>
        /// <param name="start">start</param>
        /// <param name="end">end</param>
        /// <returns>List<SpotRate></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("conversionhistory/{base}")]
        public IActionResult ConversionHistory(string @base)
        {
            return Ok(dbContext.ExchangeRate.ToList<SpotRate>());
        }

        /// <summary>
        /// Test persistence to exchange rate history table
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("SaveSpotRateTest")]
        public IActionResult SaveSpotRateTest()
        {
            try
            {
                ExchangeRateData exchangeRateData = new ExchangeRateData(dbContext);
                exchangeRateData.Save(new SpotRate { disclaimer = "disc", timestamp = 1707062413, license = "" });
                return Ok(0);


            }
            catch (Exception ex)
            {
                _Logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Removes cached base rate from cache 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("TestDeleteSpotRateFromCache")]
        public IActionResult DeleteSpotRateFromCache(string @base)
        {

            _cacheRepository.RemoveData(@base);
            return Ok();
        }

    }
}
