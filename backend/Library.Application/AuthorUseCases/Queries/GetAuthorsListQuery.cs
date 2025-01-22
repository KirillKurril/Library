using Library.Application.Common.Models;

namespace Library.Application.AuthorUseCases.Queries
{
    public sealed record GetAuthorsListQuery(
        string? SearchTerm,
        int? PageNo,
        int? ItemsPerPage) : IRequest<PaginationListModel<Author>>;
}
