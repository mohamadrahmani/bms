using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public abstract class BaseEntityConfiguration<TEntity, TKey>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedNever();

            // Soft Delete
            builder.Property(e => e.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            // Audit
            builder.Property(e => e.CreatedAtUtc)
                   .IsRequired()
                   .ValueGeneratedNever();

            builder.Property(e => e.UpdatedAtUtc)
                   .ValueGeneratedNever();

            // Indexes
            builder.HasIndex(e => e.IsDeleted);
            builder.HasIndex(e => e.CreatedAtUtc);
        }
    }

}
