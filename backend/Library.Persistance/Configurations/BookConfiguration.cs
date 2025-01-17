using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Library.Persistance.Configurations
{
    internal class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder
                .HasKey(b => b.Id);

            builder
                .Property(b => b.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder
                .Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(17);

            builder
                .Property(b => b.Description)
                .HasMaxLength(2000);

            builder
                .Property(b => b.Quantity)
                .IsRequired();

            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Book_Quantity_NonNegative", $"{nameof(Book.Quantity)} >= 0"));



            builder
                .Property(b => b.AuthorId)
                .IsRequired();

            builder
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(b => b.GenreId)
                .IsRequired();

            builder
                .HasOne(b => b.Genre)
                .WithMany()
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(b => b.ImageUrl)
                .HasMaxLength(500);

            builder
                .Ignore(b => b.IsAvailable);
        }
    }
}
