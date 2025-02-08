using Library.Application.Common.Models;
using Library.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Library.Application.BookUseCases.Queries
{
    public class GetBorrowedBooksQueryHandler : IRequestHandler<GetBorrowedBooksQuery,PaginationListModel<BookLendingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public GetBorrowedBooksQueryHandler(
            IUnitOfWork unitOfWork,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<PaginationListModel<BookLendingDTO>> Handle(GetBorrowedBooksQuery request, CancellationToken cancellationToken)
        {
            var searchTerm = request.SearchTerm?.ToLower();

            var query = _unitOfWork.BookLendingRepository
                .GetQueryable()
                .Where(bl => bl.UserId == request.UserId)
                .Join(
                    _unitOfWork.BookRepository.GetQueryable(),
                    bl => bl.BookId,
                    b => b.Id,
                    (bl, b) => new JoinLendingDTO(){ Book = b, BookLending = bl }
                )
                .Where( b => (string.IsNullOrEmpty(searchTerm) ||
                        b.Book.Title.ToLower().Contains(searchTerm)))
                .ProjectToType<BookLendingDTO>();


            var totalItems = query.Count();
            var pageSize = request.ItemsPerPage ?? 1;
            var pageNumber = request.PageNo ?? 1;

            if(request.ItemsPerPage != null)
            {
                query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();
            }


            var items = query.ToList();

            var result = new PaginationListModel<BookLendingDTO>()
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalItems / pageSize + ((totalItems % pageSize) > 0 ? 1 : 0)
            };
            return result;
        }
    }
}
