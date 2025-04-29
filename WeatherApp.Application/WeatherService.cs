using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WeatherApp.Domain;
using WeatherApp.Infrastructure;
using WeatherApp.Infrastructure.External.WeatherApi;

namespace WeatherApp.Application
{
    public class WeatherService
    {
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly WeatherDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IWeatherApiClient weatherApiClient, WeatherDbContext dbContext, IMemoryCache cache, ILogger<WeatherService> logger)
        {
            _weatherApiClient = weatherApiClient;
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }
        public async Task<WeatherInfo> GetCurrentWeatherAsync(string city)
        {
            return await _cache.GetOrCreateAsync($"current_{city}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var weather = await _weatherApiClient.GetCurrentWeatherAsync(city);

                _dbContext.WeatherRecords.Add(new WeatherRecord
                {
                    City = weather.City,
                    Date = weather.Date,
                    Description = weather.Description,
                    Temperature = weather.Temperature,
                });
                await _dbContext.SaveChangesAsync();

                return weather;
            })!;
        }

        public async Task<List<WeatherInfo>> GetForecastAsync(string city)
        {
            return await _cache.GetOrCreateAsync($"forecast_{city}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _weatherApiClient.GetForecastAsync(city);
            })!;
        }

    }
}
