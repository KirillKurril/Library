using Library.Application.Common.Models;
using Library.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Library.Application.BookUseCases.Queries
{
    public class GetBorrowedBooksQueryHandler : IRequestHandler<GetBorrowedBooksQuery,PaginationListModel<BookLendingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public GetBorrowedBooksQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        public async Task<PaginationListModel<BookLendingDTO>> Handle(GetBorrowedBooksQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.BookLendingRepository
                .GetQueryable()
                .Where(bl => bl.UserId == request.UserId)
                .Join(
                    _unitOfWork.BookRepository.GetQueryable(),
                    bl => bl.BookId,
                    b => b.Id,
                    (bl, b) => new JoinLendingDTO(){ Book = b, BookLending = bl }
                )
                .ProjectToType<BookLendingDTO>();

            var totalItems = query.Count();
            var pageSize = request.ItemsPerPage
                ?? int.Parse(_config.GetSection("ItemsPerPage").Value);

            var pageNumber = request.PageNo ?? 1;

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

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
