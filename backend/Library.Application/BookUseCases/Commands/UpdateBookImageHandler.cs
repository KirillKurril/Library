namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookImageHandler : IRequestHandler<UpdateBookImageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookImageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateBookImageCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);

            book.ImageUrl = request.ImageUrl;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
