using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Application;
using WeatherApp.Domain;
using WeatherApp.Infrastructure;
using WeatherApp.Infrastructure.External.WeatherApi;

namespace WeatherApp.Tests
{
    public class WeatherServiceCachingTests
    {
        private readonly Mock<IWeatherApiClient> _mockWeatherApiClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<WeatherService> _logger;
        private readonly WeatherDbContext _dbContext;
        private readonly WeatherService _weatherService;

        public WeatherServiceCachingTests()
        {
            _mockWeatherApiClient = new Mock<IWeatherApiClient>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _logger = Mock.Of<ILogger<WeatherService>>();

            var options = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            _dbContext = new WeatherDbContext(options);

            _weatherService = new WeatherService(
                _mockWeatherApiClient.Object,
                _dbContext,
                _memoryCache,
                _logger
            );
        }

        [Fact]
        public async Task GetCurrentWeather_ShouldCacheData()
        {
            // Arrange
            var expectedWeatherInfo = new WeatherInfo
            {
                City = "San Salvador",
                Temperature = 27.5f,
                Description = "clear sky",
                Date = System.DateTime.UtcNow
            };

            _mockWeatherApiClient.Setup(client => client.GetCurrentWeatherAsync("San Salvador"))
                                 .ReturnsAsync(expectedWeatherInfo);

            // First call should hit the API
            var result1 = await _weatherService.GetCurrentWeatherAsync("San Salvador");

            // Second call should return cached data
            var result2 = await _weatherService.GetCurrentWeatherAsync("San Salvador");

            // Assert
            Assert.Equal(result1, result2);  // Results should be the same if caching is working
            _mockWeatherApiClient.Verify(client => client.GetCurrentWeatherAsync(It.IsAny<string>()), Times.Once);  // Ensure API was called only once

        }
    }
}