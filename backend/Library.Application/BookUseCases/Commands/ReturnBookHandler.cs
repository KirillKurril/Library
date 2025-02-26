using Library.Application.Common.Interfaces;
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
            var lendingSpec = new BookLendingByBookIdUserIdSpecification(request.BookId, request.UserId);
            var lending = await _unitOfWork.BookLendingRepository.FirstOrDefault(lendingSpec, cancellationToken);

            if (lending == null)
                throw new NotFoundException($"Lending bookId: {request.BookId}, userId: {request.UserId}");

            _unitOfWork.BookLendingRepository.Delete(lending);

            var bookSpec = new BookByIdSpecification(request.BookId);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(bookSpec, cancellationToken);

            book.Quantity += 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
