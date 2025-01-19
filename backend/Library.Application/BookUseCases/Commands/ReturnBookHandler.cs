using Library.Application.Common.Interfaces;
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
            var lending = await _unitOfWork.BookLendingRepository.FirstOrDefaultAsync(
                bl => bl.UserId == request.UserId &&
                bl.BookId == request.BookId);

            if (lending == null)
                throw new NotFoundException($"Lending bookId: {request.BookId}, userId: {request.UserId}");

            _unitOfWork.BookLendingRepository.Delete(lending);

            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId);

            book.Quantity += 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
