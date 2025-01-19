using Library.Application.BookUseCases.Commands;
using Library.Application.Common.Interfaces;

namespace Library.Application.BookUseCases.Validators
{
    public class LendBookCommandValidator : AbstractValidator<LendBookCommand>
    {
        public LendBookCommandValidator(
            IUnitOfWork unitOfWork,
            IUserDataAccessor userDataAccessor)
        {
            RuleFor(x => x.BookId)
                 .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.BookId)
                .MustAsync(async (bookId, ct) =>
                {
                    var book = await unitOfWork.BookRepository
                        .GetByIdAsync(bookId, ct);

                    return book != null && book.IsAvailable;
                }).WithMessage("Book is not available for borrowing or does not exist.")
                .When(x => x.BookId != Guid.Empty);

            RuleFor(x => x.UserId)
                .MustAsync(async (userId, ct) =>
                {
                    var userExist = await userDataAccessor.UserExist(userId);
                    return userExist;
                }).WithMessage($"User doesn't exist");
        }
    }
}
