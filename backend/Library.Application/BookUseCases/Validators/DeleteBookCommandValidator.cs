using Library.Application.BookUseCases.Commands;
using Library.Domain.Abstractions;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.BookSpecifications;
using MediatR;
using System.Threading;

namespace Library.Application.BookUseCases.Validators
{
    public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required");
        }
    }
}
