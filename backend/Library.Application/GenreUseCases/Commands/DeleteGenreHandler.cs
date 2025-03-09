using Library.Domain.Abstractions;
using Library.Domain.Entities;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.GenreUseCases.Commands
{
    public class DeleteGenreHandler : IRequestHandler<DeleteGenreCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGenreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {
            var genreSpec = new GenreByIdSpecification(request.Id);
            var bookSpec = new BookCatalogCountSpecification(null, request.Id, null, null);


            var genre = await _unitOfWork.GenreRepository.FirstOrDefault(genreSpec);
            if (genre == null)
                throw new ValidationException($"Genre with specified ID ({request.Id}) does not exist");

            if (await _unitOfWork.BookRepository.CountAsync(bookSpec) != 0)
                throw new ValidationException($"No books should belong to the genre being removed ");


            _unitOfWork.GenreRepository.Delete(genre);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
