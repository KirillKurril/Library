using FluentAssertions;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.EntitiesDbConfigurations
{
    public class GenreConfigurationTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public GenreConfigurationTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GenreConfiguration_PrimaryKey_ShouldBeConfiguredCorrectly()
        {
            using var context = new AppDbContext(_options);
            var entityType = context.Model.FindEntityType(typeof(Genre));

            var primaryKey = entityType.FindPrimaryKey();
            primaryKey.Should().NotBeNull();
            primaryKey.Properties.Single().Name.Should().Be(nameof(Genre.Id));
        }

        [Fact]
        public void GenreConfiguration_RequiredProperties_ShouldBeConfiguredCorrectly()
        {
            using var context = new AppDbContext(_options);
            var entityType = context.Model.FindEntityType(typeof(Genre));

            var nameProperty = entityType.FindProperty(nameof(Genre.Name));
            nameProperty.Should().NotBeNull();
            nameProperty.IsNullable.Should().BeFalse();
            nameProperty.GetMaxLength().Should().Be(100);
        }

        [Fact]
        public async Task GenreConfiguration_DefaultId_ShouldBeGenerated()
        {
            using var context = new AppDbContext(_options);
            var genre = new Genre { Name = "Test Genre" };

            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            genre.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task GenreConfiguration_UniqueConstraint_ShouldPreventDuplicateNames()
        {
            using var context = new AppDbContext(_options);
            
            var genre1 = new Genre { Name = "Test Genre" };
            context.Genres.Add(genre1);
            await context.SaveChangesAsync();

            var genre2 = new Genre { Name = "Test Genre" };
            context.Genres.Add(genre2);

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task GenreConfiguration_NameMaxLength_ShouldEnforceLimit()
        {
            using var context = new AppDbContext(_options);
            
            var longName = new string('A', 101); // Превышаем максимальную длину
            var genre = new Genre { Name = longName };

            context.Genres.Add(genre);
            
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task GenreConfiguration_BooksRelationship_ShouldWorkCorrectly()
        {
            using var context = new AppDbContext(_options);
            
            var genre = new Genre { Name = "Test Genre" };
            var author = new Author { Name = "Test Author" };
            
            context.Genres.Add(genre);
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            var books = new[]
            {
                new Book 
                { 
                    Title = "Book 1",
                    ISBN = "1234567890123",
                    Quantity = 1,
                    GenreId = genre.Id,
                    AuthorId = author.Id
                },
                new Book 
                { 
                    Title = "Book 2",
                    ISBN = "1234567890124",
                    Quantity = 1,
                    GenreId = genre.Id,
                    AuthorId = author.Id
                }
            };

            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var savedGenre = await context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == genre.Id);

            savedGenre.Should().NotBeNull();
            savedGenre.Books.Should().HaveCount(2);
        }
    }
}
