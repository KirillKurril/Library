using FluentAssertions;
using Library.Domain.Entities;
using Library.Domain.Specifications;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Library.Domain.Specifications.GenreSpecification;
using Library.Persistance.Contexts;
using Library.Persistance.Repositories;

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


            var authorSpec = new AuthorByIdSpecification(author.Id);
            var bookSpec = new BookByIdSpecification(book.Id);
            var genreSpec = new GenreByIdSpecification(genre.Id);

            var savedAuthor = await _unitOfWork.AuthorRepository.FirstOrDefault(authorSpec);
            var savedBook = await _unitOfWork.BookRepository.FirstOrDefault(bookSpec);
            var savedGenre = await _unitOfWork.GenreRepository.FirstOrDefault(genreSpec);

            savedBook.Should().NotBeNull();
            savedAuthor.Should().NotBeNull();
            savedGenre.Should().NotBeNull();
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
                var authorListSpec = new AllItemsSpecification<Author>();
                var bookListSpec = new AllItemsSpecification<Book>();

                var authors = await _unitOfWork.AuthorRepository.GetAsync(authorListSpec);
                var books = await _unitOfWork.BookRepository.GetAsync(bookListSpec);

                authors.Should().BeEmpty();
                books.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task UnitOfWork_SaveChanges_ShouldPersistAllChangesTransactionally()
        {
            var authorListSpec = new AllItemsSpecification<Author>();
            var bookListSpec = new AllItemsSpecification<Book>();
            var genreListSpec = new AllItemsSpecification<Genre>();

            var initialBookCount = (await _unitOfWork.BookRepository.GetAsync(bookListSpec)).Count();
            var initialAuthorCount = (await _unitOfWork.AuthorRepository.GetAsync(authorListSpec)).Count();
            var initialGenreCount = (await _unitOfWork.GenreRepository.GetAsync(genreListSpec)).Count();

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

            var authorByIdSpec = new AuthorByIdSpecification(author.Id);
            var bookByIdSpec = new BookByIdSpecification(book.Id);
            var genreByIdSpec = new GenreByIdSpecification(genre.Id);

            var savedAuthor = await _unitOfWork.AuthorRepository.FirstOrDefault(authorByIdSpec);
            var savedGenre = await _unitOfWork.GenreRepository.FirstOrDefault(genreByIdSpec);
            var savedBook = await _unitOfWork.BookRepository.FirstOrDefault(bookByIdSpec);

            savedAuthor.Should().NotBeNull();
            savedAuthor.Name.Should().Be("Test Author");

            savedGenre.Should().NotBeNull();
            savedGenre.Name.Should().Be("Test Genre");

            savedBook.Should().NotBeNull();
            savedBook.Title.Should().Be("Test Book");
            savedBook.AuthorId.Should().Be(author.Id);
            savedBook.GenreId.Should().Be(genre.Id);

            var finalBookCount = (await _unitOfWork.BookRepository.GetAsync(bookListSpec)).Count();
            var finalAuthorCount = (await _unitOfWork.AuthorRepository.GetAsync(authorListSpec)).Count();
            var finalGenreCount = (await _unitOfWork.GenreRepository.GetAsync(genreListSpec)).Count();

            finalBookCount.Should().Be(initialBookCount + 1);
            finalAuthorCount.Should().Be(initialAuthorCount + 1);
            finalGenreCount.Should().Be(initialGenreCount + 1);
        }

        [Fact]
        public async Task UnitOfWork_RollbackTransaction_ShouldRevertChanges()
        {
            var authorListSpec = new AllItemsSpecification<Author>();
            var bookListSpec = new AllItemsSpecification<Book>();
            var genreListSpec = new AllItemsSpecification<Genre>();

            var initialBookCount = (await _unitOfWork.BookRepository.GetAsync(bookListSpec)).Count();
            var initialAuthorCount = (await _unitOfWork.AuthorRepository.GetAsync(authorListSpec)).Count();
            var initialGenreCount = (await _unitOfWork.GenreRepository.GetAsync(genreListSpec)).Count();

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
                
                var finalBookCount = (await _unitOfWork.BookRepository.GetAsync(bookListSpec)).Count();
                var finalAuthorCount = (await _unitOfWork.AuthorRepository.GetAsync(authorListSpec)).Count();
                var finalGenreCount = (await _unitOfWork.GenreRepository.GetAsync(genreListSpec)).Count();

                finalBookCount.Should().Be(initialBookCount);
                finalAuthorCount.Should().Be(initialAuthorCount);
                finalGenreCount.Should().Be(initialGenreCount);
            }
        }
    }
}
