using MediatR;
using BMS.Application.Location.Sites.Dtos;

namespace BMS.Application.Location.Sites.Queries;

public sealed record GetSiteByIdQuery(Guid Id)
    : IRequest<SiteDto?>;
