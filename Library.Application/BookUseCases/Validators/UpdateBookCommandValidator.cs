using FluentValidation;
using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;

namespace Library.Application.BookUseCases.Validators
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.ISBN)
                .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters")
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
                }).WithMessage("Author with specified ID does not exist")
                .When(x => x.AuthorId != null);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }
    }
}
