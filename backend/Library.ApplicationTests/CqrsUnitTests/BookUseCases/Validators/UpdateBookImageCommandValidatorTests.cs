using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Validators
{
    public class UpdateBookImageCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateBookImageCommandValidator _validator;

        public UpdateBookImageCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _validator = new UpdateBookImageCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookImageCommand(
                bookId,
                "http://valid-url.com/image.jpg"
            );

            _mockUnitOfWork.Setup(x => x.BookRepository.CountAsync(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Validate_EmptyImageUrl_ShouldHaveValidationError(string imageUrl)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookImageCommand(bookId, imageUrl);

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                .WithErrorMessage("Image url is required");
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("invalid://url")]
        [InlineData("just some text")]
        public async Task Validate_InvalidImageUrl_ShouldHaveValidationError(string imageUrl)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookImageCommand(bookId, imageUrl);

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ImageUrl)
                .WithErrorMessage("A valid URL must be provided");
        }

        [Theory]
        [InlineData("http://valid-url.com/image.jpg")]
        [InlineData("https://another-valid-url.com/image.png")]
        public async Task Validate_ValidImageUrl_ShouldNotHaveValidationError(string imageUrl)
        {
            var bookId = Guid.NewGuid();
            var command = new UpdateBookImageCommand(bookId, imageUrl);

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ImageUrl);
        }
    }
}
