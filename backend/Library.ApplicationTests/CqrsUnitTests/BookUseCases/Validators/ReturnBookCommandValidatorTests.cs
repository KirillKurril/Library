using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Domain.Specifications.BookSpecifications;
using Moq;
using System.Linq.Expressions;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Validators
{
    public class ReturnBookCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ReturnBookCommandValidator _validator;

        public ReturnBookCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _validator = new ReturnBookCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_BookBorrowedByUser_ShouldNotHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new ReturnBookCommand(bookId, userId);

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.CountAsync(
                It.IsAny<ISpecification<BookLending>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_BookNotBorrowedByUser_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new ReturnBookCommand(bookId, userId);

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefault(
                It.IsAny<ISpecification<BookLending>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookLending)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Book has not been borrowed by this user.");
        }

        [Fact]
        public async Task Validate_DifferentUserBorrowedBook_ShouldHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var command = new ReturnBookCommand(bookId, userId);

            var checkSpec = new BookLendingByBookIdUserIdSpecification(bookId, userId);
            var getblSpec = new BookLendingsByBookIdSpecification(bookId);

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefault(
                checkSpec,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookLending)null);

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefault(
                getblSpec,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookLending
                {
                    BookId = bookId,
                    UserId = otherUserId
                });

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("Book has not been borrowed by this user.");
        }
    }
}
