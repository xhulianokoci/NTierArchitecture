namespace Entities.Exeptions;

public sealed class BadRequestException : Exception
{
    public BadRequestException(string message)
    : base(message)
    {
    }
}
