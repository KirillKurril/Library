namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBookByIsbnQuery(string ISBN) : IRequest<Book>;
}
