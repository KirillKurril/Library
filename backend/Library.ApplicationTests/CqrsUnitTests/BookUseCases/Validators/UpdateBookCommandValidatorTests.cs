using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Validators
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

            _mockUnitOfWork.Setup(x => x.BookRepository.CountAsync(
                It.IsAny<BookByIdSpecification>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.BookRepository.CountAsync(
                It.IsAny<UniqueIsbnCheckSpecification>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            _mockUnitOfWork.Setup(x => x.AuthorRepository.CountAsync(
                It.IsAny<ISpecification<Author>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.GenreRepository.CountAsync(
                It.IsAny<ISpecification<Genre>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ISBN);
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must not exceed 2000 characters");
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
        }
    }
}
