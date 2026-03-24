using BMS.Application.Common.Pagination;
using BMS.Application.CommandDefinitions.Dtos;
using MediatR;

namespace BMS.Application.CommandDefinitions.Queries;

public class GetCommandDefinitionsQuery : PagedRequest, IRequest<PagedResult<CommandDefinitionDto>>
{
}
