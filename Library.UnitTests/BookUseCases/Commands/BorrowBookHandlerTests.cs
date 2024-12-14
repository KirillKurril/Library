using Library.Application.BookUseCases.Commands;
using Library.Application.Common.Interfaces;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using FluentValidation;

namespace Library.UnitTests.BookUseCases.Commands
{
    public class BorrowBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IRepository<BookLending>> _mockBookLendingRepository;
        private readonly Mock<ILibrarySettings> _mockLibrarySettings;
        private readonly BorrowBookHandler _handler;

        public BorrowBookHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockBookLendingRepository = new Mock<IRepository<BookLending>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLibrarySettings = new Mock<ILibrarySettings>();

            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.BookLendingRepository)
                .Returns(_mockBookLendingRepository.Object);
            
            _mockLibrarySettings.Setup(s => s.DefaultLoanPeriodInDays)
                .Returns(14);

            _handler = new BorrowBookHandler(
                _mockUnitOfWork.Object,
                _mockLibrarySettings.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateLendingAndUpdateBook()
        {
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new BorrowBookCommand(bookId, userId);
            
            var book = new Book 
            { 
                Id = bookId,
                ISBN = "978-985-6020-09-7",
                Title = "Test Book",
                Quantity = 1,
                AuthorId = Guid.NewGuid(),
                GenreId = Guid.NewGuid()
            };

            _mockBookRepository.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            BookLending capturedLending = null;
            _mockBookLendingRepository.Setup(r => r.Add(It.IsAny<BookLending>()))
                .Callback<BookLending>(lending => capturedLending = lending);

            await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(capturedLending);
            Assert.Equal(bookId, capturedLending.BookId);
            Assert.Equal(userId, capturedLending.UserId);
            Assert.NotNull(capturedLending.BorrowedAt);
            Assert.NotNull(capturedLending.ReturnDate);
            Assert.Equal(0, book.Quantity);

            _mockBookLendingRepository.Verify(r => r.Add(It.IsAny<BookLending>()), Times.Once);
            _mockBookRepository.Verify(r => r.Update(book), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
