using Library.Application.DTOs;


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
            var authors = await _unitOfWork.AuthorRepository.ListAllAsync(cancellationToken);
            var authorBriefDTOs = authors.Adapt<IEnumerable<AuthorBriefDTO>>();
            return authorBriefDTOs;
        }
    }
}
