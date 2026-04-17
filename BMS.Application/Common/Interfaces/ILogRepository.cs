using BMS.Domain.Entities.Logs;
using System.Linq;
using System.Threading.Tasks;

namespace BMS.Application.Common.Interfaces
{
    public interface ILogRepository
    {
        IQueryable<Log> Logs { get; }
        Task<Log?> GetByIdAsync(Guid id);
        //Task WriteAsync(Log log);
        //void Add(Log log);
        Task AddAsync(Log log);
    }
}
