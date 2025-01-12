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
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(request.Id, cancellationToken);
            if (genre == null)
                throw new NotFoundException($"Genre with ID {request.Id} not found");

            return genre;
        }
    }
}
