namespace Library.Application.Common.Interfaces
{
    public interface ILibrarySettings
    {
        int DefaultLoanPeriodInDays { get; }
        int IsbnLength { get; }
    }
}
