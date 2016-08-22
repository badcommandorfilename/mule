using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;

namespace Mule
{
    /// Example model class - we want to show weather information scraped from yahoo.com/weather/
    public class Weather
    {
        [Key]//Unique id for object in our model
        public string City { get; set; } = "";

        [DataType(DataType.MultilineText)] //Input style/formatting hint
        public string Description { get; set; } = "";

        [HiddenInput(DisplayValue = false)] //Property won't show in edit form
        public DateTime Updated { get; private set; } = DateTime.Now;

        [Cached(ExpireSeconds = 600)] //The result of this method is cached for 10m
        public string Condition()
        {
            var response = apiResponse();
            return response.query.results.channel.item.condition.text;
        }

        [Background(IntervalSeconds = 60)] //Update the result on a background worker every 60s
        public int Temperature()
        {
            var response = apiResponse();
            var degf = response.query.results.channel.item.condition.temp; //In Fahrenheit
            return ((degf - 32) * 5) / 9; //In Celsius
        }

        ///Private method, won't show in View. See https://developer.yahoo.com/weather/
        string apiURL() =>
            $"https://query.yahooapis.com/v1/public/yql?q=select item.condition from weather.forecast where woeid in (select woeid from geo.places(1) where text=\"{City.ToLower()}\")&format=json";

        /// Expecting {"query":{"count":1,"results":{"channel":{"item":{"condition":{"temp":"51","text":"Showers"}}}}}}
        dynamic apiResponse()
        {
            using (var handler = new HttpClientHandler()) //Context for http requests
            {
                handler.ServerCertificateCustomValidationCallback = delegate { return true; }; //Ignore self-signed cert
                using (var client = new HttpClient(handler))
                {   
                    var response = client.GetStringAsync(apiURL()).Result;
                    return JsonConvert.DeserializeObject(response); //Convert JSON into a C# object
                }
            }
        }

        /// Equality Comparer (determines if DB row will be overwritten or created)
        public override bool Equals(object obj) => City == (obj as Weather)?.City;

        /// Unique key definition (for looking up CacheValues)
        public override int GetHashCode() => City.GetHashCode();
    }

    [Route("")]
    [Route("weather")]
    //Declare controller for routing
    public class WeatherController : ItemController<Weather>
    {
        //Default actions inherit from ItemController
        public WeatherController(IRepository<Weather> repository) : base(repository)
        {
        }
    }
}