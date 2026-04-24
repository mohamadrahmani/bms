using BMS.Application.Common.Interfaces;
using BMS.Application.Logs.Queries;
using BMS.Domain.Entities.Logs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Logs.Handlers
{
    public class GetLogByIdHandler
    : IRequestHandler<GetLogByIdQuery, Log?>
    {
        private readonly ILogRepository _logRepository;

        public GetLogByIdHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<Log?> Handle(
            GetLogByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _logRepository.GetByIdAsync(request.Id);
        }
    }
}
