using MediatR;
using Microsoft.AspNetCore.Mvc;
using BMS.Application.DeviceSchedules.Commands;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceSchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceSchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeviceScheduleCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id in route and body do not match.");

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
