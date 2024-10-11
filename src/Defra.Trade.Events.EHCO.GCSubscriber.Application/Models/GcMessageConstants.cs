// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

public static class GcMessageConstants
{
    public const string BrokerLabel = "ehco.remos.application.create";
    public const string ContentErrorMessage = "{PropertyName} must be application/json";
    public const string LabelErrorMessage = "{PropertyName} must be ehco.remos.application.create";
    public const string MessageContentType = "application/json";
    public const string PublisherIdErrorMessage = "{PropertyName} must be EHCO";
    public const string SchemaVersion = "1";
    public const string SchemaVersionMessage = "{PropertyName} must be 2";
    public const string SchemaVersionV2 = "2";
    public const string Status = "Approved";
    public const string StatusErrorMessage = "{PropertyName} must be Approved";
    public const string TypeErrorMessage = "{PropertyName} must be Internal";
}