using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;

namespace Library.Application.BookUseCases.Validators
{
    public class UpdateBookImageCommandValidator : AbstractValidator<UpdateBookImageCommand>
    {
        public UpdateBookImageCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(bookId);
                    return author != null;
                }).WithMessage("Author with specified ID does not exist");

            RuleFor(x => x.ImageUrl)
                .NotEmpty()
                .WithMessage("Image URL cannot be empty")
                .Must(BeAValidUrl)
                .WithMessage("A valid URL must be provided");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
