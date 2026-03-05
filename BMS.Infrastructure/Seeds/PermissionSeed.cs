using BMS.Domain.Entities;
using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bms.Infrastructure.Seeds
{
    public static class PermissionSeed
    {
        public static readonly List<Permission> AllPermissions = new()
        {
            new Permission(1, "Roles.View", "مشاهده گروه‌های دسترسی", "UsersAndAccess", 1),
            new Permission(2, "Roles.Manage", "مدیریت گروه‌های دسترسی", "UsersAndAccess", 2),

            new Permission(3, "Dashboard.View", "مشاهده داشبورد", "Dashboard", 0),

            new Permission(4, "Controllers.View", "مشاهده کنترلرها", "Controllers", 0),
            new Permission(5, "Controllers.Manage", "مدیریت کنترلرها", "Controllers", 2),

            new Permission(6, "Devices.View", "مشاهده تجهیزات", "Devices", 0),
            new Permission(7, "Devices.Definition.Manage", "مدیریت مشخصات/عکس تجهیزات", "Devices", 1),
            new Permission(8, "Devices.Command.Execute", "اجرای فرمان روشن/خاموش", "Devices", 2),

            new Permission(9, "Trends.View", "مشاهده نمودارها", "Trends", 0),
            new Permission(10, "Trends.Export", "خروجی اکسل نمودارها", "Trends", 1),

            new Permission(11, "Scheduler.View", "مشاهده زمان‌بندی‌ها", "Scheduler", 0),
            new Permission(12, "Scheduler.Manage", "مدیریت زمان‌بندی‌ها", "Scheduler", 2),

            new Permission(13, "SharedMemory.View", "مشاهده حافظه‌های مشترک", "SharedMemory", 1),
            new Permission(14, "SharedMemory.Manage", "مدیریت لیست حافظه‌های مشترک", "SharedMemory", 2),
            new Permission(15, "SharedMemory.Edit", "ویرایش مقدار حافظه‌های مشترک", "SharedMemory", 2),
            new Permission(16, "SharedMemory.Delete", "حذف حافظه مشترک", "SharedMemory", 2),

            new Permission(17, "SystemLogs.View", "مشاهده لاگ‌های سیستمی", "Logs", 1),
            new Permission(18, "UserAudit.View", "مشاهده لاگ تغییرات کاربران", "Logs", 2),

            new Permission(19, "PM.View", "مشاهده تعمیر و نگهداری", "PM", 0),
            new Permission(20, "PM.Manage", "مدیریت تعمیر و نگهداری", "PM", 1)
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(AllPermissions);
        }
    }
}
