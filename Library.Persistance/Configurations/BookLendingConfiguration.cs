using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Persistance.Configurations
{
    internal class BookLendingConfiguration : IEntityTypeConfiguration<BookLending>
    {
        public void Configure(EntityTypeBuilder<BookLending> builder)
        {
            builder
                .HasKey(bl => bl.Id);

            builder
                .Property(bl => bl.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(bl => bl.BookId)
                .IsRequired();

            builder
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(bl => bl.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder
                .Property(bl => bl.UserId)
                .IsRequired();

            builder
                .Property(bl => bl.BorrowedAt)
                .IsRequired();

            builder
                .Property(bl => bl.ReturnDate)
                .IsRequired();

            // Добавляем ограничение на уровне БД
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_BookLending_ReturnDate_After_BorrowedAt",
                "ReturnDate > BorrowedAt"));
        }
    }
}
