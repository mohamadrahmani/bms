using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BMS.Domain.Entities.BMS;

public class PointConfiguration : IEntityTypeConfiguration<Point>
{
    public void Configure(EntityTypeBuilder<Point> builder)
    {
        builder.ToTable("Points");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.StoreHistory)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(p => p.Device)
            .WithMany(d => d.DevicePoints)
            .HasForeignKey(p => p.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(p => p.Location, location =>
        {
            location.Property(p => p.SiteId).HasColumnName("SiteId");
            location.Property(p => p.BuildingId).HasColumnName("BuildingId");
            location.Property(p => p.FloorId).HasColumnName("FloorId");
            location.Property(p => p.WardId).HasColumnName("WardId");
            location.Property(p => p.RoomId).HasColumnName("RoomId");
        });
    }
}
