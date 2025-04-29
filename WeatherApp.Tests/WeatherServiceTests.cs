using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp.Application;
using WeatherApp.Domain;
using WeatherApp.Infrastructure;
using WeatherApp.Infrastructure.External.WeatherApi;

namespace WeatherApp.Tests
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherApiClient> _mockWeatherApiClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<WeatherService> _logger;
        private readonly WeatherDbContext _dbContext;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
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
        public async Task GetCurrentWeatherAsync_CachesAndPersists()
        {
            // Arrange
            var mockApiClient = new Mock<IWeatherApiClient>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var logger = Mock.Of<ILogger<WeatherService>>();

            var options = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase("WeatherDbTest")
                .Options;
            var context = new WeatherDbContext(options);

            var expected = new WeatherInfo
            {
                City = "San Salvador",
                Date = DateTime.UtcNow,
                Temperature = 30,
                Description = "Clear"
            };

            mockApiClient.Setup(a => a.GetCurrentWeatherAsync("San Salvador"))
                .ReturnsAsync(expected);

            var service = new WeatherService(mockApiClient.Object, context, memoryCache, logger);

            // Act
            var result = await service.GetCurrentWeatherAsync("San Salvador");

            // Assert
            Assert.Equal(expected.City, result.City);
            Assert.Single(context.WeatherRecords);
        }
        [Fact]
        public async Task GetCurrentWeather_ShouldReturnValidWeatherInfo()
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

            // Act
            var result = await _weatherService.GetCurrentWeatherAsync("San Salvador");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("San Salvador", result.City);
            Assert.Equal(27.5f, result.Temperature);
            Assert.Equal("clear sky", result.Description);
        }

        [Fact]
        public async Task GetForecast_ShouldReturnValidForecast()
        {
            // Arrange
            var forecast = new List<WeatherInfo>
        {
            new WeatherInfo { City = "San Salvador", Temperature = 30.0f, Description = "sunny", Date = System.DateTime.UtcNow.AddDays(1) },
            new WeatherInfo { City = "San Salvador", Temperature = 28.5f, Description = "cloudy", Date = System.DateTime.UtcNow.AddDays(2) }
        };

            _mockWeatherApiClient.Setup(client => client.GetForecastAsync("San Salvador"))
                                 .ReturnsAsync(forecast);

            // Act
            var result = await _weatherService.GetForecastAsync("San Salvador");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Ensure we have two forecast entries
            Assert.Equal("sunny", result[0].Description);
            Assert.Equal(30.0f, result[0].Temperature);
        }
    }
}
