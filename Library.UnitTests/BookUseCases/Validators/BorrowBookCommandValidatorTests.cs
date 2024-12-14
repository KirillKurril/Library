using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;


namespace Library.UnitTests.BookUseCases.Validators
{
    public class BorrowBookCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly BorrowBookCommandValidator _validator;

        public BorrowBookCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _validator = new BorrowBookCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task BookId_ShouldHaveError_WhenEmpty()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.Empty;
            var command = new BorrowBookCommand(bookId, userId);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BookId)
                .WithErrorMessage("Book ID is required");
        }

        [Fact]
        public async Task BookId_ShouldHaveError_WhenBookNotExists()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var command = new BorrowBookCommand(bookId, userId);
            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BookId)
                .WithErrorMessage("Book is not available for borrowing or does not exist.");
        }

        [Fact]
        public async Task BookId_ShouldHaveError_WhenBookAlreadyBorrowed()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var command = new BorrowBookCommand(bookId, userId);
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Quantity = 0
            };

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Book, object>>>()))
                .ReturnsAsync(book);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BookId)
                .WithErrorMessage("Book is not available for borrowing or does not exist.");
        }
    }
}
