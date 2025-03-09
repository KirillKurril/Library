using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.AuthorUseCases.Commands;

public class UpdateAuthorHandler : IRequestHandler<UpdateAuthorCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAuthorHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorSpec = new AuthorByIdSpecification(request.Id);

        var oldAuthor = await _unitOfWork.AuthorRepository.FirstOrDefault(authorSpec, cancellationToken);
        if (oldAuthor == null)
            throw new ValidationException($"Author with specified ID ({request.Id}) doesn't exist");

        var updatedAuthor = _mapper.Map(request, oldAuthor);
        _unitOfWork.AuthorRepository.Update(updatedAuthor);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
