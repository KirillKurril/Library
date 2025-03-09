using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;

namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookImageHandler : IRequestHandler<UpdateBookImageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookImageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateBookImageCommand request, CancellationToken cancellationToken)
        {
            var bookSpec = new BookByIdSpecification(request.BookId);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(bookSpec, cancellationToken);
            if (book == null)
                throw new ValidationException($"Book with specified ID ({request.BookId}) does not exist");

            book.ImageUrl = request.ImageUrl;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
