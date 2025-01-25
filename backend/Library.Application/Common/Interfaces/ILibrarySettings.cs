namespace Library.Application.Common.Interfaces
{
    public interface ILibrarySettings
    {
        double DefaultLoanPeriodInDays { get; }
        double DebtorReviewIntervalInDays { get; }
        string DefaulCoverFileName { get; }
    }
}
