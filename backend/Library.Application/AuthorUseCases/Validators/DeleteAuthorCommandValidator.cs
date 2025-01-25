using Library.Application.AuthorUseCases.Commands;

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
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(authorId, ct);
                    return author != null;
                }).WithMessage($"Author being deleted doesn't exist")
                .MustAsync(async (authorId, ct) =>
                {
                    var hasBooks = await unitOfWork.BookRepository.FirstOrDefaultAsync(
                        b => b.AuthorId == authorId, ct);
                    return hasBooks == null;
                }).WithMessage($"Author being deleted must have no books");
    }
}
