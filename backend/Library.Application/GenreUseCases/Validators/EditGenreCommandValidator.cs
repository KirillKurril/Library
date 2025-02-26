using Library.Application.GenreUseCases.Commands;
using Library.Domain.Entities;
using Library.Domain.Specifications.GenreSpecification;

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
                    var spec = new GenreByIdSpecification(genreId);
                    var exist = await unitOfWork.GenreRepository.CountAsync(spec);
                    return exist == 1;
                }).WithMessage($"Genre being deleted doesn't exist");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Genre name is required")
                .MaximumLength(100).WithMessage("Genre name must not exceed 100 characters")
                .MustAsync(async (name, ct) =>
                {
                    var spec = new GenreFiltredListCountSpecification(name);
                    var exist = await unitOfWork.GenreRepository.CountAsync(spec);
                    return exist == 0;
                }).WithMessage("A genre with this name already exists");
        }

    }
}

