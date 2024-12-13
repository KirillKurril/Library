using Library.Application.GenreUseCases.Commands;

namespace Library.Application.GenreUseCases.Validators;

public class DeleteGenreCommandValidator : AbstractValidator<DeleteGenreCommand>
{
    public DeleteGenreCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Genre ID is required")
            .MustAsync(async (genreId, ct) =>
            {
                var genre = await unitOfWork.GenreRepository.GetByIdAsync(genreId, ct);
                return genre != null;
            }).WithMessage($"Genre being deleted doesn't exist");
    }
}
