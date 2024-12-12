using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Persistance.Configurations
{
    internal class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(a => a.Surname)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
