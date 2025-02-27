using Library.Application.Common.Models;
using Library.Application.DTOs;
using Library.Domain.Specifications.BookSpecifications;
using Microsoft.Extensions.Configuration;

namespace Library.Application.BookUseCases.Queries
{
    public class GetBorrowedBooksQueryHandler : IRequestHandler<GetBorrowedBooksQuery,PaginationListModel<BookLendingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public GetBorrowedBooksQueryHandler(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<PaginationListModel<BookLendingDTO>> Handle(GetBorrowedBooksQuery request, CancellationToken cancellationToken)
        {
            var itemsPerPage = request.ItemsPerPage ?? _configuration.GetValue<int?>("LibrarySettings:DefaultItemsPerPage") ?? 3;
            var pageNumber = request.PageNo ?? 1;

            var itemsSpec = new BorrowedBooksSpecification(
                request.UserId,
                request.SearchTerm,
                pageNumber,
                itemsPerPage
            );

            var items = await _unitOfWork.BookLendingRepository.GetAsync(itemsSpec, cancellationToken);

            var countSpec = new BorrowedBooksCountSpecification(
                request.UserId,
                request.SearchTerm
            );
            var totalItems = await _unitOfWork.BookLendingRepository.CountAsync(countSpec, cancellationToken);

            var bookLendingDTOs = items.Adapt<List<BookLendingDTO>>();

            return new PaginationListModel<BookLendingDTO>()
            {
                Items = bookLendingDTOs,
                CurrentPage = (request.PageNo ?? 1),
                TotalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage)
            };
        }
    }
}
