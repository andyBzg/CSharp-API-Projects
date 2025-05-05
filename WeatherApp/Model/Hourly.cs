using System.Text.Json.Serialization;

namespace WeatherApp.Model
{
    public class Hourly
    {
        public List<string> Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature2m { get; set; }
    }
}