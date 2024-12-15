using FluentAssertions;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.Persistence
{
    public class AppDbContextTests : TestBase
    {
        [Fact]
        public async Task DbContext_SaveChanges_ShouldPersistData()
        {
            using var context = CreateContext();

            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };

            context.Authors.Add(author);
            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            context.Authors.Count().Should().Be(1);
            context.Genres.Count().Should().Be(1);

            var savedAuthor = await context.Authors.FirstOrDefaultAsync();
            var savedGenre = await context.Genres.FirstOrDefaultAsync();

            var book = new Book {
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 5,
                AuthorId = savedAuthor.Id,
                GenreId = savedGenre.Id};

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var a = context.Books.Count();
            var savedBook = await context.Books.FirstOrDefaultAsync();

            savedBook.Should().NotBeNull();
            savedBook.Title.Should().Be("Test Book");

            savedAuthor.Should().NotBeNull();
            savedAuthor.Name.Should().Be("Test Author");

            savedGenre.Should().NotBeNull();
            savedGenre.Name.Should().Be("Test Genre");
        }

        [Fact]
        public async Task DbContext_Relationships_ShouldWorkCorrectly()
        {
            using var context = CreateContext();

            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            var book = new Book 
            { 
                Title = "Test Book", 
                ISBN = "1234567890123",
                Author = author,
                Genre = genre
            };

            context.Books.Add(book);
            await context.SaveChangesAsync();
            
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

        [Fact]
        public async Task DbContext_AuthorWithMultipleBooks_ShouldWorkCorrectly()
        {
            using var context = CreateContext();

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
            using var context = CreateContext();

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
    }
}
