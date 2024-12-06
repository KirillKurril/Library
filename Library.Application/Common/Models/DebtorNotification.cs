namespace Library.Application.Common.Models
{
    public class DebtorNotification
    {
        public int UserID { get; set; }
        public IReadOnlyList<BookBrief> ExpiredBooks { get; set; }
    }
}
