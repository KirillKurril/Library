using Library.Application.BookUseCases.Commands;
using MediatR;
using System.Text.RegularExpressions;

namespace Library.Application.BookUseCases.Validators
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var book = await unitOfWork.BookRepository.GetByIdAsync(bookId);
                    return book != null;
                }).WithMessage($"Book being updated doesn't exist");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .MaximumLength(17).WithMessage("ISBN must not exceed 17 characters")
                .Matches(@"^(?=(?:\d[-\s]?){9}[\dX]$|(?:\d[-\s]?){13}$)(?:97[89][-\s]?\d{1,5}[-\s]?\d+[-\s]?\d+[-\s]?\d$|\d{1,5}[-\s]?\d+[-\s]?\d+[-\s]?[\dX]$)")
                .WithMessage("Invalid ISBN format")
                .When(x => x.ISBN != null);

            RuleFor(x => x)
                .MustAsync(async (br, ct) =>
                {
                    var book = await unitOfWork.BookRepository
                        .FirstOrDefaultAsync(b => b.ISBN == br.ISBN && b.Id != br.Id, ct);
                    return book == null;
                })
                .WithMessage("Book with such ISBN already exists")
                .When(x => !string.IsNullOrEmpty(x.ISBN));

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .When(x => x.Title != null);

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity is required")
                .When(x => x.Quantity != null);

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
                .When(x => x.Description != null); ;

            RuleFor(x => x.AuthorId)
                .MustAsync(async (authorId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(authorId.Value);
                    return author != null;
                }).WithMessage("Author with specified ID does not exist")
                .When(x => x.AuthorId != null);

            RuleFor(x => x.GenreId)
                .MustAsync(async (genreId, ct) =>
                {
                    var genre = await unitOfWork.GenreRepository.GetByIdAsync(genreId.Value);
                    return genre != null;
                }).WithMessage("Genre with specified ID does not exist")
                .When(x => x.GenreId != null);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(200).WithMessage("Image URL must not exceed 500 characters")
                .Must(BeAValidUrl)
                .WithMessage("A valid URL must be provided")
                .When(x => x.ImageUrl != null); 
        }
        private bool BeAValidUrl(string url)
        {
            string pattern = @"^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$";
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }
    }
}
