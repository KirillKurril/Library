using Library.Application.DTOs;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.BookUseCases.Commands
{
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, CreateEntityResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateEntityResponse> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var isbnSpec = new BookByIsbnSpecification(request.ISBN);
            var genreSpec = new GenreByIdSpecification(request.GenreId);
            var authorSpec = new AuthorByIdSpecification(request.AuthorId);

            if( await _unitOfWork.BookRepository.CountAsync(isbnSpec, cancellationToken) == 1)
                throw new ValidationException("A book with this ISBN already exists");

            if (await _unitOfWork.GenreRepository.CountAsync(genreSpec, cancellationToken) == 0)
                throw new ValidationException("Genre with specified ID does not exist");

            if (await _unitOfWork.AuthorRepository.CountAsync(authorSpec, cancellationToken) == 0)
                throw new ValidationException("Author with specified ID does not exist");


            var bookToCreate = request.Adapt<Book>();

            var createdBook = _unitOfWork.BookRepository.Add(bookToCreate);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateEntityResponse()
            {
                Id = createdBook.Id,
            };

            return response;
        }
    }
}
