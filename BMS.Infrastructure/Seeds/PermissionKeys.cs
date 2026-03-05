namespace Bms.Infrastructure.Seeds
{
    public static class PermissionKeys
    {
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
            "Controllers.View",
            "Controllers.Manage",
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
