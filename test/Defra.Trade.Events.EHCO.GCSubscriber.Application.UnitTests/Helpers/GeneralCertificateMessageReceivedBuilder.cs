// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Generic;
using Azure.Messaging.ServiceBus;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.UnitTests.Helpers;

public class GeneralCertificateMessageReceivedBuilder
{
    private readonly Dictionary<string, object> _applicationProperties = [];
    private string _correlationId = string.Empty;
    private ServiceBusReceivedMessage _message;
    private string _messageId = string.Empty;
    private string _subject = string.Empty;

    public ServiceBusReceivedMessage Build()
    {
        _message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            messageId: _messageId,
            correlationId: _correlationId,
            subject: _subject,
            properties: _applicationProperties
        );

        return _message;
    }

    public GeneralCertificateMessageReceivedBuilder WithCorrelationId(string value)
    {
        _correlationId = value;

        return this;
    }

    public GeneralCertificateMessageReceivedBuilder WithMessageId(string value)
    {
        _messageId = value;

        return this;
    }

    public GeneralCertificateMessageReceivedBuilder WithProperty(string key, object value)
    {
        _applicationProperties.Add(key, value);

        return this;
    }

    public GeneralCertificateMessageReceivedBuilder WithSubject(string value)
    {
        _subject = value;

        return this;
    }
}
