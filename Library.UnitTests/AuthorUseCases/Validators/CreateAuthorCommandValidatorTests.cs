using FluentValidation.TestHelper;
using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Validators;
using Xunit;

namespace Library.UnitTests.AuthorUseCases.Validators
{
    public class CreateAuthorCommandValidatorTests
    {
        private readonly CreateAuthorCommandValidator _validator;

        public CreateAuthorCommandValidatorTests()
        {
            _validator = new CreateAuthorCommandValidator();
        }

        [Fact]
        public async Task Validate_WithValidData_ShouldNotHaveValidationError()
        {
            var command = new CreateAuthorCommand(
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Validate_WithEmptyName_ShouldHaveValidationError(string name)
        {
            var command = new CreateAuthorCommand(
                name,
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Validate_WithNullSurname_ShouldNotHaveValidationError()
        {
            var command = new CreateAuthorCommand(
                "John",
                null,
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Surname);
        }

        [Fact]
        public async Task Validate_WithTooLongSurname_WhenSurnameProvided_ShouldHaveValidationError()
        {
            var command = new CreateAuthorCommand(
                "John",
                new string('a', 101),
                DateTime.UtcNow.AddYears(-30),
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Surname);
        }

        [Fact]
        public async Task Validate_WithNullBirthDate_ShouldNotHaveValidationError()
        {
            var command = new CreateAuthorCommand(
                "John",
                "Doe",
                null,
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
        }

        [Fact]
        public async Task Validate_WithFutureBirthDate_WhenBirthDateProvided_ShouldHaveValidationError()
        {
            var command = new CreateAuthorCommand(
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(1),
                "USA"
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BirthDate);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Validate_WithEmptyCountry_ShouldHaveValidationError(string country)
        {
            var command = new CreateAuthorCommand(
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                country
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Country);
        }

        [Fact]
        public async Task Validate_WithTooLongCountry_WhenCountryProvided_ShouldHaveValidationError()
        {
            var command = new CreateAuthorCommand(
                "John",
                "Doe",
                DateTime.UtcNow.AddYears(-30),
                new string('a', 101)
            );

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Country);
        }
    }
}
