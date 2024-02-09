using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CEM.Service
{
    public static class JsonHelper
    {
        static readonly IContractResolver _defaultResolver = Newtonsoft.Json.JsonSerializer.CreateDefault().ContractResolver;
        public static Newtonsoft.Json.Serialization.JsonProperty GetJsonProperty<T>(T obj, string jsonName, bool exact = false,
            IContractResolver? resolver = null)
        {
            

            ArgumentNullException.ThrowIfNull(obj);
            resolver ??= _defaultResolver;
            var objType = typeof(T);
            if (resolver.ResolveContract(objType) is not JsonObjectContract contract)
                throw new ArgumentException($"{objType} is not serialized as a JSON object");
            var property = contract.Properties.GetProperty(jsonName, exact
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase);
            if (property == null)
                throw new ArgumentException($"Property {jsonName} was not found.");
            return property;
        }
        public static double getTargetCurrencyRate(string spotrate, string target)
        {

            JObject obj = JObject.Parse(spotrate);
            var ratesElement = (JObject)obj["rates"];
            foreach (var prop in ratesElement)
            {
                if (prop.Key == target)
                {
                    return Convert.ToDouble(prop.Value);
                }
            }
            return default;
        }
    }
}
