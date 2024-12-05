namespace Library.Application.BookUseCases.Queries
{
    public class GetBookByIsbnQueryHandler : IRequestHandler<GetBookByIsbnQuery, Book>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBookByIsbnQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Book> Handle(GetBookByIsbnQuery request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.FirstOrDefault(
                b => b.ISBN == request.ISBN,
                cancellationToken);

            if (book == null)
            {
                throw new NotFoundException(nameof(Book), $"ISBN: {request.ISBN}");
            }

            return book;
        }
    }
}
