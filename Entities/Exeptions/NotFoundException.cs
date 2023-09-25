namespace Entities.Exeptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message)
    : base(message)
    { }
}
