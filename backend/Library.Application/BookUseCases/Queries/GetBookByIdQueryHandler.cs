using Library.Application.DTOs;
using Library.Domain.Specifications.AuthorSpecification;

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
            var spec = new BookByIdSpecification(request.Id);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(spec, cancellationToken);

            if (book == null)
                throw new NotFoundException(nameof(Book), $"ID: {request.Id}");

            BookDetailsDTO response = book.Adapt<BookDetailsDTO>();

            return response;
        }
    }
}
