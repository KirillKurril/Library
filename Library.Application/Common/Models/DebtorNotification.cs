namespace Library.Application.Common.Models
{
    public class DebtorNotification
    {
        public Guid UserID { get; set; }
        public IReadOnlyList<BookBrief> ExpiredBooks { get; set; }
    }
}
