using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Commands
{
    public class UpdateBookImageHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly UpdateBookImageHandler _handler;

        public UpdateBookImageHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _handler = new UpdateBookImageHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUpdateBookImage()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookImageCommand(bookId, "new-image-path.jpg");
            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                ImageUrl = "http://example.com/image.jpg"
            };

            _mockBookRepository.Setup(r => r.FirstOrDefault(It.IsAny<ISpecification<Book>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(command.ImageUrl, book.ImageUrl);
            _mockBookRepository.Verify(r => r.FirstOrDefault(It.IsAny<ISpecification<Book>>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync());
        }
    }
}
