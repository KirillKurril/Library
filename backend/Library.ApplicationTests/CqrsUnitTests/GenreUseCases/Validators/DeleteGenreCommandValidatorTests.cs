using FluentValidation.TestHelper;
using Library.Application.GenreUseCases.Commands;
using Library.Application.GenreUseCases.Validators;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Library.ApplicationTests.CqrsUnitTests.GenreUseCases.Validators
{
    public class DeleteGenreCommandValidatorTests
    {
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteGenreCommandValidator _validator;
        private readonly Mock<IRepository<Genre>> _mockGenreRepository;


        public DeleteGenreCommandValidatorTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockGenreRepository = new();
            _mockUnitOfWork = new();

            _mockUnitOfWork.Setup(uow => uow.GenreRepository).Returns(_mockGenreRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.BookRepository).Returns(_mockBookRepository.Object);

            _mockBookRepository.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);
            _validator = new DeleteGenreCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Validate_WithExistingId_ShouldNotHaveValidationError()
        {
            var genreId = Guid.NewGuid();
            var command = new DeleteGenreCommand(genreId);
            var genre = new Genre { Id = genreId, Name = "Fiction" };

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.GetByIdAsync(genreId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(genre);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithNonExistingId_ShouldHaveValidationError()
        {
            var genreId = Guid.NewGuid();
            var command = new DeleteGenreCommand(genreId);

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.GetByIdAsync(genreId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Genre)null);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public async Task Validate_WithEmptyId_ShouldHaveValidationError()
        {
            var command = new DeleteGenreCommand(Guid.Empty);

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Genre ID is required");
        }
    }
}
