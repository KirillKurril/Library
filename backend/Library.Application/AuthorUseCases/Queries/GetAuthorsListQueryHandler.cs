using Library.Application.Common.Models;
using Library.Domain.Specifications.AuthorSpecification;
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
        var itemsPerPage = request.ItemsPerPage ?? _configuration.GetValue<int?>("LibrarySettings:DefaultItemsPerPage") ?? 3;
        var pageNumber = request.PageNo ?? 1;

        var itemsSpec = new AuthorsFiltredListSpecification(
            request.SearchTerm,
            pageNumber,
            itemsPerPage
            );

        var items = await _unitOfWork.AuthorRepository.GetAsync(itemsSpec, cancellationToken);

        var countSpec = new AuthorsFiltredListCountSpecification(request.SearchTerm);
        var totalItems = await _unitOfWork.AuthorRepository.CountAsync(countSpec, cancellationToken);

        return new PaginationListModel<Author>()
        {
            Items = items,
            CurrentPage = (request.PageNo ?? 1),
            TotalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage)
        };
    }
}
