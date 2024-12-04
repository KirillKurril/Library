namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetAllBooksQuery : IRequest<IEnumerable<Book>>;
}
