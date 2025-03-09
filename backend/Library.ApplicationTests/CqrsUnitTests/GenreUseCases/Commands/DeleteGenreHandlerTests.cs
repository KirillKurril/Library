using Library.Application.GenreUseCases.Commands;
using Library.Domain.Entities;
using Library.Domain.Abstractions;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.GenreUseCases.Commands
{
    public class DeleteGenreHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteGenreHandler _handler;

        public DeleteGenreHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteGenreHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldDeleteGenre()
        {
            var genreId = Guid.NewGuid();
            var command = new DeleteGenreCommand(genreId);
            var genre = new Genre { Id = genreId, Name = "Fiction" };

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.FirstOrDefault(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(genre);

            _mockUnitOfWork.Setup(uow => uow.BookRepository.CountAsync(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _mockUnitOfWork.Verify(uow => uow.GenreRepository.Delete(genre), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
