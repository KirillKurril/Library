using Library.Application.DTOs;
using Library.Domain.Specifications;


namespace Library.Application.AuthorUseCases.Queries
{
    public class GetAllForFiltrationAuthorsQueryHandler :
        IRequestHandler<GetAllForFiltrationAuthorsQuery, IEnumerable<AuthorBriefDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllForFiltrationAuthorsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AuthorBriefDTO>> Handle(GetAllForFiltrationAuthorsQuery request, CancellationToken cancellationToken)
        {
            var spec = new AllItemsSpecification<Author>();
            var authors = await _unitOfWork.AuthorRepository.GetAsync(spec, cancellationToken);

            var authorBriefDTOs = authors.Adapt<IEnumerable<AuthorBriefDTO>>();
            return authorBriefDTOs;
        }
    }
}
