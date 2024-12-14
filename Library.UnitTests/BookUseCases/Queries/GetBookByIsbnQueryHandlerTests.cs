using Library.Application.BookUseCases.Queries;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Application.Common.Exceptions;
using Moq;
using Xunit;
using System.Linq.Expressions;
using System.Threading;

namespace Library.UnitTests.BookUseCases.Queries
{
    public class GetBookByIsbnQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetBookByIsbnQueryHandler _handler;

        public GetBookByIsbnQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetBookByIsbnQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingBook_ReturnsBookDetailsDTO()
        {
            var bookId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var genreId = Guid.NewGuid();
            var isbn = "978-3-16-148410-0";

            var book = new Book 
            { 
                Id = bookId,
                ISBN = isbn,
                Title = "Test Book",
                Description = "Test Description",
                Quantity = 1,
                GenreId = genreId,
                AuthorId = authorId,
                ImageUrl = "test-image.jpg",
                Author = new Author { Id = authorId, Name = "Test Author" },
                Genre = new Genre { Id = genreId, Name = "Test Genre" }
            };

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Book, object>>>(),
                It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync(book);

            var query = new GetBookByIsbnQuery(isbn);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(isbn, result.ISBN);
            Assert.Equal(book.Description, result.Description);
            Assert.Equal(book.IsAvailable, result.IsAvailable);
            Assert.Equal(authorId, result.AuthorId);
            Assert.Equal(genreId, result.GenreId);
            Assert.Equal(book.ImageUrl, result.ImageUrl);
            Assert.Equal(book.Author.Name, result.Author.Name);
            Assert.Equal(book.Genre.Name, result.Genre.Name);
        }

        [Fact]
        public async Task Handle_NonExistingBook_ThrowsNotFoundException()
        {
            var isbn = "978-3-16-148410-0";
            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefaultAsync(
                b => b.ISBN == isbn,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Book, object>>>(),
                It.IsAny<Expression<Func<Book, object>>>()))
                .ReturnsAsync((Book)null);

            var query = new GetBookByIsbnQuery(isbn);

            await Assert.ThrowsAsync<NotFoundException>(() => 
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
