using Microsoft.Extensions.Logging;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Jobs.BackgroundJobs
{
    public class WeatherDataUpsertJob
    {
        private readonly IWeatherService _weatherService;
        private readonly IRepository<WeatherData> _repository;
        private readonly ILogger<WeatherDataUpsertJob> _logger;

        private readonly List<string> _cities = new()
        {
            "Berlin",
            "Hamburg",
            "Munich",
            "Köln",
            "Frankfurt am Main",
            "Stuttgart",
            "Düsseldorf",
            "Dortmund",
            "Essen",
            "Leipzig",
            "Bremen",
            "Dresden",
            "Hannover",
            "Nuremberg",
            "Regensburg",
            "Maxhütte-Haidhof",
        };

        public WeatherDataUpsertJob(
            IWeatherService weatherService,
            IRepository<WeatherData> repository,
            ILogger<WeatherDataUpsertJob> logger)
        {
            _weatherService = weatherService;
            _repository = repository;
            _logger = logger;
        }

        private async Task CleanupOldCities()
        {
            try
            {
                var allWeatherData = await _repository.GetAllAsync();

                // Clean up duplicates
                var duplicateCities = allWeatherData
                    .GroupBy(w => w.City)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g.OrderBy(w => w.LastModified).Take(g.Count() - 1))
                    .ToList();

                foreach (var duplicate in duplicateCities)
                {
                    await _repository.DeleteAsync(duplicate);
                    _logger.LogInformation("Deleted duplicate weather data for {City}", duplicate.City);
                }

                // Existing cleanup code...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old cities data");
            }
        }

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation("Starting weather data collection for {Count} cities at {Time}", _cities.Count, DateTime.UtcNow);

                foreach (var city in _cities)
                {
                    try
                    {
                        _logger.LogInformation("Fetching weather data for {City}", city);
                        var newWeatherData = await _weatherService.GetWeatherDataAsync(city);

                        // Check if we already have data for this city
                        var existingWeatherData = await _repository.GetByCityAsync(city);

                        if (existingWeatherData != null)
                        {
                            // Update existing record
                            existingWeatherData.Temperature = newWeatherData.Temperature;
                            existingWeatherData.Humidity = newWeatherData.Humidity;
                            existingWeatherData.Description = newWeatherData.Description;
                            existingWeatherData.WindSpeed = newWeatherData.WindSpeed;
                            existingWeatherData.Icon = newWeatherData.Icon;
                            existingWeatherData.Pressure = newWeatherData.Pressure;  // Add this line
                            existingWeatherData.Timestamp = DateTime.UtcNow;
                            existingWeatherData.LastModified = DateTime.UtcNow;

                            await _repository.UpdateAsync(existingWeatherData);
                            _logger.LogInformation("Updated existing weather data for {City}", city);
                        }
                        else
                        {
                            // Add new record
                            await _repository.AddAsync(newWeatherData);
                            _logger.LogInformation("Added new weather data for {City}", city);
                        }

                        _logger.LogInformation(
                            "Processed weather data for {City}: Temp: {Temp}°C, Humidity: {Humidity}%, Wind: {Wind}m/s",
                            city,
                            newWeatherData.Temperature,
                            newWeatherData.Humidity,
                            newWeatherData.WindSpeed);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing weather data for {City}", city);
                        continue;
                    }
                }

                // Clean up data for cities no longer in the list
                await CleanupOldCities();

                _logger.LogInformation("Completed weather data update at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during weather data update operation");
                throw;
            }
        }
    }
}