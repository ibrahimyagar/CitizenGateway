namespace CitizenGateway.Domain.Exceptions;

public sealed class InvalidTcNoException : DomainException
{
    public InvalidTcNoException(string message) : base(message)
    {
    }
}
