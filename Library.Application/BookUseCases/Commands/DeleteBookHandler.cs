namespace Library.Application.BookUseCases.Commands
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, Book>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Book> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.Id);
            
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.Id);
            }

            if (!book.IsAvailable)
            {
                throw new ValidationException("Cannot delete a book that is currently borrowed");
            }

            await _unitOfWork.BookRepository.DeleteAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
