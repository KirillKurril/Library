using Library.Domain.Specifications;
using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.AuthorUseCases.Commands;

public class DeleteAuthorHandler : IRequestHandler<DeleteAuthorCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAuthorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpecification(request.Id);
        var author = await _unitOfWork.AuthorRepository.FirstOrDefault(spec, cancellationToken);

        _unitOfWork.AuthorRepository.Delete(author);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
