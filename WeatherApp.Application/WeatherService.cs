using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Infrastructure.External.WeatherApi;
using WeatherApp.Infrastructure;
using WeatherApp.Domain;

namespace WeatherApp.Application
{
    public class WeatherService
    {
        private readonly IWeatherApiClient _apiClient;
        private readonly WeatherDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IWeatherApiClient apiClient, WeatherDbContext db, IMemoryCache cache, ILogger<WeatherService> logger)
        {
            _apiClient = apiClient;
            _db = db;
            _cache = cache;
            _logger = logger;
        }
        public async Task<WeatherInfo> GetCurrentWeatherAsync(string city)
        {
            return await _cache.GetOrCreateAsync($"current_{city}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var weather = await _apiClient.GetCurrentWeatherAsync(city);

                _db.WeatherRecords.Add(new WeatherRecord
                {
                    City = weather.City,
                    Date = weather.Date,
                    Description = weather.Description,
                    Temperature = weather.Temperature,
                });
                await _db.SaveChangesAsync();

                return weather;
            })!;
        }

        public async Task<List<WeatherInfo>> GetForecastAsync(string city)
        {
            return await _cache.GetOrCreateAsync($"forecast_{city}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _apiClient.GetForecastAsync(city);
            })!;
        }

    }
}
