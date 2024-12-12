using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Library.Persistance.Configurations
{
    internal class BookLendingConguration : IEntityTypeConfiguration<BookLending>
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
        }
    }
}
