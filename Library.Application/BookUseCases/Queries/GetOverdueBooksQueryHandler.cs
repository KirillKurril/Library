namespace Library.Application.BookUseCases.Queries
{
    public class GetOverdueBooksQueryHandler : IRequestHandler<GetOverdueBooksQuery, IEnumerable<Book>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOverdueBooksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> Handle(GetOverdueBooksQuery request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow;
            return await _unitOfWork.BookRepository.ListAsync(
                book => !book.IsAvailable && book.ReturnDate < currentDate,
                cancellationToken);
        }
    }
}
