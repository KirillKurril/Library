using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Application.AuthorUseCases.Commands;
using Library.Application.DTOs;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using MapsterMapper;
using Moq;
using FluentValidation;
using Xunit;

namespace Library.UnitTests.AuthorUseCases.Commands
{
    public class CreateAuthorHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IValidator<CreateAuthorCommand>> _mockValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<Author>> _mockAuthorRepository;
        private readonly CreateAuthorHandler _handler;

        public CreateAuthorHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<CreateAuthorCommand>>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthorRepository = new Mock<IRepository<Author>>();
            
            _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
                .Returns(_mockAuthorRepository.Object);

            _handler = new CreateAuthorHandler(
                _mockUnitOfWork.Object,
                _mockValidator.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateAuthorAndReturnId()
        {
            var command = new CreateAuthorCommand("John", "Doe", DateTime.Now, "USA");
            var author = new Author 
            { 
                Id = Guid.NewGuid(),
                Name = command.Name,
                Surname = command.Surname,
                BirthDate = command.BirthDate,
                Country = command.Country
            };

            _mockMapper.Setup(m => m.Map<Author>(command))
                .Returns(author);

            _mockAuthorRepository.Setup(r => r.Add(It.IsAny<Author>()))
                .Returns(author);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(author.Id, result.Id);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
            _mockAuthorRepository.Verify(r => r.Add(It.IsAny<Author>()), Times.Once);
        }
    }
}
