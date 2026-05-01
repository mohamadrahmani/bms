using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bms.Infrastructure.Seeds
{
    public static class PermissionSeed
    {
        public static readonly List<Permission> AllPermissions = new()
        {
            // Roles
            new Permission(1, PermissionKeys.Roles.View, "مشاهده گروه‌های دسترسی", "UsersAndAccess", 1),
            new Permission(2, PermissionKeys.Roles.Manage, "مدیریت گروه‌های دسترسی", "UsersAndAccess", 2),

            // Dashboard
            new Permission(3, PermissionKeys.Dashboard.View, "مشاهده داشبورد", "Dashboard", 0),

            // Controllers
            new Permission(4, PermissionKeys.Controllers.View, "مشاهده کنترلرها", "Controllers", 0),
            new Permission(5, PermissionKeys.Controllers.Create, "ایجاد کنترلر", "Controllers", 1),
            new Permission(6, PermissionKeys.Controllers.Update, "ویرایش کنترلر", "Controllers", 2),
            new Permission(7, PermissionKeys.Controllers.Delete, "حذف کنترلر", "Controllers", 3),

            // Devices
            new Permission(8, PermissionKeys.Devices.View, "مشاهده تجهیزات", "Devices", 0),
            new Permission(9, PermissionKeys.Devices.Create, "ایجاد تجهیز", "Devices", 1),
            new Permission(10, PermissionKeys.Devices.Update, "ویرایش تجهیز", "Devices", 2),
            new Permission(11, PermissionKeys.Devices.Delete, "حذف تجهیز", "Devices", 3),
            new Permission(12, PermissionKeys.Devices.DefinitionManage, "مدیریت مشخصات تجهیزات", "Devices", 4),
            new Permission(13, PermissionKeys.Devices.CommandExecute, "اجرای فرمان تجهیز", "Devices", 5),

            // Points
            new Permission(14, PermissionKeys.Points.View, "مشاهده نقاط", "Points", 0),
            new Permission(15, PermissionKeys.Points.Create, "ایجاد نقطه", "Points", 1),
            new Permission(16, PermissionKeys.Points.Update, "ویرایش نقطه", "Points", 2),
            new Permission(17, PermissionKeys.Points.Delete, "حذف نقطه", "Points", 3),
            new Permission(18, PermissionKeys.Points.Manage, "مدیریت نقاط", "Points", 4),

            // Users
            new Permission(19, PermissionKeys.Users.View, "مشاهده کاربران", "Users", 0),
            new Permission(20, PermissionKeys.Users.Create, "ایجاد کاربر", "Users", 1),
            new Permission(21, PermissionKeys.Users.Update, "ویرایش کاربر", "Users", 2),
            new Permission(22, PermissionKeys.Users.Delete, "حذف کاربر", "Users", 3),
            new Permission(23, PermissionKeys.Users.Manage, "مدیریت کاربران", "Users", 4),

            // Persons
            new Permission(24, PermissionKeys.Persons.View, "مشاهده اشخاص", "Persons", 0),
            new Permission(25, PermissionKeys.Persons.Create, "ایجاد شخص", "Persons", 1),
            new Permission(26, PermissionKeys.Persons.Update, "ویرایش شخص", "Persons", 2),
            new Permission(27, PermissionKeys.Persons.Delete, "حذف شخص", "Persons", 3),

            // Sites
            new Permission(28, PermissionKeys.Sites.View, "مشاهده سایت‌ها", "Sites", 0),
            new Permission(29, PermissionKeys.Sites.Create, "ایجاد سایت", "Sites", 1),
            new Permission(30, PermissionKeys.Sites.Update, "ویرایش سایت", "Sites", 2),
            new Permission(31, PermissionKeys.Sites.Delete, "حذف سایت", "Sites", 3),

            // Buildings
            new Permission(32, PermissionKeys.Buildings.View, "مشاهده ساختمان‌ها", "Buildings", 0),
            new Permission(33, PermissionKeys.Buildings.Create, "ایجاد ساختمان", "Buildings", 1),
            new Permission(34, PermissionKeys.Buildings.Update, "ویرایش ساختمان", "Buildings", 2),
            new Permission(35, PermissionKeys.Buildings.Delete, "حذف ساختمان", "Buildings", 3),

            // Floors
            new Permission(36, PermissionKeys.Floors.View, "مشاهده طبقات", "Floors", 0),
            new Permission(37, PermissionKeys.Floors.Create, "ایجاد طبقه", "Floors", 1),
            new Permission(38, PermissionKeys.Floors.Update, "ویرایش طبقه", "Floors", 2),
            new Permission(39, PermissionKeys.Floors.Delete, "حذف طبقه", "Floors", 3),

            // Wards
            new Permission(40, PermissionKeys.Wards.View, "مشاهده بخش‌ها", "Wards", 0),
            new Permission(41, PermissionKeys.Wards.Create, "ایجاد بخش", "Wards", 1),
            new Permission(42, PermissionKeys.Wards.Update, "ویرایش بخش", "Wards", 2),
            new Permission(43, PermissionKeys.Wards.Delete, "حذف بخش", "Wards", 3),

            // Rooms
            new Permission(44, PermissionKeys.Rooms.View, "مشاهده اتاق‌ها", "Rooms", 0),
            new Permission(45, PermissionKeys.Rooms.Create, "ایجاد اتاق", "Rooms", 1),
            new Permission(46, PermissionKeys.Rooms.Update, "ویرایش اتاق", "Rooms", 2),
            new Permission(47, PermissionKeys.Rooms.Delete, "حذف اتاق", "Rooms", 3),

            // Logs
            new Permission(48, PermissionKeys.Logs.SystemLogsView, "مشاهده لاگ‌های سیستمی", "Logs", 1),
            new Permission(49, PermissionKeys.Logs.UserAuditView, "مشاهده تغییرات کاربران", "Logs", 2),

            // Scheduler
            new Permission(50, PermissionKeys.Scheduler.View, "مشاهده زمان‌بندی‌ها", "Scheduler", 0),
            new Permission(51, PermissionKeys.Scheduler.Manage, "مدیریت زمان‌بندی", "Scheduler", 1),

            // SharedMemory
            new Permission(52, PermissionKeys.SharedMemory.View, "مشاهده حافظه‌های مشترک", "SharedMemory", 0),
            new Permission(53, PermissionKeys.SharedMemory.Manage, "مدیریت حافظه‌های مشترک", "SharedMemory", 1),
            new Permission(54, PermissionKeys.SharedMemory.Edit, "ویرایش حافظه مشترک", "SharedMemory", 2),
            new Permission(55, PermissionKeys.SharedMemory.Delete, "حذف حافظه مشترک", "SharedMemory", 3),

            // Trends
            new Permission(56, PermissionKeys.Trends.View, "مشاهده روندها", "Trends", 0),
            new Permission(57, PermissionKeys.Trends.Export, "خروجی گرفتن از روندها", "Trends", 1),

            // PM
            new Permission(58, PermissionKeys.PM.View, "مشاهده PM", "PM", 0),
            new Permission(59, PermissionKeys.PM.Manage, "مدیریت PM", "PM", 1)
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(AllPermissions);
        }
    }
}
