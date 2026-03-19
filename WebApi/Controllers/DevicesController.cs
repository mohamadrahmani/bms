using BMS.Application.Devices.Commands;
using BMS.Application.Devices.DTOs;
using BMS.Application.Devices.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDeviceByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("controller/{controllerId}")]
        public async Task<ActionResult<List<DeviceDto>>> GetByController(Guid controllerId)
        {
            var result = await _mediator.Send(new GetDevicesByControllerQuery(controllerId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DeviceDto>> Create(CreateDeviceCommand command)
        {
            //var result = await _mediator.Send(new CreateDeviceCommand(dto));
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        //[HttpPut]
        //public async Task<ActionResult<DeviceDto>> Update(UpdateDeviceDto dto)
        //{
        //    var result = await _mediator.Send(new UpdateDeviceCommand(dto));
        //    return Ok(result);
        //}
        [HttpPut]
        public async Task<ActionResult<DeviceDto>> Update(UpdateDeviceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDeviceCommand(id));

            if (!result)
                return NotFound();

            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var result = await _mediator.Send(new GetDevicesQuery());
            return Ok(result);
        }
    }
}
