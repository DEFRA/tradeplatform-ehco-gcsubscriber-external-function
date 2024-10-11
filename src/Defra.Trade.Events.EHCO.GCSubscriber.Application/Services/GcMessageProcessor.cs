// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.API.CertificatesStore.V1.ApiClient.Api;
using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Extensions;
using Defra.Trade.Events.EHCO.GCSubscriber.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Services;

/// <summary>
/// Object initializer.
/// </summary>
/// <param name="ehcoGeneralCertificateApplicationApi"></param>
/// <param name="logger"></param>
/// <exception cref="ArgumentNullException"></exception>
public class GcMessageProcessor(
    ILogger<GcMessageProcessor> logger,
    IEhcoGeneralCertificateApplicationApi ehcoGeneralCertificateApplicationApi) : IGcMessageProcessor
{
    private const string CacheStoreApiVersion = "v1";

    private readonly IEhcoGeneralCertificateApplicationApi _ehcoGeneralCertificateApplicationApi = ehcoGeneralCertificateApplicationApi
                                                    ?? throw new ArgumentNullException(nameof(ehcoGeneralCertificateApplicationApi));

    private readonly ILogger<GcMessageProcessor> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task ProcessMessage(GeneralCertificateRequest gcMessageRequest)
    {
        _logger.SendingMessageToCertificateStore(gcMessageRequest.ExchangedDocument.Id);

        var application = new EhcoGeneralCertificateApplication(
            gcMessageRequest.ExchangedDocument,
            gcMessageRequest.SupplyChainConsignment);

        await _ehcoGeneralCertificateApplicationApi.SaveEHCOGeneralCertificateApplicationAsync(
            CacheStoreApiVersion,
            application);

        _logger.SendingMessageToCertificateStoreSuccess(gcMessageRequest.ExchangedDocument.Id);
    }
}