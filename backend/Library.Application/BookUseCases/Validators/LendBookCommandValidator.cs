using Library.Application.BookUseCases.Commands;

namespace Library.Application.BookUseCases.Validators
{
    public class LendBookCommandValidator : AbstractValidator<LendBookCommand>
    {
        public LendBookCommandValidator(IUnitOfWork unitOfWork, Common.Interfaces.IUserDataAccessor @object)
        {
            RuleFor(x => x.BookId)
                 .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.UserId)
                 .NotEmpty().WithMessage("User ID is required");
        }
    }
}
