namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetAvailableBooksQuery : IRequest<IEnumerable<Book>>;
}
