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
                .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.ReturnDate)
                .NotEmpty().WithMessage("Return date is required")
                .Must(returnDate => returnDate > DateTime.UtcNow)
                .WithMessage("Return date must be in the future");

            RuleFor(x => x)
                .MustAsync(async (command, ct) =>
                {
                    var borrowedBooks = await unitOfWork.BookRepository.ListAsync(
                        b => b.UserId == command.UserId && !b.IsAvailable, ct);
                    return borrowedBooks.Count < 5; // Максимум 5 книг на руках
                })
                .WithMessage("User cannot borrow more than 5 books at a time");
        }
    }
}
