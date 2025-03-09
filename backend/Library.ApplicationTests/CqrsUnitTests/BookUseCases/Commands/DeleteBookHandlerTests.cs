using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Commands
{
    public class DeleteBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteBookHandler _handler;

        public DeleteBookHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteBookHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldDeleteBook()
        {
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Quantity = 5
            };

            _mockUnitOfWork.Setup(uow => uow.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            _mockUnitOfWork.Setup(uow => uow.BookLendingRepository.CountAsync(
                It.IsAny<ISpecification<BookLending>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            await _handler.Handle(command, CancellationToken.None);

            _mockUnitOfWork.Verify(uow => uow.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _mockUnitOfWork.Verify(uow => uow.BookRepository.Delete(book), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
