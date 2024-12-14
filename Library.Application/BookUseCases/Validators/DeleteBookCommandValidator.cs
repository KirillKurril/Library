using Library.Application.BookUseCases.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.BookUseCases.Validators
{
    public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var book = await unitOfWork.BookRepository.GetByIdAsync(bookId, ct);
                    return book != null;
                }).WithMessage($"Book being deleted doesn't exist");
        }
    }
}
