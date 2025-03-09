using Library.Application.BookUseCases.Commands;
using Library.Application.Common.Interfaces;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Commands
{
    public class LendBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserDataAccessor> _mockUserDataAccessor;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IRepository<BookLending>> _mockBookLendingRepository;
        private readonly Mock<ILibrarySettings> _mockLibrarySettings;
        private readonly LendBookHandler _handler;

        public LendBookHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUserDataAccessor = new Mock<IUserDataAccessor>();
            _mockBookLendingRepository = new Mock<IRepository<BookLending>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLibrarySettings = new Mock<ILibrarySettings>();

            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _mockUserDataAccessor.Setup(uda => uda.UserExist(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _mockUnitOfWork.Setup(uow => uow.BookLendingRepository)
                .Returns(_mockBookLendingRepository.Object);

            _mockLibrarySettings.Setup(s => s.DefaultLoanPeriodInDays)
                .Returns(14);


            _handler = new LendBookHandler(
                _mockUnitOfWork.Object,
                _mockLibrarySettings.Object,
                _mockUserDataAccessor.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateLendingAndUpdateBook()
        {
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new LendBookCommand(bookId, userId);

            var book = new Book
            {
                Id = bookId,
                ISBN = "978-985-6020-09-7",
                Title = "Test Book",
                Quantity = 1,
                AuthorId = Guid.NewGuid(),
                GenreId = Guid.NewGuid()
            };

            _mockBookRepository.Setup(r => r.FirstOrDefault(It.IsAny<ISpecification<Book>>(), It.IsAny<CancellationToken>()))
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
