using Bms.Infrastructure.Seeds;
using BMS.Application.Devices.Commands;
using BMS.Application.Devices.DTOs;
using BMS.Application.Devices.Queries;
using BMS.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;

namespace BMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DevicesController(IMediator mediator)    
        {
            _mediator = mediator;
        }

        // GET BY ID
        [RequirePermission(PermissionKeys.Devices.View)]
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDeviceByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET BY CONTROLLER ID
        [RequirePermission(PermissionKeys.Devices.View)]
        [HttpGet("controller/{controllerId}")]
        public async Task<ActionResult<List<DeviceDto>>> GetByController(Guid controllerId)
        {
            var result = await _mediator.Send(new GetDevicesByControllerQuery(controllerId));
            return Ok(result);
        }

        // CREATE DEVICE
        [RequirePermission(PermissionKeys.Devices.Create)]
        [HttpPost]
        public async Task<ActionResult<DeviceDto>> Create(CreateDeviceCommand command)
        {
            //var result = await _mediator.Send(new CreateDeviceCommand(dto));
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // UPDATE DEVICE
        [RequirePermission(PermissionKeys.Devices.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult<DeviceDto>> Update(UpdateDeviceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE DEVICE
        [RequirePermission(PermissionKeys.Devices.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDeviceCommand(id));

            if (!result)
                return NotFound();

            return NoContent();
        }

        // GET LIST
        [RequirePermission(PermissionKeys.Devices.View)]
        [HttpGet]
        public async Task<IActionResult> GetDevices([FromQuery] GetDevicesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
