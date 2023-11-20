using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using RandomMessageApp.Core.CommonServices.Implementation;
using RandomMessageApp.Core.CommonServices.Interfaces;

namespace RandomMessageApp.Core.Tests.CommonServices;

public class PartitionKeyGeneratorTests
{
    private IPartitionKeyGenerator _partitionKeyGenerator;

    [SetUp]
    public void SetUp()
    {
        _partitionKeyGenerator = new PartitionKeyGenerator();
    }

    [Test]
    [TestCase("2023-11-19 09:59:59", "2517019199999999999")]
    [TestCase("2023-11-19 10:00:00", "2517019163999999999")]
    [TestCase("2023-11-19 10:00:01", "2517019163999999999")]
    [TestCase("2023-11-19 10:59:59", "2517019163999999999")]
    [TestCase("2023-11-19 11:00:00", "2517019127999999999")]
    public void Generate_ForSpecificDateTime_ShouldGeneratePartitionKey(string dateTimeString, string expectedPartitionKey)
    {
        // Given
        var dateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

        // When
        var result = _partitionKeyGenerator.Generate(dateTime);

        // Then
        result.Should().Be(expectedPartitionKey);
    }
}
