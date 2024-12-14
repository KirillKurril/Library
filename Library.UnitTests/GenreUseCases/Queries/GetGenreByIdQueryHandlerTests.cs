using Library.Application.GenreUseCases.Queries;
using Library.Application.Common.Exceptions;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.UnitTests.GenreUseCases.Queries
{
    public class GetGenreByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetGenreByIdQueryHandler _handler;

        public GetGenreByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetGenreByIdQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnGenre()
        {
            var genreId = Guid.NewGuid();
            var genre = new Genre { Id = genreId, Name = "Fiction" };
            var query = new GetGenreByIdQuery(genreId);

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.GetByIdAsync(genreId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(genre);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(genre.Id, result.Id);
            Assert.Equal(genre.Name, result.Name);
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetByIdAsync(genreId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldThrowNotFoundException()
        {
            var genreId = Guid.NewGuid();
            var query = new GetGenreByIdQuery(genreId);

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.GetByIdAsync(genreId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Genre)null);

            await Assert.ThrowsAsync<NotFoundException>(() => 
                _handler.Handle(query, CancellationToken.None));
            
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.GetByIdAsync(genreId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
