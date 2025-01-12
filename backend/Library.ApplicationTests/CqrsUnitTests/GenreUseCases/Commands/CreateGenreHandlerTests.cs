using FluentValidation;
using Library.Application.DTOs;
using Library.Application.GenreUseCases.Commands;
using Library.Domain.Entities;
using Library.Domain.Abstractions;
using MapsterMapper;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.GenreUseCases.Commands
{
    public class CreateGenreHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidator<CreateGenreCommand>> _mockValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateGenreHandler _handler;

        public CreateGenreHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<CreateGenreCommand>>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateGenreHandler(_mockUnitOfWork.Object, _mockValidator.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCreateGenreAndReturnResponse()
        {
            var command = new CreateGenreCommand("Fiction");
            var mappedGenre = new Genre { Name = "Fiction" };
            var expectedResponse = new CreateEntityResponse { Id = mappedGenre.Id };

            _mockMapper.Setup(m => m.Map<Genre>(command))
                .Returns(mappedGenre);

            _mockUnitOfWork.Setup(uow => uow.GenreRepository.Add(It.IsAny<Genre>()))
                .Returns(mappedGenre);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            _mockMapper.Verify(m => m.Map<Genre>(command), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.GenreRepository.Add(It.IsAny<Genre>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

    }
}
