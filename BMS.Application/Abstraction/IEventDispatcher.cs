using System.Threading.Tasks;
using BMS.Domain.Events;

namespace BMS.Application.Interfaces
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}
