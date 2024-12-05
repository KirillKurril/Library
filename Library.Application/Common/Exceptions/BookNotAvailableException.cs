namespace Library.Application.Common.Exceptions;

public class BookNotAvailableException : Exception
{
    public BookNotAvailableException()
    {
    }

    public BookNotAvailableException(int key)
        : base($"Book ({key}) is borrowed already.")
    {
    }
}
