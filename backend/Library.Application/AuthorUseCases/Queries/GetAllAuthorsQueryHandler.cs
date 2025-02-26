using Library.Domain.Specifications;

namespace Library.Application.AuthorUseCases.Queries;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<Author>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllAuthorsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Author>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var spec = new AllItemsSpecification<Author>();
        return await _unitOfWork.AuthorRepository.GetAsync(spec, cancellationToken);
    }
}