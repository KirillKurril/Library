using Library.Application.Common.Models;
using Library.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Library.Application.BookUseCases.Queries
{
    public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PaginationListModel<BookCatalogDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public SearchBooksQueryHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<PaginationListModel<BookCatalogDTO>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
        {
            var searchTerm = request.SearchTerm?.ToLower();

            var query = _unitOfWork.BookRepository.GetQueryable()
                .Where(b => (string.IsNullOrEmpty(searchTerm) ||
                        b.Title.ToLower().Contains(searchTerm))
                        && (request.GenreId == null || b.GenreId == request.GenreId)
                        && (request.AuthorId == null || b.AuthorId == request.AuthorId));

            if(request.availableOnly == true)
                query = query.Where(b => b.IsAvailable);

            var totalItems = query.Count();
            var pageSize = request.ItemsPerPage ?? 1;
            var pageNumber = request.PageNo ?? 1;

            if(request.ItemsPerPage != null)
            {
                query = query.OrderBy(b => b.Id)
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize);
            }


            var items = query.ProjectToType<BookCatalogDTO>().ToList();

            return new PaginationListModel<BookCatalogDTO>()
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalItems / pageSize + (totalItems % pageSize > 1 ? 1 : 0)
            };
        }
    }
}
