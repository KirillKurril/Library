using Library.Application.BookUseCases.Commands;

namespace Library.Application.BookUseCases.Validators
{
    public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
    {
        public ReturnBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}