﻿using Library.Application.AuthorUseCases.Commands;

namespace Library.Application.AuthorUseCases.Validators;

public class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Author ID is required")
                .MustAsync(async (authorId, ct) =>
                {
                    var author = await unitOfWork.AuthorRepository.GetByIdAsync(authorId);
                    return author != null;
                }).WithMessage($"Author being deleted doesn't exist");
    }
}
