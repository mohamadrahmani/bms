using BMS.Application.Common.Interfaces;
using BMS.Application.Controllers.Commands;
using MediatR;

namespace BMS.Application.Controllers.Handlers
{
    public class DeleteControllerCommandHandler
        : IRequestHandler<DeleteControllerCommand>
    {
        private readonly IControllerRepository _controllerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteControllerCommandHandler(
            IControllerRepository controllerRepository,
            IUnitOfWork unitOfWork)
        {
            _controllerRepository = controllerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            DeleteControllerCommand request,
            CancellationToken cancellationToken)
        {
            var controller = await _controllerRepository
                .GetByIdAsync(request.ControllerId, cancellationToken);

            if (controller == null)
                throw new Exception("Controller not found");

            _controllerRepository.Remove(controller);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
