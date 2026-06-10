// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Extensions;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Functions;

public class GcCreatedServiceBusTriggerFunction
{
    private readonly IBaseMessageProcessorService<GeneralCertificateInbound> _baseMessageProcessorService;
    private readonly IMessageRetryService _messageRetryService;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<GcCreatedServiceBusTriggerFunction> _logger;

    public GcCreatedServiceBusTriggerFunction(
        IBaseMessageProcessorService<GeneralCertificateInbound> baseMessageProcessorService,
        IMessageRetryService messageRetryService,
        ServiceBusClient serviceBusClient,
        ILogger<GcCreatedServiceBusTriggerFunction> logger)
    {
        ArgumentNullException.ThrowIfNull(baseMessageProcessorService);
        ArgumentNullException.ThrowIfNull(messageRetryService);
        ArgumentNullException.ThrowIfNull(serviceBusClient);
        ArgumentNullException.ThrowIfNull(logger);
        _baseMessageProcessorService = baseMessageProcessorService;
        _messageRetryService = messageRetryService;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    [Function(nameof(GcCreatedServiceBusTriggerFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(GcSubscriberSettings.DefaultQueueName, Connection = GcSubscriberSettings.ConnectionStringConfigurationKey, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        FunctionContext executionContext)
    {
        await RunInternalAsync(message, messageActions, executionContext);
    }

    private static string GetGcId(BinaryData messageBody)
    {
        var gcInbound = JsonConvert.DeserializeObject<dynamic>(messageBody.ToString());

        return gcInbound?.exchangedDocument?.id ?? string.Empty;
    }

    private async Task RunInternalAsync(
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        FunctionContext executionContext)
    {
        try
        {
            string gcId = GetGcId(message.Body);

            _logger.MessageReceived(message.MessageId, executionContext.FunctionDefinition.Name, gcId);

            await using var eventStoreSender = _serviceBusClient.CreateSender(GcSubscriberSettings.TradeEventInfo);
            await using var retrySender = _serviceBusClient.CreateSender(GcSubscriberSettings.DefaultQueueName);

            _messageRetryService.SetContext(message, retrySender);

            await _baseMessageProcessorService.ProcessAsync(executionContext.InvocationId,
                GcSubscriberSettings.DefaultQueueName,
                GcSubscriberSettings.PublisherId,
                message,
                messageActions,
                eventStoreSender,
                originalCrmPublisherId: GcSubscriberSettings.PublisherId,
                originalSource: GcSubscriberSettings.DefaultQueueName,
                originalRequestName: "Create");

            _logger.ProcessMessageSuccess(message.MessageId, executionContext.FunctionDefinition.Name, gcId);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }
    }
}
