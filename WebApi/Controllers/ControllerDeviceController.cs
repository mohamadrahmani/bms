using System.Drawing;
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
                    point.Value.ToString());

            }
            return Ok();
        }


        [HttpGet("{twinId}/AutoUpdate")]
        public async Task<IActionResult> AutoUpdate(Guid twinId)
        {
            var device = _store.Get(twinId);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000001"), PointDataType.Int32);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000002"), PointDataType.Int32);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000003"), PointDataType.Int32);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000004"), PointDataType.Int32);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000005"), PointDataType.Int32);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000006"), PointDataType.Int32);
            device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000007"), PointDataType.Int32);

            //while (true)
            //{
            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000001"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000002"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000003"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000004"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000005"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000006"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    await _useCase.ExecuteAsync(
            //        Guid.Empty,
            //        Guid.Parse("00000000-0000-0000-0000-000000000007"),
            //        Convert.ToInt32((new Random()).Next(1, 101)));

            //    System.Threading.Thread.Sleep(1000);
            //}

            _AutoUpdate(twinId);
            return Ok();
        }

        async Task<bool> _AutoUpdate(Guid twinId)
        {
            while (true)
            {
                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    (new Random()).Next(1, 101).ToString());

                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    (new Random()).Next(1, 101).ToString());

                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    (new Random()).Next(1, 101).ToString());

                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    (new Random()).Next(1, 101).ToString());

                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000005"),
                    (new Random()).Next(1, 101).ToString());

                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000006"),
                    (new Random()).Next(1, 101).ToString());

                await _useCase.ExecuteAsync(
                    Guid.Empty,
                    Guid.Parse("00000000-0000-0000-0000-000000000007"),
                    (new Random()).Next(1, 101).ToString());

                System.Threading.Thread.Sleep(1000);
            }
            
        }
    }
}
