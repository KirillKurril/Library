using MediatR;

namespace Library.Application.GenreUseCases.Commands
{
    public class UpdateGenreHandler : IRequestHandler<UpdateGenreCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateGenreHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            var existingGenre = await _unitOfWork.GenreRepository.GetByIdAsync(request.Id, cancellationToken);
            var updatedGenre = _mapper.Map(request, existingGenre);
            _unitOfWork.GenreRepository.Update(updatedGenre);
            await _unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
