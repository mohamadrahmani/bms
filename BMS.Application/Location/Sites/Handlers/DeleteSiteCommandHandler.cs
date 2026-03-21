//using MediatR;
//using BMS.Application.Location.Sites.Commands;
//using BMS.Application.Common.Interfaces;
//using BMS.Application.Common.Exceptions;

//namespace BMS.Application.Location.Sites.Handlers;

//public sealed class DeleteSiteCommandHandler
//    : IRequestHandler<DeleteSiteCommand>
//{
//    private readonly ISiteRepository _siteRepository;
//    private readonly IUnitOfWork _unitOfWork;

//    public DeleteSiteCommandHandler(
//        ISiteRepository siteRepository,
//        IUnitOfWork unitOfWork)
//    {
//        _siteRepository = siteRepository;
//        _unitOfWork = unitOfWork;
//    }

//    public async Task Handle(
//        DeleteSiteCommand request,
//        CancellationToken cancellationToken)
//    {
//        var site = await _siteRepository
//            .GetByIdAsync(request.Id, cancellationToken);

//        if (site is null)
//            throw new NotFoundException(
//                $"Site with id '{request.Id}' not found.");

//        _siteRepository.Delete(site);

//        await _unitOfWork.SaveChangesAsync(cancellationToken);
//    }
//}
