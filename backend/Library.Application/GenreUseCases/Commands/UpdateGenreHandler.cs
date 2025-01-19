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
            _unitOfWork.GenreRepository.Update(request.Adapt<Genre>());
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
