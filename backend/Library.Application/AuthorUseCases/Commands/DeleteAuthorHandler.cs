using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Domain.Specifications;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;

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
        var authorSpec = new AuthorByIdSpecification(request.Id);
        var bookSpec = new BookCatalogSpecification(null, null, request.Id, null);

        var author = await _unitOfWork.AuthorRepository.FirstOrDefault(authorSpec, cancellationToken);
        if (author == null)
            throw new ValidationException($"Author with specified ID ({request.Id}) doesn't exist");

        if (await _unitOfWork.BookRepository.CountAsync(bookSpec, cancellationToken) != 0)
            throw new ValidationException($"Author being deleted must have no books");

        
        _unitOfWork.AuthorRepository.Delete(author);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
