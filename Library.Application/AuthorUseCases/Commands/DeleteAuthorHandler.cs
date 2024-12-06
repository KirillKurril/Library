namespace Library.Application.AuthorUseCases.Commands;

public class DeleteAuthorHandler : IRequestHandler<DeleteAuthorCommand, Author>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAuthorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Author> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (author == null)
            throw new NotFoundException($"Author with ID {request.Id} not found");

        _unitOfWork.AuthorRepository.Delete(author);
        await _unitOfWork.SaveChangesAsync();
        
        return author;
    }
}
