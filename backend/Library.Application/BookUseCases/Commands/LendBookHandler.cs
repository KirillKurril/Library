using Library.Application.Common.Interfaces;
using Library.Domain.Abstractions;
using Library.Domain.Specifications.BookSpecifications;

namespace Library.Application.BookUseCases.Commands
{
    public class LendBookHandler : IRequestHandler<LendBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILibrarySettings _librarySettings;
        private readonly IUserDataAccessor _userDataAccessor;

        public LendBookHandler(
            IUnitOfWork unitOfWork,
            ILibrarySettings librarySettings,
            IUserDataAccessor userDataAccessor)
        {
            _unitOfWork = unitOfWork;
            _librarySettings = librarySettings;
            _userDataAccessor = userDataAccessor;
        }

        public async Task<Unit> Handle(LendBookCommand request, CancellationToken cancellationToken)
        {
            var bookAvailableSpec = new BookExistAndAvailableSpecification(request.BookId);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(bookAvailableSpec, cancellationToken);
            
            if (book == null)
                throw new ValidationException("Book is not available for borrowing or does not exist.");

            if(!await _userDataAccessor.UserExist(request.UserId))
                throw new ValidationException($"User {request.UserId} doesn't exist");


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

            book.Quantity -= 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
