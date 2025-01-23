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
                        && (request.AuthorId == null || b.AuthorId == request.AuthorId))
               .OrderBy(b => b.Id)
               .ProjectToType<BookCatalogDTO>().ToList();


            var totalItems = query.Count();
            var pageSize = request.ItemsPerPage
                ?? _configuration.GetValue<int>("ItemsPerPage");

            var pageNumber = request.PageNo ?? 1;

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PaginationListModel<BookCatalogDTO>()
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalItems / pageSize + (totalItems % pageSize > 1 ? 1 : 0)
            };
        }
    }
}
