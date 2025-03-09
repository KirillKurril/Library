using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;

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

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book());

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.FirstOrDefault(
                It.IsAny<ISpecification<BookLending>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookLending)null);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            var bookId = Guid.NewGuid();
            var command = new DeleteBookCommand(bookId);

            _mockUnitOfWork.Setup(x => x.BookRepository.CountAsync(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.CountAsync(
                It.IsAny<ISpecification<BookLending>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

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
    }
}
