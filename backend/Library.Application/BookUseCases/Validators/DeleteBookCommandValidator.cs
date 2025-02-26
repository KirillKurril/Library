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
            .NotEmpty().WithMessage("Book ID is required")
            .MustAsync(async (bookId, ct) =>
            {
                var spec = new BookByIdSpecification(bookId);
                var exist = await unitOfWork.BookRepository.CountAsync(spec, ct);
                return exist == 1;
            }).WithMessage($"Book with specified ID does not exist");

            RuleFor(x => x.Id)
                .MustAsync(async (bookId, ct) =>
                {
                    var spec = new BookLendingsByBookIdSpecification(bookId);
                    var exist = await unitOfWork.BookLendingRepository.CountAsync(spec, ct);
                    return exist == 0;
                }).WithMessage($"Cannot delete book that is currently lent");
        }
    }
}
