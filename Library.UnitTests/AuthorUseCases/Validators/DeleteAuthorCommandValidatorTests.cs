using FluentValidation.TestHelper;
using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.UnitTests.AuthorUseCases.Validators
{
    public class DeleteAuthorCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly DeleteAuthorCommandValidator _validator;

        public DeleteAuthorCommandValidatorTests()
        {
            _mockAuthorRepository = new Mock<IRepository<Author>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);
            _validator = new DeleteAuthorCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_WithExistingId_ShouldNotHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new DeleteAuthorCommand(authorId);
            var author = new Author { Id = authorId, Name = "John", Surname = "Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithNonExistingId_ShouldHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new DeleteAuthorCommand(authorId);

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Author being deleted doesn't exist");
        }

        [Fact]
        public async Task Validate_WithEmptyId_ShouldHaveValidationError()
        {
            var command = new DeleteAuthorCommand(Guid.Empty);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Author ID is required");
        }
    }
}
