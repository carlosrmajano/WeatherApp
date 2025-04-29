using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Domain
{
    public class WeatherRecord
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public float Temperature { get; set; }
    }
}
