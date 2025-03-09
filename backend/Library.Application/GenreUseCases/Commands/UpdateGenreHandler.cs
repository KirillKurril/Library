using Library.Domain.Specifications.GenreSpecification;
using System.Xml.Linq;

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
            var oldGenreSpec = new GenreByIdSpecification(request.Id);
            var filtredGenreSpec = new GenreFiltredListCountSpecification(request.Name);

            var oldGenre = await _unitOfWork.GenreRepository.FirstOrDefault(oldGenreSpec, cancellationToken);
            if(oldGenre == null)
                throw new ValidationException($"Genre with specified ID ({request.Id}) doesn't exist");

            if(await _unitOfWork.GenreRepository.CountAsync(filtredGenreSpec, cancellationToken) != 0)
                throw new ValidationException("A genre with this name already exists");

            var updatedGenre = _mapper.Map(request, oldGenre);
            _unitOfWork.GenreRepository.Update(updatedGenre);
            await _unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
