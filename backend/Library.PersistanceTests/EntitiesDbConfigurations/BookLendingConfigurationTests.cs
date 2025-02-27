using FluentAssertions;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.IntegrationTests.EntitiesBdConfigurations
{
    public class BookLendingConfigurationTests : TestBase
    {
        [Fact]
        public void BookLendingConfiguration_PrimaryKey_ShouldBeConfiguredCorrectly()
        {
            using var context = CreateContext();
            var entityType = context.Model.FindEntityType(typeof(BookLending));


            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Single().Name.Should().Be(nameof(BookLending.Id));
        }

        [Fact]
        public void BookLendingConfiguration_RequiredProperties_ShouldBeConfiguredCorrectly()
        {

            using var context = CreateContext();
            var entityType = context.Model.FindEntityType(typeof(BookLending));


            var bookIdProperty = entityType.FindProperty(nameof(BookLending.BookId));
            bookIdProperty.Should().NotBeNull();
            bookIdProperty.IsNullable.Should().BeFalse();

            var userIdProperty = entityType.FindProperty(nameof(BookLending.UserId));
            userIdProperty.Should().NotBeNull();
            userIdProperty.IsNullable.Should().BeFalse();

            var borrowedAtProperty = entityType.FindProperty(nameof(BookLending.BorrowedAt));
            borrowedAtProperty.Should().NotBeNull();
            borrowedAtProperty.IsNullable.Should().BeFalse();

            var returnDateProperty = entityType.FindProperty(nameof(BookLending.ReturnDate));
            returnDateProperty.Should().NotBeNull();
            returnDateProperty.IsNullable.Should().BeFalse();
        }

        [Fact]
        public async Task BookLendingConfiguration_DateConstraints_ShouldBeValid()
        {

            using var context = CreateContext();
            (var author, var genre) = CreateTestEntities(context);
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                AuthorId = author.Id,
                GenreId = genre.Id
            };

            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            var bookLending = new BookLending
            {
                BookId = book.Id,
                UserId = Guid.NewGuid(),
                BorrowedAt = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(-1) 
            };

            
            context.BookLendings.Add(bookLending);


            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task BookLendingConfiguration_ForeignKeyConstraint_ShouldWorkCorrectly()
        {

            using var context = CreateContext();
            var bookLending = new BookLending
            {
                BookId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BorrowedAt = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(14)
            };

            
            context.BookLendings.Add(bookLending);


            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task BookLendingConfiguration_ValidLending_ShouldBeCreated()
        {

            using var context = CreateContext();
            var book = new Book 
            { 
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                Author = new Author { Name = "Test Author" },
                Genre = new Genre { Name = "Test Genre" }
            };

            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            var userId = Guid.NewGuid();
            var borrowedAt = DateTime.UtcNow;
            var returnDate = borrowedAt.AddDays(14);

            var bookLending = new BookLending
            {
                BookId = book.Id,
                UserId = userId,
                BorrowedAt = borrowedAt,
                ReturnDate = returnDate
            };

            
            context.BookLendings.Add(bookLending);
            await context.SaveChangesAsync();


            var savedLending = await context.BookLendings.FirstOrDefaultAsync(bl => bl.BookId == book.Id);
            savedLending.Should().NotBeNull();
            savedLending.UserId.Should().Be(userId);
            savedLending.BorrowedAt.Should().BeCloseTo(borrowedAt, TimeSpan.FromSeconds(1));
            savedLending.ReturnDate.Should().BeCloseTo(returnDate, TimeSpan.FromSeconds(1));
        }
    }
}
