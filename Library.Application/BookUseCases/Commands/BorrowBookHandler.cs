using Library.Application.Common.Interfaces;

namespace Library.Application.BookUseCases.Commands
{
    public class BorrowBookHandler : IRequestHandler<BorrowBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BorrowBookCommand> _validator;
        private readonly IUserService _userService;
        private readonly ILibrarySettings _librarySettings;

        public BorrowBookHandler(
            IUnitOfWork unitOfWork,
            IValidator<BorrowBookCommand> validator,
            IUserService userService,
            ILibrarySettings librarySettings)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userService = userService;
            _librarySettings = librarySettings;
        }

        public async Task Handle(BorrowBookCommand request, CancellationToken cancellationToken)
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

            var userExists = await _userService.UserExistsAsync(request.UserId);
            if (!userExists)
            {
                throw new NotFoundException($"User with id {request.UserId} doesn' exist");
            }

            if (!book.IsAvailable)
            {
                throw new ValidationException($"Book ({request.BookId}) is not available for borrowing");
            }

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

            await _unitOfWork.SaveChangesAsync();


        }
    }
}
