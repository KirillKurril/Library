using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Library.Application.Common.Exceptions;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using MediatR;

namespace Library.Application.BookUseCases.Commands
{
    public class BorrowBookHandler : IRequestHandler<BorrowBookCommand, Book>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BorrowBookCommand> _validator;

        public BorrowBookHandler(
            IUnitOfWork unitOfWork,
            IValidator<BorrowBookCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Book> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.BookId);
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            if (!book.IsAvailable)
            {
                throw new ValidationException("Book is not available for borrowing");
            }

            book.IsAvailable = false;
            book.UserId = request.UserId;
            book.BorrowedAt = DateTime.UtcNow;
            book.ReturnDate = request.ReturnDate;
            book.ActualReturnDate = null;

            await _unitOfWork.BookRepository.UpdateAsync(book, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
