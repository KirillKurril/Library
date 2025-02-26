using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.AuthorUseCases.Queries;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Author>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuthorByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Author> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpecification(request.Id);
        var author = await _unitOfWork.AuthorRepository.FirstOrDefault(spec, cancellationToken);

        if (author == null)
            throw new NotFoundException(nameof(Author), $"ID: {request.Id}");

        return author;
    }
}
