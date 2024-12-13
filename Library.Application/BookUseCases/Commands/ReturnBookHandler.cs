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
            var lending = await _unitOfWork.BookLendingRepository.FirstOrDefaultAsync(
                bl => bl.UserId == request.UserId &&
                bl.BookId == request.BookId);

            _unitOfWork.BookLendingRepository.Delete(lending);

            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.UserId);
            book.Quantity += 1;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
