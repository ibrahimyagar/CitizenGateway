using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.Services;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;
using FluentAssertions;
using Moq;

namespace CitizenGateway.UnitTests.Services;

public sealed class AuditLoggerTests
{
    [Fact]
    public async Task LogAccessAsync_WhenCalled_PersistsEntryWithCorrectData()
    {
        var repo = new Mock<IAuditLogRepository>();
        AuditLogEntry? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<AuditLogEntry>(), It.IsAny<CancellationToken>()))
            .Callback<AuditLogEntry, CancellationToken>((e, _) => captured = e)
            .Returns(Task.CompletedTask);

        var sut = new AuditLogger(repo.Object);
        var userId = Guid.NewGuid();

        await sut.LogAccessAsync(
            userId,
            UserRole.Personel,
            TestData.ValidTc1,
            "GET /api/citizen/71151275166/summary",
            "127.0.0.1");

        repo.Verify(
            r => r.AddAsync(It.IsAny<AuditLogEntry>(), It.IsAny<CancellationToken>()),
            Times.Once);

        captured.Should().NotBeNull();
        captured!.UserId.Should().Be(userId);
        captured.UserRole.Should().Be(UserRole.Personel);
        captured.AccessedCitizenTcNo.Should().Be(TestData.ValidTc1);
        captured.AccessedEndpoint.Should().Be("GET /api/citizen/71151275166/summary");
        captured.IpAddress.Should().Be("127.0.0.1");
        captured.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
