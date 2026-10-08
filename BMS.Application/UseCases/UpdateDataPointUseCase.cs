using BMS.Application.Common.Interfaces;
using BMS.Application.Interfaces;
using BMS.Application.Models;
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
        private readonly IHistorianWriter _historianWriter;

        public UpdateDataPointUseCase(
            IDeviceStateStore store,
            IEventDispatcher dispatcher,
             IMemoryCache memoryCache,
             IPointRepository repository,
             IHistorianWriter historianWriter)
        {
            _store = store;
            _dispatcher = dispatcher;
            _memoryCache = memoryCache;
            _repository = repository;
            _historianWriter = historianWriter;
        }

        public async Task ExecuteAsync(
            Guid deviceId,
            Guid pointId,
            string? value,
            DateTime? timestampUtc = null)
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

            if (point == null)
                return;

            var receivedAtUtc = timestampUtc.HasValue
                ? timestampUtc.Value.Kind switch
                {
                    DateTimeKind.Utc => timestampUtc.Value,
                    DateTimeKind.Local => timestampUtc.Value.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(timestampUtc.Value, DateTimeKind.Utc)
                }
                : DateTime.UtcNow;

            if (point.StoreHistory)
            {
                _historianWriter.Enqueue(new DataPointDeltaModel(
                    deviceId,
                    pointId,
                    value,
                    receivedAtUtc));
            }

            var domainEvent = device.UpdatePoint(point, value);

            if (domainEvent == null)
                return;

            await _dispatcher.DispatchAsync(domainEvent);
        }
    }
}
