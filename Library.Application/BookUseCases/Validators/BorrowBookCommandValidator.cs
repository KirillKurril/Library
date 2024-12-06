using System;
using FluentValidation;
using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using System.Linq;

namespace Library.Application.BookUseCases.Validators
{
    public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
    {
        public BorrowBookCommandValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
