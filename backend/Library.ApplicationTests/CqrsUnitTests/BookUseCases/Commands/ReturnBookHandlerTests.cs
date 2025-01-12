using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Commands
{
    public class ReturnBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IRepository<BookLending>> _mockBookLendingRepository;
        private readonly ReturnBookHandler _handler;

        public ReturnBookHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockBookLendingRepository = new Mock<IRepository<BookLending>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.BookLendingRepository)
                .Returns(_mockBookLendingRepository.Object);

            _handler = new ReturnBookHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidReturn_ShouldUpdateBookQuantityAndDeleteLending()
        {
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new ReturnBookCommand(bookId, userId);

            var book = new Book
            {
                Id = bookId,
                ISBN = "978-985-6020-09-7",
                Title = "Test Book",
                Quantity = 0,
                AuthorId = Guid.NewGuid(),
                GenreId = Guid.NewGuid()
            };

            var lending = new BookLending
            {
                BookId = bookId,
                UserId = userId,
                BorrowedAt = DateTime.UtcNow.AddDays(-7),
                ReturnDate = DateTime.UtcNow.AddDays(7)
            };

            _mockBookLendingRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(lending);


            _mockBookRepository.Setup(r => r.GetByIdAsync(bookId, CancellationToken.None))
                .ReturnsAsync(book);

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(1, book.Quantity);
            _mockBookLendingRepository.Verify(r => r.Delete(lending), Times.Once);
            _mockBookRepository.Verify(r => r.Update(book), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
