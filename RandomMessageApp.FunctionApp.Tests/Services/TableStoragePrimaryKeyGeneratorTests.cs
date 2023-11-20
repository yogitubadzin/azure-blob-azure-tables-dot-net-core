using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.FunctionApp.Services.Implementation;
using RandomMessageApp.FunctionApp.Services.Interfaces;

namespace RandomMessageApp.FunctionApp.Tests.Services;

public class TableStoragePrimaryKeyGeneratorTests
{
    private Mock<IDateTimeService> _dateTimeServiceMock;
    private Mock<IPartitionKeyGenerator> _partitionKeyGeneratorMock;
    private ITableStoragePrimaryKeyGenerator _tableStoragePrimaryKeyGenerator;

    [SetUp]
    public void SetUp()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _dateTimeServiceMock = fixture.Freeze<Mock<IDateTimeService>>();
        _partitionKeyGeneratorMock = fixture.Freeze<Mock<IPartitionKeyGenerator>>();
        _tableStoragePrimaryKeyGenerator = fixture.Create<TableStoragePrimaryKeyGenerator>();
    }

    [Test]
    public void Generate_ShouldGeneratePartionKeyAndRowKey()
    {
        // Given
        var now = DateTime.Now;

        _dateTimeServiceMock
            .Setup(x => x.UtcNow)
            .Returns(now);

        var partitionKey = "2517019127999999999";
        _partitionKeyGeneratorMock
            .Setup(x => x.Generate(now))
            .Returns(partitionKey);

        // When
        var (partitionKeyResult, rowKeyResult) = _tableStoragePrimaryKeyGenerator.Generate();

        // Then
        partitionKeyResult.Should().Be(partitionKey);
        rowKeyResult.Should().NotBe(Guid.Empty.ToString());
    }
}
