using Library.Application.AuthorUseCases.Commands;
using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.AuthorUseCases.Validators;

public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Author ID is required");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Name)); ;

        RuleFor(x => x.Surname)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Surname));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow)
            .When(x => x.BirthDate != null);

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Country));
    }
}
