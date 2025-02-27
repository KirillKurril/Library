using Library.Application.Common.Models;
using Library.Domain.Specifications.GenreSpecification;
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
            var itemsPerPage = request.ItemsPerPage ??_configuration.GetValue<int?>("LibrarySettings:DefaultItemsPerPage") ?? 3;
            var pageNumber = request.PageNo ?? 1;

            var specItems = new GenreFiltredListSpecification(
                request.SearchTerm,
                pageNumber,
                itemsPerPage);

            var items = await _unitOfWork.GenreRepository.GetAsync(specItems, cancellationToken);

            var specCount = new GenreFiltredListCountSpecification(request.SearchTerm);
            var totalItems = await _unitOfWork.GenreRepository.CountAsync(specCount);

            return new PaginationListModel<Genre>()
            {
                Items = items,
                CurrentPage = (request.PageNo ?? 1),
                TotalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage)
            };
        }
    }
}
