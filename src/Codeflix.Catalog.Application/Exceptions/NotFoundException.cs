namespace Codeflix.Catalog.Application.Exceptions;
public class NotFoundException(string? message) : ApplicationException(message)
{
    public static void ThrowIfNull(object? obj, string message)
    {
        if (obj is null)
            throw new NotFoundException(message);
    }
}