using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Abstraction
{
    public interface IPlcClientProvider
    {
        /// <summary>
        /// لیست فعلی PLC Clientها را برمی‌گرداند.
        /// اگر کش خالی باشد، از دیتابیس خوانده و ساخته می‌شود.
        /// </summary>
        Task<IReadOnlyList<IPlcClient>> GetClientsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// کش را Invalid می‌کند.
        /// دفعهٔ بعد که GetClientsAsync صدا زده شود، مجدداً از دیتابیس خوانده می‌شود.
        /// </summary>
        Task InvalidateAsync(CancellationToken cancellationToken = default);
    }
}
