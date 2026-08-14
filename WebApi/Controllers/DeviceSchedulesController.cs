using MediatR;
using Microsoft.AspNetCore.Mvc;
using BMS.Application.DeviceSchedules.Commands;
using BMS.Application.Controllers.Queries;
using Bms.Infrastructure.Seeds;
using WebApi.Security.Authorization;
using BMS.Application.DeviceSchedules.Queries;

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

        // CREATE
        [RequirePermission(PermissionKeys.DeviceSchedules.Create)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateDeviceScheduleCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeviceScheduleCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id in route and body do not match.");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("SetActivation/{id}")]
        public async Task<IActionResult> SetActivation(Guid id, [FromBody] UpdateDeviceScheduleCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id in route and body do not match.");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // GET LIST
        // Todo:
        //[RequirePermission(PermissionKeys.DeviceSchedules.View)]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetDeviceSchedulesListQuery query)
            {
            var list = await _mediator.Send(query);

            return Ok(list);
        }
    }
}
