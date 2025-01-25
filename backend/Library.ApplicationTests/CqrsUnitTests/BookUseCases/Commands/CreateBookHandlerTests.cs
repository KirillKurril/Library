using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Mapster;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Commands
{
    public class CreateBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly CreateBookHandler _handler;

        public CreateBookHandlerTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);

            _handler = new CreateBookHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateBook()
        {
            var command = new CreateBookCommand(
                ISBN: "978-3-16-148410-0",
                Title: "Test Book",
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var expectedBook = command.Adapt<Book>();
            _mockBookRepository.Setup(r => r.Add(It.IsAny<Book>()))
                .Returns(expectedBook);

            var result = await _handler.Handle(command, CancellationToken.None);

            _mockBookRepository.Verify(r => r.Add(It.Is<Book>(b =>
                b.ISBN == command.ISBN &&
                b.Title == command.Title &&
                b.Description == command.Description &&
                b.Quantity == command.Quantity &&
                b.GenreId == command.GenreId &&
                b.AuthorId == command.AuthorId)), Times.Once);

            Assert.Equal(expectedBook.Id, result.Id);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
