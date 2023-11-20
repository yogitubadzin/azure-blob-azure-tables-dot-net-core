using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using RandomMessageApp.Application.Models;
using RandomMessageApp.Application.Services.Implementation;
using RandomMessageApp.Application.Services.Interfaces;
using RandomMessageApp.Interfaces.RandomMessages;
using RandomMessageApp.Storage.Services.Interfaces;

namespace RandomMessageApp.Application.Tests.Services;

public class RandomMessagesServiceTests
{
    private Mock<ITableStorage> _tableStorageMock;
    private Mock<IFileStorage> _fileStorageMock;
    private Mock<IMapper> _mapperMock;
    private IRandomMessagesService _randomMessagesService;

    [SetUp]
    public void SetUp()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _tableStorageMock = fixture.Freeze<Mock<ITableStorage>>();
        _fileStorageMock = fixture.Freeze<Mock<IFileStorage>>();
        _mapperMock = fixture.Freeze<Mock<IMapper>>();
        _randomMessagesService = fixture.Create<RandomMessagesService>();
    }

    [Test]
    public async Task GetMessagesAsync_ForRangeDate_ShouldReturnMessages()
    {
        // Given
        var now = DateTime.Now;
        var from = now.Date.AddDays(-2);
        var to = now.Date.AddDays(-1);

        var rowKey1 = Guid.NewGuid().ToString();
        var rowKey2 = Guid.NewGuid().ToString();
        var timestamp1 = from.AddMinutes(1).ToDateTimeOffset();
        var timestamp2 = from.AddMinutes(2).ToDateTimeOffset();
        var result1 = "success";
        var result2 = "failure";

        var randomMessageLogTableEntries = new List<RandomMessageLogTableEntry>
        {
            CreateRandomMessageLogTableEntry(rowKey1, timestamp1, result1),
            CreateRandomMessageLogTableEntry(rowKey2, timestamp2, result2),
        };

        _tableStorageMock
            .Setup(x => x.GetEntitiesAsync<RandomMessageLogTableEntry>(from, to))
            .ReturnsAsync(randomMessageLogTableEntries);

        // When
        var result = await _randomMessagesService.GetMessagesAsync(from, to);

        // Then
        result.Should().HaveCount(2);

        result[0].Id.Should().Be(rowKey1);
        result[0].DateTime.Should().Be(timestamp1.DateTime);
        result[0].Result.Should().Be(result1);

        result[1].Id.Should().Be(rowKey2);
        result[1].DateTime.Should().Be(timestamp2.DateTime);
        result[1].Result.Should().Be(result2);
    }

    [Test]
    public async Task GetMessageAsync_ForGivenMessageId_ShouldReturnFilePayload()
    {
        // Given
        var messageId = Guid.NewGuid();
        var fileName = $"{messageId}.json";

        var randomMessage = new RandomMessage
        {
            Count = 1,
            Entries = new List<RandomMessageEntry>
            {
                new RandomMessageEntry()
            }
        };

        _fileStorageMock
            .Setup(x => x.ReadFileAsync<RandomMessage>(fileName))
            .ReturnsAsync(randomMessage);

        var randomMessageModel = new RandomMessageModel
        {
            Count = 1,
            Entries = new List<RandomMessageEntryModel>
            {
                new RandomMessageEntryModel()
            }
        };

        _mapperMock
            .Setup(x => x.Map<RandomMessageModel>(randomMessage))
            .Returns(randomMessageModel);


        // When
        var result = await _randomMessagesService.GetMessageAsync(messageId);

        // Then
        result.Should().Be(randomMessageModel);

    }

    private static RandomMessageLogTableEntry CreateRandomMessageLogTableEntry(string rowKey1, DateTimeOffset timestamp1, string result1)
    {
        return new RandomMessageLogTableEntry
        {
            RowKey = rowKey1,
            Timestamp = timestamp1,
            Result = result1
        };
    }
}
