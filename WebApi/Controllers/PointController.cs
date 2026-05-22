using Bms.Infrastructure.Seeds;
using BMS.Application.Interfaces;
using BMS.Application.Points.Commands;
using BMS.Application.Points.Dtos;
using BMS.Application.Points.Queries;
using BMS.Application.UseCases;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Devices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Domain.Twin.Services;
using WebApi.Realtime.Models;
using WebApi.Realtime.Services;
using WebApi.Security.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/points")]
    public class PointController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger<PointController> _logger;
        private readonly ITwinService _twinService;
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;
        private readonly UpdateDataPointUseCase _useCase;
        public PointController(ILogger<PointController> logger,// ITwinRealtimePublisher publisher, ITwinService twinService,
            IDeviceStateStore store
            , IEventDispatcher dispatcher,
            UpdateDataPointUseCase useCase,
            IMediator mediator
            )
        {
            _logger = logger;
            //_publisher = publisher;
            //_twinService = twinService;
            _store = store;
            _dispatcher = dispatcher;
            _useCase = useCase;
            _mediator = mediator;
        }


        //[HttpPost("{twinId}/update")]
        //public async Task<IActionResult> Update(
        //Guid twinId,
        //[FromBody] UpdateRequest request)
        //{
        //    //await _twinService.UpdateAsync(
        //    //    twinId,
        //    //    request.DataPointId,
        //    //    Convert.ToInt32( request.Value.ToString()));

        //    var device = _store.Get(twinId);
        //    //device.RegisterPoint(request.DataPointId, PointDataType.Int32);
        //    //var domainEvent = device.UpdatePoint(request.DataPointId, Convert.ToInt32(request.Value.ToString()));

        //    await _useCase.ExecuteAsync(
        //        twinId,
        //        request.DataPointId,
        //        request.Value.ToString());

        //    //if (domainEvent == null)
        //    //    return;

        //    //await _dispatcher.DispatchAsync(domainEvent);

        //    return Ok();
        //}

        // GET ALL
        [RequirePermission(PermissionKeys.Points.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetPointsQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // CREATE
        [RequirePermission(PermissionKeys.Points.Create)]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePointCommand command)
        {
            var id = await _mediator.Send(command);

            try
            {
                using var httpClient = new HttpClient();

                var url = "http://localhost:5055/api/cache/invalidate";

                var request = new WritePointCommandRequest
                {

                };

                var response = await httpClient.PostAsJsonAsync(url, request);

            }
            catch { }
            return Ok(id);
        }

        // UPDATE
        [RequirePermission(PermissionKeys.Points.Update)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePoint(Guid id, UpdatePointCommand command)
        {
            if (id != command.Id)
                return BadRequest("Route id and command id do not match");

            var res = await _mediator.Send(command);

            if (res.Success)
            {
                try
                {
                    using var httpClient = new HttpClient();

                    var url = "http://localhost:5055/api/cache/invalidate";

                    var request = new WritePointCommandRequest
                    {

                    };

                    var response = await httpClient.PostAsJsonAsync(url, request);
                }
                catch { }
            }
            return StatusCode(res.StatusCode, res);
        }

        // DELETE
        [RequirePermission(PermissionKeys.Points.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeletePointCommand(id));
            try
            {
                using var httpClient = new HttpClient();

                var url = "http://localhost:5055/api/cache/invalidate";

                var request = new WritePointCommandRequest
                {

                };

                var response = await httpClient.PostAsJsonAsync(url, request);

            }
            catch { }
            return Ok();
        }

        // GET BY ID
        [RequirePermission(PermissionKeys.Points.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPointByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET BY DEVICE
        [RequirePermission(PermissionKeys.Points.View)]
        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDevice(Guid deviceId)
        {
            var result = await _mediator.Send(new GetPointsByDeviceQuery(deviceId));

            return Ok(result);
        }
    }

    public class UpdateRequest
    {
        public Guid DataPointId { get; set; }
        public object? Value { get; set; }
    }
}

