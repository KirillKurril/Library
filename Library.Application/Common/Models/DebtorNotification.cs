namespace Library.Application.Common.Models
{
    public class DebtorNotification
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IReadOnlyList<BookBrief> ExpiredBooks { get; set; }
    }
}
