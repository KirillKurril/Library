using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;

namespace Library.Application.BookUseCases.Validators
{
    public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
    {
        public ReturnBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x)
               .MustAsync(async (command, ct) =>
               {
                   var lending = await unitOfWork.BookLendingRepository
                       .FirstOrDefaultAsync(
                           bl => bl.UserId == command.UserId && bl.BookId == command.BookId, ct);
                   return lending != null;
               }).WithMessage("Book has not been borrowed by this user.");
        }
    }
}