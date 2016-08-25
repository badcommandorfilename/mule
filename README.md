# MULE
## Magical Unicorn Land (Enterprise Edition)
![Robocorn loves MULE](https://s-media-cache-ak0.pinimg.com/236x/26/18/64/261864688a35b354af71a5da97c4a457.jpg)

C# NET Core Web UI for sharing a simple collection of custom objects with your team.

## Wow!

MULE is just a simple Web UI that wraps up Create, Read, Update and Delete actions on an SQLite database.

Have you ever had a conversation that goes like this?
>  "Well, in an ideal world, with magic and unicorns, we'd have a cool web portal where people could log in and update which servers they were using and what version of the software they were running! Until then, I guess we'll all just leave post-it notes everywhere and track everything on this ugly whiteboard."

## Such Magic!

You can use MULE as a seed app to build and share your own dashboards. Create your datamodel class as shown below, and the view engine will magically update the GUI for you! Add methods to calculate or scrape live data from other sources.

Create as many models as you want!

```csharp
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
```

* Each model type needs a Key [Attribute](https://msdn.microsoft.com/en-us/library/aa288454.aspx) to evaluate uniqueness
* Properties are user settable values for each model entry. They are saved to the database between sessions
* Public methods are can be used to create derived values from properties:
** Use the Cached attribute to speed up loading for long running methods
** Use the Background attribute to keep a Cached result up-to-date

The unicorns will use [EF7](https://github.com/aspnet/EntityFramework) and [SQLite](https://www.nuget.org/packages/EntityFramework.SQLite/) to persist your data.

## Unicorns Exist!

MULE uses C# & .NET Core and should run on:
* Windows 7, 8, 10...
* Linux (Ubuntu 14.04, 16.04, Linux Mint 17+, Debian 8+, Fedora 23+...)
* Mac OS X 10+
* If one of those doesn't work, use [Docker](https://github.com/badcommandorfilename/mule/blob/master/muleapp/Dockerfile)!

## Install
* Install/Update [.NET Core](https://www.microsoft.com/net/core#ubuntu)
* Checkout repo
* Open the "muleapp" directory
* run "dotnet restore"
* run "dotnet run"

## Troubleshooting
If you change your datamodel, don't forget to use:
* [dotnet ef migrations add "Schema Change"](https://docs.asp.net/en/latest/tutorials/first-mvc-app/adding-model.html)
* [dotnet ef database update](http://ef.readthedocs.io/en/latest/)
