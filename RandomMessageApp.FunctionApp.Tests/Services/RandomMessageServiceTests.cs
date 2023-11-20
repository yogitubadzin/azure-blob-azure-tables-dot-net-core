using AutoFixture;
using AutoFixture.AutoMoq;
using Azure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RandomMessageApp.Core.CommonServices.Interfaces;
using RandomMessageApp.Core.CommonServices.Models;
using RandomMessageApp.FunctionApp.Configuration;
using RandomMessageApp.FunctionApp.Models;
using RandomMessageApp.FunctionApp.Services.Implementation;
using RandomMessageApp.FunctionApp.Services.Interfaces;
using RandomMessageApp.Interfaces.RandomMessages;
using RandomMessageApp.Storage.Services.Interfaces;

namespace RandomMessageApp.FunctionApp.Tests.Services;

public class RandomMessageServiceTests
{
    private const string Url = "https://api.publicapis.org/random?auth=null";
    private readonly string _partitionKey = "2517019127999999999";
    private readonly string _rowKey = Guid.NewGuid().ToString();
    private readonly DateTime _now = DateTime.Now;
    private string _fileName;

    private RandomMessageServiceSettings _randomMessageServiceSettings;
    private Mock<IHttpClientService> _httpClientServiceMock;
    private Mock<IFileStorage> _fileStorageMock;
    private Mock<ITableStorage> _tableStorageMock;
    private Mock<IDateTimeService> _dateTimeServiceMock;
    private Mock<ITableStoragePrimaryKeyGenerator> _tableStoragePrimaryKeyGeneratorMock;
    private IRandomMessageService _randomMessageService;

    [SetUp]
    public void SetUp()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _randomMessageServiceSettings = fixture.Freeze<RandomMessageServiceSettings>();
        _httpClientServiceMock = fixture.Freeze<Mock<IHttpClientService>>();
        _fileStorageMock = fixture.Freeze<Mock<IFileStorage>>();
        _tableStorageMock = fixture.Freeze<Mock<ITableStorage>>();
        _dateTimeServiceMock = fixture.Freeze<Mock<IDateTimeService>>();
        _tableStoragePrimaryKeyGeneratorMock = fixture.Freeze<Mock<ITableStoragePrimaryKeyGenerator>>();
        _randomMessageService = fixture.Create<RandomMessageService>();

        _randomMessageServiceSettings.Url = Url;
        _fileName = $"{_rowKey}.json";
    }

    [Test]
    public async Task RunAsync_WhenResultIsSuccess_ShouldLogMessageInTableStorageAndFileStorage()
    {
        // Given
        var randomMessage = new RandomMessage
        {
            Count = 1,
            Entries = new List<RandomMessageEntry>
            {
                new RandomMessageEntry()
            }
        };

        var httpResponseResult = new HttpResponseResult<RandomMessage>
        {
            Result = randomMessage
        };

        SetUpHttpClientServiceMock(httpResponseResult);
        SetUpTableStoragePrimaryKeyGeneratorMock();

        RandomMessageLogTableEntry entryResult = null;

        _tableStorageMock
            .Setup(x => x.UpsertEntityAsync(It.IsAny<RandomMessageLogTableEntry>()))
            .Callback<RandomMessageLogTableEntry>(entry =>
            {
                entryResult = entry;
            });

        _dateTimeServiceMock
            .Setup(x => x.UtcNow)
            .Returns(_now);

        // When
        await _randomMessageService.RunAsync();

        // Then
        _fileStorageMock.Verify(x => x.UploadFileAsync(_fileName, randomMessage), Times.Once);

        entryResult.Should().NotBeNull();
        entryResult.PartitionKey.Should().Be(_partitionKey);
        entryResult.RowKey.Should().Be(_rowKey);
        entryResult.FileName.Should().Be(_fileName);
        entryResult.Result.Should().Be(RandomMessageResult.Success.ToString());
        entryResult.Timestamp.Should().Be(_now);
        entryResult.ETag.Should().Be(ETag.All);
    }

    [Test]
    public async Task RunAsync_WhenResultIsFailure_ShouldLogMessageInTableStorage()
    {
        // Given
        var httpResponseResult = new HttpResponseResult<RandomMessage>
        {
            Error = new HttpResponseError
            {
                Message = "Network is unreachable"
            }
        };

        SetUpHttpClientServiceMock(httpResponseResult);
        SetUpTableStoragePrimaryKeyGeneratorMock();

        RandomMessageLogTableEntry entryResult = null;

        _tableStorageMock
            .Setup(x => x.UpsertEntityAsync(It.IsAny<RandomMessageLogTableEntry>()))
            .Callback<RandomMessageLogTableEntry>(entry =>
            {
                entryResult = entry;
            });

        _dateTimeServiceMock
            .Setup(x => x.UtcNow)
            .Returns(_now);

        // When
        await _randomMessageService.RunAsync();

        // Then
        _fileStorageMock.Verify(x => x.UploadFileAsync(_fileName, It.IsAny<RandomMessage>()), Times.Never);

        entryResult.Should().NotBeNull();
        entryResult.PartitionKey.Should().Be(_partitionKey);
        entryResult.RowKey.Should().Be(_rowKey);
        entryResult.FileName.Should().BeNull();
        entryResult.Result.Should().Be(RandomMessageResult.Failure.ToString());
        entryResult.Timestamp.Should().Be(_now);
        entryResult.ETag.Should().Be(ETag.All);
    }

    private void SetUpHttpClientServiceMock(HttpResponseResult<RandomMessage> httpResponseResult)
    {
        _httpClientServiceMock
            .Setup(x => x.GetAsync<RandomMessage>(Url))
            .ReturnsAsync(httpResponseResult);
    }

    private void SetUpTableStoragePrimaryKeyGeneratorMock()
    {
        _tableStoragePrimaryKeyGeneratorMock
            .Setup(x => x.Generate())
            .Returns((_partitionKey, _rowKey));
    }
}
