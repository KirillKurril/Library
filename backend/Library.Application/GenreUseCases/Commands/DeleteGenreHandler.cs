namespace Library.Application.GenreUseCases.Commands
{
    public class DeleteGenreHandler : IRequestHandler<DeleteGenreCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGenreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(request.Id, cancellationToken);

            if (genre == null)
                throw new NotFoundException(request.Id.ToString());

            _unitOfWork.GenreRepository.Delete(genre);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
