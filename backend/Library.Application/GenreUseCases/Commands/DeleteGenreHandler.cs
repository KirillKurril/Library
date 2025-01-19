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
            var genre = await _unitOfWork.GenreRepository.GetByIdAsync(request.Id, cancellationToken);

            _unitOfWork.GenreRepository.Delete(genre);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
