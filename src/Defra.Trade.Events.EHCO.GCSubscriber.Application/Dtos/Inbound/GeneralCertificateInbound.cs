// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Dtos.Inbound;

public class GeneralCertificateInbound
{
    /// <summary>
    /// The header document information for a use of this master message assembly.
    /// </summary>
    public ExchangedDocument ExchangedDocument { get; set; }

    /// <summary>
    /// A supply chain consignment specified for a use of this master message assembly.
    /// </summary>
    public SupplyChainConsignment SupplyChainConsignment { get; set; }
}