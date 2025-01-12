using Library.Application.AuthorUseCases.Queries;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Queries
{
    public class GetAllAuthorsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly GetAllAuthorsQueryHandler _handler;

        public GetAllAuthorsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthorRepository = new Mock<IRepository<Author>>();

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);

            _handler = new GetAllAuthorsQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllAuthors()
        {
            var authors = new List<Author>
            {
                new Author { Id = Guid.NewGuid(), Name = "John", Surname = "Doe" },
                new Author { Id = Guid.NewGuid(), Name = "Jane", Surname = "Smith" }
            };

            _mockAuthorRepository.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(authors);

            var result = await _handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(authors.Count, result.Count());
            Assert.Equal(authors, result);
            _mockAuthorRepository.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoAuthors_ShouldReturnEmptyList()
        {
            var emptyList = new List<Author>();

            _mockAuthorRepository.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            var result = await _handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockAuthorRepository.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
