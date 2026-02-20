using BMS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Polly;
using BMS.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BMS.Infrastructure.Persistence
{
    public class BMSDbContext : DbContext
    {
        public DbSet<DataPointHistoryEntity> DataPointHistory =>
            Set<DataPointHistoryEntity>();
        public BMSDbContext(DbContextOptions<BMSDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BMSDbContext).Assembly);
        }
    }
}