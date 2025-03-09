using Library.Application.GenreUseCases.Commands;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.GenreUseCases.Validators
{
    public class EditGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
    {
        public EditGenreCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Genre ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Genre name is required")
                .MaximumLength(100).WithMessage("Genre name must not exceed 100 characters");

        }

    }
}

