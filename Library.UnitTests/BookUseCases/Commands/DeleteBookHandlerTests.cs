using Library.Application.BookUseCases.Commands;
using Library.Application.Common.Exceptions;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using Xunit;

namespace Library.UnitTests.BookUseCases.Commands
{
    public class DeleteBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly DeleteBookHandler _handler;

        public DeleteBookHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _handler = new DeleteBookHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldDeleteBook()
        {
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);
            var book = new Book {
                Id = bookId,
                Title = "Test Book",
                Quantity = 5
            };

            _mockBookRepository.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            await _handler.Handle(command, CancellationToken.None);

            _mockBookRepository.Verify(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
            _mockBookRepository.Verify(r => r.Delete(book), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
