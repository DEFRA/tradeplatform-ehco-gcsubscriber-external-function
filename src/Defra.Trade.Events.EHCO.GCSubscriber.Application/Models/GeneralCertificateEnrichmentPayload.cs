// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.API.CertificatesStore.V1.ApiClient.Model;

namespace Defra.Trade.Events.EHCO.GCSubscriber.Application.Models;

public class GeneralCertificateEnrichmentPayload
{
    public string GcId { get; set; }

    public Applicant Applicant { get; set; }

    public Consignor Consignor { get; set; }

    public Consignee Consignee { get; set; }

    public LocationInfo DispatchLocation { get; set; }

    public LocationInfo DestinationLocation { get; set; }
}