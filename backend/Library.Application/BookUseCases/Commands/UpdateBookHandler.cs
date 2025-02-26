using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateBookHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var spec = new BookByIdSpecification(request.Id);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(spec, cancellationToken);

            _mapper.Map(request, book);
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
