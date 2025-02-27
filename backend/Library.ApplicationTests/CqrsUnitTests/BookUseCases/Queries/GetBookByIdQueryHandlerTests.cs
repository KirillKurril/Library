using Library.Application.BookUseCases.Queries;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Application.Common.Exceptions;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Queries
{
    public class GetBookByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetBookByIdQueryHandler _handler;

        public GetBookByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetBookByIdQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingBook_ReturnsBookDetailsDTO()
        {
            var bookId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var genreId = Guid.NewGuid();

            var book = new Book
            {
                Id = bookId,
                ISBN = "978-3-16-148410-0",
                Title = "Test Book",
                Description = "Test Description",
                Quantity = 1,
                GenreId = genreId,
                AuthorId = authorId,
                ImageUrl = "test-image.jpg",
                Author = new Author { Id = authorId, Name = "Test Author" },
                Genre = new Genre { Id = genreId, Name = "Test Genre" }
            };

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var query = new GetBookByIdQuery(bookId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.ISBN, result.ISBN);
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
            var bookId = Guid.NewGuid();
            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            var query = new GetBookByIdQuery(bookId);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
