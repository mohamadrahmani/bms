using BMS.Domain.Events;

namespace BMS.Application.Interfaces
{
    public interface IAlarmEvaluator
    {
        void Evaluate(DataPointUpdatedDomainEvent domainEvent);
    }
}
