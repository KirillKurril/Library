using Library.Application.Common.Interfaces;
using Library.Domain.Abstractions;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
namespace Library.Application.BookUseCases.Commands
{
    public class ReturnBookHandler : IRequestHandler<ReturnBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;


        public ReturnBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var blSpec = new BookLendingByBookIdUserIdSpecification(request.BookId, request.UserId);
            var bookSpec = new BookByIdSpecification(request.BookId);

            var bookLending = await _unitOfWork.BookLendingRepository.FirstOrDefault(blSpec, cancellationToken);

            if (bookLending == null)
                throw new ValidationException($"Book {request.BookId} has not been borrowed by user {request.UserId}");


            _unitOfWork.BookLendingRepository.Delete(bookLending);

            var book = await _unitOfWork.BookRepository.FirstOrDefault(bookSpec, cancellationToken);

            book.Quantity += 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
