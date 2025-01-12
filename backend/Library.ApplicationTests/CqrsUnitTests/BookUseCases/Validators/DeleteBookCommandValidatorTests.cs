using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Validators
{
    public class DeleteBookCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteBookCommandValidator _validator;

        public DeleteBookCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _validator = new DeleteBookCommandValidator(_mockUnitOfWork.Object);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book());

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookLending)null);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<BookLending, object>>[]>()))
                .ReturnsAsync((BookLending)null);

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_EmptyId_ShouldHaveValidationError()
        {
            var command = new DeleteBookCommand(Guid.Empty);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Book ID is required");
        }

        [Fact]
        public async Task Validate_NonExistentBook_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Book with specified ID does not exist");
        }

        [Fact]
        public async Task Validate_BookWithActiveLending_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetByIdAsync(
                bookId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId });

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookLending { BookId = bookId });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Cannot delete book that is currently lent");
        }
    }
}
