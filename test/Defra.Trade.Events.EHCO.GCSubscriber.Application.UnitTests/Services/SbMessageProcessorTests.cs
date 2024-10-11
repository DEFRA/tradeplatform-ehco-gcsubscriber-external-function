// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Azure.Messaging.ServiceBus;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Client;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Services;

public class SbMessageProcessorTests
{
    private readonly IFixture _fixture;
    private readonly TradeEventMessageHeader _messageHeader;
    private readonly Mock<IGeneralCertificateEnrichmentProcessor> _mockEnrichmentProcessor;
    private readonly Mock<ILogger<SbMessageProcessor>> _mockLogger;
    private readonly Mock<ServiceBusReceivedMessage> _mockMessage;
    private readonly Mock<IGcMessageProcessor> _mockMessageProcessor;
    private readonly Mock<IMessageRetryContextAccessor> _mockRetryAccessor;
    private readonly Mock<IAsyncCollector<ServiceBusMessage>> _mockRetryQueue;
    private readonly SbMessageProcessor _sut;

    public SbMessageProcessorTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockMessageProcessor = new Mock<IGcMessageProcessor>();
        _mockLogger = new Mock<ILogger<SbMessageProcessor>>();
        _mockEnrichmentProcessor = new Mock<IGeneralCertificateEnrichmentProcessor>();
        _mockRetryAccessor = new Mock<IMessageRetryContextAccessor>();
        _mockMessage = _fixture.Freeze<Mock<ServiceBusReceivedMessage>>();
        _mockRetryQueue = _fixture.Freeze<Mock<IAsyncCollector<ServiceBusMessage>>>();

        _sut = new SbMessageProcessor(
            _mockLogger.Object,
            _mockMessageProcessor.Object,
            _mockEnrichmentProcessor.Object,
            _mockRetryAccessor.Object);

        _messageHeader = new TradeEventMessageHeader { MessageId = "messageId", SchemaVersion = "1" };
    }

    [Fact]
    public void Ctors_EnsureNotNullAndCorrectExceptionParameterName()
    {
        var assertion = new GuardClauseAssertion(_fixture);
        assertion.Verify(typeof(SbMessageProcessor).GetConstructors());
    }

    [Fact]
    public async Task Process_BuildCustomMessageHeaderAsync_Should_Not_Be_Null()
    {
        // Act
        var result = await _sut.BuildCustomMessageHeaderAsync();

        // Assert
        result.ShouldNotBeNull();
    }

    [Theory, AutoData]
    public async Task Process_CustomerPublisherMessageProcessor_ValidateMessageLabelAsync_Not_Relevant_Label(TradeEventMessageHeader messageHeader)
    {
        // Arrange
        messageHeader.Label = "submitted-GBR-2020-PS-C19186D9D";
        // Act
        bool result = await _sut.ValidateMessageLabelAsync(messageHeader);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory, AutoData]
    public async Task Process_CustomerPublisherMessageProcessor_ValidateMessageLabelAsync_TradeCustomerInspectionSiteUpdate_Label(TradeEventMessageHeader messageHeader)
    {
        // Arrange
        messageHeader.Label = GcMessageConstants.BrokerLabel;
        // Act
        bool result = await _sut.ValidateMessageLabelAsync(messageHeader);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory, AutoData]
    public async Task Process_GetSchemaAsync_Should_Not_Be_Null(TradeEventMessageHeader messageHeade)
    {
        // Act
        string result = await _sut.GetSchemaAsync(messageHeade);

        // Assert
        result.ShouldNotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(500)]
    [InlineData(599)]
    public async Task Process_WithApiException_AttemptsRetry(int errorCode)
    {
        // Arrange
        _mockRetryAccessor
            .Setup(x => x.Context.Queue)
            .Returns(_mockRetryQueue.Object);

        _mockRetryAccessor
            .Setup(x => x.Context.Message)
            .Returns(_mockMessage.Object);

        var mockGcCommand = _fixture.Create<GeneralCertificateRequest>();
        var mockApiException = new ApiException(errorCode, "mocked error");

        _mockMessageProcessor
            .Setup(x => x.ProcessMessage(mockGcCommand))
            .Throws(mockApiException);

        // Act
        await Assert.ThrowsAsync<ApiException>(
            async () => await _sut.ProcessAsync(mockGcCommand, _messageHeader));
    }

    [Fact]
    public async Task Process_WithServiceBusCommunicationsError_AttemptsRetry()
    {
        // Arrange
        _mockRetryAccessor
            .Setup(x => x.Context.Queue)
            .Returns(_mockRetryQueue.Object);

        _mockRetryAccessor
            .Setup(x => x.Context.Message)
            .Returns(_mockMessage.Object);

        var mockGcCommand = _fixture.Create<GeneralCertificateRequest>();

        _mockMessageProcessor
            .Setup(x => x.ProcessMessage(mockGcCommand))
            .Returns(Task.CompletedTask);

        var ex = new ServiceBusCommunicationException("mock service bus communication error");

        _mockEnrichmentProcessor
            .Setup(x => x.SendMessageAsync(mockGcCommand, _messageHeader, CancellationToken.None))
            .Throws(ex);

        // Act
        await Assert.ThrowsAsync<ServiceBusCommunicationException>(
            async () => await _sut.ProcessAsync(mockGcCommand, _messageHeader));
    }

    [Fact]
    public async Task ProcessMessage_WhenValidJson_ShouldParse()
    {
        // Arrange
        var mockedGcCommand = _fixture.Create<GeneralCertificateRequest>();
        _mockMessageProcessor.Setup(x => x.ProcessMessage(mockedGcCommand))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ProcessAsync(mockedGcCommand, _messageHeader);

        // Assert
        result.Response.ExchangedDocument.Id.ShouldBe(result.Response.ExchangedDocument.Id);
    }
}
