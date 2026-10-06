using BMS.Domain.Entities.Logs;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BMS.Application.Common.Interfaces
{
    public interface ILogRepository
    {
        IQueryable<Log> Logs { get; }
        Task<Log?> GetByIdAsync(Guid id);
        Task<string?> GetObjectDisplayNameAsync(string objectName, Guid id, CancellationToken cancellationToken = default);
        Task EnrichAsync(IEnumerable<Log> logs, CancellationToken cancellationToken = default);
        //Task WriteAsync(Log log);
        //void Add(Log log);
        Task AddAsync(Log log, CancellationToken cancellationToken = default);
    }
}
