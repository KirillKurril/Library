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
                    var bookExist = await unitOfWork.BookLendingRepository
                    .FirstOrDefaultAsync(bl => bl.BookId == bookId, ct);
                    return bookExist != null;
                }).WithMessage($"Current book hasn't been boeeowed by this user");
        }
    }
}
