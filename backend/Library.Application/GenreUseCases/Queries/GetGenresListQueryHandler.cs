using Library.Application.Common.Models;
using Library.Application.DTOs;
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
            var specItems = new GenreFiltredListSpecification(
                request.SearchTerm,
                request.PageNo,
                request.ItemsPerPage);

            var items = await _unitOfWork.GenreRepository.GetAsync(specItems, cancellationToken);

            var specCount = new GenreFiltredListCountSpecification(request.SearchTerm);
            var totalItems = await _unitOfWork.GenreRepository.CountAsync(specCount);

            return new PaginationListModel<Genre>()
            {
                Items = items,
                CurrentPage = request.PageNo ?? 1,
                TotalPages = totalItems / request.ItemsPerPage ?? 10
                    + (totalItems % (request.ItemsPerPage ?? 10) > 0 ? 1 : 0)
            };
        }
    }
}
