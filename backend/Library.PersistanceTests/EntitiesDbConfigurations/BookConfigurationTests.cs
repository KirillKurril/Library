using FluentAssertions;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.EntitiesBdConfigurations
{
    public class BookConfigurationTests : TestBase
    {
        [Fact]
        public void BookConfiguration_PrimaryKey_ShouldBeConfiguredCorrectly()
        {

            using var context = CreateContext();
            var entityType = context.Model.FindEntityType(typeof(Book));

            
            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Single().Name.Should().Be(nameof(Book.Id));
        }

        [Fact]
        public void BookConfiguration_RequiredProperties_ShouldBeConfiguredCorrectly()
        {

            using var context = CreateContext();
            var entityType = context.Model.FindEntityType(typeof(Book));

            
            var titleProperty = entityType.FindProperty(nameof(Book.Title));
            titleProperty.Should().NotBeNull();
            titleProperty.IsNullable.Should().BeFalse();
            titleProperty.GetMaxLength().Should().Be(200);

            var isbnProperty = entityType.FindProperty(nameof(Book.ISBN));
            isbnProperty.Should().NotBeNull();
            isbnProperty.IsNullable.Should().BeFalse();
            isbnProperty.GetMaxLength().Should().Be(17);

            var quantityProperty = entityType.FindProperty(nameof(Book.Quantity));
            quantityProperty.Should().NotBeNull();
            quantityProperty.IsNullable.Should().BeFalse();
        }

        [Fact]
        public void BookConfiguration_ForeignKeys_ShouldBeConfiguredCorrectly()
        {

            using var context = CreateContext();
            var entityType = context.Model.FindEntityType(typeof(Book));

            
            var authorForeignKey = entityType.GetForeignKeys()
                .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Author));
            authorForeignKey.Should().NotBeNull();
            authorForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);

            var genreForeignKey = entityType.GetForeignKeys()
                .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Genre));
            genreForeignKey.Should().NotBeNull();
            genreForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
        }

        [Fact]
        public async Task BookConfiguration_QuantityConstraint_ShouldPreventNegativeValues()
        {

            using var context = CreateContext();
            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = -1,
                Author = author,
                Genre = genre
            };

            context.Books.Add(book);

            
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task BookConfiguration_NavigationProperties_ShouldLoadCorrectly()
        {

            using var context = CreateContext();
            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                Author = author,
                Genre = genre
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();

            
            var loadedBook = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == book.Id);

            loadedBook.Should().NotBeNull();
            loadedBook.Author.Should().NotBeNull();
            loadedBook.Author.Name.Should().Be("Test Author");
            loadedBook.Genre.Should().NotBeNull();
            loadedBook.Genre.Name.Should().Be("Test Genre");
        }

        [Fact]
        public async Task BookConfiguration_IsAvailable_ShouldWorkCorrectly()
        {

            using var context = CreateContext();
            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            
            var availableBook = new Book
            {
                Title = "Available Book",
                ISBN = "1234567890123",
                Quantity = 1,
                Author = author,
                Genre = genre
            };

            var unavailableBook = new Book
            {
                Title = "Unavailable Book",
                ISBN = "1234567890124",
                Quantity = 0,
                Author = author,
                Genre = genre
            };

            context.Books.AddRange(availableBook, unavailableBook);
            await context.SaveChangesAsync();

            
            availableBook.IsAvailable.Should().BeTrue();
            unavailableBook.IsAvailable.Should().BeFalse();
        }
    }
}
