// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Generic;
using Azure.Messaging.ServiceBus;

namespace Defra.Trade.Events.EHCO.GCSubscriber.UnitTests.Helpers;

public class ServiceBusReceivedMessageBuilder
{
    private readonly Dictionary<string, object> _applicationProperties = [];
    private BinaryData _body;
    private string _correlationId = string.Empty;
    private ServiceBusReceivedMessage _message = null;
    private string _messageId = string.Empty;
    private string _subject = string.Empty;

    public ServiceBusReceivedMessage Build()
    {
        _message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            messageId: _messageId,
            correlationId: _correlationId,
            body: _body,
            subject: _subject,
            properties: _applicationProperties
        );

        return _message;
    }

    public ServiceBusReceivedMessageBuilder WithBody(BinaryData body)
    {
        _body = body;
        return this;
    }

    public ServiceBusReceivedMessageBuilder WithCorrelationId(string value)
    {
        _correlationId = value;

        return this;
    }

    public ServiceBusReceivedMessageBuilder WithMessageId(string value)
    {
        _messageId = value;

        return this;
    }

    public ServiceBusReceivedMessageBuilder WithProperty(string key, object value)
    {
        _applicationProperties.Add(key, value);

        return this;
    }

    public ServiceBusReceivedMessageBuilder WithSubject(string value)
    {
        _subject = value;

        return this;
    }
}
