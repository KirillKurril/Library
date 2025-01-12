using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Persistance.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder
                .HasKey(g => g.Id);

            builder
                .Property(g => g.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}
