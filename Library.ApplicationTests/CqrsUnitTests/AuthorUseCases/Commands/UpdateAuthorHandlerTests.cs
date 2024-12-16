using Library.Application.AuthorUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using MapsterMapper;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Commands
{
    public class UpdateAuthorHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly UpdateAuthorHandler _handler;

        public UpdateAuthorHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthorRepository = new Mock<IRepository<Author>>();

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);

            _handler = new UpdateAuthorHandler(
                _mockUnitOfWork.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUpdateAuthor()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                DateTime.Now,
                "USA"
            );

            var existingAuthor = new Author
            {
                Id = authorId,
                Name = "Old Name",
                Surname = "Old Surname",
                BirthDate = DateTime.Now.AddYears(-1),
                Country = "Old Country"
            };

            var updatedAuthor = new Author
            {
                Id = authorId,
                Name = command.Name,
                Surname = command.Surname,
                BirthDate = command.BirthDate,
                Country = command.Country
            };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAuthor);

            _mockMapper.Setup(m => m.Map(command, existingAuthor))
                .Returns(updatedAuthor);

            await _handler.Handle(command, CancellationToken.None);

            _mockAuthorRepository.Verify(r => r.Update(updatedAuthor), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}
