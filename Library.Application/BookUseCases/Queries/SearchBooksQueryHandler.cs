using System.Linq.Expressions;

namespace Library.Application.BookUseCases.Queries
{
    public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, IEnumerable<Book>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchBooksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
        {
            var searchTerm = request.SearchTerm?.ToLower();
            
            return await _unitOfWork.BookRepository.ListAsync(
                book => (string.IsNullOrEmpty(searchTerm) || 
                        book.Title.ToLower().Contains(searchTerm) ||
                        book.Description.ToLower().Contains(searchTerm) ||
                        book.ISBN.ToLower().Contains(searchTerm)) &&
                       (request.Genre == null || book.Genre.ToString().ToLower() == request.Genre.ToString().ToLower())
                       && (!request.IsAvailable.HasValue || book.IsAvailable == request.IsAvailable.Value),
                cancellationToken);
        }
    }
}
