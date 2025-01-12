using Library.Application.Common.Models;
using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record SearchBooksQuery(
        string? SearchTerm,
        Guid? GenreId,
        Guid? AuthorId,
        int? PageNo,
        int? ItemsPerPage) : IRequest<PaginationListModel<BookCatalogDTO>>;
}
