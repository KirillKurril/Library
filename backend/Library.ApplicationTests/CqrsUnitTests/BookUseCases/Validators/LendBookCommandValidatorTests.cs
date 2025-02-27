using FluentValidation.TestHelper;
using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Validators;
using Library.Application.Common.Interfaces;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;


namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Validators
{
    public class LendBookCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserDataAccessor> _userDataAccessor;
        private readonly LendBookCommandValidator _validator;

        public LendBookCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _userDataAccessor = new Mock<IUserDataAccessor>();
            _validator = new LendBookCommandValidator(_mockUnitOfWork.Object, _userDataAccessor.Object);
        }

        [Fact]
        public async Task BookId_ShouldHaveError_WhenEmpty()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.Empty;
            var command = new LendBookCommand(bookId, userId);

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            _userDataAccessor.Setup(x => x.UserExist(
                 It.IsAny<Guid>()))
                 .ReturnsAsync(true);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BookId)
                .WithErrorMessage("Book ID is required");
        }

        [Fact]
        public async Task BookId_ShouldHaveError_WhenBookNotExists()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var command = new LendBookCommand(bookId, userId);
            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            _userDataAccessor.Setup(x => x.UserExist(
                 It.IsAny<Guid>()))
                 .ReturnsAsync(true);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BookId)
                .WithErrorMessage("Book is not available for borrowing or does not exist.");
        }

        [Fact]
        public async Task BookId_ShouldHaveError_WhenBookAlreadyBorrowed()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var command = new LendBookCommand(bookId, userId);
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Quantity = 0
            };

            _mockUnitOfWork.Setup(x => x.BookRepository.FirstOrDefault(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            _userDataAccessor.Setup(x => x.UserExist(
                 It.IsAny<Guid>()))
                 .ReturnsAsync(true);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BookId)
                .WithErrorMessage("Book is not available for borrowing or does not exist.");
        }
    }
}
