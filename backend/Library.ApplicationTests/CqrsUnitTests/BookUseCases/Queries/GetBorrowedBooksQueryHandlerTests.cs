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
            _mapper = new Mapper(TypeAdapterConfig.GlobalSettings);

            TypeAdapterConfig<JoinLendingDTO, BookLendingDTO>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Book.Id)
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Description, src => src.Book.Description)
                .Map(dest => dest.GenreId, src => src.Book.GenreId)
                .Map(dest => dest.AuthorId, src => src.Book.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.Book.ImageUrl)
                .Map(dest => dest.ReturnDate, src => src.BookLending.ReturnDate);

            _mockConfig.Setup(x => x.GetSection("ItemsPerPage").Value)
                .Returns("10");

            _handler = new GetBorrowedBooksQueryHandler(
                _mockUnitOfWork.Object,
                _mockConfig.Object);
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
                ReturnDate = DateTime.Now.AddDays(14)
            };

            var bookLendings = new List<BookLending> { bookLending };
            var books = new List<Book> { book };

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.GetQueryable(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<Expression<Func<BookLending, object>>[]>()))
                .Returns(bookLendings.AsQueryable());

            _mockUnitOfWork.Setup(x => x.BookRepository.GetQueryable(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>[]>()))
                .Returns(books.AsQueryable());

            var query = new GetBorrowedBooksQuery(userId, 1, 10, null);

            var result = await _handler.Handle(query, CancellationToken.None);

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
        public async Task Handle_WithDefaultPagination_UsesConfigurationValue()
        {
            var userId = Guid.NewGuid();
            var books = Enumerable.Range(0, 20)
                .Select(i => new Book
                {
                    Id = Guid.NewGuid(),
                    Title = $"Test Book {i}",
                    Description = "Test Description",
                    ISBN = "978-3-16-148410-0",
                    GenreId = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid()
                }).ToList();

            var bookLendings = new List<BookLending>();

            foreach (var book in books)
            {
                bookLendings.Add(new BookLending()
                {
                    Id = Guid.NewGuid(),
                    BookId = book.Id,
                    UserId = userId,
                    ReturnDate = DateTime.Now.AddDays(14)
                });
            }


            _mockUnitOfWork.Setup(x => x.BookLendingRepository.GetQueryable(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<Expression<Func<BookLending, object>>[]>()))
                .Returns(bookLendings.AsQueryable());


            _mockUnitOfWork.Setup(x => x.BookRepository.GetQueryable(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>[]>()))
                .Returns(books.AsQueryable());

            var query = new GetBorrowedBooksQuery(userId, null, null, null);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(10, result.Items.Count);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);

        }

        [Fact]
        public async Task Handle_WithNoBooks_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();
            var bookLendings = new List<BookLending>().AsQueryable();
            var books = new List<Book>().AsQueryable();

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.GetQueryable(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<Expression<Func<BookLending, object>>>()))
                .Returns(bookLendings);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetQueryable(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>>()))
                .Returns(books);

            var query = new GetBorrowedBooksQuery(userId, 1, 10, null);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(0, result.TotalPages);
        }
    }
}
