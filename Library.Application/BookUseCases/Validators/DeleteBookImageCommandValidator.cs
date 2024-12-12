using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;

namespace Library.Application.BookUseCases.Validators
{
    public class DeleteBookImageCommandValidator : AbstractValidator<DeleteBookImageCommand>
    {
        public DeleteBookImageCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Book ID being deleted is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(bookId);
                    return author != null;
                }).WithMessage("Book with specified ID does not exist");
        }
    }
}
