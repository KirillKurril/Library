using Library.Application.GenreUseCases.Queries;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using Xunit;

namespace Library.UnitTests.GenreUseCases.Queries
{
    public class GetAllGenresQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetAllGenresQueryHandler _handler;

        public GetAllGenresQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetAllGenresQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllGenres()
        {
            var genres = new List<Genre>
            {
                new Genre { Id = Guid.NewGuid(), Name = "Fiction" },
                new Genre { Id = Guid.NewGuid(), Name = "Non-Fiction" }
            };

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(genres);

            var result = await _handler.Handle(new GetAllGenresQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(genres.Count, result.Count());
            Assert.Equal(genres, result);
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.ListAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoGenres_ShouldReturnEmptyList()
        {
            var emptyList = new List<Genre>();

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            var result = await _handler.Handle(new GetAllGenresQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.ListAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
