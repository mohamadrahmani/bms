using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BMS.Infrastructure.Persistence;
using BMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Security
{
    public sealed class PermissionResolver : IPermissionResolver
    {
        private readonly BMSDbContext _context;

        public PermissionResolver(BMSDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<string>> ResolveAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            // دریافت لیست کلیدهای دسترسی مربوط به نقش‌های فعال یک کاربر
            // ابتدا نقش‌های کاربر از جدول ارتباط کاربر و نقش گرفته می‌شود
            // سپس دسترسی‌های هر نقش از جدول نقش و دسترسی استخراج می‌شود
            // در نهایت فقط مقدار کلید هر دسترسی انتخاب و برگردانده می‌شود
            var rolePermissionKeys = await (
                from ur in _context.UserRoles.AsNoTracking()
                join rp in _context.RolePermissions.AsNoTracking()
                    on ur.RoleId equals rp.RoleId
                join p in _context.Permissions.AsNoTracking()
                    on rp.PermissionId equals p.Id
                where ur.UserId == userId && ur.IsActive
                select p.Key
            ).ToListAsync(cancellationToken);

            // دریافت لیست دسترسی‌های تنظیم‌شده مستقیم برای یک کاربر
            // اطلاعات از جدول دسترسی‌های کاربر گرفته می‌شود
            // سپس برای هر مورد، کلید دسترسی از جدول دسترسی‌ها استخراج می‌شود
            // نتیجه شامل نام دسترسی و وضعیت مجاز بودن آن برای کاربر است
            // این لیست هم موارد مجاز و هم موارد غیرمجاز را شامل می‌شود
            var userOverrides = await (
                from up in _context.UserPermissions.AsNoTracking()
                join p in _context.Permissions.AsNoTracking()
                    on up.PermissionId equals p.Id
                where up.UserId == userId
                select new { p.Key, up.IsGranted }
            ).ToListAsync(cancellationToken);

            // 3) اعمال Override
            var granted = new HashSet<string>(rolePermissionKeys);

            foreach (var ov in userOverrides)
            {
                if (ov.IsGranted) granted.Add(ov.Key);
                else granted.Remove(ov.Key);
            }

            return granted.OrderBy(x => x).ToList();
        }
    }
}
