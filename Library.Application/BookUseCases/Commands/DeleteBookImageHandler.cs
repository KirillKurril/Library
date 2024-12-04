namespace Library.Application.BookUseCases.Commands
{
    public class DeleteBookImageHandler : IRequestHandler<DeleteBookImageCommand, Book>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookImageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Book> Handle(DeleteBookImageCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);
            
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.BookId);
            }

            book.ImageUrl = null;
            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
