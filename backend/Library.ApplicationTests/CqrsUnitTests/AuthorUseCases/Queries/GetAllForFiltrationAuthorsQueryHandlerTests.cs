using Library.Application.AuthorUseCases.Queries;
using Library.Application.DTOs;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Queries
{
    public class GetAllForFiltrationAuthorsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly GetAllForFiltrationAuthorsQueryHandler _handler;

        public GetAllForFiltrationAuthorsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthorRepository = new Mock<IRepository<Author>>();

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);

            _handler = new GetAllForFiltrationAuthorsQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllAuthorsAsDTOs()
        {
            var authors = new List<Author>
            {
                new Author { Id = Guid.NewGuid(), Name = "John", Surname = "Doe" },
                new Author { Id = Guid.NewGuid(), Name = "Jane", Surname = "Smith" }
            };

            var expectedDtos = authors.Select(a => new AuthorBriefDTO
            {
                Id = a.Id,
                Name = $"{a.Name} {a.Surname}"
            });

            _mockAuthorRepository.Setup(r => r.GetAsync(It.IsAny<ISpecification<Author>>() ,It.IsAny<CancellationToken>()))
                .ReturnsAsync(authors);

            var result = await _handler.Handle(new GetAllForFiltrationAuthorsQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(authors.Count, result.Count());
            _mockAuthorRepository.Verify(r => r.GetAsync(It.IsAny<ISpecification<Author>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoAuthors_ShouldReturnEmptyList()
        {
            var emptyList = new List<Author>();

            _mockAuthorRepository.Setup(r => r.GetAsync(It.IsAny<ISpecification<Author>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            var result = await _handler.Handle(new GetAllForFiltrationAuthorsQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockAuthorRepository.Verify(r => r.GetAsync(It.IsAny<ISpecification<Author>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
