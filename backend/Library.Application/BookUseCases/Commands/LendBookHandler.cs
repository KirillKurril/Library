using Library.Application.Common.Interfaces;
using Library.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Library.Application.BookUseCases.Commands
{
    public class LendBookHandler : IRequestHandler<LendBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILibrarySettings _librarySettings;

        public LendBookHandler(
            IUnitOfWork unitOfWork,
            ILibrarySettings librarySettings)
        {
            _unitOfWork = unitOfWork;
            _librarySettings = librarySettings;
        }

        public async Task<Unit> Handle(LendBookCommand request, CancellationToken cancellationToken)
        {
            var dateNow = DateTime.UtcNow;
            var loanPeriod = _librarySettings.DefaultLoanPeriodInDays;
            var returnDate = dateNow.AddDays(loanPeriod);
            var lending = new BookLending()
            {
                BookId = request.BookId,
                UserId = request.UserId,
                BorrowedAt = dateNow,
                ReturnDate = returnDate
            };

            _unitOfWork.BookLendingRepository.Add(lending);

            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);
            
            book.Quantity -= 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
