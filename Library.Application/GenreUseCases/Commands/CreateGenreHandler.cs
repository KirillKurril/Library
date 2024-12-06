using Library.Application.DTOs;

namespace Library.Application.GenreUseCases.Commands
{
    public class CreateGenreHandler : IRequestHandler<CreateGenreCommand, CreateEntityResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateGenreCommand> _validator;
        private readonly IMapper _mapper;

        public CreateGenreHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateGenreCommand> validator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<CreateEntityResponse> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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
