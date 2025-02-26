using Library.Application.AuthorUseCases.Commands;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;

namespace Library.Application.AuthorUseCases.Validators;

public class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Author ID is required")
                .MustAsync(async (authorId, ct) =>
                {
                    var spec = new AuthorByIdSpecification(authorId);
                    var exist = await unitOfWork.AuthorRepository.CountAsync(spec, ct);
                    return exist == 1;
                }).WithMessage($"Author being deleted doesn't exist")
                .MustAsync(async (authorId, ct) =>
                {
                    var spec = new BookCatalogSpecification(null, null, authorId, null);
                    var exist = await unitOfWork.BookRepository.CountAsync(spec, ct);
                    return exist == 0;
                }).WithMessage($"Author being deleted must have no books");
    }
}
