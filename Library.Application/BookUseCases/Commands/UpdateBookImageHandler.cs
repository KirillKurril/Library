using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookImageHandler : IRequestHandler<UpdateBookImageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateBookImageCommand> _validator;

        public UpdateBookImageHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateBookImageCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task Handle(UpdateBookImageCommand request, CancellationToken cancellationToken)
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

            book.ImageUrl = request.ImageUrl;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
