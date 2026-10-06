using System.Threading;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;

namespace BMS.Infrastructure.Logging
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ILogRepository _logRepository;

        public AuditLogger(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        //public async Task WriteAsync(Log log, CancellationToken cancellationToken = default)
        //{
        //    await _logRepository.WriteAsync(log);
        //}
        //public void Add(Log log)
        //{
        //    _logRepository.Add(log);
        //}
        public async Task Add(Log log, CancellationToken cancellationToken = default)
        {
            await _logRepository.AddAsync(log, cancellationToken);
        }

    }
}
