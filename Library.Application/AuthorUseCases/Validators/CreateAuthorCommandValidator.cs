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
