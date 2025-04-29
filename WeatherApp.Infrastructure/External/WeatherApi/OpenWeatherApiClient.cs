using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using WeatherApp.Domain;
using WeatherApp.Infrastructure.Utilities;

namespace WeatherApp.Infrastructure.External.WeatherApi
{
    public class OpenWeatherApiClient: IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherApiClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["OpenWeatherMap:ApiKey"]!;
        }
        public async Task<WeatherInfo> GetCurrentWeatherAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetFromJsonAsync<OpenWeatherCurrentResponse>(url);

            return new WeatherInfo
            {
                City = city,
                Date = DateTime.UtcNow,
                Description = response!.Weather[0].Description,
                Temperature = (float)response.Main.Temp,
            };
        }
        public async Task<List<WeatherInfo>> GetForecastAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetFromJsonAsync<OpenWeatherForecastResponse>(url);

            return response!.List
                .GroupBy(entry => entry.DtTxt.Date)
                .Select(group =>
                {
                    var midday = group.FirstOrDefault(e => e.DtTxt.Hour == 12) ?? group.First();
                    return new WeatherInfo
                    {
                        City = city,
                        Date = group.Key,
                        Description = midday.Weather[0].Description,
                        Temperature = (float)group.Average(e => e.Main.Temp),
                        MinTemperature = (float)group.Min(e => e.Main.Temp),
                        MaxTemperature = (float)group.Max(e => e.Main.Temp)
                    };
                }).ToList();
        }

        public class OpenWeatherCurrentResponse
        {
            public List<WeatherDescription> Weather { get; set; } = new();
            public MainData Main { get; set; } = new();

            public class WeatherDescription { public string Description { get; set; } = ""; }
            public class MainData { public double Temp { get; set; } }
        }

        public class OpenWeatherForecastResponse
        {
            public List<ForecastEntry> List { get; set; } = new();

            public class ForecastEntry
            {
                public MainData Main { get; set; } = new();
                public List<WeatherDescription> Weather { get; set; } = new();

                [JsonPropertyName("dt_txt")]
                [JsonConverter(typeof(DateTimeConverter))]
                public DateTime DtTxt { get; set; }
            }

            public class WeatherDescription { public string Description { get; set; } = ""; }
            public class MainData { public double Temp { get; set; } }
        }
    }
}
