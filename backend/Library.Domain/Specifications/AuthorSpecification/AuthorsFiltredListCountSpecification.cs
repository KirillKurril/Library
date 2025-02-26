using Library.Domain.Abstractions;
using Library.Domain.Entities;


namespace Library.Domain.Specifications.AuthorSpecification
{
    public class AuthorsFiltredListCountSpecification : BaseSpecification<Author>
    {
        public AuthorsFiltredListCountSpecification(string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Criteria = a =>
                    (a.Name + " " + a.Surname).ToLower().Contains(searchTerm.ToLower());
            }

            ApplyOrderBy(a => a.Id);

        }
    }
}
