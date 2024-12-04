namespace Library.Application.BookUseCases.Queries
{
    public class GetBorrowedBooksQueryHandler : IRequestHandler<GetBorrowedBooksQuery, IEnumerable<Book>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBorrowedBooksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> Handle(GetBorrowedBooksQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BookRepository.ListAsync(
                book => book.UserId == request.UserId && !book.IsAvailable,
                cancellationToken);
        }
    }
}
