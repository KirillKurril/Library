namespace Library.Application.GenreUseCases.Commands
{
    public class DeleteGenreHandler : IRequestHandler<DeleteGenreCommand, Genre>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGenreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Genre> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(request.Id, cancellationToken);
            if (genre == null)
                throw new NotFoundException($"Genre with ID {request.Id} not found");

            await _unitOfWork.GenreRepository.DeleteAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return genre;
        }
    }
}
