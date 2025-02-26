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
        var spec = new AuthorByIdSpecification(request.Id);
        var existingAuthor = await _unitOfWork.AuthorRepository.FirstOrDefault(spec, cancellationToken);

        var updatedAuthor = _mapper.Map(request, existingAuthor);
        _unitOfWork.AuthorRepository.Update(updatedAuthor);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
