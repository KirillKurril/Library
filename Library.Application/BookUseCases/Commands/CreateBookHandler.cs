namespace Library.Application.BookUseCases.Commands
{
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, Book>
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

        public async Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var book = _mapper.Map<Book>(request);
            book.IsAvailable = true;

            await _unitOfWork.BookRepository.AddAsync(book, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
