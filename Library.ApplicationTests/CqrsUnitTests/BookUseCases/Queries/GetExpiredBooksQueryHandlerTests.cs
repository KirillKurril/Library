using Library.Application.BookUseCases.Queries;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Moq;
using System.Linq.Expressions;

namespace Library.ApplicationTests.CqrsUnitTests.BookUseCases.Queries
{
    public class GetExpiredBooksQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetExpiredBooksQueryHandler _handler;

        public GetExpiredBooksQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetExpiredBooksQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithExpiredBooks_ReturnsDebtorNotifications()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var bookId1 = Guid.NewGuid();
            var bookId2 = Guid.NewGuid();
            var authorId1 = Guid.NewGuid();
            var authorId2 = Guid.NewGuid();

            var bookLendings = new List<BookLending>
            {
                new BookLending
                {
                    Id = Guid.NewGuid(),
                    UserId = userId1,
                    BookId = bookId1,
                    ReturnDate = DateTime.UtcNow.AddDays(-10)
                },
                new BookLending
                {
                    Id = Guid.NewGuid(),
                    UserId = userId2,
                    BookId = bookId2,
                    ReturnDate = DateTime.UtcNow.AddDays(-5)
                }
            }.AsQueryable();

            var books = new List<Book>
            {
                new Book
                {
                    Id = bookId1,
                    Title = "Expired Book 1",
                    AuthorId = authorId1
                },
                new Book
                {
                    Id = bookId2,
                    Title = "Expired Book 2",
                    AuthorId = authorId2
                }
            }.AsQueryable();

            var authors = new List<Author>
            {
                new Author { Id = authorId1, Name = "Author 1" },
                new Author { Id = authorId2, Name = "Author 2" }
            }.AsQueryable();

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.GetQueryable(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<Expression<Func<BookLending, object>>[]>()))
                .Returns(bookLendings);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetQueryable(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>[]>()))
                .Returns(books);

            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetQueryable(
                It.IsAny<Expression<Func<Author, bool>>>(),
                It.IsAny<Expression<Func<Author, object>>[]>()))
                .Returns(authors);

            var result = await _handler.Handle(new GetExpiredBooksQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var notification1 = result.First(n => n.UserID == userId1);
            Assert.Single(notification1.ExpiredBooks);
            Assert.Equal("Expired Book 1", notification1.ExpiredBooks[0].BookName);
            Assert.Equal("Author 1", notification1.ExpiredBooks[0].AuthorName);

            var notification2 = result.First(n => n.UserID == userId2);
            Assert.Single(notification2.ExpiredBooks);
            Assert.Equal("Expired Book 2", notification2.ExpiredBooks[0].BookName);
            Assert.Equal("Author 2", notification2.ExpiredBooks[0].AuthorName);
        }

        [Fact]
        public async Task Handle_WithNoExpiredBooks_ReturnsEmptyList()
        {
            var bookLendings = new List<BookLending>
            {
                new BookLending
                {
                    Id = Guid.NewGuid(),
                    ReturnDate = DateTime.UtcNow.AddDays(10)
                }
            }.AsQueryable();

            _mockUnitOfWork.Setup(x => x.BookLendingRepository.GetQueryable(
                It.IsAny<Expression<Func<BookLending, bool>>>(),
                It.IsAny<Expression<Func<BookLending, object>>>()))
                .Returns(bookLendings);

            _mockUnitOfWork.Setup(x => x.BookRepository.GetQueryable(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Expression<Func<Book, object>>>()))
                .Returns(new List<Book>().AsQueryable());

            _mockUnitOfWork.Setup(x => x.AuthorRepository.GetQueryable(
                It.IsAny<Expression<Func<Author, bool>>>(),
                It.IsAny<Expression<Func<Author, object>>>()))
                .Returns(new List<Author>().AsQueryable());

            var result = await _handler.Handle(new GetExpiredBooksQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}