using Microsoft.AspNetCore.Mvc;
using MediatR;
using BMS.Application.Controllers.Commands;
using BMS.Application.Controllers.Queries;


namespace BMS.API.Controllers
{
    [ApiController]
    [Route("api/controllers")]
    public class ControllersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ControllersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateControllerCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateControllerCommand command)
        {
            command.ControllerId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteControllerCommand
            {
                ControllerId = id
            });

            return NoContent();
        }

        // GET LIST
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var list = await _mediator.Send(
                new GetControllersListQuery());

            return Ok(list);
        }

        // GET BY ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var controller = await _mediator.Send(
                new GetControllerByIdQuery
                {
                    ControllerId = id
                });

            if (controller == null)
                return NotFound();

            return Ok(controller);
        }
    }
}
