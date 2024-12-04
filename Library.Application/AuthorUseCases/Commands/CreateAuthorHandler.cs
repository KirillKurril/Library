namespace Library.Application.AuthorUseCases.Commands;

public class CreateAuthorHandler : IRequestHandler<CreateAuthorCommand, Author>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAuthorCommand> _validator;
    private readonly IMapper _mapper;

    public CreateAuthorHandler(
        IUnitOfWork unitOfWork,
        IValidator<CreateAuthorCommand> validator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Author> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var author = _mapper.Map<Author>(request);
        await _unitOfWork.AuthorRepository.AddAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        
        return author;
    }
}
