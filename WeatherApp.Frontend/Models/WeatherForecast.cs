namespace WeatherApp.Frontend.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public string City { get; set; }
    }
}
