
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
            var updatedBook = request.Adapt<Book>();
            _unitOfWork.BookRepository.Update(updatedBook);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
