using Library.Domain.Specifications;

namespace Library.Application.GenreUseCases.Queries
{
    public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, IEnumerable<Genre>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllGenresQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Genre>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
        {
            var spec = new AllItemsSpecification<Genre>();
            return await _unitOfWork.GenreRepository.GetAsync(spec, cancellationToken);
        }
    }
}