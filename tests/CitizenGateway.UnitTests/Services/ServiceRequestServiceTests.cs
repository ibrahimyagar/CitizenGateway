using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.DTOs;
using CitizenGateway.Application.DTOs.External;
using CitizenGateway.Application.Services;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;
using CitizenGateway.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CitizenGateway.UnitTests.Services;

public sealed class ServiceRequestServiceTests
{
    private readonly Mock<ICitizenRepository> _citizens = new();
    private readonly Mock<IServiceRequestRepository> _requests = new();
    private readonly Mock<IExternalServiceClient> _external = new();
    private readonly ServiceRequestService _sut;

    public ServiceRequestServiceTests()
    {
        _sut = new ServiceRequestService(
            _citizens.Object,
            _requests.Object,
            _external.Object,
            NullLogger<ServiceRequestService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_PersistsAndReturnsDto()
    {
        var citizen = TestData.CreateCitizen();
        _citizens.Setup(c => c.GetByTcNoAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(citizen);
        _external.Setup(e => e.GetSporTesisiAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SporTesisiDto?)null);

        ServiceRequest? captured = null;
        _requests.Setup(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ServiceRequest, CancellationToken>((req, _) => captured = req)
            .Returns(Task.CompletedTask);
        _requests.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(TestData.ValidTc1, new CreateServiceRequestDto(RequestType.KursKaydi));

        result.TcNo.Should().Be(TestData.ValidTc1);
        result.RequestType.Should().Be(RequestType.KursKaydi);
        result.TargetService.Should().Be(TargetService.SporTesisi);
        result.Status.Should().Be(RequestStatus.Beklemede);
        captured.Should().NotBeNull();
        _requests.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenTcInvalid_ThrowsInvalidTcNoException()
    {
        var act = async () => await _sut.CreateAsync(
            TestData.InvalidTc,
            new CreateServiceRequestDto(RequestType.KursKaydi));

        await act.Should().ThrowAsync<InvalidTcNoException>();
        _requests.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCitizenMissing_ThrowsCitizenNotFoundException()
    {
        _citizens.Setup(c => c.GetByTcNoAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Citizen?)null);

        var act = async () => await _sut.CreateAsync(
            TestData.ValidTc1,
            new CreateServiceRequestDto(RequestType.SikayetAcma));

        await act.Should().ThrowAsync<CitizenNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WhenExternalServiceFails_StillPersistsRequest()
    {
        var citizen = TestData.CreateCitizen();
        _citizens.Setup(c => c.GetByTcNoAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(citizen);
        _external.Setup(e => e.GetKutuphaneAsync(TestData.ValidTc1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("timeout"));

        _requests.Setup(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _requests.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(
            TestData.ValidTc1,
            new CreateServiceRequestDto(RequestType.RandevuTalebi));

        result.TargetService.Should().Be(TargetService.Kutuphane);
        _requests.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
