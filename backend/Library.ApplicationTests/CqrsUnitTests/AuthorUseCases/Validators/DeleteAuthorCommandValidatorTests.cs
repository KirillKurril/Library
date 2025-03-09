using FluentValidation.TestHelper;
using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace Library.ApplicationTests.CqrsUnitTests.AuthorUseCases.Validators
{
    public class DeleteAuthorCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly DeleteAuthorCommandValidator _validator;

        public DeleteAuthorCommandValidatorTests()
        {
            _mockAuthorRepository = new Mock<IRepository<Author>>();
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.BookRepository)
                .Returns(_mockBookRepository.Object);
            _validator = new DeleteAuthorCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_WithExistingId_ShouldNotHaveValidationError()
        {
            var authorId = Guid.NewGuid();
            var command = new DeleteAuthorCommand(authorId);
            var author = new Author { Id = authorId, Name = "John", Surname = "Doe" };

            _mockAuthorRepository.Setup(r => r.CountAsync(
                It.IsAny<ISpecification<Author>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _mockBookRepository.Setup(r => r.CountAsync(
                It.IsAny<ISpecification<Book>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithEmptyId_ShouldHaveValidationError()
        {
            var command = new DeleteAuthorCommand(Guid.Empty);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Author ID is required");
        }
    }
}
