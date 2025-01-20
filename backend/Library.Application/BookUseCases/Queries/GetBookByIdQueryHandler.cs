using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDetailsDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBookByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BookDetailsDTO> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.BookRepository.GetByIdAsync(
                request.Id,
                cancellationToken,
                b => b.Author,
                b => b.Genre);

            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.Id);
            }

            BookDetailsDTO response = book.Adapt<BookDetailsDTO>();

            return response;
        }
    }
}
