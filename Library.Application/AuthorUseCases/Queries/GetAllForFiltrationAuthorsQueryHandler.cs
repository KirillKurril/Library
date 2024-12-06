using Library.Application.BookUseCases.Commands;
using Library.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Author>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
        {
            var authos = await _unitOfWork.AuthorRepository.ListAllAsync(cancellationToken);
            foreach
        }
    }
}
