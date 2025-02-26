using Library.Domain.Specifications.AuthorSpecification;

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
            var spec = new BookByIdSpecification(request.BookId);
            var book = await _unitOfWork.BookRepository.FirstOrDefault(spec, cancellationToken);

            book.ImageUrl = request.ImageUrl;
            _unitOfWork.BookRepository.Update(book);

            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
