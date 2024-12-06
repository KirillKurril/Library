using Library.Application.Common.Models;
using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record SearchBooksQuery(
        string? SearchTerm,
        string? Genre,
        int? AuthorId,
        int? PageNo,
        int? ItemsPerPage) : IRequest<PaginationListModel<BookCatalogDTO>>;
}
