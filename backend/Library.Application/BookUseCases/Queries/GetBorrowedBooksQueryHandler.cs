using Library.Application.Common.Models;
using Library.Application.DTOs;
using Library.Domain.Specifications.BookSpecifications;
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
            var itemsSpec = new BorrowedBooksSpecification(
                request.UserId,
                request.SearchTerm,
                request.PageNo,
                request.ItemsPerPage
            );

            var items = await _unitOfWork.BookLendingRepository.GetAsync(itemsSpec, cancellationToken);

            var countSpec = new BorrowedBooksCountSpecification(
                request.UserId,
                request.SearchTerm
            );
            var totalItems = await _unitOfWork.BookLendingRepository.CountAsync(countSpec, cancellationToken);

            var mappedItems = items.Adapt<List<BookLendingDTO>>();

            return new PaginationListModel<BookLendingDTO>()
            {
                Items = mappedItems,
                CurrentPage = request.PageNo ?? 1,
                TotalPages = totalItems / request.ItemsPerPage ?? 10
                    + (totalItems % (request.ItemsPerPage ?? 10) > 0 ? 1 : 0)
            };
        }
    }
}
