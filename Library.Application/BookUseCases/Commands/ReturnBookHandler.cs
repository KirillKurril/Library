using System;
using System.Threading;
using System.Threading.Tasks;
using Library.Application.Common.Exceptions;
using Library.Domain.Abstractions;
using Library.Domain.Entities;
using MediatR;

namespace Library.Application.BookUseCases.Commands
{
    public class ReturnBookHandler : IRequestHandler<ReturnBookCommand, Book>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Book> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId);
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.BookId);
            }

            if (book.IsAvailable)
            {
                throw new ValidationException("Book is already returned");
            }

            book.IsAvailable = true;
            book.ActualReturnDate = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
