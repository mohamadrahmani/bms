using BMS.Application.Interfaces;
using BMS.Application.UseCases;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApi.Domain.Twin.Services;
using WebApi.Realtime.Models;
using WebApi.Realtime.Services;

using BMS.Application.CommandDefinitions.Commands;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Application.CommandDefinitions.Queries;
using MediatR;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/CommandDefinitions")]
    public class CommandDefinitionController : ControllerBase
    {
        private readonly IMediator _mediator;

        //private readonly ITwinRealtimePublisher _publisher;


        private readonly ILogger<CommandDefinitionController> _logger;
        private readonly ITwinService _twinService;
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;
        //private readonly UpdateDataCommandDefinitionUseCase _useCase;
        public CommandDefinitionController(ILogger<CommandDefinitionController> logger,// ITwinRealtimePublisher publisher, ITwinService twinService,
            //IDeviceStateStore store,
            IEventDispatcher dispatcher,
           // UpdateDataCommandDefinitionUseCase useCase,
            IMediator mediator
            )
        {
            _logger = logger;
            //_publisher = publisher;
            //_twinService = twinService;
            //_store = store;
            _dispatcher = dispatcher;
            //_useCase = useCase;
            _mediator = mediator;
        }


        [HttpPost("{twinId}/update")]
        public async Task<IActionResult> Update(
        Guid twinId,
        [FromBody] UpdateCommandDefinitionRequest request)
        {
            //await _twinService.UpdateAsync(
            //    twinId,
            //    request.DataCommandDefinitionId,
            //    Convert.ToInt32( request.Value.ToString()));

            //var device = _store.Get(twinId);
            //device.RegisterCommandDefinition(request.DataCommandDefinitionId, CommandDefinitionDataType.Int32);
            //var domainEvent = device.UpdateCommandDefinition(request.DataCommandDefinitionId, Convert.ToInt32(request.Value.ToString()));

            //await _useCase.ExecuteAsync(
            //    twinId,
            //    request.DataCommandDefinitionId,
            //    request.Value.ToString());

            //if (domainEvent == null)
            //    return;

            //await _dispatcher.DispatchAsync(domainEvent);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCommandDefinitionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommandDefinitionCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommandDefinition(Guid id, UpdateCommandDefinitionCommand command)
        {
            if (id != command.Id)
                return BadRequest("Route id and command id do not match");

            var res = await _mediator.Send(command);

            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCommandDefinitionCommand(id));

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCommandDefinitionByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDevice(Guid deviceId)
        {
            var result = await _mediator.Send(new GetCommandDefinitionsByDeviceQuery(deviceId));

            return Ok(result);
        }
    }

    public class UpdateCommandDefinitionRequest
    {
        public Guid DataCommandDefinitionId { get; set; }
        public object? Value { get; set; }
    }
}
