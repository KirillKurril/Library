using Library.Application.DTOs;

namespace Library.Application.AuthorUseCases.Commands;

public class CreateAuthorHandler : IRequestHandler<CreateAuthorCommand, CreateEntityResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAuthorHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateAuthorCommand> validator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CreateEntityResponse> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = _mapper.Map<Author>(request);
        var createdAuthor = _unitOfWork.AuthorRepository.Add(author);
        await _unitOfWork.SaveChangesAsync();

        return new CreateEntityResponse()
        {
            Id = createdAuthor.Id
        };

    }
}
