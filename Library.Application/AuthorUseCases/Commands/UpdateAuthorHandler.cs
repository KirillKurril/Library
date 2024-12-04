namespace Library.Application.AuthorUseCases.Commands;

public class UpdateAuthorHandler : IRequestHandler<UpdateAuthorCommand, Author>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateAuthorCommand> _validator;
    private readonly IMapper _mapper;

    public UpdateAuthorHandler(
        IUnitOfWork unitOfWork,
        IValidator<UpdateAuthorCommand> validator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Author> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingAuthor = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingAuthor == null)
            throw new NotFoundException($"Author with ID {request.Id} not found");

        var updatedAuthor = _mapper.Map(request, existingAuthor);
        await _unitOfWork.AuthorRepository.UpdateAsync(updatedAuthor, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        
        return updatedAuthor;
    }
}
