using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using MapsterMapper;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Commands
{
    public class UpdateBookHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateBookHandler _handler;

        public UpdateBookHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdateBookHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object);
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

            _mockUnitOfWork.Setup(uow => uow.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(uow => uow.BookRepository.CountAsync(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository.CountAsync(
                It.IsAny<ISpecification<Author>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.CountAsync(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);


            _mockUnitOfWork.Setup(uow => uow.BookRepository.Update(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book);

            await _handler.Handle(command, CancellationToken.None);
        }
    }
}
