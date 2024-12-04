namespace Library.Application.BookUseCases.Queries
{
    public sealed record SearchBooksQuery(
        string? SearchTerm,
        string? Genre = null,
        int? AuthorId = null) : IRequest<IEnumerable<Book>>;
}
