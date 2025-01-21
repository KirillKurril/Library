using Library.Application.BookUseCases.Commands;

namespace Library.Application.BookUseCases.Validators
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .MaximumLength(17).WithMessage("ISBN must not exceed 17 characters")
                .Matches(@"^(?=(?:\d[-\s]?){9}[\dX]$|(?:\d[-\s]?){13}$)(?:97[89][-\s]?\d{1,5}[-\s]?\d+[-\s]?\d+[-\s]?\d$|\d{1,5}[-\s]?\d+[-\s]?\d+[-\s]?[\dX]$)")
                .WithMessage("Invalid ISBN format");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .MustAsync(async (isbn, ct) =>
                {
                    var book = await unitOfWork.BookRepository.FirstOrDefaultAsync(b => b.ISBN == isbn, ct);
                    return book == null;
                }).WithMessage("A book with this ISBN already exists");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity is required")
                .GreaterThan(0);

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.GenreId)
                .NotEmpty().WithMessage("Genre is required")
                .MustAsync(async (genreId, ct) =>
                {
                    var genre = await unitOfWork.GenreRepository.GetByIdAsync(genreId);
                    return genre != null;
                }).WithMessage("Genre with specified ID does not exist");


            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Author ID is required")
                .MustAsync(async (authorId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(authorId);
                    return author != null;
                }).WithMessage("Author with specified ID does not exist");
        }
    }
}
