using FluentValidation;
using Library.Application.BookUseCases.Commands;
using Library.Application.Common.Interfaces;
using Library.Domain.Abstractions;

namespace Library.Application.BookUseCases.Validators
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters")
                .MustAsync(async (isbn, ct) =>
                {
                    var existingBook = await unitOfWork.BookRepository.FirstOrDefaultAsync(b => b.ISBN == isbn, ct);
                    return existingBook == null;
                }).WithMessage("A book with this ISBN already exists");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

            RuleFor(x => x.GenreId)
                .NotEmpty().WithMessage("Genre is required");


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
