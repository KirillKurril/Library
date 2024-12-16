using FluentValidation.TestHelper;
using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Validators
{
    public class UpdateAuthorCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly UpdateAuthorCommandValidator _validator;

        public UpdateAuthorCommandValidatorTests()
        {
            _mockAuthorRepository = new Mock<IRepository<Author>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);
            _validator = new UpdateAuthorCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_WithValidData_ShouldNotHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithNonExistingId_ShouldHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Author being updated doesn't exist");
        }

        [Fact]
        public async Task Validate_WithEmptyId_ShouldHaveValidationError()
        {
            var command = new UpdateAuthorCommand(
                Guid.Empty,
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Author ID is required");
        }

        [Fact]
        public async Task Validate_WithNullName_ShouldNotHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                null,
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Validate_WithTooLongName_WhenNameProvided_ShouldHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                new string('a', 101),
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Validate_WithNullBirthDate_ShouldNotHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                null,
                "USA"
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
        }

        [Fact]
        public async Task Validate_WithFutureBirthDate_WhenBirthDateProvided_ShouldHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(1),
                "USA"
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BirthDate);
        }

        [Fact]
        public async Task Validate_WithNullCountry_ShouldNotHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                null
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Country);
        }

        [Fact]
        public async Task Validate_WithTooLongCountry_WhenCountryProvided_ShouldHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new UpdateAuthorCommand(
                authorId,
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                new string('a', 101)
            );
            var author = new Author { Id = authorId, Name = "Old John", Surname = "Old Doe" };

            _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Country);
        }
    }
}
