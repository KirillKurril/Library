using Library.Application.AuthorUseCases.Commands;

namespace Library.Application.AuthorUseCases.Validators;

public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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
