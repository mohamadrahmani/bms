using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BMS.Application.Devices.Handlers
{
    public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, bool>
    {
        private readonly IDeviceRepository _repository;

        public DeleteDeviceCommandHandler(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _repository.GetByIdAsync(request.Id);

            if (device == null)
                return false;

            await _repository.DeleteAsync(device);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
