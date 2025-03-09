using Library.Application.GenreUseCases.Commands;

namespace Library.Application.GenreUseCases.Validators;

public class DeleteGenreCommandValidator : AbstractValidator<DeleteGenreCommand>
{
    public DeleteGenreCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Genre ID is required");
    }
}
