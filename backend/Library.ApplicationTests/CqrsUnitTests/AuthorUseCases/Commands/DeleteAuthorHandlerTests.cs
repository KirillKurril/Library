using Library.Application.AuthorUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Commands
{
    public class DeleteAuthorHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly DeleteAuthorHandler _handler;

        public DeleteAuthorHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthorRepository = new Mock<IRepository<Author>>();

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);

            _handler = new DeleteAuthorHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ShouldDeleteAuthorAndReturnIt()
        {
            var authorId = Guid.NewGuid();
            var author = new Author
            {
                Id = authorId,
                Name = "John",
                Surname = "Doe"
            };
            var command = new DeleteAuthorCommand(authorId);

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            await _handler.Handle(command, CancellationToken.None);

            _mockAuthorRepository.Verify(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
            _mockAuthorRepository.Verify(r => r.Delete(author), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
