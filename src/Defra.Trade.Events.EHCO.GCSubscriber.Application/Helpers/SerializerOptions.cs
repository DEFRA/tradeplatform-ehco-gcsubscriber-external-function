// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Helpers;

public static class SerializerOptions
{
    public static JsonSerializerOptions GetSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}