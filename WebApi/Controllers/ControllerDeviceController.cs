using System.Drawing;
using BMS.Application.Common.Interfaces;
using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Application.UseCases;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Logs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
        private IPointRepository _pointRepository;
        private readonly IAuditLogger _auditLogger;

        public ControllerDeviceController(ILogger<PointController> logger,// ITwinRealtimePublisher publisher, ITwinService twinService,
            IDeviceStateStore store,
            IEventDispatcher dispatcher,
            UpdateDataPointUseCase useCase,
            IPointRepository pointRepository,
            IAuditLogger auditLogger
            )
        {
            _logger = logger;
            //_publisher = publisher;
            //_twinService = twinService;
            _store = store;
            _dispatcher = dispatcher;
            _useCase = useCase;
            _pointRepository = pointRepository;
            _auditLogger = auditLogger;
        }

        [HttpPost("{deviceId}/update")]
        public async Task<IActionResult> Update(
        Guid deviceId,
        [FromBody] TelemetryMessage request)
        {
// todo: Amini: remove log

            await _auditLogger.Add(new Log
            {
                //UserId = user.Id,
                EventType = EventType.Login,
                Result = OperationResult.Failed,
                ResultMessage = "Telemetry",
                //IpAddress = request.IpAddress,
                //Source = "LoginCommandHandler",
                Source = "UpdateDeviceTelemetry",
                LogDate = DateTime.UtcNow,
                ObjectName = "Users",
                ObjectId = deviceId.ToString(),
                RequestBody = JsonSerializer.Serialize(request.Points)
            });

            var device = _store.Get(deviceId);

            foreach(var point in request.Points)
            {
                if (!device.DevicePoints.Any(p => p.Id == point.Id))
                {
                    var _p = await _pointRepository.GetByIdAsync(point.Id);
                    device.RegisterPoint(_p);
                }

                await _useCase.ExecuteAsync(
                    deviceId,
                    point.Id,
                    point.Value.ToString());

            }
            return Ok();
        }


        [HttpGet("{twinId}/AutoUpdate")]
        public async Task<IActionResult> AutoUpdate(Guid twinId)
        {
            var device = _store.Get(twinId);
            //device.RegisterPoint(Guid.Parse("20239AF5-B49E-49CE-8503-1CCAF8303E3A"), PointDataType.Int32);
            //device.RegisterPoint(Guid.Parse("5805D680-28F0-4CE9-A6F3-20CC16681F5F"), PointDataType.Int32);
            //device.RegisterPoint(Guid.Parse("B090C2E1-8BFD-471A-9FD0-6B4D1EE96C1D"), PointDataType.Int32);
            //device.RegisterPoint(Guid.Parse("D6CFEAE7-A3A1-48DE-A37A-8DB40EB874A0"), PointDataType.Int32);
            //device.RegisterPoint(Guid.Parse("DD6E78F4-C1E2-4726-BE31-91D19E872FD6"), PointDataType.Int32);
            //device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000006"), PointDataType.Int32);
            //device.RegisterPoint(Guid.Parse("00000000-0000-0000-0000-000000000007"), PointDataType.Int32);

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
