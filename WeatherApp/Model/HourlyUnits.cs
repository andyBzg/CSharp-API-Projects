using System.Text.Json.Serialization;

namespace WeatherApp.Model
{
    public class HourlyUnits
    {
        public string Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string Temperature2m { get; set; }
    }
}