using Library.Application.Common.Models;
using Library.Application.DTOs;
using Library.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace Library.Application.BookUseCases.Queries
{
    public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PaginationListModel<BookCatalogDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public SearchBooksQueryHandler(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<PaginationListModel<BookCatalogDTO>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
        {
            var searchTerm = request.SearchTerm?.ToLower();

            var query = _unitOfWork.BookRepository.GetQueryable()
                .Where(b => string.IsNullOrEmpty(searchTerm) ||
                        b.Title.ToLower().Contains(searchTerm)
                        && (request.Genre == null || b.Genre.Name == request.Genre)
                        && (request.AuthorId == null || b.Genre.Name == request.Genre))
                .Select(cb => new BookCatalogDTO()
                {
                    Id = cb.Id,
                    Title = cb.Title,
                    Description = cb.Description,
                    GenreId = cb.GenreId,
                    AuthorId = cb.AuthorId,
                    ImageUrl = cb.ImageUrl,
                });

            var totalItems = query.Count();
            var pageSize = request.ItemsPerPage
                ?? int.Parse(_config.GetSection("ItemsPerPage").Value);

            var pageNumber = request.PageNo ?? 1;

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PaginationListModel<BookCatalogDTO>()
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalItems
            };
        }
    }
}
