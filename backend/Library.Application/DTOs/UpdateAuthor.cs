namespace Library.Application.DTOs
{
    public class UpdateAuthorDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
    }
}
