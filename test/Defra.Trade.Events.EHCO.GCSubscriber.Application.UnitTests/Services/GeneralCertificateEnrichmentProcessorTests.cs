// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Services;

public class GeneralCertificateEnrichmentProcessorTests
{
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IServiceBusManagerClient> _serviceBusManagerClient;
    private readonly Mock<ILogger<GeneralCertificateEnrichmentProcessor>> _logger;
    private readonly IFixture _fixture;
    private readonly GeneralCertificateEnrichmentProcessor _sut;

    public GeneralCertificateEnrichmentProcessorTests()
    {
        _mapper = new Mock<IMapper>();
        _fixture = new Fixture();
        _serviceBusManagerClient = new Mock<IServiceBusManagerClient>();
        _logger = new();
        _sut = new GeneralCertificateEnrichmentProcessor(_mapper.Object, _serviceBusManagerClient.Object, _logger.Object);
    }

    [Fact]
    public async Task ProcessMessage_Should_SendMessage()
    {
        // Arrange
        var generalCertificateRequest = _fixture.Create<GeneralCertificateRequest>();
        var messageHeader = _fixture.Create<TradeEventMessageHeader>();

        _serviceBusManagerClient.Setup(x =>
            x.SendMessageAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.SendMessageAsync(generalCertificateRequest, messageHeader);

        //Assert
        _serviceBusManagerClient.Verify(x =>
            x.SendMessageAsync(It.IsAny<Message>()), Times.Once());
    }
}
