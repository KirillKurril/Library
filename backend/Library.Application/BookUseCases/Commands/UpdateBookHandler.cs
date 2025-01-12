
namespace Library.Application.BookUseCases.Commands
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var updatedBook = request.Adapt<Book>();
            _unitOfWork.BookRepository.Update(updatedBook);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
