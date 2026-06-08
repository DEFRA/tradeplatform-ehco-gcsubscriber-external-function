// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using AutoFixture;
using AutoFixture.AutoMoq;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Functions;
using Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.FunctionTestExtensions;
using Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Functions;

public class GcCreatedServiceBusTriggerFunctionTests
{
    private readonly GcCreatedServiceBusTriggerFunction _sut;
    private readonly Mock<IBaseMessageProcessorService<GeneralCertificateInbound>> _mockBaseMessageProcessorService;
    private readonly Mock<ILogger<GcCreatedServiceBusTriggerFunction>> _mockLogger;
    private readonly Mock<ServiceBusMessageActions> _mockServiceBusMessageActions;
    private readonly Mock<ServiceBusClient> _mockServiceBusClient;

    public GcCreatedServiceBusTriggerFunctionTests()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        _mockBaseMessageProcessorService = fixture.Freeze<Mock<IBaseMessageProcessorService<GeneralCertificateInbound>>>();
        _mockLogger = fixture.Freeze<Mock<ILogger<GcCreatedServiceBusTriggerFunction>>>();
        _mockServiceBusMessageActions = new Mock<ServiceBusMessageActions>();
        _mockServiceBusClient = new Mock<ServiceBusClient>();

        var mockSender = new Mock<ServiceBusSender>();
        _mockServiceBusClient
            .Setup(x => x.CreateSender(It.IsAny<string>(), It.IsAny<ServiceBusSenderOptions>()))
            .Returns(mockSender.Object);

        var mockRetryService = fixture.Freeze<Mock<IMessageRetryService>>();

        _sut = new GcCreatedServiceBusTriggerFunction(
            _mockBaseMessageProcessorService.Object,
            mockRetryService.Object,
            _mockServiceBusClient.Object,
            _mockLogger.Object);
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

        var executionContext = new Mock<FunctionContext>();
        var mockFunctionDefinition = new Mock<FunctionDefinition>();
        mockFunctionDefinition.Setup(x => x.Name).Returns(nameof(GcCreatedServiceBusTriggerFunction));
        executionContext.Setup(x => x.FunctionDefinition).Returns(mockFunctionDefinition.Object);
        executionContext.Setup(x => x.InvocationId).Returns(Guid.NewGuid().ToString());

        _mockBaseMessageProcessorService
            .Setup(x => x.ProcessAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                message,
                It.IsAny<ServiceBusMessageActions>(),
                It.IsAny<ServiceBusSender>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(false)
            .Verifiable();

        // Act
        var result = _sut.RunAsync(message, _mockServiceBusMessageActions.Object, executionContext.Object);

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
        var executionContext = new Mock<FunctionContext>();
        var mockFunctionDefinition = new Mock<FunctionDefinition>();
        mockFunctionDefinition.Setup(x => x.Name).Returns(nameof(GcCreatedServiceBusTriggerFunction));
        executionContext.Setup(x => x.FunctionDefinition).Returns(mockFunctionDefinition.Object);
        executionContext.Setup(x => x.InvocationId).Returns(Guid.NewGuid().ToString());
        _mockBaseMessageProcessorService.Setup(
            x => x.ProcessAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<ServiceBusMessageActions>(),
                It.IsAny<ServiceBusSender>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>())).Throws(exception);

        // Act
        await _sut.RunAsync(message, _mockServiceBusMessageActions.Object, executionContext.Object);

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
