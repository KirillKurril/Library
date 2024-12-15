using Library.Application.GenreUseCases.Commands;

namespace Library.Application.GenreUseCases.Validators;

public class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Genre name is required")
            .MaximumLength(100).WithMessage("Genre name must not exceed 100 characters")
            .MustAsync(async (name, ct) =>
            {
                var genre = await unitOfWork.GenreRepository.FirstOrDefaultAsync(g => g.Name == name, ct);
                return genre == null;
            }).WithMessage("A genre with this name already exists");
    }
}
