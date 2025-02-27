using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Validators
{
    public class CreateBookCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateBookCommandValidator _validator;

        public CreateBookCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(x => x.BookRepository.CountAsync(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.GenreRepository.CountAsync(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.AuthorRepository.CountAsync(
                It.IsAny<ISpecification<Author>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _validator = new CreateBookCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            _mockUnitOfWork.Setup(x => x.BookRepository.CountAsync(
                It.IsAny<BookByIsbnSpecification>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var command = new CreateBookCommand(
                "978-0-7478-3269-9",
                "Test Book",
                "Test Description",
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-isbn")]
        [InlineData("978-0-7475-3269-X")] 
        public async Task Validate_InvalidISBN_ShouldHaveValidationError(string isbn)
        {
            var command = new CreateBookCommand(
                isbn,
                "Test Book",
                "Test Description",
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN);
        }

        [Fact]
        public async Task Validate_DuplicateISBN_ShouldHaveValidationError()
        {
            var existingIsbn = "978-0-7475-3269-9";
            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { ISBN = existingIsbn });

            var command = new CreateBookCommand(
                existingIsbn,
                "Test Book",
                "Test Description",
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN)
                .WithErrorMessage("A book with this ISBN already exists");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        public async Task Validate_InvalidTitle_ShouldHaveValidationError(string title)
        {
            var command = new CreateBookCommand(
                "978-0-7475-3269-9",
                title,
                "Test Description",
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Validate_InvalidQuantity_ShouldHaveValidationError(int quantity)
        {
            var command = new CreateBookCommand(
                "978-0-7475-3269-9",
                "Test Book",
                "Test Description",
                quantity,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public async Task Validate_DescriptionTooLong_ShouldHaveValidationError()
        {
            var longDescription = new string('*', 2001);
            var command = new CreateBookCommand(
                "978-0-7475-3269-9",
                "Test Book",
                longDescription,
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must not exceed 2000 characters");
        }

        [Fact]
        public async Task Validate_NonExistentGenre_ShouldHaveValidationError()
        {
            _mockUnitOfWork.Setup(x => x.GenreRepository.CountAsync(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var command = new CreateBookCommand(
                "978-0-7475-3269-9",
                "Test Book",
                "Test Description",
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.GenreId)
                .WithErrorMessage("Genre with specified ID does not exist");
        }

        [Fact]
        public async Task Validate_NonExistentAuthor_ShouldHaveValidationError()
        {
            _mockUnitOfWork.Setup(x => x.AuthorRepository.CountAsync(
                It.IsAny<ISpecification<Author>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var command = new CreateBookCommand(
                "978-0-7475-3269-9",
                "Test Book",
                "Test Description",
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.AuthorId)
                .WithErrorMessage("Author with specified ID does not exist");
        }

        [Fact]
        public async Task Validate_MultipleValidationErrors_ShouldReturnAllErrors()
        {
            var command = new CreateBookCommand(
                "",
                "",
                new string('*', 2001),
                0,
                Guid.Empty,
                Guid.Empty); 

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ISBN);
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
            result.ShouldHaveValidationErrorFor(x => x.GenreId);
            result.ShouldHaveValidationErrorFor(x => x.AuthorId);
        }

        [Fact]
        public async Task Validate_NullDescription_ShouldNotHaveValidationError()
        {
            var command = new CreateBookCommand(
                "978-0-7475-3269-9",
                "Test Book",
                null,
                10,
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

    }
}
