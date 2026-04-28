namespace Bms.Infrastructure.Seeds
{
    public static class PermissionKeys
    {
        public static class Controllers
        {
            public const string View = "Controllers.View";
            public const string Create = "Controllers.Create";
            public const string Update = "Controllers.Update";
            public const string Delete = "Controllers.Delete";
        }

        public static class Roles
        {
            public const string View = "Roles.View";
            public const string Manage = "Roles.Manage";
        }
        // دسترسی‌های پایه برای اپراتورها (Operator)
        public static readonly string[] OperatorPermissions =
        {
            "Devices.Command.Execute",
            "Devices.View",
            "Devices.Definition.Manage",
            "Trends.View",
            "Trends.Export",
            "Scheduler.View",
            "Scheduler.Manage",
            "SharedMemory.View"
        };

        // دسترسی‌های بیننده/مشاهده‌کننده (Viewer)
        public static readonly string[] ViewerPermissions =
        {
            "Dashboard.View",
            "Users.View",
            "Roles.View",
            "Controllers.View",
            "Trends.View",
            "PM.View"
        };

        // دسترسی‌های مدیر سیستم / Admin (دسترسی کامل به همه Permissionها)
        public static readonly string[] AdminPermissions =
        {
            "Roles.View",
            "Roles.Manage",
            "Dashboard.View",

            PermissionKeys.Controllers.View,
            PermissionKeys.Controllers.Create,
            PermissionKeys.Controllers.Update,
            PermissionKeys.Controllers.Delete,


            "Devices.View",
            "Devices.Definition.Manage",
            "Devices.Command.Execute",
            "Trends.View",
            "Trends.Export",
            "Scheduler.View",
            "Scheduler.Manage",
            "SharedMemory.View",
            "SharedMemory.Manage",
            "SharedMemory.Edit",
            "SharedMemory.Delete",
            "SystemLogs.View",
            "UserAudit.View",
            "PM.View",
            "PM.Manage"
        };
    }
}
