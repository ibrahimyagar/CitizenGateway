using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.External;
using CitizenGateway.Application.Features.Citizens;
using CitizenGateway.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CitizenGateway.UnitTests.Services;

public sealed class CitizenSummaryServiceTests
{
    private readonly Mock<ICitizenRepository> _citizens = new();
    private readonly Mock<IExternalServiceClient> _external = new();
    private readonly CitizenSummaryService _sut;

    public CitizenSummaryServiceTests()
    {
        _sut = new CitizenSummaryService(
            _citizens.Object,
            _external.Object,
            NullLogger<CitizenSummaryService>.Instance);
    }

    [Fact]
    public async Task GetSummaryAsync_WhenAllServicesSucceed_ReturnsMergedResult()
    {
        var citizen = TestData.CreateCitizen();
        _citizens.Setup(c => c.GetByTcNoAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(citizen);

        var spor = new SporTesisiDto(TestData.ValidTc1, citizen.AdSoyad, true, "Merkez", 10, new DateOnly(2027, 1, 1));
        var kutuphane = new KutuphaneDto(TestData.ValidTc1, citizen.AdSoyad, 2, []);
        var cozum = new CozumMerkeziDto(TestData.ValidTc1, citizen.AdSoyad, 1, []);

        _external.Setup(e => e.GetSporTesisiAsync(TestData.ValidTc1, It.IsAny<CancellationToken>())).ReturnsAsync(spor);
        _external.Setup(e => e.GetKutuphaneAsync(TestData.ValidTc1, It.IsAny<CancellationToken>())).ReturnsAsync(kutuphane);
        _external.Setup(e => e.GetCozumMerkeziAsync(TestData.ValidTc1, It.IsAny<CancellationToken>())).ReturnsAsync(cozum);

        var result = await _sut.GetSummaryAsync(TestData.ValidTc1);

        result.TcNo.Should().Be(TestData.ValidTc1);
        result.AdSoyad.Should().Be(citizen.AdSoyad);
        result.SporTesisi.Should().Be(spor);
        result.Kutuphane.Should().Be(kutuphane);
        result.CozumMerkezi.Should().Be(cozum);
        result.PartialFailure.Should().BeFalse();
        result.FailedServices.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSummary_WhenOneServiceFails_ReturnsPartialResult()
    {
        var citizen = TestData.CreateCitizen();
        _citizens.Setup(c => c.GetByTcNoAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(citizen);

        _external.Setup(e => e.GetSporTesisiAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SporTesisiDto(TestData.ValidTc1, citizen.AdSoyad, true, "Merkez", 5, new DateOnly(2027, 1, 1)));
        _external.Setup(e => e.GetKutuphaneAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Kutuphane down"));
        _external.Setup(e => e.GetCozumMerkeziAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CozumMerkeziDto(TestData.ValidTc1, citizen.AdSoyad, 0, []));

        var result = await _sut.GetSummaryAsync(TestData.ValidTc1);

        result.PartialFailure.Should().BeTrue();
        result.FailedServices.Should().ContainSingle().Which.Should().Be("Kutuphane");
        result.SporTesisi.Should().NotBeNull();
        result.Kutuphane.Should().BeNull();
        result.CozumMerkezi.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSummaryAsync_WhenTcInvalid_ThrowsInvalidTcNoException()
    {
        var act = async () => await _sut.GetSummaryAsync(TestData.InvalidTc);

        await act.Should().ThrowAsync<InvalidTcNoException>();
        _external.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetSummaryAsync_WhenCitizenMissing_ThrowsCitizenNotFoundException()
    {
        _citizens.Setup(c => c.GetByTcNoAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Citizen?)null);

        var act = async () => await _sut.GetSummaryAsync(TestData.ValidTc1);

        await act.Should().ThrowAsync<CitizenNotFoundException>();
    }
}
