using Library.Application.Common.Models;
using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBorrowedBooksQuery(
        Guid UserId,
        int? PageNo,
        int? ItemsPerPage
        ) : IRequest<PaginationListModel<BookLendingDTO>>;
}
