namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookImageHandler : IRequestHandler<UpdateBookImageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookImageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateBookImageCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);
            if (book == null)
                throw new NotFoundException(request.BookId.ToString());

            book.ImageUrl = request.ImageUrl;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
