using MediatR;
using BMS.Application.Location.Sites.Queries;
using BMS.Application.Location.Sites.Dtos;
using BMS.Application.Common.Interfaces;

namespace BMS.Application.Location.Sites.Handlers;

public sealed class GetSitesListQueryHandler
    : IRequestHandler<GetSitesListQuery, IReadOnlyList<SiteDto>>
{
    private readonly ISiteRepository _siteRepository;

    public GetSitesListQueryHandler(ISiteRepository siteRepository)
    {
        _siteRepository = siteRepository;
    }

    public async Task<IReadOnlyList<SiteDto>> Handle(
        GetSitesListQuery request,
        CancellationToken cancellationToken)
    {
        var sites = await _siteRepository
            .GetAllAsync(cancellationToken);

        return sites
            .Select(site => new SiteDto
            {
                Id = site.Id,
                Name = site.Name,
                Address = site.Address,
                Description = site.Description
            })
            .ToList();
    }
}
