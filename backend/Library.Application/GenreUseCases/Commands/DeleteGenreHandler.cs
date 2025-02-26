using Library.Domain.Abstractions;
using Library.Domain.Specifications.AuthorSpecification;
using Library.Domain.Specifications.GenreSpecification;

namespace Library.Application.GenreUseCases.Commands
{
    public class DeleteGenreHandler : IRequestHandler<DeleteGenreCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGenreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {
            var spec = new GenreByIdSpecification(request.Id);
            var genre = await _unitOfWork.GenreRepository.FirstOrDefault(spec);

            _unitOfWork.GenreRepository.Delete(genre);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
