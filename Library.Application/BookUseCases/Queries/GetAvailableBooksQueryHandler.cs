namespace Library.Application.BookUseCases.Queries
{
    public class GetAvailableBooksQueryHandler : IRequestHandler<GetAvailableBooksQuery, IEnumerable<Book>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableBooksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> Handle(GetAvailableBooksQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BookRepository.ListAsync(b => b.IsAvailable, cancellationToken);
        }
    }
}
