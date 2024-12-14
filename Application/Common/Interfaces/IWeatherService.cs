using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherDataAsync(string city);
        Task<IEnumerable<WeatherData>> GetWeatherForCitiesAsync(IEnumerable<string> cities);
    }
}