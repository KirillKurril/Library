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
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);

            book.ImageUrl = request.ImageUrl;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
