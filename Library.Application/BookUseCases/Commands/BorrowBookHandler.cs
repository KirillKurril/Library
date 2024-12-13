using Library.Application.Common.Interfaces;

namespace Library.Application.BookUseCases.Commands
{
    public class BorrowBookHandler : IRequestHandler<BorrowBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BorrowBookCommand> _validator;
        private readonly IUserDataAccessor _userDataAccessor;
        private readonly ILibrarySettings _librarySettings;

        public BorrowBookHandler(
            IUnitOfWork unitOfWork,
            IValidator<BorrowBookCommand> validator,
            IUserDataAccessor userDataAccessor,
            ILibrarySettings librarySettings)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userDataAccessor = userDataAccessor;
            _librarySettings = librarySettings;
        }

        public async Task Handle(BorrowBookCommand request, CancellationToken cancellationToken)
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

            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.UserId);
            book.Quantity -= 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();


        }
    }
}
