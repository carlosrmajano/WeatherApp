using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application;

namespace WeatherApp.API.Controllers
{
    [Route("weather/")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _service;

        public WeatherController(WeatherService service)
        {
            _service = service;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent([FromQuery] string city = "San Salvador")
        {
            try
            {
                var result = await _service.GetCurrentWeatherAsync(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(503, $"Weather service unavailable: {ex.Message}");
            }
        }
        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast([FromQuery] string city = "San Salvador")
        {
            try
            {
                var result = await _service.GetForecastAsync(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(503, $"Forecast service unavailable: {ex.Message}");
            }
        }
    }
}
