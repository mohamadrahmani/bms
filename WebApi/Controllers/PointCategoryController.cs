using Bms.Infrastructure.Seeds;
using BMS.Application.Points.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/point-categories")]
public class PointCategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public PointCategoryController(IMediator mediator) => _mediator = mediator;

    [RequirePermission(PermissionKeys.Points.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetPointCategoriesQuery query)
        => Ok(await _mediator.Send(query));
}
