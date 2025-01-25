namespace Library.Application.Common.Interfaces
{
    public interface ISmtpSettings
    {

        string Server { get; }
        int Port { get; }
        string SenderEmail { get; }
        string Password { get; }
    }
}
