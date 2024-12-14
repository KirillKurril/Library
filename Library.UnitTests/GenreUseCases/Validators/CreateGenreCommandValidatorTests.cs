using FluentValidation.TestHelper;
using Library.Application.GenreUseCases.Commands;
using Library.Application.GenreUseCases.Validators;
using Xunit;

namespace Library.UnitTests.GenreUseCases.Validators
{
    public class CreateGenreCommandValidatorTests
    {
        private readonly CreateGenreCommandValidator _validator;

        public CreateGenreCommandValidatorTests()
        {
            _validator = new CreateGenreCommandValidator();
        }

        [Fact]
        public async Task Validate_WithValidName_ShouldNotHaveValidationError()
        {
            var command = new CreateGenreCommand("Fiction");

            var result = await _validator.TestValidateAsync(command);
            
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Validate_WithEmptyOrNullName_ShouldHaveValidationError(string name)
        {
            var command = new CreateGenreCommand(name);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Validate_WithTooLongName_ShouldHaveValidationError()
        {
            var command = new CreateGenreCommand(new string('a', 51)); 

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
    }
}
