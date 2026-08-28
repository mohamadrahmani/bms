using Bms.Infrastructure.Seeds;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using BMS.Domain.Entities.Logs;
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
        public DbSet<Device> Devices { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<CommandDefinition> CommandDefinitions { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<DeviceSchedule> DeviceSchedules { get; set; }
        public DbSet<PmSchedule> PmSchedules { get; set; }
        public DbSet<PmServiceHistory> PmServiceHistories { get; set; }


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
