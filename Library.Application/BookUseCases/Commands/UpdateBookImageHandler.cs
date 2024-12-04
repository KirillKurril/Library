namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookImageHandler : IRequestHandler<UpdateBookImageCommand, Book>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookImageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Book> Handle(UpdateBookImageCommand request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.BookId, cancellationToken);
            
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.BookId);
            }

            book.ImageUrl = request.ImageUrl;
            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
