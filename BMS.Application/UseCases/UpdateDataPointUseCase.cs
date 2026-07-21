using BMS.Application.Common.Interfaces;
using BMS.Application.Interfaces;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace BMS.Application.UseCases
{
    public class UpdateDataPointUseCase
    {
        private readonly IDeviceStateStore _store;
        private readonly IEventDispatcher _dispatcher;
        private readonly IMemoryCache _memoryCache;
        private readonly IPointRepository _repository;

        public UpdateDataPointUseCase(
            IDeviceStateStore store,
            IEventDispatcher dispatcher,
             IMemoryCache memoryCache,
             IPointRepository repository)
        {
            _store = store;
            _dispatcher = dispatcher;
            _memoryCache = memoryCache;
            _repository = repository;
        }

        public async Task ExecuteAsync(
            Guid deviceId,
            Guid pointId,
            string? value)
        {
            var device = _store.Get(deviceId);

            string cacheKey = $"point_{pointId}";

            if (!_memoryCache.TryGetValue(cacheKey, out Point point))
            {
                // ۲. اگر در کش نبود، از دیتابیس می‌خوانیم
                point = await _repository.Points
                    //.ProjectTo<PointDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(p => p.Id == pointId);

                if (point != null)
                {
                    // ۳. تنظیم گزینه‌های کش (مثلاً ۱۰ دقیقه زنده ماندن)
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // اگر در ۲ دقیقه استفاده نشد، پاک شود

                    _memoryCache.Set(cacheKey, point, cacheEntryOptions);
                }
            }

            var domainEvent = device.UpdatePoint(point, value);

            if (domainEvent == null)
                return;

            await _dispatcher.DispatchAsync(domainEvent);
        }
    }
}
