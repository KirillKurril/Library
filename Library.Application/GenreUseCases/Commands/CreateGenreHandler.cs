namespace Library.Application.GenreUseCases.Commands
{
    public class CreateGenreHandler : IRequestHandler<CreateGenreCommand, Genre>
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

        public async Task<Genre> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var genre = _mapper.Map<Genre>(request);
            await _unitOfWork.GenreRepository.AddAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            
            return genre;
        }
    }
}
