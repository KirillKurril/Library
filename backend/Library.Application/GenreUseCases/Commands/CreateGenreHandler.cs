using Library.Application.DTOs;

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
