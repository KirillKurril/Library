using Library.Application.Common.Models;
using Microsoft.Extensions.Configuration;

namespace Library.Application.GenreUseCases.Queries
{
    public class GetGenresListQueryHandler : IRequestHandler<GetGenresListQuery, PaginationListModel<Genre>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public GetGenresListQueryHandler(
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<PaginationListModel<Genre>> Handle(GetGenresListQuery request, CancellationToken cancellationToken)
        {
            var searchTerm = request.SearchTerm?.ToLower();

            var query = _unitOfWork.GenreRepository.GetQueryable()
                   .Where(b => (string.IsNullOrEmpty(searchTerm) ||
                           (b.Name).ToLower().Contains(searchTerm)));

            var totalItems = query.Count();
            var pageSize = request.ItemsPerPage
                ?? _configuration.GetValue<int>("ItemsPerPage");

            var pageNumber = request.PageNo ?? 1;

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            return new PaginationListModel<Genre>()
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalItems / pageSize + (totalItems % pageSize > 1 ? 1 : 0)
            };
        }
    }
}
