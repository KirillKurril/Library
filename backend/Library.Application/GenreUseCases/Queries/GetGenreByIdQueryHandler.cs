using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.GenreUseCases.Queries
{
    public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Genre>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGenreByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Genre> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GenreByIdSpecification(request.Id);
            var genre = await _unitOfWork.GenreRepository.FirstOrDefault(spec, cancellationToken);

            if (genre == null)
                throw new NotFoundException(nameof(Genre), $"ID: {request.Id}");

            return genre;
        }
    }
}
