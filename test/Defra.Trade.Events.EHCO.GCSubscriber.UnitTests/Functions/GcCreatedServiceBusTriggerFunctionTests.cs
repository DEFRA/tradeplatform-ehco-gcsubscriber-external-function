// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture;
using AutoFixture.AutoMoq;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Functions;
using Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.FunctionTestExtensions;
using Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Shouldly;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using ServiceBusMessage = Azure.Messaging.ServiceBus.ServiceBusMessage;

namespace Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Functions;

public class GcCreatedServiceBusTriggerFunctionTests
{
    private readonly GcCreatedServiceBusTriggerFunction _sut;
    private readonly Mock<IBaseMessageProcessorService<GeneralCertificateInbound>> _mockBaseMessageProcessorService;
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<ServiceBusMessageActions> _mockServiceBusMessageActions;
    private readonly Mock<IAsyncCollector<ServiceBusMessage>> _mockRetryQueue;

    public GcCreatedServiceBusTriggerFunctionTests()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        _mockBaseMessageProcessorService = fixture.Freeze<Mock<IBaseMessageProcessorService<GeneralCertificateInbound>>>();
        _mockLogger = fixture.Freeze<Mock<ILogger>>();
        _mockServiceBusMessageActions = new Mock<ServiceBusMessageActions>();
        _mockRetryQueue = fixture.Freeze<Mock<IAsyncCollector<ServiceBusMessage>>>();

        _sut = fixture.Create<GcCreatedServiceBusTriggerFunction>();
    }

    [Fact]
    public void RunAsync_HasServiceBusTrigger_WithCorrectProperties()
    {
        FunctionTriggerAssertionHelpers.ShouldHaveServiceBusTrigger<GcCreatedServiceBusTriggerFunction>(
            nameof(GcCreatedServiceBusTriggerFunction.RunAsync), GcSubscriberSettings.DefaultQueueName);
    }

    [Fact]
    public void RunAsync_WhenTrigger_ShouldCallMessageProcessor()
    {
        // Arrange
        const string Json = "{\"OrderId\":92,\"BillingAddress\":{\"CustomerAddressId\":null,\"FirstName\":\"Jamie\",\"LastName\":\"Bowman\",\"StreetAddress1\":\"123 Street\",\"StreetAddress2\":\"Apt #2\",\"City\":\"Saint Louis\",\"State\":\"MO\",\"PostalCode\":\"12345\",\"Country\":\"USA\"},\"ShippingAddress\":{\"CustomerAddressId\":null,\"FirstName\":\"Jamie\",\"LastName\":\"Bowman\",\"StreetAddress1\":\"123 Street\",\"StreetAddress2\":\"Apt #2\",\"City\":\"Saint Louis\",\"State\":\"MO\",\"PostalCode\":\"12345\",\"Country\":\"USA\"},\"Subtotal\":404,\"Tax\":28.28,\"Total\":432.28}";

        var message = new ServiceBusReceivedMessageBuilder().WithBody(BinaryData.FromString(Json)).Build();

        var executionContext = new Mock<ExecutionContext>();

        _mockBaseMessageProcessorService
            .Setup(x => x.ProcessAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                message,
                _mockServiceBusMessageActions.Object,
                It.IsAny<IAsyncCollector<ServiceBusMessage>>(),
                It.IsAny<IAsyncCollector<ServiceBusMessage>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(false)
            .Verifiable();

        // Act
        var result = _sut.RunAsync(message, _mockServiceBusMessageActions.Object, executionContext.Object, null, _mockRetryQueue.Object, _mockLogger.Object);

        // Assert
        _ = result.ShouldNotBeNull();
        result.Status.ShouldBe(TaskStatus.RanToCompletion);
    }

    [Fact]
    public async Task RunAsync_WhenTriggeredWithInvalidMessage_ShouldThrowException()
    {
        // Arrange
        const string Json = "invalid-json";

        var message = new ServiceBusReceivedMessageBuilder().WithBody(BinaryData.FromString(Json)).Build();
        var exception = new Exception();
        var executionContext = new Mock<ExecutionContext>();
        _mockBaseMessageProcessorService.Setup(
            x => x.ProcessAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<ServiceBusMessageActions>(),
                It.IsAny<IAsyncCollector<ServiceBusMessage>>(),
                It.IsAny<IAsyncCollector<ServiceBusMessage>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).Throws(exception);

        // Act
        await _sut.RunAsync(message, _mockServiceBusMessageActions.Object, executionContext.Object, null, _mockRetryQueue.Object, _mockLogger.Object);

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                It.Is<LogLevel>(level => level == LogLevel.Critical),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, type) => @object.ToString()!.Length != 0),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
