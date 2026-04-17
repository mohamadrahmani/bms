using System;
using System.Threading;
using System.Threading.Tasks;
using BMS.Domain.Entities.Logs;

namespace BMS.Application.Common.Interfaces
{
    public interface IAuditLogger
    {
        Task Add(Log log, CancellationToken cancellationToken = default);
        //void Add(Log log);
    }
}
