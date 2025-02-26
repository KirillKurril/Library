using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Specifications.BookSpecifications;

namespace Library.Application.BookUseCases.Validators
{
    public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
    {
        public ReturnBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x)
               .MustAsync(async (command, ct) =>
               {
                   var spec = new BookLendingByBookIdUserIdSpecification(command.BookId, command.UserId);
                   var exist = await unitOfWork.BookLendingRepository.CountAsync(spec, ct);
                   return exist == 1;
               }).WithMessage("Book has not been borrowed by this user.");
        }
    }
}