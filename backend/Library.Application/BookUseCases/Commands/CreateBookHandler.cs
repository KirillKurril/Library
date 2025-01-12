using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, CreateEntityResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateEntityResponse> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var bookToCreate = request.Adapt<Book>();

            var createdBook = _unitOfWork.BookRepository.Add(bookToCreate);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateEntityResponse()
            {
                Id = createdBook.Id,
            };

            return response;
        }
    }
}
