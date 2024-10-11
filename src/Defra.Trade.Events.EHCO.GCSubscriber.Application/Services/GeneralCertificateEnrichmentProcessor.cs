// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text.Json;
using System.Threading;
using AutoMapper;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Extensions;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Helpers;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

public class GeneralCertificateEnrichmentProcessor(
    IMapper mapper,
    IServiceBusManagerClient serviceBusManagerClient,
    ILogger<GeneralCertificateEnrichmentProcessor> logger) : IGeneralCertificateEnrichmentProcessor
{
    private const string ContentType = "application/json";
    private const string Label = "trade.remos.enrichment";
    private const string PublisherId = "TradeApi";
    private const string SchemaVersion = "1";
    private const string Status = "Complete";
    private const string Type = "Internal";
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IServiceBusManagerClient _serviceBusManagerClient = serviceBusManagerClient ?? throw new ArgumentNullException(nameof(serviceBusManagerClient));
    private readonly ILogger<GeneralCertificateEnrichmentProcessor> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task SendMessageAsync(GeneralCertificateRequest messageRequest, TradeEventMessageHeader messageHeader, CancellationToken cancellationToken = default)
    {
        var message = BuildMessage(_mapper, messageRequest, messageHeader);

        _logger.SendingMessageToEnrichmentQueue(messageRequest.ExchangedDocument.Id);

        await _serviceBusManagerClient.SendMessageAsync(message);

        _logger.SendingMessageToEnrichmentQueueSuccess(messageRequest.ExchangedDocument.Id);
    }

    private static Message BuildMessage(IMapper mapper, GeneralCertificateRequest messageRequest, TradeEventMessageHeader messageHeader)
    {
        MessageBuilder messageBuilder = new();

        messageBuilder
            .WithUserProperty("EntityKey", messageRequest.ExchangedDocument.Id)
            .WithUserProperty("CausationId", messageHeader.MessageId)
            .WithUserProperty("PublisherId", PublisherId)
            .WithUserProperty("SchemaVersion", SchemaVersion)
            .WithUserProperty("Status", Status)
            .WithUserProperty("Type", Type)
            .WithUserProperty("TimestampUtc", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString());

        var toSend = mapper.Map<GeneralCertificateEnrichmentPayload>(messageRequest);

        var message = messageBuilder
            .MessageId(Guid.NewGuid().ToString())
            .CorrelationId(messageHeader.CorrelationId)
            .ContentType(ContentType)
            .Label(Label)
            .WithBody(JsonSerializer.Serialize(toSend, SerializerOptions.GetSerializerOptions()))
            .Build();

        return message;
    }
}