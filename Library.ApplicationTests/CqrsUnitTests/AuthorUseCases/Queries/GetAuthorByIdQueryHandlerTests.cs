using Library.Application.AuthorUseCases.Queries;
using Library.Application.Common.Exceptions;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Queries
{
    public class GetAuthorByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly GetAuthorByIdQueryHandler _handler;

        public GetAuthorByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthorRepository = new Mock<IRepository<Author>>();

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);

            _handler = new GetAuthorByIdQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnAuthor()
        {
            var authorId = Guid.NewGuid();
            var author = new Author
            {
                Id = authorId,
                Name = "John",
                Surname = "Doe"
            };
            var query = new GetAuthorByIdQuery(authorId);

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(authorId, result.Id);
            Assert.Equal(author.Name, result.Name);
            Assert.Equal(author.Surname, result.Surname);
            _mockAuthorRepository.Verify(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldThrowNotFoundException()
        {
            var authorId = Guid.NewGuid();
            var query = new GetAuthorByIdQuery(authorId);

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));

            _mockAuthorRepository.Verify(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
