using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;

namespace Library.Application.BookUseCases.Validators
{
    public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
    {
        public ReturnBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var bookExist = await unitOfWork.BookLendingRepository
                    .FirstOrDefaultAsync(bl=> bl.BookId == bookId, ct);
                    return bookExist != null;
                }).WithMessage($"Current book hasn't been boeeowed by this user"); 
        }
    }
}