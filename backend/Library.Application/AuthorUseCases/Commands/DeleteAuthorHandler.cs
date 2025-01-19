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
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id, cancellationToken);

        _unitOfWork.AuthorRepository.Delete(author);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
