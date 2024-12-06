namespace Library.Application.BookUseCases.Commands
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.Id);
            
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.Id);
            }

            if (!book.IsAvailable)
            {
                throw new ValidationException($"Cannot delete a book ({book.Id}) that is currently borrowed");
            }

            _unitOfWork.BookRepository.Delete(book);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
