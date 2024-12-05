namespace Library.Application.BookUseCases.Queries
{
    public sealed record SearchBooksQuery(
        string? SearchTerm,
        string? Genre,
        int? AuthorId,
        int? pageNo,
        int? itemsPerPage) : IRequest<IEnumerable<Book>>;
}
