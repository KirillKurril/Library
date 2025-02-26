using Library.Application.Common.Models;
using Library.Application.DTOs;
using Library.Domain.Specifications.BookSpecifications;
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
            var booksSpec = new BookCatalogSpecification(
                     request.SearchTerm,
                     request.GenreId,
                     request.AuthorId,
                     request.availableOnly,
                     request.PageNo,
                     request.ItemsPerPage
                 );
            var items = await _unitOfWork.BookRepository.GetAsync(booksSpec, cancellationToken);

            var countSpec = new BookCatalogCountSpecification(
                request.SearchTerm,
                request.GenreId,
                request.AuthorId,
                request.availableOnly
            );
            var totalItems = await _unitOfWork.BookRepository.CountAsync(countSpec, cancellationToken);

            var bookCatalogDtos = items.Adapt<List<BookCatalogDTO>>();

            return new PaginationListModel<BookCatalogDTO>()
            {
                Items = bookCatalogDtos,
                CurrentPage = request.PageNo ?? 1,
                TotalPages = totalItems / request.ItemsPerPage ?? 10
                    + (totalItems % (request.ItemsPerPage ?? 10) > 0 ? 1 : 0)
            };
        }
    }
}
