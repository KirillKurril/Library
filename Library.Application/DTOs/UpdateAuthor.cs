namespace Library.Application.DTOs
{
    public class UpdateAuthor
    {
        public int Id { get; }
        public string? Name { get; }
        public string? Surname { get; }
        public DateTime? BirthDate { get; }
        public string? Country { get; }
    }
}
