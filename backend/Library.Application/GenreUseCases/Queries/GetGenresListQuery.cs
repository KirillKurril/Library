using Library.Application.Common.Models;
using Library.Application.DTOs;

namespace Library.Application.GenreUseCases.Queries
{
    public sealed record GetGenresListQuery(
        string? SearchTerm,
        int? PageNo,
        int? ItemsPerPage) : IRequest<PaginationListModel<Genre>>;
}
