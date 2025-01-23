using Library.Application.Common.Models;
using Library.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Library.Application.AuthorUseCases.Queries;

public class GetAuthorsListQueryHandler : IRequestHandler<GetAuthorsListQuery, PaginationListModel<Author>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public GetAuthorsListQueryHandler(
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<PaginationListModel<Author>> Handle(GetAuthorsListQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm?.ToLower();

        var query = _unitOfWork.AuthorRepository.GetQueryable()
               .Where(a => (string.IsNullOrEmpty(searchTerm) ||
                       (a.Name + " " + a.Surname).ToLower().Contains(searchTerm)))
               .OrderBy(a => a.Id);



        var totalItems = query.Count();
        var pageSize = request.ItemsPerPage
            ?? _configuration.GetValue<int>("ItemsPerPage");

        var pageNumber = request.PageNo ?? 1;

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();

        return new PaginationListModel<Author>()
        {
            Items = items,
            CurrentPage = pageNumber,
            TotalPages = totalItems / pageSize + (totalItems % pageSize > 1 ? 1 : 0)
        };
    }
}
