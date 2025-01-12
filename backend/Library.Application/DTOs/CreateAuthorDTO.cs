namespace Library.Application.DTOs
{
    public class CreateAuthorDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }
    }
}
