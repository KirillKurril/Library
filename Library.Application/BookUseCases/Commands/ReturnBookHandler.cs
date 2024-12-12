using Library.Application.Common.Interfaces;
namespace Library.Application.BookUseCases.Commands
{
    public class ReturnBookHandler : IRequestHandler<ReturnBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ReturnBookCommand> _validator;
        private readonly IUserDataAccessor _userDataService;
        private readonly ILibrarySettings _librarySettings;


        public ReturnBookHandler(
            IUnitOfWork unitOfWork,
            IValidator<ReturnBookCommand> validator,
            IUserDataAccessor userDataService,
            ILibrarySettings librarySettings)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userDataService = userDataService;
            _librarySettings = librarySettings;
        }

        public async Task Handle(ReturnBookCommand request, CancellationToken cancellationToken)
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

            var userExists = await _userDataService.UserExist(request.UserId);
            if (!userExists)
            {
                throw new NotFoundException($"User with id {request.UserId} doesn' exist");
            }

            var lending = await _unitOfWork.BookLendingRepository.FirstOrDefaultAsync(
                bl => bl.UserId == request.UserId &&
                bl.BookId == request.BookId
                );

            if (lending == null)
            {
                throw new NotFoundException($"Book ({book.Id}) has not been borrowed");
            }

            _unitOfWork.BookLendingRepository.Delete(lending);
            book.Quantity += 1;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
