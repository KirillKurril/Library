using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace Library.UnitTests.BookUseCases.Validators
{
    public class CreateBookCommandValidatorTests
    {
        private readonly CreateBookCommandValidator _validator;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Book>> _mockBookRepository;

        public CreateBookCommandValidatorTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);

            _validator = new CreateBookCommandValidator(_mockUnitOfWork.Object);
        }

        [Theory]
        [InlineData("978-985-6020-09-7")]  // Valid ISBN-13
        [InlineData("0-306-40615-2")]      // Valid ISBN-10
        [InlineData("0-306-40615-X")]      // Valid ISBN-10 with X
        [InlineData("ISBN-13: 978-3-16-148410-0")] // Valid ISBN-13 with prefix
        public async Task ISBN_ValidFormat_ShouldNotHaveValidationError(string isbn)
        {
            var command = new CreateBookCommand(
                ISBN: isbn,
                Title: "Test Book",
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.ISBN);
        }

        [Theory]
        [InlineData("")] // Empty
        [InlineData("123")] // Too short
        [InlineData("123456789012345678")] // Too long (>17)
        [InlineData("abc-def-ghij-k")] // Contains letters
        [InlineData("123-456-789")] // Incomplete format
        [InlineData("123.456.789.0")] // Wrong separators
        [InlineData("978-985-6020-09-Y")] // Invalid last character (only X allowed)
        public async Task ISBN_InvalidFormat_ShouldHaveValidationError(string isbn)
        {
            var command = new CreateBookCommand(
                ISBN: isbn,
                Title: "Test Book",
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN);
        }

        [Fact]
        public async Task ISBN_DuplicateISBN_ShouldHaveValidationError()
        {
            var existingIsbn = "978-985-6020-09-7";
            var command = new CreateBookCommand(
                ISBN: existingIsbn,
                Title: "Test Book",
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            _mockBookRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { ISBN = existingIsbn });

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN)
                .WithErrorMessage("A book with this ISBN already exists");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Title_EmptyOrNull_ShouldHaveValidationError(string title)
        {
            var command = new CreateBookCommand(
                ISBN: "978-985-6020-09-7",
                Title: title,
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public async Task Title_TooLong_ShouldHaveValidationError()
        {
            var command = new CreateBookCommand(
                ISBN: "978-985-6020-09-7",
                Title: new string('a', 201), // 201 characters
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Quantity_ZeroOrNegative_ShouldHaveValidationError(int quantity)
        {
            var command = new CreateBookCommand(
                ISBN: "978-985-6020-09-7",
                Title: "Test Book",
                Description: "Test Description",
                Quantity: quantity,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public async Task Description_TooLong_ShouldHaveValidationError()
        {
            var command = new CreateBookCommand(
                ISBN: "978-985-6020-09-7",
                Title: "Test Book",
                Description: new string('a', 2001), // 2001 characters
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public async Task AuthorId_NonExistent_ShouldHaveValidationError()
        {
            var command = new CreateBookCommand(
                ISBN: "978-985-6020-09-7",
                Title: "Test Book",
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid());

            _mockUnitOfWork.Setup(uow => uow.AuthorRepository.GetByIdAsync(command.AuthorId, CancellationToken.None))
                .ReturnsAsync((Author)null);

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.AuthorId)
                .WithErrorMessage("Author with specified ID does not exist");
        }

        [Fact]
        public async Task ImageUrl_TooLong_ShouldHaveValidationError()
        {
            var command = new CreateBookCommand(
                ISBN: "978-985-6020-09-7",
                Title: "Test Book",
                Description: "Test Description",
                Quantity: 5,
                GenreId: Guid.NewGuid(),
                AuthorId: Guid.NewGuid())
            {
                ImageUrl = new string('a', 501) // 501 characters
            };

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ImageUrl);
        }
    }
}
