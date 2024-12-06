using Library.Application.BookUseCases.Commands;

namespace Library.Application.BookUseCases.Validators
{
    public class UpdateBookImageCommandValidator : AbstractValidator<UpdateBookImageCommand>
    {
        public UpdateBookImageCommandValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0)
                .WithMessage("Book ID must be greater than 0");

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
