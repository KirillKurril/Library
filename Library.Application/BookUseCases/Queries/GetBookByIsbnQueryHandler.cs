using Library.Application.DTOs;

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
            var book = await _unitOfWork.BookRepository.FirstOrDefaultAsync(
                b => b.ISBN == request.ISBN,
                cancellationToken);

            if (book == null)
            {
                throw new NotFoundException(nameof(Book), $"ISBN: {request.ISBN}");
            }

            BookDetailsDTO response = book.Adapt<BookDetailsDTO>();

            return response;
        }
    }
}
