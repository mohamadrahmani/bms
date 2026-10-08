using Bms.Infrastructure.Seeds;
using BMS.Application.Controllers.Commands;
using BMS.Application.Controllers.Queries;
using BMS.Application.Users.Queries;
using BMS.Infrastructure.Devices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;


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
        [RequirePermission(PermissionKeys.Controllers.Create)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateControllerCommand command)
        {
            var id = await _mediator.Send(command);

            try
            {
                using var httpClient = new HttpClient();

                var url = "http://localhost:5055/api/cache/invalidate";

                var request = new WritePointCommandRequest
                {

                };

                var response = httpClient.PostAsJsonAsync(url, request);
            }
            catch 
            { 
            }
            return Ok(id);
        }

        // UPDATE
        [RequirePermission(PermissionKeys.Controllers.Update)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateControllerCommand command)
        {
            command.ControllerId = id;
            await _mediator.Send(command);
            try { 
            using var httpClient = new HttpClient();

            var url = "http://localhost:5055/api/cache/invalidate";

            var request = new WritePointCommandRequest
            {

            };

            var response = httpClient.PostAsJsonAsync(url, request);
            }
            catch 
            {
            }
            return NoContent();
        }

        // DELETE
        [RequirePermission(PermissionKeys.Controllers.Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteControllerCommand
            {
                ControllerId = id
            });
            try
            {
                using var httpClient = new HttpClient();

                var url = "http://localhost:5055/api/cache/invalidate";

                var request = new WritePointCommandRequest
                {

                };

                var response = httpClient.PostAsJsonAsync(url, request);

            }
            catch 
            { 
            }
                return NoContent();
        }

        // GET LIST
        [RequirePermission(PermissionKeys.Controllers.View)]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetControllersListQuery query)
        {
            var list = await _mediator.Send(query);

            return Ok(list);
        }

        // GET BY ID
        [RequirePermission(PermissionKeys.Controllers.View)]
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
