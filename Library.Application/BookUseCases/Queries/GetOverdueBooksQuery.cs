namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetOverdueBooksQuery : IRequest<IEnumerable<Book>>;
}
