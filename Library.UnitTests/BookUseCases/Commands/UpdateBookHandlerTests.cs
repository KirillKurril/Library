using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using Xunit;

namespace Library.UnitTests.BookUseCases.Commands
{
    public class UpdateBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly UpdateBookHandler _handler;

        public UpdateBookHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _handler = new UpdateBookHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUpdateBook()
        {
            var bookId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var genreId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                "Updated Title",
                "Updated Description",
                "978-985-6020-09-7",
                5,
                authorId,
                genreId,
                "http://example.com/image.jpg"
            );

            Book capturedBook = null;
            _mockBookRepository.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockBookRepository.Setup(r => r.Update(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedBook);
            Assert.Equal(command.Title, capturedBook.Title);
            Assert.Equal(command.Description, capturedBook.Description);
            Assert.Equal(command.ISBN, capturedBook.ISBN);
            Assert.Equal(command.Quantity, capturedBook.Quantity);
            Assert.Equal(command.AuthorId, capturedBook.AuthorId);
            Assert.Equal(command.GenreId, capturedBook.GenreId);
            Assert.Equal(command.ImageUrl, capturedBook.ImageUrl);
        }
    }
}
