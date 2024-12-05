namespace Library.Application.Common.Exceptions;

public class BookInUseException : Exception
{
    public BookInUseException()
    {
    }

    public BookInUseException(int id)
        : base($"Cannot delete book ({id}) that is currently borrowed")
    {
    }
}
