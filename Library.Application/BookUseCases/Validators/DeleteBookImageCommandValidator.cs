using Library.Application.BookUseCases.Commands;

namespace Library.Application.BookUseCases.Validators
{
    public class DeleteBookImageCommandValidator : AbstractValidator<DeleteBookImageCommand>
    {
        public DeleteBookImageCommandValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0);
        }
    }
}
