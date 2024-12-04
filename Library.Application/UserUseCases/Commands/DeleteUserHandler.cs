namespace Library.Application.UserUseCases.Commands;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, User>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User with ID {request.Id} not found");

        await _unitOfWork.UserRepository.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        
        return user;
    }
}
