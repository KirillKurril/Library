namespace Library.Application.Common.Exceptions;

public class DuplicateIsbnException : Exception
{
    public DuplicateIsbnException()
    {
    }

    public DuplicateIsbnException(string isbn)
        : base($"Book with such isbn ({isbn}) exist already.")
    {
    }
}
