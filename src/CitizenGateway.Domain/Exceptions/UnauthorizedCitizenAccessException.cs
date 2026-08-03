namespace CitizenGateway.Domain.Exceptions;

/// <summary>
/// Vatandaş rolünün başka bir TC'ye erişmeye çalışması.
/// Authorization middleware/policy bu exception veya eşdeğer 403 ile sonuçlanır.
/// </summary>
public sealed class UnauthorizedCitizenAccessException : DomainException
{
    public UnauthorizedCitizenAccessException(string message)
        : base(message)
    {
    }

    public static UnauthorizedCitizenAccessException ForTcNo(string requestedTcNo) =>
        new($"Vatandaş yalnızca kendi verisine erişebilir. İstenen TC: {requestedTcNo}");
}
