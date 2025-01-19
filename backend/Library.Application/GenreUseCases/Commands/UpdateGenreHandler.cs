using System.Reflection.Metadata;

namespace Library.Application.GenreUseCases.Commands
{
    public class UpdateGenreHandler : IRequestHandler<UpdateGenreCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateGenreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(request.id, cancellationToken);

            if (genre == null)
                throw new NotFoundException(request.id.ToString());

            _unitOfWork.GenreRepository.Update(new Genre(){ Id = request.id, Name = request.genreName });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
