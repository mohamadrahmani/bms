using BMS.Application.Interfaces;
using BMS.Domain.Events;
using BMS.Application.Interfaces;
using BMS.Domain.Events;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Alarm
{
    public class DataPointUpdatedAlarmHandler :
        IEventHandler<DataPointUpdatedDomainEvent>
    {
        private readonly IAlarmEvaluator _alarmEvaluator;

        public DataPointUpdatedAlarmHandler(
            IAlarmEvaluator alarmEvaluator)
        {
            _alarmEvaluator = alarmEvaluator;
        }

        public Task HandleAsync(
            DataPointUpdatedDomainEvent domainEvent)
        {
            _alarmEvaluator.Evaluate(domainEvent);
            return Task.CompletedTask;
        }
    }
}