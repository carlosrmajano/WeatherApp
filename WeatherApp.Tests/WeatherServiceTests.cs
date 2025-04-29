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
using WeatherApp.Infrastructure.External.WeatherApi;
using WeatherApp.Infrastructure;

namespace WeatherApp.Tests
{
    public class WeatherServiceTests
    {
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
    }
}
