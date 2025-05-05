namespace WeatherApp.Model
{
    internal class WeatherData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double GenerationtimeMs { get; set; }
        public int UtcOffsetSeconds { get; set; }
        public string Timezone { get; set; }
        public string TimezoneAbbreviation { get; set; }
        public double Elevation { get; set; }
        public HourlyUnits HourlyUnits { get; set; }
        public Hourly Hourly { get; set; }
    }
}
