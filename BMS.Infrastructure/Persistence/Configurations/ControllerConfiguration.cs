using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public class ControllerConfiguration : IEntityTypeConfiguration<Controller>
    {
        public void Configure(EntityTypeBuilder<Controller> builder)
        {
            builder.ToTable("Controllers");

            builder.HasKey(c => c.Id);
            builder.OwnsOne(c => c.Location, location =>
            {
                location.Property(p => p.SiteId).HasColumnName("SiteId");
                location.Property(p => p.BuildingId).HasColumnName("BuildingId");
                location.Property(p => p.FloorId).HasColumnName("FloorId");
                location.Property(p => p.WardId).HasColumnName("WardId");
                location.Property(p => p.RoomId).HasColumnName("RoomId");
            });

            builder.Navigation(c => c.Location).IsRequired(false);



            builder.Ignore("_sync");

            builder.HasMany(c => c.Devices)
                .WithOne(d => d.Controller)
                .HasForeignKey(d => d.ControllerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
