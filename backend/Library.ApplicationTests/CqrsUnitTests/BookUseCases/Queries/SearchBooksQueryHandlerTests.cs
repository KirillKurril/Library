using Library.Application.BookUseCases.Queries;
using Library.Application.DTOs;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Queries
{
    public class SearchBooksQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly IMapper _mapper;
        private readonly SearchBooksQueryHandler _handler;

        public SearchBooksQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockConfig = new Mock<IConfiguration>();

            _mockConfig.Setup(x => x.GetSection("ItemsPerPage").Value)
                .Returns("10");

            _handler = new SearchBooksQueryHandler(
                _mockUnitOfWork.Object,
                _mockConfig.Object);

            _mapper = new Mapper(TypeAdapterConfig.GlobalSettings);

            TypeAdapterConfig<Book, BookCatalogDTO>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);
        }

        [Fact]
        public async Task Handle_WithSearchTerm_ReturnsFilteredBooks()
        {
            var books = new List<Book>
        {
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Clean Code",
                GenreId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Genre = new Genre(),
                Author = new Author()
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Design Patterns",
                GenreId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Genre = new Genre(),
                Author = new Author()
            }
        };

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(x => x.GetSection("LibrarySettings:DefaultItemsPerPage").Value)
                .Returns("10");

            _mockUnitOfWork
                .Setup(x => x.BookRepository.GetAsync(
                    It.IsAny<ISpecification<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Book> { books[0] });

            _mockUnitOfWork
                .Setup(x => x.BookRepository.CountAsync(
                    It.IsAny<ISpecification<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var handler = new SearchBooksQueryHandler(_mockUnitOfWork.Object, mockConfiguration.Object);

            var query = new SearchBooksQuery("clean", null, null, 1, 10, false);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Clean Code", result.Items[0].Title);
        }

        [Fact]
        public async Task Handle_WithGenreFilter_ReturnsFilteredBooks()
        {
            var genreId = Guid.NewGuid();
            var books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Programming Book",
                    GenreId = genreId,
                    AuthorId = Guid.NewGuid(),
                    Genre = new Genre { Id = genreId },
                    Author = new Author()
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Another Book",
                    GenreId = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid(),
                    Genre = new Genre(),
                    Author = new Author()
                }
            };

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(x => x.GetSection("LibrarySettings:DefaultItemsPerPage").Value)
                .Returns("10");

            _mockUnitOfWork
                .Setup(x => x.BookRepository.GetAsync(
                    It.IsAny<ISpecification<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Book> { books[0] });

            _mockUnitOfWork
                .Setup(x => x.BookRepository.CountAsync(
                    It.IsAny<ISpecification<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var handler = new SearchBooksQueryHandler(_mockUnitOfWork.Object, mockConfiguration.Object);

            var query = new SearchBooksQuery(null, genreId, null, 1, 10, false);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(genreId, result.Items[0].GenreId);
        }
    }
}