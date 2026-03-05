using BMS.Domain.Entities;
using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bms.Infrastructure.Seeds
{
    public static class RoleSeed
    {
        public const int SuperAdminId = 1;
        public const int OperatorId = 2;
        public const int ViewerId = 3;

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role(SuperAdminId, "SuperAdmin", "دسترسی کامل به کل سیستم", true),
                new Role(OperatorId, "Operator", "اپراتور سیستم و تجهیزات", true),
                new Role(ViewerId, "Viewer", "فقط مشاهده اطلاعات", true)
            );
        }
    }
}
