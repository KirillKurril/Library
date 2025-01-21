namespace Library.Application.GenreUseCases.Commands
{
    public class UpdateGenreHandler : IRequestHandler<UpdateGenreCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateGenreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            var updatedGenre = request.Adapt<Genre>();
            _unitOfWork.GenreRepository.Update(updatedGenre);
            await _unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
