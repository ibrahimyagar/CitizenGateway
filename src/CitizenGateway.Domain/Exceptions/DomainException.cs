namespace CitizenGateway.Domain.Exceptions;

/// <summary>
/// Tüm domain hatalarının ortak tabanı.
/// API katmanı bu tipi yakalayıp 400/403'e map'leyebilir — Infrastructure bilmez, Domain bilir.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
