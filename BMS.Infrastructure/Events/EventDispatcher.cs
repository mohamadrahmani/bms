using BMS.Application.Interfaces;
using BMS.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BMS.Application.Interfaces;
using BMS.Domain.Events;
using System;
using System.Threading.Tasks;

namespace BMS.Infrastructure.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(
            IServiceScopeFactory scopeFactory,
            ILogger<EventDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            if (domainEvent == null)
                return;

            using var scope = _scopeFactory.CreateScope();

            var provider = scope.ServiceProvider;

            var eventType = domainEvent.GetType();

            var handlerType = typeof(IEventHandler<>)
                .MakeGenericType(eventType);

            var handlers = provider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                try
                {
                    var method = handlerType
                        .GetMethod(nameof(IEventHandler<IDomainEvent>.HandleAsync));

                    if (method == null)
                        continue;

                    var task = (Task?)method.Invoke(
                        handler,
                        new object[] { domainEvent });

                    if (task != null)
                        await task;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error handling event {EventType}",
                        eventType.Name);
                }
            }
        }
    }
}