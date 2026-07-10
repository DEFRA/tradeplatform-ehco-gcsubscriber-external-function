// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Collections.Generic;
using Defra.Trade.Common.Function.Health.HealthChecks.ApiCheck;
using DocStoreAPI = Defra.Trade.API.CertificatesStore.V1.ApiClient.Api;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

public class DocStoreHealthApi(DocStoreAPI.IHealthApi healthApi) : IHealthApi
{

    private readonly DocStoreAPI.IHealthApi _healthApi = healthApi;

    public string GetBasePath() => _healthApi.GetBasePath();
    public IDictionary<string, string> GetDefaultHeaders() => _healthApi.Configuration.DefaultHeaders;
}
