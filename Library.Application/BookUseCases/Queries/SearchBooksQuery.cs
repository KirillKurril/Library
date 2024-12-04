namespace Library.Application.BookUseCases.Queries
{
    public sealed record SearchBooksQuery(
        string SearchTerm,
        Genre? Genre = null,
        bool? IsAvailable = null) : IRequest<IEnumerable<Book>>;
}
