using Library.Domain.Specifications.BookSpecifications;

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
            var bookSpec = new BookByIdSpecification(request.Id);
            var blSepc = new BookLendingsByBookIdSpecification(request.Id);

            var book = await _unitOfWork.BookRepository.FirstOrDefault(bookSpec, cancellationToken);
            if (book == null)
                throw new ValidationException($"Book with specified ID ({request.Id}) does not exist");

            if (await _unitOfWork.BookLendingRepository.CountAsync(blSepc, cancellationToken) != 0)
                    throw new ValidationException($"Cannot delete book that is currently lent");
            
            
            _unitOfWork.BookRepository.Delete(book);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
