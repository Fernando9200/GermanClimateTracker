using Microsoft.AspNetCore.Mvc;
using Application.Common.Interfaces;
using Domain.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IRepository<WeatherData> _repository;
        private readonly IWeatherService _weatherService;

        public WeatherController(
            IRepository<WeatherData> repository,
            IWeatherService weatherService)
        {
            _repository = repository;
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherData>>> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{city}")]
        public async Task<ActionResult<WeatherData>> GetByCity(string city)
        {
            try
            {
                var data = await _weatherService.GetWeatherDataAsync(city);
                return Ok(data);
            }
            catch (HttpRequestException)
            {
                return NotFound($"Weather data for city '{city}' not found");
            }
        }
    }
}