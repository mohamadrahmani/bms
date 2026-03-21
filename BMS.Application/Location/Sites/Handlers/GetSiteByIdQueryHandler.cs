using MediatR;
using BMS.Application.Location.Sites.Queries;
using BMS.Application.Location.Sites.Dtos;
using BMS.Application.Common.Interfaces;

namespace BMS.Application.Location.Sites.Handlers;

public sealed class GetSiteByIdQueryHandler
    : IRequestHandler<GetSiteByIdQuery, SiteDto?>
{
    private readonly ISiteRepository _siteRepository;

    public GetSiteByIdQueryHandler(ISiteRepository siteRepository)
    {
        _siteRepository = siteRepository;
    }

    public async Task<SiteDto?> Handle(
        GetSiteByIdQuery request,
        CancellationToken cancellationToken)
    {
        var site = await _siteRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (site is null)
            return null;

        return new SiteDto
        {
            Id = site.Id,
            Name = site.Name,
            Address = site.Address,
            Description = site.Description
        };
    }
}
