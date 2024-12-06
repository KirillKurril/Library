using Library.Application.Common.Models;
using System.Data;

namespace Library.Application.BookUseCases.Queries
{
    public class GetExpiredBooksQueryHandler : IRequestHandler<GetExpiredBooksQuery, IReadOnlyList<DebtorNotification>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetExpiredBooksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<DebtorNotification>> Handle(GetExpiredBooksQuery request, CancellationToken cancellationToken)
        {

            var response = _unitOfWork.BookLendingRepository.GetQueryable()
                .Where(bl => bl.ReturnDate < DateTime.UtcNow)
                .Join(
                    _unitOfWork.BookRepository.GetQueryable(),
                    bl => bl.BookId,
                    b => b.Id,
                    (bl, b) => new { BookLending = bl, Book = b }
                )
                .Join(
                    _unitOfWork.AuthorRepository.GetQueryable(),
                    bb => bb.Book.AuthorId,
                    a => a.Id,
                    (bb, a) => new { bb.BookLending, bb.Book, AuthorName = a.Name }
                )
                .GroupBy(exb => exb.BookLending.UserId)
                .Select(g => new DebtorNotification
                {
                    UserID = g.Key,
                    ExpiredBooks = g.Select(exb => new BookBrief
                    {
                        BookName = exb.Book.Title,
                        AuthorName = exb.AuthorName
                    }).ToList()
                }).ToList().AsReadOnly();

            return response;
        }
    }
}
