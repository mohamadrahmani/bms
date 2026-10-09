using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public class PointCategoryConfiguration : IEntityTypeConfiguration<PointCategory>
{
    public void Configure(EntityTypeBuilder<PointCategory> builder)
    {
        builder.ToTable("PointCategories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).ValueGeneratedNever();
        builder.Property(category => category.Code).HasMaxLength(100).IsRequired();
        builder.Property(category => category.Title).HasMaxLength(200).IsRequired();
        builder.HasIndex(category => category.Code).IsUnique();
        builder.HasIndex(category => new { category.DeviceType, category.SortOrder });
    }
}
