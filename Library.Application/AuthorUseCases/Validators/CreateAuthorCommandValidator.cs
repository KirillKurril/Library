using Library.Application.AuthorUseCases.Commands;

namespace Library.Application.AuthorUseCases.Validators;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Surname)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Surname));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow)
            .When(x => x.BirthDate != null);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Surname));
    }
}
