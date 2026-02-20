using BMS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BMS.Infrastructure.Persistence.Entities;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public class DataPointHistoryConfiguration
        : IEntityTypeConfiguration<DataPointHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<DataPointHistoryEntity> builder)
        {
            builder.ToTable("DataPointHistory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PointId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ValueString)
                .HasMaxLength(256);

            builder.Property(x => x.TimestampUtc)
                .IsRequired();

            builder.HasIndex(x => new { x.DeviceId, x.PointId, x.TimestampUtc });
        }
    }
}