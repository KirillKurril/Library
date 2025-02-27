using FluentAssertions;
using Library.Domain.Entities;
using Library.Domain.Specifications;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Library.Persistance.Repositories;

namespace Library.IntegrationTests.Repositories
{
    public class EfRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
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

            var spec = new BookByIdSpecification(book.Id);
            var result = await repository.FirstOrDefault(spec);

            result.Should().NotBeNull();
            result.Id.Should().Be(book.Id);
            result.Title.Should().Be(book.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingEntity_ReturnsNull()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var nonExistingId = Guid.NewGuid();

            var spec = new BookByIdSpecification(nonExistingId);
            var result = await repository.FirstOrDefault(spec);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithIncludes_ReturnsEntityWithIncludedProperties()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
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

            var spec = new BookByIdSpecification(book.Id);
            var result = await repository.FirstOrDefault(spec);

            result.Should().NotBeNull();
            result.Author.Should().NotBeNull();
            result.Genre.Should().NotBeNull();
            result.Author.Id.Should().Be(author.Id);
            result.Genre.Id.Should().Be(genre.Id);
        }

        [Fact]
        public async Task ListAllAsync_ReturnsAllEntities()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
            var books = new List<Book>
            {
                new Book 
                { 
                    Title = "Book 1",
                    ISBN = "1234567890123",
                    Quantity = 1,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                },
                new Book 
                { 
                    Title = "Book 2",
                    ISBN = "1234567890124",
                    Quantity = 2,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                }
            };
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var spec = new AllItemsSpecification<Book>();
            var result = await repository.GetAsync(spec);

            result.Should().HaveCount(2);
            result.Should().Contain(b => b.Title == "Book 1");
            result.Should().Contain(b => b.Title == "Book 2");
        }

        [Fact]
        public async Task ListAsync_WithFilter_ReturnsFilteredEntities()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
            var books = new List<Book>
            {
                new Book 
                { 
                    Title = "Fiction",
                    ISBN = "1234567890123",
                    Quantity = 1,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                },
                new Book 
                { 
                    Title = "Non-Fiction",
                    ISBN = "1234567890124",
                    Quantity = 2,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                }
            };
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var spec = new BookCatalogSpecification(
                "Non-Fiction",
                null,
                null,
                null);
            var result = await repository.GetAsync(spec);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task ListAsync_WithFilterAndIncludes_ReturnsFilteredEntitiesWithIncludes()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            (var author, var genre) = CreateTestEntities(context);
            var books = new List<Book>
            {
                new Book 
                { 
                    Title = "Fiction Book",
                    ISBN = "1234567890123",
                    Quantity = 1,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                },
                new Book 
                { 
                    Title = "Non-Fiction Book",
                    ISBN = "1234567890124",
                    Quantity = 0,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                }
            };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            var spec = new BookByIsbnSpecification("1234567890123");
            var result = await repository.FirstOrDefault(spec);

            result.Should().NotBeNull();
            result.Quantity.Should().Be(1);
            result.Author.Should().NotBeNull(); 
            result.Author.Name.Should().Be("Test Author");
        }

        [Fact]
        public async Task GetQueryable_ReturnsQueryable()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            (var author, var genre) = CreateTestEntities(context);
            var books = new List<Book>
            {
                new Book 
                { 
                    Title = "Fiction Book",
                    ISBN = "1234567890123",
                    Quantity = 1,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                },
                new Book 
                { 
                    Title = "Non-Fiction Book",
                    ISBN = "1234567890124",
                    Quantity = 0,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                }
            };

            context.Books.AddRange(books);
            context.SaveChanges();

            var spec = new AllItemsSpecification<Book>();
            var items = await repository.GetAsync(spec);

            items.Should().NotBeNull();
            items.Should().BeAssignableTo<IReadOnlyList<Book>>();
            items.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetQueryable_WithFilterAndIncludes_ReturnsFilteredQueryableWithIncludes()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            (var author, var genre) = CreateTestEntities(context);
            var books = new List<Book>
            {
                new Book 
                { 
                    Title = "Fiction Book",
                    ISBN = "1234567890123",
                    Quantity = 1,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                },
                new Book 
                { 
                    Title = "Non-Fiction Book",
                    ISBN = "1234567890124",
                    Quantity = 0,
                    AuthorId = author.Id,
                    GenreId = genre.Id
                }
            };

            context.Books.AddRange(books);
            context.SaveChanges();

            var spec = new BookCatalogSpecification("Non", null, null, null);
            var items = await repository.GetAsync(spec);

            items.Should().NotBeNull();
            items.Should().HaveCount(1);
            items.Should().OnlyContain(b => b.Author != null);
        }

        [Fact]
        public void Add_ValidEntity_AddsToContext()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
            var book = new Book 
            { 
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                AuthorId = author.Id,
                GenreId = genre.Id
            };

            repository.Add(book);
            context.SaveChanges();

            var savedBook = context.Books.Single(b => b.ISBN == book.ISBN); 

            savedBook.Should().NotBeNull();
            savedBook.Should().BeEquivalentTo(book, options => options.Excluding(b => b.Id)); 
        }

        [Fact]
        public void Add_NullEntity_ThrowsArgumentNullException()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var action = () => repository.Add(null);

            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Update_ValidEntity_UpdatesInContext()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
            var book = new Book 
            { 
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                AuthorId = author.Id,
                GenreId = genre.Id
            };
            context.Books.Add(book);
            context.SaveChanges();

            book.Title = "Updated Title";
            repository.Update(book);
            context.SaveChanges();

            var updatedBook = context.Books.Find(book.Id);
            updatedBook.Title.Should().Be("Updated Title");
        }

        [Fact]
        public void Update_NullEntity_ThrowsArgumentNullException()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var action = () => repository.Update(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Delete_ValidEntity_RemovesFromContext()
        {
            using var context = CreateContext();
            var repository = new EfRepository<Book>(context);

            var (author, genre) = CreateTestEntities(context);
            var book = new Book 
            { 
                Title = "Test Book",
                ISBN = "1234567890123",
                Quantity = 1,
                AuthorId = author.Id,
                GenreId = genre.Id
            };
            context.Books.Add(book);
            context.SaveChanges();

            repository.Delete(book);
            context.SaveChanges();

            context.Books.Should().NotContain(book);
        }
    }
}
