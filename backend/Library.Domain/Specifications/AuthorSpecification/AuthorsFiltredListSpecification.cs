using Library.Domain.Abstractions;
using Library.Domain.Entities;


namespace Library.Domain.Specifications.AuthorSpecification
{
    public class AuthorsFiltredListSpecification : BaseSpecification<Author>
    {
        public AuthorsFiltredListSpecification(
            string? searchTerm,
            int? pageNumber,
            int? pageSize)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Criteria = a =>
                    (a.Name + " " + a.Surname).ToLower().Contains(searchTerm.ToLower());
            }

            ApplyOrderBy(a => a.Id);

            if(pageNumber != null && pageSize != null)
                ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }
}
