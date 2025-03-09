using Library.Application.DTOs;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.GenreUseCases.Commands
{
    public class CreateGenreHandler : IRequestHandler<CreateGenreCommand, CreateEntityResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateGenreHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateEntityResponse> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
           var spec = new GenreFiltredListCountSpecification(request.Name);
           if (await _unitOfWork.GenreRepository.CountAsync(spec) != 0)
              throw new ValidationException("A genre with this name already exists");

            var genre = _mapper.Map<Genre>(request);
            var createdGenre = _unitOfWork.GenreRepository.Add(genre);
            await _unitOfWork.SaveChangesAsync();
            
            return new CreateEntityResponse()
            {
                Id = createdGenre.Id,
            };
        }
    }
}
