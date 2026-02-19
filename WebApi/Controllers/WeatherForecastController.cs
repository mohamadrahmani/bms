using Microsoft.AspNetCore.Mvc;
using WebApi.Realtime.Models;
using WebApi.Realtime.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ITwinRealtimePublisher _publisher;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITwinRealtimePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            while (true)
            {
                await _publisher.PublishAsync(new RealtimeUpdate
                {
                    DatapointId = "ahu1.temp.supply",
                    Value = Random.Shared.Next(-20, 55),
                    Quality = "Good"
                });
                System.Threading.Thread.Sleep(1000);
            }

            return Ok();

            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
        }
    }
}
