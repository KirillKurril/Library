namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBorrowedBooksQuery(
        int UserId,
        int? pageNo,
        int? itemsPerPage
        ) : IRequest<IEnumerable<Book>>;
}
