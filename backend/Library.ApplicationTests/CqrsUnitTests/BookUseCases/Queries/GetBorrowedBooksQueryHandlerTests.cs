using Library.Application.BookUseCases.Queries;
using Library.Application.DTOs;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Queries
{
    public class GetBorrowedBooksQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly GetBorrowedBooksQueryHandler _handler;
        private readonly IMapper _mapper;

        public GetBorrowedBooksQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockConfig = new Mock<IConfiguration>();

            var mapperConfig = new TypeAdapterConfig();

            mapperConfig.NewConfig<JoinLendingDTO, BookLendingDTO>()
                .Map(dest => dest.Id, src => src.Book.Id)
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Description, src => src.Book.Description)
                .Map(dest => dest.GenreId, src => src.Book.GenreId)
                .Map(dest => dest.AuthorId, src => src.Book.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.Book.ImageUrl)
                .Map(dest => dest.ReturnDate, src => src.BookLending.ReturnDate);

            mapperConfig.NewConfig<BookLending, BookLendingDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Description, src => src.Book.Description)
                .Map(dest => dest.AuthorId, src => src.Book.AuthorId)
                .Map(dest => dest.GenreId, src => src.Book.GenreId)
                .Map(dest => dest.ImageUrl, src => src.Book.ImageUrl)
                .Map(dest => dest.ReturnDate, src => src.ReturnDate);
            
            _mapper = new Mapper(mapperConfig);


            _mockConfig.Setup(x => x.GetSection("ItemsPerPage").Value)
                .Returns("10");

            _handler = new GetBorrowedBooksQueryHandler(
                _mockUnitOfWork.Object,
                _mockConfig.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsPaginatedBookLendings()
        {
            var userId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var genreId = Guid.NewGuid();

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Description = "Test Description",
                ISBN = "978-3-16-148410-0",
                GenreId = genreId,
                AuthorId = authorId,
                ImageUrl = "http://test.com/image.jpg"
            };

            var bookLending = new BookLending
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                ReturnDate = DateTime.Now.AddDays(14),
                Book = book  
            };

            var bookLendings = new List<BookLending> { bookLending };

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(x => x.GetSection("LibrarySettings:DefaultItemsPerPage").Value)
                .Returns("10");

            _mockUnitOfWork
                .Setup(x => x.BookLendingRepository.GetAsync(
                    It.IsAny<ISpecification<BookLending>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookLendings);

            _mockUnitOfWork
                .Setup(x => x.BookLendingRepository.CountAsync(
                    It.IsAny<ISpecification<BookLending>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookLendings.Count);

            var handler = new GetBorrowedBooksQueryHandler(_mockUnitOfWork.Object, mockConfiguration.Object, _mapper);

            var query = new GetBorrowedBooksQuery(userId, 1, null, null);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);

            var bookLendingDto = result.Items.First();
            Assert.Equal(bookId, bookLendingDto.Id);
            Assert.Equal(book.Title, bookLendingDto.Title);
            Assert.Equal(book.Description, bookLendingDto.Description);
            Assert.Equal(book.GenreId, bookLendingDto.GenreId);
            Assert.Equal(book.AuthorId, bookLendingDto.AuthorId);
            Assert.Equal(book.ImageUrl, bookLendingDto.ImageUrl);
            Assert.Equal(bookLending.ReturnDate, bookLendingDto.ReturnDate);
        }

        [Fact]
        public async Task Handle_WithNoBooks_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(x => x.GetSection("LibrarySettings:DefaultItemsPerPage").Value)
                .Returns("10");

            _mockUnitOfWork
                .Setup(x => x.BookLendingRepository.GetAsync(
                    It.IsAny<ISpecification<BookLending>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BookLending>());

            _mockUnitOfWork
                .Setup(x => x.BookLendingRepository.CountAsync(
                    It.IsAny<ISpecification<BookLending>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var handler = new GetBorrowedBooksQueryHandler(_mockUnitOfWork.Object, mockConfiguration.Object, _mapper);

            var query = new GetBorrowedBooksQuery(userId, 1, null, null);
            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(0, result.TotalPages);
        }
    }
}
