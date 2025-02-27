using FluentValidation.TestHelper;
using Library.Application.GenreUseCases.Commands;
using Library.Application.GenreUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace Library.ApplicationTests.CqrsUnitTests.GenreUseCases.Validators
{
    public class CreateGenreCommandValidatorTests
    {
        private readonly CreateGenreCommandValidator _validator;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public CreateGenreCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _validator = new CreateGenreCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_WithValidName_ShouldNotHaveValidationError()
        {
            var command = new CreateGenreCommand("Fiction");
            _mockUnitOfWork.Setup(x => x.GenreRepository.FirstOrDefault(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Genre)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Validate_WithEmptyOrNullName_ShouldHaveValidationError(string name)
        {
            var command = new CreateGenreCommand(name);
            _mockUnitOfWork.Setup(x => x.GenreRepository.FirstOrDefault(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Genre)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Genre name is required");
        }

        [Fact]
        public async Task Validate_WithTooLongName_ShouldHaveValidationError()
        {
            var command = new CreateGenreCommand(new string('a', 101));
            _mockUnitOfWork.Setup(x => x.GenreRepository.FirstOrDefault(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Genre)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Genre name must not exceed 100 characters");
        }

        [Fact]
        public async Task Validate_WithDuplicateName_ShouldHaveValidationError()
        {
            var existingGenre = new Genre { Id = Guid.NewGuid(), Name = "Fiction" };
            var command = new CreateGenreCommand("Fiction");

            _mockUnitOfWork.Setup(x => x.GenreRepository.CountAsync(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("A genre with this name already exists");
        }
    }
}
