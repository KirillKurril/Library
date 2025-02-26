using Library.Application.GenreUseCases.Commands;
using Library.Domain.Specifications.BookSpecifications;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.GenreUseCases.Validators;

public class DeleteGenreCommandValidator : AbstractValidator<DeleteGenreCommand>
{
    public DeleteGenreCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Genre ID is required")
            .MustAsync(async (genreId, ct) =>
            {
                var spec = new GenreByIdSpecification(genreId);
                var exist = await unitOfWork.GenreRepository.CountAsync(spec);
                return exist == 0;
            }).WithMessage($"Genre being deleted doesn't exist")
            .MustAsync(async (genreId, ct) =>
            {
                var spec = new BookCatalogCountSpecification(null, genreId, null, null);
                var exist = await unitOfWork.BookRepository.CountAsync(spec);
                return exist == 0;
            }).WithMessage($"No books should belong to the genre being removed ");
    }
}
