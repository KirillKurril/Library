using FluentAssertions;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.IntegrationTests.Persistence
{
    public class AppDbContextTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public AppDbContextTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void DbContext_Initialize_ShouldCreateDatabase()
        {
            using var context = new AppDbContext(_options);

            context.Database.IsInMemory().Should().BeTrue();
            context.Database.EnsureCreated().Should().BeTrue();
        }

        [Fact]
        public async Task DbContext_SaveChanges_ShouldPersistData()
        {
            var book = new Book { Title = "Test Book", ISBN = "1234567890123" };
            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };

            using (var context = new AppDbContext(_options))
            {
                context.Books.Add(book);
                context.Authors.Add(author);
                context.Genres.Add(genre);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_options))
            {
                context.Books.Count().Should().Be(1);
                context.Authors.Count().Should().Be(1);
                context.Genres.Count().Should().Be(1);

                var savedBook = await context.Books.FirstOrDefaultAsync();
                var savedAuthor = await context.Authors.FirstOrDefaultAsync();
                var savedGenre = await context.Genres.FirstOrDefaultAsync();

                savedBook.Should().NotBeNull();
                savedBook.Title.Should().Be("Test Book");

                savedAuthor.Should().NotBeNull();
                savedAuthor.Name.Should().Be("Test Author");

                savedGenre.Should().NotBeNull();
                savedGenre.Name.Should().Be("Test Genre");
            }
        }

        [Fact]
        public async Task DbContext_Relationships_ShouldWorkCorrectly()
        {
            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            var book = new Book 
            { 
                Title = "Test Book", 
                ISBN = "1234567890123",
                Author = author,
                Genre = genre
            };

            using (var context = new AppDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_options))
            {
                var savedBook = await context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .FirstOrDefaultAsync();

                savedBook.Should().NotBeNull();
                savedBook.Author.Should().NotBeNull();
                savedBook.Author.Name.Should().Be("Test Author");
                savedBook.Genre.Should().NotBeNull();
                savedBook.Genre.Name.Should().Be("Test Genre");
            }
        }

        [Fact]
        public async Task DbContext_AuthorWithMultipleBooks_ShouldWorkCorrectly()
        {
            using var context = new AppDbContext(_options);

            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };

            var books = new[]
            {
                new Book
                {
                    Title = "First Book",
                    ISBN = "1234567890123",
                    Description = "First Description",
                    Quantity = 2,
                    Author = author,
                    Genre = genre
                },
                new Book
                {
                    Title = "Second Book",
                    ISBN = "1234567890124",
                    Description = "Second Description",
                    Quantity = 1,
                    Author = author,
                    Genre = genre
                }
            };

            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var savedBooks = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync();

            savedBooks.Should().HaveCount(2);
            savedBooks.Should().AllSatisfy(book =>
            {
                book.Author.Should().NotBeNull();
                book.Author.Name.Should().Be("Test Author");
                book.Genre.Should().NotBeNull();
                book.Genre.Name.Should().Be("Test Genre");
            });


            var authorId = savedBooks.First().Author.Id;
            savedBooks.Should().AllSatisfy(book => book.AuthorId.Should().Be(authorId));
        }

        [Fact]
        public async Task DbContext_GenreWithMultipleBooks_ShouldTrackAvailability()
        {
            using var context = new AppDbContext(_options);

            var genre = new Genre { Name = "Science Fiction" };
            var author = new Author { Name = "Test Author" };

            var availableBook = new Book
            {
                Title = "Available Book",
                ISBN = "1234567890123",
                Quantity = 5,
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

            var booksInGenre = await context.Books
                .Include(b => b.Genre)
                .Where(b => b.Genre.Name == "Science Fiction")
                .ToListAsync();

            booksInGenre.Should().HaveCount(2);

            var available = booksInGenre.Single(b => b.Title == "Available Book");
            var unavailable = booksInGenre.Single(b => b.Title == "Unavailable Book");

            available.IsAvailable.Should().BeTrue();
            available.Quantity.Should().Be(5);

            unavailable.IsAvailable.Should().BeFalse();
            unavailable.Quantity.Should().Be(0);
        }

        [Fact]
        public async Task DbContext_DeleteEntity_ShouldRemoveFromDatabase()
        {
            var book = new Book { Title = "Test Book", ISBN = "1234567890123" };

            using (var context = new AppDbContext(_options))
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_options))
            {
                context.Books.Remove(book);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_options))
            {
                var books = await context.Books.ToListAsync();
                books.Should().BeEmpty();
            }
        }
    }
}