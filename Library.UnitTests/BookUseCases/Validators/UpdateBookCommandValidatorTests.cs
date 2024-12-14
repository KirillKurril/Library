using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Library.UnitTests.BookUseCases.Validators
{
    public class UpdateBookCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateBookCommandValidator _validator;

        public UpdateBookCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _validator = new UpdateBookCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var genreId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                "978-0-7475-3269-9",
                "Updated Book",
                "Updated Description",
                5,
                genreId,
                authorId,
                "http://test.com/image.jpg"
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefaultAsync(
                b => b.ISBN == command.ISBN && b.Id != command.Id,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetByIdAsync(
                authorId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Author { Id = authorId });

            _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(
                genreId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Genre { Id = genreId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_NonExistentBook_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null, null, null, null, null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Book being updated doesn't exist");
        }

        [Theory]
        [InlineData("123456789")]
        [InlineData("978-wrong-format")]
        [InlineData("978-0-7475-3269-X")]
        public async Task Validate_InvalidISBN_ShouldHaveValidationError(string isbn)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                isbn,
                null, null, null, null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ISBN);
        }

        [Fact]
        public async Task Validate_DuplicateISBN_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var isbn = "978-0-7475-3269-9";
            var command = new UpdateBookCommand(
                bookId,
                isbn,
                null, null, null, null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { ISBN = isbn });

            var book = await _mockUnitOfWork.Object.BookRepository
                .FirstOrDefaultAsync(b => b.ISBN == command.ISBN && b.Id != command.Id);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Book with such ISBN already exists");
        }

        [Fact]
        public async Task Validate_TitleTooLong_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                new string('a', 201),
                null, null, null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Title must not exceed 200 characters");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Validate_InvalidQuantity_ShouldHaveValidationError(int quantity)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                null,
                quantity,
                null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public async Task Validate_DescriptionTooLong_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                new string('a', 2001),
                null,
                null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must not exceed 2000 characters");
        }

        [Fact]
        public async Task Validate_NonExistentAuthor_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                null,
                null,
                null,
                authorId,
                null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetByIdAsync(
                authorId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.AuthorId)
                .WithErrorMessage("Author with specified ID does not exist");
        }

        [Fact]
        public async Task Validate_NonExistentGenre_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var genreId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                null,
                null,
                genreId,
                null,
                null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(
                genreId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Genre)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.GenreId)
                .WithErrorMessage("Genre with specified ID does not exist");
        }

        [Fact]
        public async Task Validate_ImageUrlTooLong_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                null,
                null,
                null, null, 
                new string('a', 501)
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                .WithErrorMessage("Image URL must not exceed 500 characters");
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("invalid://url")]
        [InlineData("just some text")]
        public async Task Validate_InvalidImageUrl_ShouldHaveValidationError(string imageUrl)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                null,
                null,
                null,
                null,
                imageUrl
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                .WithErrorMessage("A valid URL must be provided");
        }

        [Fact]
        public async Task Validate_AllPropertiesNull_ExceptId_ShouldNotHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null, null, null, null, null, null, null
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ISBN);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
            result.ShouldNotHaveValidationErrorFor(x => x.GenreId);
            result.ShouldNotHaveValidationErrorFor(x => x.AuthorId);
            result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
        }

        [Theory]
        [InlineData("http://valid-url.com/image.jpg")]
        [InlineData("https://another-valid-url.com/image.png")]
        [InlineData("ftp://some-url.org/image.gif")]
        public async Task Validate_ValidImageUrl_ShouldNotHaveValidationError(string imageUrl)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookCommand(
                bookId,
                null,
                null,
                null,
                null,
                null,
                null,
                imageUrl
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
        }
    }
}
