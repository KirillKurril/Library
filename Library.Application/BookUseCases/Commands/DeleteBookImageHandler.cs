namespace Library.Application.BookUseCases.Commands
{
    public class DeleteBookImageHandler : IRequestHandler<DeleteBookImageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteBookImageCommand> _validator;

        public DeleteBookImageHandler(
            IUnitOfWork unitOfWork,
            IValidator<DeleteBookImageCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;

        }

        public async Task Handle(DeleteBookImageCommand request, CancellationToken cancellationToken)
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

            book.ImageUrl = null;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
