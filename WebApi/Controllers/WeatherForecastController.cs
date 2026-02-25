using BMS.Application.Interfaces;
using BMS.Application.UseCases;
using BMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Domain.Twin.Services;
using WebApi.Realtime.Models;
using WebApi.Realtime.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        //private readonly ITwinRealtimePublisher _publisher;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITwinService _twinService;
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;
        private readonly UpdateDataPointUseCase _useCase;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,// ITwinRealtimePublisher publisher, ITwinService twinService,
            IDeviceStateStore store
            ,IEventDispatcher dispatcher,
            UpdateDataPointUseCase useCase
            )
        {
            _logger = logger;
            //_publisher = publisher;
            //_twinService = twinService;
            _store = store;
            _dispatcher = dispatcher;
            _useCase = useCase;
        }

        [HttpPost("{twinId}/update")]
        public async Task<IActionResult> Update(
        string twinId,
        [FromBody] UpdateRequest request)
        {
            //await _twinService.UpdateAsync(
            //    twinId,
            //    request.DataPointId,
            //    Convert.ToInt32( request.Value.ToString()));

            var device = _store.Get(twinId);
            device.RegisterPoint(request.DataPointId, BMS.Domain.Enums.DataType.Integer);
            //var domainEvent = device.UpdatePoint(request.DataPointId, Convert.ToInt32(request.Value.ToString()));

            await _useCase.ExecuteAsync(
                twinId,
                request.DataPointId,
                Convert.ToInt32(request.Value.ToString()));

            //if (domainEvent == null)
            //    return;

            //await _dispatcher.DispatchAsync(domainEvent);

            return Ok();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            //while (true)
            //{
            //    await _publisher.PublishAsync(new RealtimeUpdate
            //    {
            //        DatapointId = "ahu1.temp.supply",
            //        Value = Random.Shared.Next(-20, 55),
            //        Quality = "Good"
            //    });
            //    System.Threading.Thread.Sleep(1000);
            //}

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
    public class UpdateRequest
    {
        public string DataPointId { get; set; } = default!;
        public object? Value { get; set; }
    }
}
