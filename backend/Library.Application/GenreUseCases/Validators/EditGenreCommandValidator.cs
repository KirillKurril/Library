using Library.Application.GenreUseCases.Commands;

namespace Library.Application.GenreUseCases.Validators
{
    public class EditGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
    {
        public EditGenreCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Genre ID is required")
                .MustAsync(async (genreId, ct) =>
                {
                    var genre = await unitOfWork.GenreRepository.GetByIdAsync(genreId, ct);
                    return genre != null;
                }).WithMessage($"Genre being deleted doesn't exist");

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
}

