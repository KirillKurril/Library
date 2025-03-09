using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateBookHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var bookSpec = new BookByIdSpecification(request.Id);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(bookSpec, cancellationToken);
            if (book == null)
                throw new ValidationException($"Book with specified ID ({request.Id}) does not exist");

            if(request.ISBN != null)
            {
                var isbnSpec = new UniqueIsbnCheckSpecification(request.Id, request.ISBN);
                if (await _unitOfWork.BookRepository.CountAsync(isbnSpec, cancellationToken) == 1)
                    throw new ValidationException("A book with this ISBN already exists");
            }

            if (request.GenreId != null)
            {
                var genreSpec = new GenreByIdSpecification(request.GenreId.Value);
                if (await _unitOfWork.GenreRepository.CountAsync(genreSpec, cancellationToken) == 0)
                    throw new ValidationException("Genre with specified ID does not exist");
            }

            if (request.AuthorId != null)
            {
                var authorSpec = new AuthorByIdSpecification(request.AuthorId.Value);
                if (await _unitOfWork.AuthorRepository.CountAsync(authorSpec, cancellationToken) == 0)
                    throw new ValidationException("Author with specified ID does not exist");
            }


            _mapper.Map(request, book);
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
