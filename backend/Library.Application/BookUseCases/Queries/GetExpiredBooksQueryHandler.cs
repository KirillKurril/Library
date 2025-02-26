using Library.Application.Common.Models;
using Library.Domain.Specifications.BookSpecifications;
using System.Data;

namespace Library.Application.BookUseCases.Queries
{
    public class GetExpiredBooksQueryHandler : IRequestHandler<GetExpiredBooksQuery, IEnumerable<DebtorNotification>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetExpiredBooksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DebtorNotification>> Handle(GetExpiredBooksQuery request, CancellationToken cancellationToken)
        {
            var spec = new ExpiredBooksSpecification();
            var expiredBooksSpec = new ExpiredBooksSpecification();

            var expiredBookLendings = await _unitOfWork.BookLendingRepository
                .GetAsync(expiredBooksSpec, cancellationToken);

            return expiredBookLendings
                .GroupBy(exb => exb.UserId)
                .Select(g => new DebtorNotification
                {
                    UserID = g.Key,
                    ExpiredBooks = g.Select(exb => new BookBrief
                    {
                        BookName = exb.Book.Title,
                        AuthorName = exb.Book.Author.Name
                    }).ToList()
                })
                .ToList();
        }
    }
}
