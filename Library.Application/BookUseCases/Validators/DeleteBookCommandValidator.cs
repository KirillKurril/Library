using Library.Application.BookUseCases.Commands;

namespace Library.Application.BookUseCases.Validators
{
    public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var book = await unitOfWork.BookRepository.GetByIdAsync(bookId, ct);
                    return book != null;
                }).WithMessage($"Book with specified ID does not exist");

            RuleFor(x => x.Id)
                .MustAsync(async (bookId, ct) =>
                {
                    var isBorrowed = await unitOfWork.BookLendingRepository
                    .FirstOrDefaultAsync(bl => bl.BookId == bookId);
                    return isBorrowed == null;
                }).WithMessage($"Cannot delete book that is currently lent");
        }
    }
}
