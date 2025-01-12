namespace Library.Application.DTOs
{
    public class UpdateAuthor
    {
        public Guid Id { get; }
        public string? Name { get; }
        public string? Surname { get; }
        public DateTime? BirthDate { get; }
        public string? Country { get; }
    }
}
