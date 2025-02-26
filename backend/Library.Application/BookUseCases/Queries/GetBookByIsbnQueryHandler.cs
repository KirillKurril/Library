using Library.Application.DTOs;
using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.BookUseCases.Queries
{
    public class GetBookByIsbnQueryHandler : IRequestHandler<GetBookByIsbnQuery, BookDetailsDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBookByIsbnQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BookDetailsDTO> Handle(GetBookByIsbnQuery request, CancellationToken cancellationToken)
        {
            var spec = new BookByIsbnSpecification(request.ISBN);
            var book = _unitOfWork.BookRepository.FirstOrDefault(spec, cancellationToken);
            
            if (book == null)
                throw new NotFoundException(nameof(Book), $"ISBN: {request.ISBN}");

            return book.Adapt<BookDetailsDTO>();
        }
    }
}
