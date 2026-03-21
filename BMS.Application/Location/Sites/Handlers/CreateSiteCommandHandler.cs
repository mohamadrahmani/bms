//using MediatR;
//using BMS.Application.Location.Sites.Commands;
//using BMS.Application.Common.Interfaces;
//using BMS.Domain.Entities.Location;

//namespace BMS.Application.Location.Sites.Handlers;

//public sealed class CreateSiteCommandHandler
//    : IRequestHandler<CreateSiteCommand, Guid>
//{
//    private readonly //ISiteRepository _siteRepository;
//   // private readonly IUnitOfWork _unitOfWork;

//    //public CreateSiteCommandHandler(
//    //    //ISiteRepository siteRepository,
//    //    IUnitOfWork unitOfWork)
//    //{
//    //   // _siteRepository = siteRepository;
//    //    //_unitOfWork = unitOfWork;
//    //}

//    //public async Task<Guid> Handle(
//    //    CreateSiteCommand request,
//    //    CancellationToken cancellationToken)
//    //{
//    //    //var site = new Site(
//    //    //    request.Name,
//    //    //    request.Address,
//    //    //    request.Description
//    //    //);

//    //    //await _siteRepository.AddAsync(site, cancellationToken);

//    //    //await _unitOfWork.SaveChangesAsync(cancellationToken);

//    //    return site.Id;
//    //}
//}
