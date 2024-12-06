using Library.Application.DTOs;

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
                var existingBook = await _unitOfWork.BookRepository.FirstOrDefaultAsync(b => b.ISBN == request.ISBN);
                if (existingBook != null)
                {
                    throw new DuplicateIsbnException($"Book with ISBN {request.ISBN} already exists");
                }
            }

            book.ISBN = request.ISBN ?? book.ISBN;
            book.Title = request.Title ?? book.Title;
            book.Description = request.Description ?? book.Description;
            book.AuthorId = request.AuthorId ?? book.AuthorId;
            book.GenreId = request.GenreId ?? book.GenreId;
            book.ImageUrl = request.ImageUrl ?? book.ImageUrl;
            book.Quantity = request.Quantity ?? book.Quantity;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
