using System.Text.Json;
using WeatherApp.Model;

namespace WeatherApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool check;
            int index, count = 0;
            string latitude = "", longitude = "", apiUrlHourlyWeather, apiUrlCurrentWeather;
            string filePath = "weatherData.json";
            List<string> menu = new List<string>() { "Karlsruhe", "Stuttgart", "Nürnberg", "Heilbronn" };

            Dictionary<string, double[]> coordinates = new Dictionary<string, double[]>();
            coordinates.Add("Karlsruhe", new double[] { 49.0094, 8.4044 });
            coordinates.Add("Stuttgart", new double[] { 48.7823, 9.177 });
            coordinates.Add("Nürnberg", new double[] { 49.4542, 11.0775 });
            coordinates.Add("Heilbronn", new double[] { 49.1399, 9.2205 });

            Console.WriteLine("========= MENÜ =========");
            menu.ForEach(s => Console.WriteLine($"({++count}) - {s}"));

            do
            {
                Console.Write("Bitte Stadt wählen: ");
                check = int.TryParse(Console.ReadLine(), out index);
                if (!check)
                    Console.WriteLine("Ungültige Eingabe");
                else if (index < 0)
                    Console.WriteLine("Bitte eine positive Zahl eingeben");
                else if (index == 0 || index > coordinates.Count)
                    Console.WriteLine($"Bitte zwischen 1 und {coordinates.Count} auswählen");
                else break;
            }
            while (true);

            latitude = $"{coordinates[$"{coordinates.ElementAt(index - 1).Key}"][0]}".Replace(",", ".");
            longitude = $"{coordinates[$"{coordinates.ElementAt(index - 1).Key}"][1]}".Replace(",", ".");

            apiUrlCurrentWeather = $"";
            apiUrlHourlyWeather = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&models=icon_seamless&timezone=Europe%2FBerlin";

            string responseBody = await GetApiResponse(apiUrlHourlyWeather);

            WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });


            if (weatherData != null)
            {
                SaveWeatherDataToFile(weatherData, filePath);

                Console.Clear();
                ShowHeader(index, coordinates);
                ShowWeatherDetails(weatherData);
                Console.WriteLine();

                Console.WriteLine("Möchten Sie den Stündlichen Wetterbericht sehen für 7 Tage? J/N ");
                if (Console.ReadKey().Key == ConsoleKey.J)
                {
                    Console.Clear();
                    ShowHeader(index, coordinates);
                    ShowHourlyWeather(weatherData);
                }

            }
        }

        private static void SaveWeatherDataToFile(WeatherData weatherData, string filePath)
        {
            if (weatherData != null)
            {
                string jsonString = JsonSerializer.Serialize(weatherData, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
                File.WriteAllText(filePath, jsonString);
            }
        }

        private static async Task<string> GetApiResponse(string apiUrlHourlyWeather)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(apiUrlHourlyWeather);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static void ShowWeatherDetails(WeatherData weatherData)
        {
            Console.WriteLine($"Latitude: {weatherData.Latitude}");
            Console.WriteLine($"Longitude: {weatherData.Longitude}");
            Console.WriteLine($"Generationtime ms: {weatherData.GenerationtimeMs}");
            Console.WriteLine($"UTC offset sec: {weatherData.UtcOffsetSeconds}");
            Console.WriteLine($"Timezone: {weatherData.Timezone}");
            Console.WriteLine($"Timezone abbreviation.: {weatherData.TimezoneAbbreviation}");
            Console.WriteLine($"Elevation: {weatherData.Elevation}");
            Console.WriteLine();
            Console.WriteLine("Hourly Units");
            Console.WriteLine($"Time format: {weatherData.HourlyUnits.Time}");
            Console.WriteLine($"Temperature: {weatherData.HourlyUnits.Temperature2m}");
        }

        private static void ShowHeader(int index, Dictionary<string, double[]> coordinates)
        {
            Console.WriteLine("========================");
            Console.WriteLine($"*** Weather in {$"{coordinates.ElementAt(index - 1).Key}"} ***");
        }

        private static void ShowHourlyWeather(WeatherData weatherData)
        {
            for (int i = 0; i < weatherData.Hourly.Time.Count; i++)
            {
                DateTime dateTime = DateTime.Parse(weatherData.Hourly.Time[i]);
                Console.WriteLine($"{dateTime.ToString("dd.MM.yyyy | HH:mm")} | {weatherData.Hourly.Temperature2m[i]} {weatherData.HourlyUnits.Temperature2m}");
            }
            Console.Beep();
        }
    }
}
