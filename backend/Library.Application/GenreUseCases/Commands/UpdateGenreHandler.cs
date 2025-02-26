using Library.Domain.Specifications.GenreSpecification;
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
            var spec = new GenreByIdSpecification(request.Id);
            var existingGenre = await _unitOfWork.GenreRepository.FirstOrDefault(spec);

            var updatedGenre = _mapper.Map(request, existingGenre);
            _unitOfWork.GenreRepository.Update(updatedGenre);
            await _unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
