using BMS.Application.Pm.Commands;
using BMS.Application.Pm.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PmController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PmController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePmCommand command)
        {
            var result =
                await _mediator.Send(command);

            return Ok(result);
        }


        [HttpGet("active/{deviceId:guid}")]
        public async Task<IActionResult> GetActive(
            Guid deviceId)
        {
            var result =
                await _mediator.Send(
                    new GetActivePmQuery(deviceId, null));

            return Ok(result);
        }

        [HttpGet("active/{deviceId:guid}/{tag}")]
        public async Task<IActionResult> GetActive(
           Guid deviceId,
           string? tag)
        {
            var result =
                await _mediator.Send(
                    new GetActivePmQuery(deviceId, tag));

            return Ok(result);
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdatePmCommand command)
        {
            command.Id = id;

            var result =
                await _mediator.Send(command);

            return Ok(result);
        }


        [HttpPost("{id:guid}/finalize")]
        public async Task<IActionResult> Finalize(
            Guid id,
            [FromBody] FinalizePmCommand command)
        {
            command.PmScheduleId = id;

            var result =
                await _mediator.Send(command);

            return Ok(result);
        }


        [HttpGet("history/{deviceId:guid}/{tag}")]
        public async Task<IActionResult> GetHistory(
            Guid deviceId, string tag)
        {
            var result =
                await _mediator.Send(
                    new GetPmHistoryQuery(deviceId, tag));

            return Ok(result);
        }

        [HttpPut("history/{id:guid}")]
        public async Task<IActionResult> UpdateHistory(
            Guid id,
            [FromBody] UpdatePmHistoryCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }
    }
}
