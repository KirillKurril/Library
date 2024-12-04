namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Book>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateBookCommand> _validator;
        private readonly IMapper _mapper;

        public UpdateBookHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateBookCommand> validator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<Book> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var book = await _unitOfWork.BookRepository.GetByIdAsync(request.Id);
            if (book == null)
            {
                throw new NotFoundException(nameof(Book), request.Id);
            }

            if (book.ISBN != request.ISBN)
            {
                var existingBook = await _unitOfWork.BookRepository.FirstOrDefaultAsync(b => b.ISBN == request.ISBN);
                if (existingBook != null)
                {
                    throw new ValidationException($"Book with ISBN {request.ISBN} already exists");
                }
            }

            _mapper.Map(request, book);
            await _unitOfWork.SaveChangesAsync();

            return book;
        }
    }
}
