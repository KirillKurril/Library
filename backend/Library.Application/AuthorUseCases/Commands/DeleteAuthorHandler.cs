namespace Library.Application.AuthorUseCases.Commands;

public class DeleteAuthorHandler : IRequestHandler<DeleteAuthorCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAuthorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (author == null)
            throw new NotFoundException(request.Id.ToString());


        _unitOfWork.AuthorRepository.Delete(author);
        await _unitOfWork.SaveChangesAsync();
    }
}
