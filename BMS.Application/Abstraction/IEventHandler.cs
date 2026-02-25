using BMS.Domain.Events;
using BMS.Domain.Events;
using System.Threading.Tasks;

namespace BMS.Application.Interfaces
{
    public interface IEventHandler<in TEvent>
        where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }
}