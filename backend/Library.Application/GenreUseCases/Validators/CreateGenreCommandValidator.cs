using Library.Application.GenreUseCases.Commands;
using Library.Domain.Entities;
using Library.Domain.Specifications.GenreSpecification;

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
                var spec = new GenreFiltredListCountSpecification(name);
                var exist = await unitOfWork.GenreRepository.CountAsync(spec);
                return exist == 0;
            }).WithMessage("A genre with this name already exists");
    }
}
