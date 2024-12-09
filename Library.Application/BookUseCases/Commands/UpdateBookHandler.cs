
namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand>
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

        public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
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
                var existingBook = await _unitOfWork.BookRepository
                    .FirstOrDefaultAsync(b => b.ISBN == request.ISBN && b.Id != request.Id);
                if (existingBook != null)
                {
                    throw new DuplicateIsbnException($"Book with ISBN {request.ISBN} already exists");
                }
            }

            var updatedBook = request.Adapt<Book>();
            _unitOfWork.BookRepository.Update(updatedBook);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
