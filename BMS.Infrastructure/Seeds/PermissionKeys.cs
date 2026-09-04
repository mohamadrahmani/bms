namespace Bms.Infrastructure.Seeds
{
    /// <summary>
    /// شامل کلیدهای استاندارد و ماژولار برای تمام بخش‌های سیستم BMS.
    /// هیچ ارتباط مستقیمی با نقش‌ها ندارد (Role-Based Permission حذف شده).
    /// </summary>
    public static class PermissionKeys
    {
        // نقش‌ها
        public static class Roles
        {
            public const string View = "Roles.View";
            public const string Manage = "Roles.Manage";
        }

        // داشبورد
        public static class Dashboard
        {
            public const string View = "Dashboard.View";
        }

        // کنترلرها
        public static class Controllers
        {
            public const string View = "Controllers.View";
            public const string Create = "Controllers.Create";
            public const string Update = "Controllers.Update";
            public const string Delete = "Controllers.Delete";
        }

        

        public static class DeviceSchedules
        {
            public const string View = "DeviceSchedules.View";
            public const string Create = "DeviceSchedules.Create";
            public const string Update = "DeviceSchedules.Update";
            public const string Delete = "DeviceSchedules.Delete";
        }


        // دستگاه‌ها (Devices)
        public static class Devices
        {
            public const string View = "Devices.View";
            public const string Create = "Devices.Create";
            public const string Update = "Devices.Update";
            public const string Delete = "Devices.Delete";
            public const string DefinitionManage = "Devices.Definition.Manage";
            public const string CommandExecute = "Devices.Command.Execute";
        }


        // دستورات دستگاه ‌ها (CommandDefinition)
        public static class CommandDefinition
        {
            public const string View = "CommandDefinitions.View";
            public const string Create = "CommandDefinitions.Create";
            public const string Update = "CommandDefinitions.Update";
            public const string Delete = "CommandDefinitions.Delete";
            public const string DefinitionManage = "CommandDefinitions.Definition.Manage";
            public const string CommandExecute = "CommandDefinitions.Command.Execute";
        }

        // نقاط (Points)
        public static class Points
        {
            public const string View = "Points.View";
            public const string Create = "Points.Create";
            public const string Update = "Points.Update";
            public const string Delete = "Points.Delete";
            public const string Manage = "Points.Manage";
        }

        // کاربران (Users)
        public static class Users
        {
            public const string View = "Users.View";
            public const string Create = "Users.Create";
            public const string Update = "Users.Update";
            public const string Delete = "Users.Delete";
            public const string Manage = "Users.Manage";
        }

        // اشخاص (Persons)
        public static class Persons
        {
            public const string View = "Persons.View";
            public const string Create = "Persons.Create";
            public const string Update = "Persons.Update";
            public const string Delete = "Persons.Delete";
        }

        // مکان/سایت‌ها (Sites)
        public static class Sites
        {
            public const string View = "Sites.View";
            public const string Create = "Sites.Create";
            public const string Update = "Sites.Update";
            public const string Delete = "Sites.Delete";
        }

        // ساختمان‌ها (Buildings)
        public static class Buildings
        {
            public const string View = "Buildings.View";
            public const string Create = "Buildings.Create";
            public const string Update = "Buildings.Update";
            public const string Delete = "Buildings.Delete";
        }

        // طیقه ‌ها / Floors
        public static class Floors
        {
            public const string View = "Floors.View";
            public const string Create = "Floors.Create";
            public const string Update = "Floors.Update";
            public const string Delete = "Floors.Delete";
        }
        // بخش ‌ها / Wards
        public static class Wards
        {
            public const string View = "Wards.View";
            public const string Create = "Wards.Create";
            public const string Update = "Wards.Update";
            public const string Delete = "Wards.Delete";
        }
        // اتاق ‌ها / Floors
        public static class Rooms
        {
            public const string View = "Rooms.View";
            public const string Create = "Rooms.Create";
            public const string Update = "Rooms.Update";
            public const string Delete = "Rooms.Delete";
        }
        // لاگ‌ها / Logs
        public static class Logs
        {
            public const string SystemLogsView = "SystemLogs.View";
            public const string UserAuditView = "UserAudit.View";
            public const string SystemErrorLogsView = "SystemErrorLogs.View";
            public const string SystemErrorLogsResolve = "SystemErrorLogs.Resolve";
        }

        // Scheduler (زمان‌بند)
        public static class Scheduler
        {
            public const string View = "Scheduler.View";
            public const string Manage = "Scheduler.Manage";
        }

        // SharedMemory
        public static class SharedMemory
        {
            public const string View = "SharedMemory.View";
            public const string Manage = "SharedMemory.Manage";
            public const string Edit = "SharedMemory.Edit";
            public const string Delete = "SharedMemory.Delete";
        }

        // روندها / Trends
        public static class Trends
        {
            public const string View = "Trends.View";
            public const string Export = "Trends.Export";
        }

        // نگهداری پیشگیرانه / PM
        public static class PM
        {
            public const string View = "PM.View";
            public const string Manage = "PM.Manage";
        }
    }
}
