namespace CitizenGateway.Domain.Exceptions;

public sealed class CitizenNotFoundException : DomainException
{
    public CitizenNotFoundException(string tcNo)
        : base($"Vatandaş bulunamadı. TC: {tcNo}")
    {
        TcNo = tcNo;
    }

    public string TcNo { get; }
}