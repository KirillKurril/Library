using Library.Domain.Specifications.AuthorSpecification;

namespace Library.Application.BookUseCases.Commands
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var spec = new BookByIdSpecification(request.Id);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(spec, cancellationToken);

            _unitOfWork.BookRepository.Delete(book);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
