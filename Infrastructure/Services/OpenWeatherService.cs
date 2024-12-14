using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        public OpenWeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeather:ApiKey"] ??
                throw new ArgumentNullException(nameof(configuration), "OpenWeather:ApiKey is not configured");
        }

        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            ArgumentNullException.ThrowIfNull(city);

            var response = await _httpClient.GetAsync(
                $"{BaseUrl}?q={city}&appid={_apiKey}&units=metric");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var weatherResponse = JsonConvert.DeserializeObject<OpenWeatherResponse>(content) ??
                throw new JsonSerializationException("Failed to deserialize weather response");

            return MapToWeatherData(weatherResponse, city);
        }

        public async Task<IEnumerable<WeatherData>> GetWeatherForCitiesAsync(IEnumerable<string> cities)
        {
            ArgumentNullException.ThrowIfNull(cities);

            var tasks = cities.Select(city => GetWeatherDataAsync(city));
            return await Task.WhenAll(tasks);
        }

        private WeatherData MapToWeatherData(OpenWeatherResponse response, string city)
        {
            ArgumentNullException.ThrowIfNull(response);
            ArgumentNullException.ThrowIfNull(city);

            if (response.Weather.Length == 0)
                throw new InvalidOperationException("Weather data is empty");

            var utcTime = DateTimeOffset.FromUnixTimeSeconds(response.Dt).UtcDateTime;

            return new WeatherData
            {
                Id = Guid.NewGuid(),
                City = city,
                Temperature = response.Main.Temp,
                Humidity = response.Main.Humidity,
                Description = response.Weather[0].Description,
                WindSpeed = response.Wind.Speed,
                Icon = response.Weather[0].Icon,
                Pressure = response.Main.Pressure,
                Timestamp = utcTime,
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };
        }
    }

    public class OpenWeatherResponse
    {
        public required MainData Main { get; set; }
        public required WindData Wind { get; set; }
        public required WeatherDescription[] Weather { get; set; }

        [JsonProperty("dt")]
        public long Dt { get; set; }
        public int Timezone { get; set; }
    }

    public class MainData
    {
        public double Temp { get; set; }

        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
    }

    public class WindData
    {
        public double Speed { get; set; }
    }

    public class WeatherDescription
    {
        public required string Description { get; set; }

        [JsonProperty("icon")]
        public required string Icon { get; set; }
    }


}