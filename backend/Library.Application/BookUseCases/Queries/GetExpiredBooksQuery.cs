using Library.Application.Common.Models;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetExpiredBooksQuery : IRequest<IEnumerable<DebtorNotification>>;
}
