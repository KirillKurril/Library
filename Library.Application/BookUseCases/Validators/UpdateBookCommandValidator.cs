using Library.Application.BookUseCases.Commands;
using MediatR;

namespace Library.Application.BookUseCases.Validators
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required")
                .MustAsync(async (bookId, ct) =>
                {
                    var book = await unitOfWork.BookRepository.GetByIdAsync(bookId);
                    return book != null;
                }).WithMessage($"Book being updated doesn't exist");

            RuleFor(x => x.ISBN)
                .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters")
                .When(x => x.ISBN != null);

            RuleFor(x => x)
                .MustAsync(async (br, ct) =>
                {
                    var book = unitOfWork.BookRepository
                    .FirstOrDefaultAsync(b => b.ISBN == br.ISBN  && b.Id != br.Id);
                    return book == null;
                })
                .WithMessage($"Book with such ISBN already exists")
                .When(x => x.ISBN != null);

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
                .When(x => x.Description != null); ;

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Author ID is required")
                .MustAsync(async (authorId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(authorId);
                    return author != null;
                }).WithMessage("Author with specified ID does not exist");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }
    }
}
