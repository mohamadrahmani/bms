using Bms.Infrastructure.Seeds;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Infrastructure.Persistence.Configurations;
using BMS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace BMS.Infrastructure.Persistence
{
    public class BMSDbContext : DbContext
    {
        public BMSDbContext(DbContextOptions<BMSDbContext> options)
            : base(options)
        {
        }

        // ===== Identity / Security =====
        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Controller> Controllers { get; set; }
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<Point> Points => Set<Point>();

        // ===== BMS Entities =====
        public DbSet<DataPointHistoryEntity> DataPointHistory =>
            Set<DataPointHistoryEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BMSDbContext).Assembly);
            // ===== Seeds =====
            PermissionSeed.Seed(modelBuilder);
            RoleSeed.Seed(modelBuilder);
            RolePermissionSeed.Seed(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
