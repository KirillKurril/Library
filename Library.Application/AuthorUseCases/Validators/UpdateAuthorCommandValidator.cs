using Library.Application.AuthorUseCases.Commands;
using Library.Domain.Abstractions;

namespace Library.Application.AuthorUseCases.Validators;

public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Author ID is required")
                .MustAsync(async (authorId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(authorId);
                    return author != null;
                }).WithMessage($"Book being updated doesn't exist");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}
