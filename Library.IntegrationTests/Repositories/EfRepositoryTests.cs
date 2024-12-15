using FluentAssertions;
using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Library.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.IntegrationTests.Repositories
{
    public class EfRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;
        private readonly EfRepository<Book> _repository;

        public EfRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(_options);
            _repository = new EfRepository<Book>(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
        {
            var book = new Book { Title = "Test Book", ISBN = "1234567890123" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(book.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(book.Id);
            result.Title.Should().Be(book.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingEntity_ReturnsNull()
        {
            var nonExistingId = Guid.NewGuid();

            var result = await _repository.GetByIdAsync(nonExistingId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithIncludes_ReturnsEntityWithIncludedProperties()
        {
            var author = new Author { Name = "Test Author" };
            var book = new Book { Title = "Test Book", ISBN = "1234567890123", Author = author };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            
            var result = await _repository.GetByIdAsync(book.Id, default, b => b.Author);

            
            result.Should().NotBeNull();
            result.Author.Should().NotBeNull();
            result.Author.Name.Should().Be(author.Name);
        }

        [Fact]
        public async Task ListAllAsync_ReturnsAllEntities()
        {
            
            var books = new List<Book>
            {
                new Book { Title = "Book 1", ISBN = "1234567890123" },
                new Book { Title = "Book 2", ISBN = "1234567890124" }
            };
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();

            
            var result = await _repository.ListAllAsync();

            
            result.Should().HaveCount(2);
            result.Should().Contain(b => b.Title == "Book 1");
            result.Should().Contain(b => b.Title == "Book 2");
        }

        [Fact]
        public async Task ListAsync_WithFilter_ReturnsFilteredEntities()
        {
            
            var books = new List<Book>
            {
                new Book { Title = "Fiction Book", ISBN = "1234567890123" },
                new Book { Title = "Non-Fiction Book", ISBN = "1234567890124" }
            };
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();

            
            var result = await _repository.ListAsync(b => b.Title.Contains("Non-Fiction"));

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task ListAsync_WithFilterAndIncludes_ReturnsFilteredEntitiesWithIncludes()
        {
            
            var author = new Author { Name = "Test Author" };
            var books = new List<Book>
            {
                new Book { Title = "Fiction Book", ISBN = "1234567890123", Author = author },
                new Book { Title = "Non-Fiction Book", ISBN = "1234567890124", Author = author }
            };
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();

            
            var result = await _repository.ListAsync(
                b => b.ISBN.Contains("1234567890123"),
                default,
                b => b.Author
            );

            
            result.Should().HaveCount(1);
            result.Should().OnlyContain(b => b.Author != null);
            result.Should().OnlyContain(b => b.Author.Name == "Test Author");
        }

        [Fact]
        public void GetQueryable_ReturnsQueryable()
        {
            
            var books = new List<Book>
            {
                new Book { Title = "Book 1", ISBN = "1234567890123" },
                new Book { Title = "Book 2", ISBN = "1234567890124" }
            };
            _context.Books.AddRange(books);
            _context.SaveChanges();

            
            var query = _repository.GetQueryable();

            
            query.Should().NotBeNull();
            query.Should().BeAssignableTo<IQueryable<Book>>();
            query.Count().Should().Be(2);
        }

        [Fact]
        public void GetQueryable_WithFilterAndIncludes_ReturnsFilteredQueryableWithIncludes()
        {
            
            var author = new Author { Name = "Test Author" };
            var books = new List<Book>
            {
                new Book { Title = "Book 1", ISBN = "1234567890123", Author = author },
                new Book { Title = "Book 2", ISBN = "1234567890124", Author = author }
            };
            _context.Books.AddRange(books);
            _context.SaveChanges();

            
            var query = _repository.GetQueryable(
                b => b.Title.Contains("1"),
                b => b.Author
            );

            
            query.Should().NotBeNull();
            var result = query.ToList();
            result.Should().HaveCount(1);
            result.Should().OnlyContain(b => b.Author != null);
        }

        [Fact]
        public void Add_ValidEntity_AddsToContext()
        {
            
            var book = new Book { Title = "New Book", ISBN = "1234567890123" };

            
            var result = _repository.Add(book);

            
            result.Should().NotBeNull();
            result.Title.Should().Be(book.Title);
            _context.Entry(result).State.Should().Be(EntityState.Added);
        }

        [Fact]
        public void Add_NullEntity_ThrowsArgumentNullException()
        {
            var action = () => _repository.Add(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Update_ValidEntity_UpdatesInContext()
        {
            
            var book = new Book { Title = "Original Title", ISBN = "1234567890123" };
            _context.Books.Add(book);
            _context.SaveChanges();

            book.Title = "Updated Title";

            
            _repository.Update(book);

            
            _context.Entry(book).State.Should().Be(EntityState.Modified);
        }

        [Fact]
        public void Update_NullEntity_ThrowsArgumentNullException()
        {
            var action = () => _repository.Update(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Delete_ValidEntity_RemovesFromContext()
        {
            
            var book = new Book { Title = "Book to Delete", ISBN = "1234567890123" };
            _context.Books.Add(book);
            _context.SaveChanges();

            
            _repository.Delete(book);

            
            _context.Entry(book).State.Should().Be(EntityState.Deleted);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ExistingEntity_ReturnsEntity()
        {
            
            var book = new Book { Title = "Test Book", ISBN = "1234567890123" };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            
            var result = await _repository.FirstOrDefaultAsync(b => b.Title == "Test Book");

            
            result.Should().NotBeNull();
            result.Title.Should().Be(book.Title);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NonExistingEntity_ReturnsNull()
        {
            
            var result = await _repository.FirstOrDefaultAsync(b => b.Title == "Non-existing Book");

            
            result.Should().BeNull();
        }

        [Fact]
        public async Task FirstOrDefaultAsync_WithIncludes_ReturnsEntityWithIncludedProperties()
        {
            
            var author = new Author { Name = "Test Author" };
            var book = new Book { Title = "Test Book", ISBN = "1234567890123", Author = author };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            
            var result = await _repository.FirstOrDefaultAsync(
                b => b.Title == "Test Book",
                default,
                b => b.Author
            );

            
            result.Should().NotBeNull();
            result.Author.Should().NotBeNull();
            result.Author.Name.Should().Be(author.Name);
        }
    }
}
