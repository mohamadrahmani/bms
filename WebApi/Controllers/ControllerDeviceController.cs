using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Application.UseCases;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Domain.Twin.Services;
using WebApi.Realtime.Models;
using WebApi.Realtime.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ControllerDeviceController : ControllerBase
    {
        private readonly ILogger<PointController> _logger;
        private readonly ITwinService _twinService;
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;
        private readonly UpdateDataPointUseCase _useCase;
        public ControllerDeviceController(ILogger<PointController> logger,// ITwinRealtimePublisher publisher, ITwinService twinService,
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
        Guid twinId,
        [FromBody] TelemetryMessage request)
        {
            var device = _store.Get(twinId);

            foreach(var point in request.Points)
            {
                device.RegisterPoint(point.Id, PointDataType.Int32);

                await _useCase.ExecuteAsync(
                    twinId,
                    point.Id,
                    Convert.ToInt32(point.Value.ToString()));

            }
            return Ok();
        }
    }
}
