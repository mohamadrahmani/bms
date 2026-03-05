using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class PersonConfiguration
        : BaseEntityConfiguration<Person, Guid>
    {
        public override void Configure(EntityTypeBuilder<Person> b)
        {
            base.Configure(b);

            b.ToTable("Persons");

            b.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.Email)
                .HasMaxLength(200);

            b.Property(x => x.Mobile)
                .HasMaxLength(20);

            b.Property(x => x.IsActive)
                .IsRequired();

            // Index حرفه‌ای
            b.HasIndex(x => x.Email)
                .HasDatabaseName("IX_Person_Email");
        }
    }
}
