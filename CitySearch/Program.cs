using CitySearch.Model;
using System.Text.Json;

namespace CitySearch
{
    internal class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };

        static async Task Main(string[] args)
        {
            string city = ReadValidatedStringInput("Search: ", "Input cannot be empty. Please provide a valid string.");
            string apiUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={city}&count=10&language=de&format=json";
            string apiResponse = await GetApiResponse(apiUrl);
                        
            SearchResult? result = JsonSerializer.Deserialize<SearchResult>(apiResponse, _serializerOptions);

            string jsonString = JsonSerializer.Serialize(result, _serializerOptions);

            File.WriteAllText("searchResults.json", jsonString);


            // Show search results
            int menuIndex = 0;
            List<Location>? locations;
            Console.Clear();
            Console.WriteLine("Search results:");
            if (result == null || result.Results?.Count == 0)
                Console.WriteLine("Location not found.");
            else
            {
                locations = result.Results;

                foreach (var location in locations ?? new List<Location>())
                {
                    if (!string.IsNullOrEmpty(location.Country))
                        Console.WriteLine($"[{++menuIndex}] {location.Name}, {location.Country}, {location.Admin1}");
                }
            }

            //TODO Location selection
            Console.WriteLine();
            bool check;
            int index;
            do
            {
                Console.Write($"Please select your location (1-{menuIndex}): ");
                check = int.TryParse(Console.ReadLine(), out index);
                if (!check)
                    Console.WriteLine("Input can not be empty");
                else if (index < 0)
                    Console.WriteLine("Please enter a positive number");
                else if (index == 0 || index > menuIndex)
                    Console.WriteLine($"Please select between 1 and {menuIndex}");
                else break;
            }
            while (true);
            index--;

            //TODO View the coordinates of selected location
            locations = result?.Results;
            if (locations != null && index >= 0 && index < locations.Count)
            {
                Console.Clear();
                Console.WriteLine($"Coordinates of {locations[index].Name}:");
                Console.WriteLine($"Latitude: {locations[index].Latitude}");
                Console.WriteLine($"Longitude: {locations[index].Longitude}");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }

            Console.WriteLine();
            WaitUserKey("Please press any key to end the program ...");
        }

        public static string ReadValidatedStringInput(string message, string errorMsg)
        {
            string userInput;
            do
            {
                Console.Write(message);
                userInput = (Console.ReadLine() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(userInput))
                    PrintColoredMessage(errorMsg, ConsoleColor.Red);

            } while (string.IsNullOrWhiteSpace(userInput));
            return userInput;
        }

        public static void WaitUserKey(string? message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);

            Console.ReadKey();
        }

        private static void PrintColoredMessage(string message, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static async Task<string> GetApiResponse(string apiUrlHourlyWeather)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrlHourlyWeather);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
