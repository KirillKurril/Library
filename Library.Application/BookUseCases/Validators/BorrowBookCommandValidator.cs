using System;
using FluentValidation;
using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using System.Linq;

namespace Library.Application.BookUseCases.Validators
{
    public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
    {
        public BorrowBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var book = await unitOfWork.BookRepository
                    .FirstOrDefaultAsync(b => b.Id == bookId, ct);

                    return book != null && book.IsAvailable;
                }).WithMessage("Book is not available for borrowing or does not exist.");
        }
    }
}
