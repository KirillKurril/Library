namespace Library.Application.DTOs
{
    public class UpdateAuthor
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
    }
}
