using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.ToTable("Devices");

            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.Controller)
                .WithMany(c => c.Devices)     // 👈 مهم
                .HasForeignKey(d => d.ControllerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(d => d.Location, location =>
            {
                location.Property(p => p.SiteId).HasColumnName("SiteId");
                location.Property(p => p.BuildingId).HasColumnName("BuildingId");
                location.Property(p => p.FloorId).HasColumnName("FloorId");
                location.Property(p => p.WardId).HasColumnName("WardId");
                location.Property(p => p.RoomId).HasColumnName("RoomId");
            });

            builder.Ignore("_sync");

            builder.HasMany(d => d.DevicePoints)
                .WithOne(p => p.Device)
                .HasForeignKey(p => p.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(d => d.DevicePoints)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
