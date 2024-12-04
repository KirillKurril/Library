namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBorrowedBooksQuery(int UserId) : IRequest<IEnumerable<Book>>;
}
