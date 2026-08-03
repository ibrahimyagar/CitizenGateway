namespace CitizenGateway.Domain.Exceptions;

/// <summary>İş kuralı / girdi doğrulama hatası — API genelde 400 döner.</summary>
public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message)
    {
    }
}
