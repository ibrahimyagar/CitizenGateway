using CitizenGateway.Domain.Enums;
using CitizenGateway.Domain.Exceptions;

namespace CitizenGateway.Application.Services;

/// <summary>
/// Rol tabanlı TC erişim kuralı.
/// Personel: herkes. Vatandas: claim'deki TC ile route TC eşleşmeli; aksi 403.
/// Ayrı sınıf: controller şişkin olmasın, unit test doğrudan bu mantığı doğrulasın.
/// </summary>
public sealed class CitizenAccessGuard
{
    public void EnsureCanAccess(UserRole role, string? callerTcNo, string requestedTcNo)
    {
        if (string.IsNullOrWhiteSpace(requestedTcNo))
            throw new InvalidTcNoException("İstenen T.C. kimlik numarası boş olamaz.");

        if (role == UserRole.Personel)
            return;

        if (role != UserRole.Vatandas)
            throw new UnauthorizedCitizenAccessException("Geçersiz rol ile erişim denemesi.");

        if (string.IsNullOrWhiteSpace(callerTcNo) ||
            !string.Equals(callerTcNo.Trim(), requestedTcNo.Trim(), StringComparison.Ordinal))
        {
            throw UnauthorizedCitizenAccessException.ForTcNo(requestedTcNo);
        }
    }
}
