using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Domain;

namespace WeatherApp.Infrastructure.External.WeatherApi
{
    public interface IWeatherApiClient
    {
        Task<WeatherInfo> GetCurrentWeatherAsync(string city);
        Task<List<WeatherInfo>> GetForecastAsync(string city);
    }
}
