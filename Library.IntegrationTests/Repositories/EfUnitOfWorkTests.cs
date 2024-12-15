using FluentAssertions;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Library.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.Repositories
{
    public class EfUnitOfWorkTests : TestBase
    {
        private readonly EfUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        public EfUnitOfWorkTests()
        {
            _context = CreateContext();
            _unitOfWork = new EfUnitOfWork(_context);
        }

        [Fact]
        public void UnitOfWork_Repositories_ShouldNotBeNull()
        {
            
            _unitOfWork.BookRepository.Should().NotBeNull();
            _unitOfWork.AuthorRepository.Should().NotBeNull();
            _unitOfWork.GenreRepository.Should().NotBeNull();
            _unitOfWork.BookLendingRepository.Should().NotBeNull();
        }

        [Fact]
        public async Task UnitOfWork_SaveChanges_ShouldPersistAllChanges()
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

            _unitOfWork.AuthorRepository.Add(author);
            _unitOfWork.GenreRepository.Add(genre);
            _unitOfWork.BookRepository.Add(book);
            await _unitOfWork.SaveChangesAsync();

            
            var savedBook = await _unitOfWork.BookRepository.GetByIdAsync(book.Id);
            var savedAuthor = await _unitOfWork.AuthorRepository.GetByIdAsync(author.Id);
            var savedGenre = await _unitOfWork.GenreRepository.GetByIdAsync(genre.Id);

            savedBook.Should().NotBeNull();
            savedAuthor.Should().NotBeNull();
            savedGenre.Should().NotBeNull();
        }


        [Fact]
        public async Task UnitOfWork_DeleteDatabase_ShouldDeleteDatabase()
        {
            await _unitOfWork.CreateDataBaseAsync();

            await _unitOfWork.DeleteDataBaseAsync();

            
            _context.Database.EnsureCreated().Should().BeTrue(); 
        }

        [Fact]
        public async Task UnitOfWork_TransactionScope_ShouldRollbackOnError()
        {
            
            var author = new Author { Name = "Test Author" };
            var book = new Book 
            { 
                Title = "Test Book", 
                ISBN = "1234567890123",
                Author = author
            };

            try
            {
                _unitOfWork.AuthorRepository.Add(author);
                _unitOfWork.BookRepository.Add(book);
                
                throw new Exception("Simulated error");
                
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                
                var authors = await _unitOfWork.AuthorRepository.ListAllAsync();
                var books = await _unitOfWork.BookRepository.ListAllAsync();

                authors.Should().BeEmpty();
                books.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task UnitOfWork_SaveChanges_ShouldPersistAllChangesTransactionally()
        {
            
            var initialBookCount = (await _unitOfWork.BookRepository.ListAllAsync()).Count();
            var initialAuthorCount = (await _unitOfWork.AuthorRepository.ListAllAsync()).Count();
            var initialGenreCount = (await _unitOfWork.GenreRepository.ListAllAsync()).Count();

            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                Description = "Test Description"
            };

            _unitOfWork.AuthorRepository.Add(author);
            _unitOfWork.GenreRepository.Add(genre);
            await _unitOfWork.SaveChangesAsync(); 

            book.AuthorId = author.Id;
            book.GenreId = genre.Id;

            _unitOfWork.BookRepository.Add(book);
            await _unitOfWork.SaveChangesAsync(); 

            
            var savedAuthor = await _unitOfWork.AuthorRepository.GetByIdAsync(author.Id);
            var savedGenre = await _unitOfWork.GenreRepository.GetByIdAsync(genre.Id);
            var savedBook = await _unitOfWork.BookRepository.GetByIdAsync(book.Id);

            savedAuthor.Should().NotBeNull();
            savedAuthor.Name.Should().Be("Test Author");

            savedGenre.Should().NotBeNull();
            savedGenre.Name.Should().Be("Test Genre");

            savedBook.Should().NotBeNull();
            savedBook.Title.Should().Be("Test Book");
            savedBook.AuthorId.Should().Be(author.Id);
            savedBook.GenreId.Should().Be(genre.Id);

            var finalBookCount = (await _unitOfWork.BookRepository.ListAllAsync()).Count();
            var finalAuthorCount = (await _unitOfWork.AuthorRepository.ListAllAsync()).Count();
            var finalGenreCount = (await _unitOfWork.GenreRepository.ListAllAsync()).Count();

            finalBookCount.Should().Be(initialBookCount + 1);
            finalAuthorCount.Should().Be(initialAuthorCount + 1);
            finalGenreCount.Should().Be(initialGenreCount + 1);
        }

        [Fact]
        public async Task UnitOfWork_RollbackTransaction_ShouldRevertChanges()
        {
            
            var initialBookCount = (await _unitOfWork.BookRepository.ListAllAsync()).Count();
            var initialAuthorCount = (await _unitOfWork.AuthorRepository.ListAllAsync()).Count();
            var initialGenreCount = (await _unitOfWork.GenreRepository.ListAllAsync()).Count();

            var author = new Author { Name = "Test Author" };
            var genre = new Genre { Name = "Test Genre" };
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                Description = "Test Description"
            };

            try
            {
                _unitOfWork.AuthorRepository.Add(author);
                _unitOfWork.GenreRepository.Add(genre);

                book.AuthorId = author.Id;
                book.GenreId = genre.Id;

                _unitOfWork.BookRepository.Add(book);

                throw new Exception("Simulated error to test transaction rollback");

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                
                var finalBookCount = (await _unitOfWork.BookRepository.ListAllAsync()).Count();
                finalBookCount.Should().Be(initialBookCount);

                var finalAuthorCount = (await _unitOfWork.AuthorRepository.ListAllAsync()).Count();
                finalAuthorCount.Should().Be(initialAuthorCount);

                var finalGenreCount = (await _unitOfWork.GenreRepository.ListAllAsync()).Count();
                finalGenreCount.Should().Be(initialGenreCount);
            }
        }

        private AppDbContext ProvideDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            return new AppDbContext(options);
        }
    }
}
