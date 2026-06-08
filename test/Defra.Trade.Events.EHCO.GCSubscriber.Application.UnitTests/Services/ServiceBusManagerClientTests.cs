// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Services;

public class ServiceBusManagerClientTests
{
    private readonly Mock<IQueueClientFactory> _queueClientFactory;
    private readonly IFixture _fixture;
    private readonly ServiceBusManagerClient _sut;

    public ServiceBusManagerClientTests()
    {
        _fixture = new Fixture();
        _queueClientFactory = new Mock<IQueueClientFactory>();
        _sut = new ServiceBusManagerClient(_queueClientFactory.Object);
    }

    [Fact]
    public async Task ServiceBusManagerClient_Should_SendMessage()
    {
        // Arrange
        var sender = new Mock<ServiceBusSender>();
        var message = new ServiceBusMessage("test");

        _queueClientFactory.Setup(x => x.CreateQueueClient())
            .Returns(sender.Object);

        sender.Setup(x => x.SendMessageAsync(message, default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.SendMessageAsync(message);

        // Assert
        sender.Verify(x => x.SendMessageAsync(message, default), Times.Once());
    }
}
