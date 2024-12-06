using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, CreateEntityResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateBookCommand> _validator;
        private readonly IMapper _mapper;

        public CreateBookHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateBookCommand> validator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<CreateEntityResponse> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var bookToCreate = _mapper.Map<Book>(request);

            var createdBook = _unitOfWork.BookRepository.Add(bookToCreate);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateEntityResponse()
            {
                Id = createdBook.Id,
            };

            return response;
        }
    }
}
