using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Specifications.AuthorSpecification;
using MediatR;
using System.Text.RegularExpressions;
using System.Threading;

namespace Library.Application.BookUseCases.Validators
{
    public class UpdateBookImageCommandValidator : AbstractValidator<UpdateBookImageCommand>
    {
        public UpdateBookImageCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image url is required")
                .MaximumLength(200).WithMessage("Image URL must not exceed 500 characters");

            RuleFor(x => x.ImageUrl)
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
