using CitizenGateway.Application.Features.Citizens;
using CitizenGateway.Domain.Enums;
using CitizenGateway.Domain.Exceptions;
using FluentAssertions;

namespace CitizenGateway.UnitTests.Services;

/// <summary>
/// JWT claim'deki TC ile route TC eşleşmesi — vatandaş başka TC'ye erişemez.
/// </summary>
public sealed class CitizenAccessGuardTests
{
    private readonly CitizenAccessGuard _sut = new();

    [Fact]
    public void EnsureCanAccess_WhenPersonel_AllowsAnyTc()
    {
        var act = () => _sut.EnsureCanAccess(UserRole.Personel, callerTcNo: null, TestData.ValidTc2);

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanAccess_WhenVatandasAccessesOwnTc_Allows()
    {
        var act = () => _sut.EnsureCanAccess(UserRole.Vatandas, TestData.ValidTc1, TestData.ValidTc1);

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanAccess_WhenVatandasAccessesOtherTc_ThrowsUnauthorized()
    {
        var act = () => _sut.EnsureCanAccess(UserRole.Vatandas, TestData.ValidTc1, TestData.ValidTc2);

        act.Should().Throw<UnauthorizedCitizenAccessException>()
            .WithMessage("*kendi verisine*");
    }

    [Fact]
    public void EnsureCanAccess_WhenVatandasHasNoTcClaim_ThrowsUnauthorized()
    {
        var act = () => _sut.EnsureCanAccess(UserRole.Vatandas, callerTcNo: null, TestData.ValidTc1);

        act.Should().Throw<UnauthorizedCitizenAccessException>();
    }

    [Fact]
    public void EnsureCanAccess_WhenRequestedTcEmpty_ThrowsInvalidTcNoException()
    {
        var act = () => _sut.EnsureCanAccess(UserRole.Personel, null, "  ");

        act.Should().Throw<InvalidTcNoException>();
    }
}
